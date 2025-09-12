using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public enum TrafficTypes
    {
        IA,
        IB
    }
    public class TrafficType : INotifyPropertyChanged
    {
        private TrafficTypes _trafficTypes;
        private string _typeIconPath;

        public TrafficTypes TrafficTypes
        {
            get 
            { 
                return _trafficTypes; 
            }
            set
            {
                if (_trafficTypes != value)
                {
                    _trafficTypes = value;
                    OnPropertyChanged(nameof(TrafficTypes));
                }
            }
        }

        public string TypeIconPath
        {
            get
            {
                return _typeIconPath;
            }
            set
            {
                if( _typeIconPath != value)
                {
                    _typeIconPath = value;
                    OnPropertyChanged(nameof(TypeIconPath));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
