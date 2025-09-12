using MVVM1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetworkService.ViewModel
{
    public class HomeViewModel : BindableBase
    {
        private string _title = "Infrastrukture Simulator";
       
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

        private MainWindowViewModel _mainWindow;
        public MyICommand NavigateNetworkEntitesCommand { get; set; }
        public MyICommand NavigateNetworkDisplayCommand { get; set; }
        public MyICommand NavigateMeasurementGraphCommand { get; set; }


        public HomeViewModel()
        {
            NavigateNetworkEntitesCommand = new MyICommand(OnNeworkEntities);
            NavigateNetworkDisplayCommand = new MyICommand(OnNetworkDisplay);
            NavigateMeasurementGraphCommand = new MyICommand(OnMeasurementGraph);
        }

        public void SetMainWindowReference(MainWindowViewModel main)
        {
            _mainWindow = main;
        }

        private void OnNeworkEntities()
        {
           _mainWindow.CurrentViewModel = _mainWindow.NetworkEntitiesViewModel;
        }

        private void OnNetworkDisplay()
        {
            _mainWindow.CurrentViewModel = _mainWindow.NetworkDisplayViewModel;
        }

        private void OnMeasurementGraph()
        {
            _mainWindow.CurrentViewModel = _mainWindow.MeasurementGraphViewModel;
        }
    }
}
