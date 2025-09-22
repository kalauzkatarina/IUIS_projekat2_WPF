using MVVM1;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {
        private string _title = "Network Display";
        private TrafficType _slectedTrafficType;
        private NetworkEntitiesViewModel _networkEntitiesViewModel;
        private DailyTraffic _selectedEntity;
        private CanvasSlot _firstSelectedSlot;

        private MainWindowViewModel _mainWindow;

        public void SetMainWindowReference(MainWindowViewModel main)
        {
            _mainWindow = main;
        }


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

        private bool _isConnectMode;
        public bool IsConnectMode
        {
            get => _isConnectMode;
            set { _isConnectMode = value; OnPropertyChanged(nameof(IsConnectMode)); }
        }

        public ObservableCollection<DailyTraffic> EntitiesForSelectedType { get; set; }
        //za 12 canvas slotova
        public ObservableCollection<CanvasSlot> CanvasSlots { get; set; }
        public ObservableCollection<Connection> Connections { get; set; }

        public TrafficRootVM TrafficRoot { get; set; }
        public MyICommand<object> ReturnEntityCommand { get; set; }
        public MyICommand ToggleConnectModeCommand { get; set; }

        public MyICommand NavigateToEntitiesCommand { get; set; }
        public MyICommand NavigateToGraphCommand { get; set; }

        public NetworkDisplayViewModel(NetworkEntitiesViewModel networkEntitiesViewModel)
        {
            _networkEntitiesViewModel = networkEntitiesViewModel;

            _networkEntitiesViewModel.EntitiesChanged += OnEntitiesChanged;

            var groups = _networkEntitiesViewModel.TrafficTypesList
            .Select(tt => new TrafficTypeGroupVM(tt, _networkEntitiesViewModel.Entities.Where(e => e.TrafficType == tt)));

            TrafficRoot = new TrafficRootVM(groups);

            EntitiesForSelectedType = new ObservableCollection<DailyTraffic>();

            CanvasSlots = new ObservableCollection<CanvasSlot>();
            Connections = new ObservableCollection<Connection>();

            int cols = 4;
            int rows = 3;
            double canvasWidth = 415;
            double canvasHeight = 400;
            int slotWidth = 80;
            int slotHeight = 100;

            double horizontalSpacing = (canvasWidth - cols * slotWidth) / (cols + 1);
            double verticalSpacing = (canvasHeight - rows * slotHeight) / (rows + 1);


            for (int i = 0; i < rows * cols; i++)
            {
                int col = i % cols;
                int row = i / cols;

                double centerX = horizontalSpacing + col * (slotWidth + horizontalSpacing) + slotWidth / 2.0;
                double centerY = verticalSpacing + row * (slotHeight + verticalSpacing) + slotHeight / 2.0;

                var slot = new CanvasSlot
                {
                    SlotId = i + 1,
                    CenterX = centerX,
                    CenterY = centerY
                };

                slot.DropCommand = new MyICommand<DailyTraffic>((entity) => PlaceEntityOnCanvas(entity, slot));
                slot.StartDragCommand = new MyICommand<DailyTraffic>((entity) => RemoveEntityFromCanvas(slot));
                slot.SelectCommand = new MyICommand(() => SelectSlot(slot));

                CanvasSlots.Add(slot);
            }

            ReturnEntityCommand = new MyICommand<object>(ReturnEntityFromCanvas);

            ToggleConnectModeCommand = new MyICommand(() => IsConnectMode = !IsConnectMode);

            NavigateToEntitiesCommand = new MyICommand(OnEntities);
            NavigateToGraphCommand = new MyICommand(OnGraph);

            _networkEntitiesViewModel.Entities.CollectionChanged += (s, e) =>
            {
                RefreshEntitiesFromNetwork();
            };
        }


        private void OnEntitiesChanged()
        {
            // Prvo ukloni obrisane entitete
            foreach (var group in TrafficRoot.Children)
            {
                for (int i = group.Entities.Count - 1; i >= 0; i--)
                {
                    var entity = group.Entities[i];
                    if (!_networkEntitiesViewModel.Entities.Contains(entity) &&
                        !CanvasSlots.Any(s => s.Entity == entity)) // ako nije na Canvasu
                    {
                        group.Entities.Remove(entity);
                    }
                }
            }

            // Prođi kroz sve entitete i dodaj nove u odgovarajuću grupu
            foreach (var entity in _networkEntitiesViewModel.Entities)
            {
                bool isOnCanvas = CanvasSlots.Any(s => s.Entity == entity);
                if (isOnCanvas) continue;

                // Provjeri da li već postoji u nekoj grupi
                bool exists = TrafficRoot.Children
                                .Any(g => g.Entities.Contains(entity));

                if (!exists)
                {
                    // Nadji grupu po tipu
                    var group = TrafficRoot.Children
                                .FirstOrDefault(g => g.TrafficType == entity.TrafficType);

                    if (group != null)
                    {
                        group.Entities.Add(entity); // ovo osvežava TreeView
                    }
                    else
                    {
                        // Ako nema grupe, kreiraj novu
                        var newGroup = new TrafficTypeGroupVM(entity.TrafficType, new List<DailyTraffic> { entity });
                        TrafficRoot.Children.Add(newGroup);
                    }
                }
            }

            UpdateEntitiesForSelectedType(); // opcionalno, da osveži panel ispod
        }

        private void OnEntities()
        {
            if (_mainWindow != null)
                _mainWindow.CurrentViewModel = _mainWindow.NetworkEntitiesViewModel;
        }

        private void OnGraph()
        {
            if (_mainWindow != null)
                _mainWindow.CurrentViewModel = _mainWindow.MeasurementGraphViewModel;
        }

        public void RefreshEntitiesFromNetwork()
        {
            foreach (var group in TrafficRoot.Children)
            {
                group.Entities.Clear();
                foreach (var entity in _networkEntitiesViewModel.Entities.Where(e => e.TrafficType == group.TrafficType))
                {
                    group.Entities.Add(entity);
                }
            }

            // Osveži panel sa entitetima za selektovani tip
            UpdateEntitiesForSelectedType();
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

            // ukloni sve veze za ovaj slot
            RemoveConnectionsForEntity(entity);

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
                if (!group.Entities.Contains(entity))
                {
                    group?.Entities.Add(entity);
                }

                // ukloni sa Canvas-a - pronađi slot koji sadrži ovaj entitet
                var slot = CanvasSlots.FirstOrDefault(s => s.Entity == entity);
                if (slot != null)
                {
                    RemoveConnectionsForEntity(entity); // promenjeno
                    slot.Entity = null;
                }

                UpdateEntitiesForSelectedType();
            }
        }

        public void ConnectEntities(DailyTraffic source, DailyTraffic target)
        {
            if (source == null || target == null || source == target) return;

            bool exists = Connections.Any(c =>
                (c.SourceEntity == source && c.TargetEntity == target) ||
                (c.SourceEntity == target && c.TargetEntity == source));
            if (exists) return;

            var conn = new Connection(source, target);
            Connections.Add(conn);

            // pretplata na sve slotove da linija prati promene
            foreach (var slot in CanvasSlots)
            {
                slot.EntityChanged += (oldEntity, newEntity) => conn.UpdatePositions(CanvasSlots);
                slot.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(CanvasSlot.CenterX) || e.PropertyName == nameof(CanvasSlot.CenterY))
                        conn.UpdatePositions(CanvasSlots);
                };
            }

            // inicijalno postavi pozicije
            conn.UpdatePositions(CanvasSlots);
        }


        public void RemoveConnectionsForEntity(DailyTraffic entity)
        {
            if (entity == null) return;

            var toRemove = Connections
                .Where(c => c.SourceEntity == entity || c.TargetEntity == entity)
                .ToList();

            foreach (var r in toRemove)
                Connections.Remove(r);
        }

        public void SelectSlot(CanvasSlot slot)
        {
            if (!IsConnectMode) return;
            if (slot.Entity == null) return; // samo ako slot ima entitet

            if (_firstSelectedSlot == null)
            {
                _firstSelectedSlot = slot;
            }
            else
            {
                var firstEntity = _firstSelectedSlot.Entity;
                var secondEntity = slot.Entity;

                if (firstEntity != null && secondEntity != null)
                {
                    ConnectEntities(firstEntity, secondEntity);
                }

                _firstSelectedSlot = null;
            }
        }
    }

    //svaki canvas ima entitet ili prazno
    public class CanvasSlot : BindableBase
    {
        private DailyTraffic _entity;
        private double _centerX;
        private double _centerY;

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
                    if (_entity != null)
                    {
                        // Odjavi starog entiteta
                        _entity.PropertyChanged -= Entity_PropertyChanged;
                    }

                    var oldEntity = _entity;
                    _entity = value;

                    if (_entity != null)
                    {
                        SubscribeToEntity(_entity);
                    }

                    OnPropertyChanged(nameof(Entity));
                    OnPropertyChanged(nameof(BorderBrushColor));

                    EntityChanged?.Invoke(oldEntity, _entity);
                }
            }
        }

        public double CenterX
        {
            get
            {
                return _centerX;
            }
            set
            {
                if( _centerX != value)
                {
                    _centerX = value;
                    OnPropertyChanged(nameof(CenterX));
                }
            }
        }

        public double CenterY
        {
            get
            {
                return _centerY;
            }
            set
            {
                if(_centerY != value)
                {
                    _centerY = value;
                    OnPropertyChanged(nameof(CenterY));
                }
            }
        }

        public void NotifyBorderBrushChanged()
        {
            OnPropertyChanged(nameof(BorderBrushColor));
        }

        public Brush BorderBrushColor
        {
            get
            {
                if (Entity == null) return Brushes.Gray;

                // Ako je vrednost iznad 10000 -> crvena, inače normalna
                return Entity.LastValue > 10000 ? Brushes.Red : Brushes.Gray;
            }
        }

        public void SubscribeToEntity(DailyTraffic entity)
        {
            if (entity != null)
            {
                entity.PropertyChanged += Entity_PropertyChanged;
            }
        }

        private void Entity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DailyTraffic.LastValue))
            {
                OnPropertyChanged(nameof(BorderBrushColor));
            }
        }

        public MyICommand<DailyTraffic> DropCommand { get; set; }
        public MyICommand<DailyTraffic> StartDragCommand { get; set; }
        public MyICommand SelectCommand { get; set; }

        public event Action<DailyTraffic, DailyTraffic> EntityChanged;

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

    public class Connection : BindableBase
    {
        private double _x1, _y1, _x2, _y2;

        public double X1
        {
            get
            {
                return _x1;
            }
            set
            {
                if (_x1 != value)
                {
                    _x1 = value;
                    OnPropertyChanged(nameof(X1));
                }
            }
        }

        public double Y1
        {
            get
            {
                return _y1;
            }
            set
            {
                if (_y1 != value)
                {
                    _y1 = value;
                    OnPropertyChanged(nameof(Y1));
                }
            }
        }

        public double X2
        {
            get
            {
                return _x2;
            }
            set
            {
                if (_x2 != value)
                {
                    _x2 = value;
                    OnPropertyChanged(nameof(X2));
                }
            }
        }

        public double Y2
        {
            get
            {
                return _y2;
            }
            set
            {
                if (_y2 != value)
                {
                    _y2 = value;
                    OnPropertyChanged(nameof(Y2));
                }
            }
        }

        public DailyTraffic SourceEntity { get; }
        public DailyTraffic TargetEntity { get; }

        public Connection(DailyTraffic source, DailyTraffic target)
        {
            SourceEntity = source;
            TargetEntity = target;
        }

        // metoda da osveži koordinate
        public void UpdatePositions(ObservableCollection<CanvasSlot> slots)
        {
            var sourceSlot = slots.FirstOrDefault(s => s.Entity == SourceEntity);
            var targetSlot = slots.FirstOrDefault(s => s.Entity == TargetEntity);

            if (sourceSlot != null)
            {
                X1 = sourceSlot.CenterX;
                Y1 = sourceSlot.CenterY;
            }

            if (targetSlot != null)
            {
                X2 = targetSlot.CenterX;
                Y2 = targetSlot.CenterY;
            }
        }
    }
}