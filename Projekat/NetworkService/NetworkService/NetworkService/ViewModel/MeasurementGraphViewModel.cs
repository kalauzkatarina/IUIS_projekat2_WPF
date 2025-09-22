using MVVM1;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : BindableBase
    {
        private string _title = "Measurement Graph";
        private string _selectedEntity;
        private ObservableCollection<string> _entities;
        private ObservableCollection<Measurement> _measurements;
        private NetworkEntitiesViewModel _networkEntitiesViewModel;

        private ObservableCollection<BarInfo> _bars;

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
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string SelectedEntity
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
                    Measurements.Clear();
                }
            }
        }

        public ObservableCollection<string> Entities
        {
            get
            {
                return _entities;
            }
            set
            {
                if (_entities != value)
                {
                    _entities = value;
                    OnPropertyChanged(nameof(Entities));
                }
            }
        }

        public ObservableCollection<Measurement> Measurements
        {
            get
            {
                return _measurements;
            }
            set
            {
                if (_measurements != value)
                {
                    _measurements = value;
                    OnPropertyChanged(nameof(Measurement));
                }
            }
        }

        public ObservableCollection<BarInfo> Bars
        {
            get
            {
                return _bars;
            }
            set
            {
                if (_bars != value)
                {
                    _bars = value;
                    OnPropertyChanged(nameof(Bars));
                }
            }
        }

        public List<double> YAxisLabels
        {
            get
            {
                int steps = 5;
                double stepValue = MaxValue / (steps - 1); // npr. 0, 5250, 10500, 15750, 21000
                return Enumerable.Range(0, steps).Select(i => i * stepValue).Reverse().ToList();
            }
        }

        public double MaxValue { get; set; } = 21000;

        //public ObservableCollection<DailyTraffic> Entities => _networkEntitiesViewModel.Entities;

        public MyICommand NavigateToEntitiesCommand { get; set; }
        public MyICommand NavigateToDisplayCommand { get; set; }


        public MeasurementGraphViewModel(NetworkEntitiesViewModel networkEntitiesViewModel)
        {
            _networkEntitiesViewModel = networkEntitiesViewModel;
            //Entities = new ObservableCollection<string>();
            Entities = new ObservableCollection<string>(
            _networkEntitiesViewModel.Entities.Select(e => e.Name));

            _networkEntitiesViewModel.Entities.CollectionChanged += (s, e) =>
            {
                // Osveži Entities kada se doda/ukloni entitet
                Entities.Clear();
                foreach (var entity in _networkEntitiesViewModel.Entities)
                    Entities.Add(entity.Name);
            };

            Measurements = new ObservableCollection<Measurement>();
            Bars = new ObservableCollection<BarInfo>();

            Measurements.CollectionChanged += (s, e) => UpdateBars();

            NavigateToEntitiesCommand = new MyICommand(OnEntities);
            NavigateToDisplayCommand = new MyICommand(OnDisplay);
        }

        public void AddMeasurementRealTime(string entityName, double value)
        {
            if (!Entities.Contains(entityName)) Entities.Add(entityName);

            if (SelectedEntity != entityName) return;

            var measurement = new Measurement
            {
                Time = DateTime.Now,
                Value = value
            };

            Measurements.Add(measurement);

            if (Measurements.Count > 5)
                Measurements.RemoveAt(0);

            UpdateBars();
        }

        private void UpdateBars()
        {

            Bars.Clear();
            if (Measurements.Count == 0) return;

            var last5 = Measurements.ToList();

            double canvasHeight = 310;
            double scale = canvasHeight / MaxValue;
            double barWidth = 40;

            foreach (var m in last5)
            {
                Bars.Add(new BarInfo
                {
                    Height = m.Value * scale,
                    Width = barWidth,
                    Fill = m.BarColor,
                    Label = m.Time.ToString("HH:mm:ss")
                });
            }
        }

        private void OnEntities()
        {
            if (_mainWindow != null)
                _mainWindow.CurrentViewModel = _mainWindow.NetworkEntitiesViewModel;
        }

        private void OnDisplay()
        {
            if (_mainWindow != null)
                _mainWindow.CurrentViewModel = _mainWindow.NetworkDisplayViewModel;
        }
    }

    public class Measurement
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }
        public Brush BarColor => Value >= 0 && Value <= 10000 ? Brushes.Green : Brushes.Red;
    }

    public class BarInfo
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public Brush Fill { get; set; }
        public string Label { get; set; } // vreme za X osu
    }
}
