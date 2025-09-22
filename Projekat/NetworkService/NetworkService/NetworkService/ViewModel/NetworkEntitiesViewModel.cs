using MVVM1;
using NetworkService.Actions;
using NetworkService.Model;
using NetworkService.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {
        private string _title = "Network Entities";

        public Action EntitiesChanged;
        public Action<IUndoableAction> ActionPerformed;
       
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
        private string _idError;
        private string _nameError;
        private string _searchValueError;
        private Brush _idBorderBrush = Brushes.Gray;
        private Brush _nameBorderBrush = Brushes.Gray;
        private Brush _searchBorderBrush = Brushes.Gray;

        private string _searchValue;
        private bool _isLessChecked;
        private bool _isGreaterChecked;
        private bool _isEqualChecked;

        private bool _isKeyboardVisible;
        private BindingExpression _activeTextBoxBinding;

        private bool _isCapsLockOn;

        private MainWindowViewModel _mainWindow;

        public void SetMainWindowReference(MainWindowViewModel main)
        {
            _mainWindow = main;
        }

        public bool IsKeyboardVisible
        {
            get
            {
                return _isKeyboardVisible;
            }
            set
            {
                if (_isKeyboardVisible != value)
                {
                    _isKeyboardVisible = value;
                    OnPropertyChanged(nameof(IsKeyboardVisible));
                }
            }
        }

        public BindingExpression ActiveTextBoxBinding
        {
            get
            {
                return _activeTextBoxBinding;
            }
            set
            {
                if( _activeTextBoxBinding != value)
                {
                    _activeTextBoxBinding = value;
                    OnPropertyChanged(nameof(ActiveTextBoxBinding));
                }
            }
        }

        public bool IsCapsLockOn
        {
            get
            {
                return _isCapsLockOn;
            }
            set
            {
                if(_isCapsLockOn != value)
                {
                    _isCapsLockOn = value;
                    OnPropertyChanged(nameof(IsCapsLockOn));
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
                    ValidateId();
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
                    ValidateString();
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

        public string IdError
        {
            get
            {
                return _idError;
            }
            set
            {
                if (_idError != value)
                {
                    _idError = value;
                    OnPropertyChanged(nameof(IdError));
                }
            }
        }

        public string NameError
        {
            get
            {
                return _nameError;
            }
            set
            {
                if (_nameError != value)
                {
                    _nameError = value;
                    OnPropertyChanged(nameof(NameError));
                    ValidateString();
                }
            }
        }

        public string SearchValueError
        {
            get
            {
                return _searchValueError;
            }
            set
            {
                if (_searchValueError != value)
                {
                    _searchValueError = value;
                    OnPropertyChanged(nameof(SearchValueError));
                }
            }
        }

        public Brush IdBorderBrush
        {
            get
            {
                return _idBorderBrush;
            }
            set
            {
                if(_idBorderBrush != value)
                {
                    _idBorderBrush = value;
                    OnPropertyChanged(nameof(IdBorderBrush));
                }
            }
        }

        public Brush NameBorderBrush
        {
            get
            {
                return _nameBorderBrush;
            }
            set
            {
                if( _nameBorderBrush != value)
                {
                    _nameBorderBrush = value;
                    OnPropertyChanged(nameof(NameBorderBrush));
                }
            }
        }

        public Brush SearchBorderBrush
        {
            get
            {
                return _searchBorderBrush;
            }
            set
            {
                if(_searchBorderBrush != value)
                {
                    _searchBorderBrush = value;
                    OnPropertyChanged(nameof(SearchBorderBrush));
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
                    ValidateSearchValue();
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
        public MyICommand NavigateToDisplayCommand { get; set; }
        public MyICommand NavigateToGraphCommand { get; set; }
        public MyICommand<string> KeyboardKeyCommand { get; set; }
        public MyICommand KeyboardBackspaceCommand { get; set; }
        public NetworkEntitiesViewModel()
        {
            LoadData();
            AddCommand = new MyICommand(OnAdd);
            DeleteCommand = new MyICommand(OnDelete, CanDelete);
            DeleteEntityCommand = new MyICommand<DailyTraffic>(OnDeleteEntity);
            ResetCommand = new MyICommand(OnReset);    
            SearchCommand = new MyICommand(OnSearch);

            NavigateToDisplayCommand = new MyICommand(OnDisplay);
            NavigateToGraphCommand = new MyICommand(OnGraph);

            SelectedTrafficType = TrafficTypesList.First();

            KeyboardKeyCommand = new MyICommand<string>(OnKeyboardKey);
            KeyboardBackspaceCommand = new MyICommand(OnBackspace);
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
                new DailyTraffic { Id = 2, Name = "Entitet_1", LastValue = 0, TrafficType = TrafficTypesList[1] },
                new DailyTraffic { Id = 3, Name = "Entitet_2", LastValue = 0, TrafficType = TrafficTypesList[1] }
            };

            AllEntities = new ObservableCollection<DailyTraffic>(Entities);
        }

        private void ValidateId()
        {
            if (string.IsNullOrWhiteSpace(IdText))
            {
                IdError = "Field cannot be empty.";
                IdBorderBrush = Brushes.Red;
            } else if(!int.TryParse(IdText, out _))
            {
                IdError = "Must be an integer";
                IdBorderBrush = Brushes.Red;
            } else
            {
                IdError = null;
                IdBorderBrush = Brushes.Gray;
            }
        }

        private void ValidateSearchValue()
        {
            if (string.IsNullOrWhiteSpace(SearchValue))
            {
                SearchValueError = "Field cannot be empty.";
                SearchBorderBrush = Brushes.Red;
            }
            else if (!int.TryParse(SearchValue, out _))
            {
                SearchValueError = "Must be an integer";
                SearchBorderBrush = Brushes.Red;
            }
            else
            {
                SearchValueError = null;
                SearchBorderBrush = Brushes.Gray;
            }
        }

        private void ValidateString()
        {
            if (string.IsNullOrWhiteSpace(NameText))
            {
                NameError = "Field cannot be empty.";
                NameBorderBrush = Brushes.Red;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(NameText, @"^[a-zA-Z0-9_\.]+$"))
            {
                NameError = "Allowed characters: letters, numbers, _ and .";
                NameBorderBrush = Brushes.Red;
            }
            else
            { 
                NameError = null;
                NameBorderBrush = Brushes.Gray;
            } 
        }

        private void OnAdd()
        {
            if (!int.TryParse(IdText, out int parsedId))          
                return;

            if (!string.IsNullOrEmpty(IdError) || !string.IsNullOrEmpty(NameError))
            {
                return;
            }

            if (Entities.Any(e => e.Id == parsedId))
            {
                System.Windows.MessageBox.Show("An entity with this ID already exists.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

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

            ActionPerformed?.Invoke(new AddEntityAction(Entities, newEntity));

            System.Windows.MessageBox.Show("Entity added successfully!", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void OnDelete()
        {
            if (SelectedEntity == null) return; // Pitaj korisnika direktno
            
            var result = System.Windows.MessageBox.Show( "Are you sure you want to delete this entity?", "Confirm Delete", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question ); 
            
            if (result == System.Windows.MessageBoxResult.Yes) 
            {
                var entityToDelete = SelectedEntity;
                Entities.Remove(entityToDelete);
                ActionPerformed?.Invoke(new DeleteEntityAction(Entities, entityToDelete)); OnEntitiesChanged(); 
                System.Windows.MessageBox.Show("Entity deleted successfully!", "Deleted", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information); 
            }
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

        private void OnDisplay()
        {
            if (_mainWindow != null)
                _mainWindow.CurrentViewModel = _mainWindow.NetworkDisplayViewModel;
        }

        private void OnGraph()
        {
            if (_mainWindow != null)
                _mainWindow.CurrentViewModel = _mainWindow.MeasurementGraphViewModel;
        }

        private void OnEntitiesChanged()
        {
            EntitiesChanged?.Invoke();
        }

        private void OnKeyboardKey(string key)
        {
            InsertCharacter(key);
        }

        private void OnBackspace()
        {
            Backspace();
        }

        public void InsertCharacter(string key)
        {
            if (ActiveTextBoxBinding?.Target is TextBox tb)
            {
                // Ako je CapsLock uključen i karakter je slovo, pretvori u veliko
                if (key.Length == 1 && char.IsLetter(key[0]))
                {
                    key = IsCapsLockOn ? key.ToUpper() : key.ToLower();
                }

                tb.Text += key;
                tb.CaretIndex = tb.Text.Length;
                tb.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            }
        }

        private void Backspace()
        {
            if(ActiveTextBoxBinding?.Target is TextBox tb && tb.Text.Length > 0)
            {
                //ukloni poslednji karakter
                tb.Text = tb.Text.Substring(0, tb.Text.Length - 1);

                //postavi kursor na kraj
                tb.CaretIndex = tb.Text.Length;

                //obavesti binding da se promenila vrednost
                tb.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            }
        }
    }
}
