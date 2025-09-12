using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace NetworkService.Model
{
    public class DailyTraffic : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private TrafficType _trafficType;
        private int _lastValue;

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if( _id != value )
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if( _name != value )
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
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
                if( _trafficType != value )
                {
                    _trafficType = value;
                    OnPropertyChanged(nameof(TrafficType));
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
                if( _lastValue != value )
                {
                    _lastValue = value;
                    OnPropertyChanged(nameof(LastValue));
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
