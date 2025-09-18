using MVVM1;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {
        private string _title = "Network Entities";

        public Action EntitiesChanged;
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

        private DailyTraffic _selectedEntity;

        private string _idText;
        private string _nameText;
        private int _lastValue;
        private TrafficType _trafficType;
        private TrafficType _selectedTrafficType;

        private string _searchValue;
        private bool _isLessChecked;
        private bool _isGreaterChecked;
        private bool _isEqualChecked;

        public DailyTraffic SelectedEntity
        {
            get 
            {
                return _selectedEntity;
            }
            set
            {
                if(_selectedEntity != value)
                {
                    _selectedEntity = value;
                    OnPropertyChanged(nameof(SelectedEntity));
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public string IdText
        {
            get
            {
                return _idText;
            }
            set
            {
                if( _idText != value)
                {
                    _idText = value;
                    OnPropertyChanged(nameof(IdText));
                }
            }
        }

        public string NameText
        {
            get
            {
                return _nameText;
            }
            set
            {
                if (_nameText != value)
                {
                    _nameText = value;
                    OnPropertyChanged(nameof(NameText));
                }
            }
        }

        public int LastValue
        {
            get
            {
                return _lastValue;
            }
            set
            {
                if(_lastValue != value)
                {
                    _lastValue = value;
                    OnPropertyChanged(nameof(LastValue));
                }
            }
        }

        public TrafficType TrafficType
        {
            get
            {
                return _trafficType;
            }
            set
            {
                if( _trafficType != value)
                {
                    _trafficType = value;
                    OnPropertyChanged(nameof(TrafficType));
                }
            }
        }

        public TrafficType SelectedTrafficType
        {
            get
            {
                return _selectedTrafficType;
            }
            set
            {
                if(_selectedTrafficType != value)
                {
                    _selectedTrafficType = value;
                    OnPropertyChanged(nameof(TrafficType));
                }
            }
        }

        public string SearchValue
        {
            get
            {
                return _searchValue;
            }
            set
            {
                if (_searchValue != value)
                {
                    _searchValue = value;
                    OnPropertyChanged(nameof(SearchValue));
                }
            }
        }

        public bool IsLessChecked
        {
            get
            {
                return _isLessChecked;
            }
            set
            {
                if (_isLessChecked != value)
                {
                    _isLessChecked = value;
                    OnPropertyChanged(nameof(IsLessChecked));
                }
            }
        }

        public bool IsGreaterChecked
        {
           get
            {
                return _isGreaterChecked;
            }
            set
            {
                if(_isGreaterChecked != value)
                {
                    _isGreaterChecked = value;
                    OnPropertyChanged(nameof(IsGreaterChecked));
                }
            }
        }

        public bool IsEqualChecked
        {
           get
            {
                return _isEqualChecked;
            }
            set
            {
                if(_isEqualChecked != value)
                {
                    _isEqualChecked = value;
                    OnPropertyChanged(nameof(IsEqualChecked));
                }
            }
        }

        public ObservableCollection<DailyTraffic> Entities { get; set; }
        public ObservableCollection<TrafficType> TrafficTypesList { get; set; }

        private ObservableCollection<DailyTraffic> AllEntities { get; set; }
        public MyICommand AddCommand { get; set; }
        public MyICommand DeleteCommand { get; set; }
        public MyICommand<DailyTraffic> DeleteEntityCommand { get; set; }
        public MyICommand ResetCommand { get; set; }
        public MyICommand SearchCommand { get; set; }


        public NetworkEntitiesViewModel()
        {
            LoadData();
            AddCommand = new MyICommand(OnAdd);
            DeleteCommand = new MyICommand(OnDelete, CanDelete);
            DeleteEntityCommand = new MyICommand<DailyTraffic>(OnDeleteEntity);
            ResetCommand = new MyICommand(OnReset);    
            SearchCommand = new MyICommand(OnSearch);

            SelectedTrafficType = TrafficTypesList.First();
        }

        public void LoadData()
        {
            TrafficTypesList = new ObservableCollection<TrafficType>
            {
                new TrafficType { TrafficTypes = TrafficTypes.IA, TypeIconPath = "/Resources/Images/IA.png" },
                new TrafficType { TrafficTypes = TrafficTypes.IB, TypeIconPath = "/Resources/Images/IB.jpg" }
            };

            Entities = new ObservableCollection<DailyTraffic>
            {
                new DailyTraffic { Id = 1, Name = "Entitet_0", LastValue = 0, TrafficType = TrafficTypesList[0] },
                new DailyTraffic { Id = 2, Name = "Entitet_1", LastValue = 0, TrafficType = TrafficTypesList[1] }
            };

            AllEntities = new ObservableCollection<DailyTraffic>(Entities);
        }

        private void OnAdd()
        {
            if (!int.TryParse(IdText, out int parsedId))
                return; 

            if (Entities.Any(e => e.Id == parsedId))
                return;

            var newEntity = new DailyTraffic
            {
                Id = parsedId,                
                Name = NameText,
                LastValue = LastValue,
                TrafficType = SelectedTrafficType
            };

            Entities.Add(newEntity);
            AllEntities.Add(newEntity);

            IdText = string.Empty;
            NameText = string.Empty;
            if (TrafficTypesList.Count > 0)
                SelectedTrafficType = TrafficTypesList[0];
            
            OnEntitiesChanged();
        }

        private void OnDelete()
        {
            if (SelectedEntity != null)
                Entities.Remove(SelectedEntity);

            OnEntitiesChanged();
        }

        private bool CanDelete()
        {
            return SelectedEntity != null;
        }

        private void OnDeleteEntity(DailyTraffic entity)
        {
            Entities.Remove(entity);
        }

        private void OnReset()
        {
            IdText = string.Empty;
            NameText = string.Empty;
            LastValue = 0;

            if (TrafficTypesList != null && TrafficTypesList.Count > 0)
                SelectedTrafficType = TrafficTypesList[0];

            Entities.Clear();
            foreach (var e in AllEntities)
                Entities.Add(e);

            SearchValue = string.Empty;
            IsLessChecked = false;
            IsGreaterChecked = false;
            IsEqualChecked = false;

            OnPropertyChanged(nameof(SelectedTrafficType));
        }

        private void OnSearch()
        {
            if (!int.TryParse(SearchValue, out int value)) return;

            ObservableCollection<DailyTraffic> FilteredEntities = new ObservableCollection<DailyTraffic>();

            
            foreach(DailyTraffic entity in Entities)
            {
                if (SelectedTrafficType.TrafficTypes == entity.TrafficType.TrafficTypes)
                {
                    if (IsLessChecked)
                    {
                        if(entity.LastValue < int.Parse(SearchValue))
                        {
                            FilteredEntities.Add(entity);
                        }
                    } else if (IsGreaterChecked)
                    {
                        if(entity.LastValue > int.Parse(SearchValue))
                        {
                            FilteredEntities.Add(entity);
                        }
                    } else if (IsEqualChecked)
                    {
                        if(entity.LastValue == int.Parse(SearchValue))
                        {
                            FilteredEntities.Add(entity);
                        }
                    } else
                    {
                        return;
                    }
                }
            }

            Entities.Clear();
            foreach(var e in FilteredEntities)
            {
                Entities.Add(e);
            }
        }

        private void OnEntitiesChanged()
        {
            EntitiesChanged?.Invoke();
        }
    }
}
