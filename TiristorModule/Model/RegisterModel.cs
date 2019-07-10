using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TiristorModule
{
    public class Register : INotifyPropertyChanged
    {
        private ushort address;
        private ushort value;

        private ushort voltageA;
        private ushort voltageB;
        private ushort voltageC;

        private ushort amperageA1;
        private ushort amperageB1;
        private ushort amperageC1;
        private ushort amperageA2;
        private ushort amperageB2;
        private ushort amperageC2;


        
        private ushort temperatureOfTiristor;
        private ushort workingStatus;//enum перечисление
        private ushort opredelenieFazRevers;//


        public event PropertyChangedEventHandler PropertyChanged;

        public ushort Address
        {
            get
            {
                return address;
            }

            set
            {
                if (address != value)
                {
                    address = value;
                    OnPropertyChanged("Address");
                }
            }
        }

        public ushort Value
        {
            get
            {
                return value;
            }

            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged("Value");
                }
            }
        }

        public ushort VoltageA
        {
            get
            {
                return voltageA;
            }

            set
            {
                if (this.voltageA != value)
                {
                    this.voltageA = value;
                    OnPropertyChanged("VoltageA");
                }
            }
        }

        public ushort VoltageB
        {
            get
            {
                return voltageB;
            }

            set
            {
                if (this.voltageB != value)
                {
                    this.voltageB = value;
                    OnPropertyChanged("VoltageB");
                }
            }
        }

        public ushort VoltageC
        {
            get
            {
                return voltageC;
            }

            set
            {
                if (this.voltageC != value)
                {
                    this.voltageC = value;
                    OnPropertyChanged("VoltageC");
                }
            }
        }

        #region Poka ne nyzhno 
        public ushort AmperageA1
        {
            get
            {
                return amperageA1;
            }

            set
            {
                if (this.amperageA1 != value)
                {
                    this.amperageA1 = value;
                    OnPropertyChanged("AmperageA1");
                }
            }
        }

        public ushort AmperageB1
        {
            get
            {
                return amperageB1;
            }

            set
            {
                if (this.amperageB1 != value)
                {
                    this.amperageB1 = value;
                    OnPropertyChanged("AmperageB1");
                }
            }
        }

        public ushort AmperageC1
        {
            get
            {
                return amperageC1;
            }

            set
            {
                if (this.amperageC1 != value)
                {
                    this.amperageC1 = value;
                    OnPropertyChanged("AmperageC1");
                }
            }
        }

        public ushort AmperageA2
        {
            get
            {
                return amperageA1;
            }

            set
            {
                if (this.amperageA2 != value)
                {
                    this.amperageA2 = value;
                    OnPropertyChanged("AmperageA2");
                }
            }
        }

        public ushort AmperageB2
        {
            get
            {
                return amperageB2;
            }

            set
            {
                if (this.amperageB2 != value)
                {
                    this.amperageB2 = value;
                    OnPropertyChanged("AmperageB2");
                }
            }
        }

        public ushort AmperageC2
        {
            get
            {
                return amperageC2;
            }


            set
            {
                if (this.amperageC2 != value)
                {
                    this.amperageC2 = value;
                    OnPropertyChanged("AmperageC2");
                }
            }
        }

        public ushort TemperatureOfTiristor
        {
            get
            {
                return temperatureOfTiristor;
            }

            set
            {
                if (temperatureOfTiristor != value)
                {
                    temperatureOfTiristor = value;
                    OnPropertyChanged("TemperatureOfTiristor");
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
