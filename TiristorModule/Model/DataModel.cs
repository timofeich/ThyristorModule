using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TiristorModule.Model
{
    public class DataModel : INotifyPropertyChanged
    {
        #region Implement INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        
        #region Fields
        private ushort voltageA;
        private ushort voltageB;
        private ushort voltageC;

        private ushort amperageA1;
        private ushort amperageB1;
        private ushort amperageC1;
        private ushort amperageA2;
        private ushort amperageB2;
        private ushort amperageC2;

        public ObservableCollection<DataModel> DataModels { get; set; }

        private ushort temperatureOfTiristor;
        private string workingStatus;
        #endregion

        #region Properties
        public ushort VoltageA
        {
            get { return voltageA; }
            set
            {
                if (voltageA != value)
                {
                    voltageA = value;
                    OnPropertyChanged("VoltageA");
                }
            }
        }

        public ushort VoltageB
        {
            get { return voltageB; }
            set
            {
                if (voltageB != value)
                {
                    voltageB = value;
                    OnPropertyChanged("VoltageB");
                }
            }
        }

        public ushort VoltageC
        {
            get { return voltageC; }
            set
            {
                if (voltageC != value)
                {
                    voltageC = value;
                    OnPropertyChanged("VoltageC");
                }
            }
        }

        public ushort AmperageA1
        {
            get { return amperageA1; }
            set
            {
                if (amperageA1 != value)
                {
                    amperageA1 = value;
                    OnPropertyChanged("AmperageA1");
                }
            }
        }

        public ushort AmperageB1
        {
            get { return amperageB1; }
            set
            {
                if (amperageB1 != value)
                {
                    amperageB1 = value;
                    OnPropertyChanged("AmperageB1");
                }
            }
        }

        public ushort AmperageC1
        {
            get { return amperageC1; }
            set
            {
                if (amperageC1 != value)
                {
                    amperageC1 = value;
                    OnPropertyChanged("AmperageC1");
                }
            }
        }

        public ushort AmperageA2
        {
            get { return amperageA1; }
            set
            {
                if (amperageA2 != value)
                {
                    amperageA2 = value;
                    OnPropertyChanged("AmperageA2");
                }
            }
        }

        public ushort AmperageB2
        {
            get { return amperageB2; }
            set
            {
                if (amperageB2 != value)
                {
                    amperageB2 = value;
                    OnPropertyChanged("AmperageB2");
                }
            }
        }

        public ushort AmperageC2
        {
            get { return amperageC2; }
            set
            {
                if (amperageC2 != value)
                {
                    amperageC2 = value;
                    OnPropertyChanged("AmperageC2");
                }
            }
        }

        public ushort TemperatureOfTiristor
        {
            get { return temperatureOfTiristor; }
            set
            {
                if (temperatureOfTiristor != value)
                {
                    temperatureOfTiristor = value;
                    OnPropertyChanged("TemperatureOfTiristor");
                }
            }
        }

        public string WorkingStatus
        {
            get { return workingStatus; }
            set
            {
                if (workingStatus != value)
                {
                    workingStatus = value;
                    OnPropertyChanged("WorkingStatus");
                }
            }
        }
        #endregion
    }
}
