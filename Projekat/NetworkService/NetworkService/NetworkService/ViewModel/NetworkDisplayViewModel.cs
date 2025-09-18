using MVVM1;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {
        private string _title = "Network Display";
        private TrafficType _slectedTrafficType;
        private NetworkEntitiesViewModel _networkEntitiesViewModel;
        private DailyTraffic _selectedEntity;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        public TrafficType SelectedTrafficType
        {
            get
            {
                return _slectedTrafficType;
            }
            set
            {
                if (_slectedTrafficType != value)
                {
                    _slectedTrafficType = value;
                    OnPropertyChanged(nameof(SelectedTrafficType));
                    UpdateEntitiesForSelectedType();
                }
            }
        }

        public DailyTraffic SelectedEntity
        {
            get
            {
                return _selectedEntity;
            }
            set
            {
                if (_selectedEntity != value)
                {
                    _selectedEntity = value;
                    OnPropertyChanged(nameof(SelectedEntity));
                }
            }
        }

        public ObservableCollection<NetworkEntitiesViewModel> AllEntities { get; set; }
        public ObservableCollection<DailyTraffic> EntitiesForSelectedType { get; set; }
        //za 12 canvas slotova
        public ObservableCollection<CanvasSlot> CanvasSlots { get; set; }
        public ObservableCollection<TrafficTypeGroupVM> TrafficTypeGroups { get; set; }

        public TrafficRootVM TrafficRoot { get; set; }
        public MyICommand<object> ReturnEntityCommand { get; set; }

        public NetworkDisplayViewModel(NetworkEntitiesViewModel networkEntitiesViewModel)
        {
            _networkEntitiesViewModel = networkEntitiesViewModel;
            var groups = _networkEntitiesViewModel.TrafficTypesList
            .Select(tt => new TrafficTypeGroupVM(tt, _networkEntitiesViewModel.Entities.Where(e => e.TrafficType == tt)));

            TrafficRoot = new TrafficRootVM(groups);


            EntitiesForSelectedType = new ObservableCollection<DailyTraffic>();

            CanvasSlots = new ObservableCollection<CanvasSlot>();

            for (int i = 0; i < 12; i++)
            {
                CanvasSlots.Add(new CanvasSlot { SlotId = i + 1 });
            }

            foreach (var slot in CanvasSlots)
            {
                slot.DropCommand = new MyICommand<DailyTraffic>((entity) => PlaceEntityOnCanvas(entity, slot));
                slot.StartDragCommand = new MyICommand<DailyTraffic>((entity) => RemoveEntityFromCanvas(slot));
            }

            ReturnEntityCommand = new MyICommand<object>(ReturnEntityFromCanvas);


        }

        private void UpdateEntitiesForSelectedType()
        {
            EntitiesForSelectedType.Clear();
            foreach (var entity in _networkEntitiesViewModel.Entities.Where(e => e.TrafficType == SelectedTrafficType))
            {
                EntitiesForSelectedType.Add(entity);
            }
        }

        public void PlaceEntityOnCanvas(DailyTraffic entity, CanvasSlot targetSlot)
        {
            if (targetSlot.Entity != null) return;

            // Pronađi slot gde je trenutno
            var currentSlot = CanvasSlots.FirstOrDefault(s => s.Entity == entity);
            if (currentSlot != null)
            {
                currentSlot.Entity = null;
            }

            targetSlot.Entity = entity;

            // uklanjanje iz globalne liste samo ako nije na Canvasu
            if (!_networkEntitiesViewModel.Entities.Contains(entity))
                _networkEntitiesViewModel.Entities.Remove(entity);

            // uklanjanje iz TreeView grupe
            var group = TrafficRoot.Children.FirstOrDefault(g => g.TrafficType == entity.TrafficType);
            group?.Entities.Remove(entity);

            UpdateEntitiesForSelectedType();
        }

        public void RemoveEntityFromCanvas(CanvasSlot slot)
        {
            if (slot.Entity == null) return;

            var entity = slot.Entity;
            slot.Entity = null;

            _networkEntitiesViewModel.Entities.Add(entity);

            var group = TrafficRoot.Children.FirstOrDefault(g => g.TrafficType == entity.TrafficType);
            group?.Entities.Add(entity);

            UpdateEntitiesForSelectedType();
        }

        // Nova metoda za vraćanje entiteta sa Canvas-a
        public void ReturnEntityFromCanvas(object obj)
        {
            if (obj is DailyTraffic entity) // sada je tip DailyTraffic
            {
                // dodaj nazad u globalnu listu
                if (!_networkEntitiesViewModel.Entities.Contains(entity))
                    _networkEntitiesViewModel.Entities.Add(entity);

                // dodaj nazad u TreeView grupu
                var group = TrafficRoot.Children.FirstOrDefault(g => g.TrafficType == entity.TrafficType);
                group?.Entities.Add(entity);

                // ukloni sa Canvas-a - pronađi slot koji sadrži ovaj entitet
                var slot = CanvasSlots.FirstOrDefault(s => s.Entity == entity);
                if (slot != null)
                    slot.Entity = null;

                UpdateEntitiesForSelectedType();
            }
        }
    }

    //svaki canvas ima entitet ili prazno
    public class CanvasSlot : BindableBase
    {
        private DailyTraffic _entity;
        public int SlotId { get; set; }
        public DailyTraffic Entity
        {
            get
            {
                return _entity;
            }
            set
            {
                if (_entity != value)
                {
                    _entity = value;
                    OnPropertyChanged(nameof(Entity));
                }
            }
        }

        // Metoda za "prazan slot"
        public bool IsEmpty
        {
            get
            {
                return Entity == null;
            }
        }

        public MyICommand<DailyTraffic> DropCommand { get; set; }
        public MyICommand<DailyTraffic> StartDragCommand { get; set; }

    }

    public class TrafficTypeGroupVM : BindableBase
    {
        public TrafficType TrafficType { get; set; }
        public ObservableCollection<DailyTraffic> Entities { get; set; }

        public TrafficTypeGroupVM(TrafficType trafficType, IEnumerable<DailyTraffic> entities)
        {
            TrafficType = trafficType;
            Entities = new ObservableCollection<DailyTraffic>(entities);
        }
    }

    public class TrafficRootVM : BindableBase
    {
        public string Name { get; set; }
        public ObservableCollection<TrafficTypeGroupVM> Children { get; set; }

        public TrafficRootVM(IEnumerable<TrafficTypeGroupVM> children)
        {
            Name = "Traffic Types";
            Children = new ObservableCollection<TrafficTypeGroupVM>(children);
        }
    }
}