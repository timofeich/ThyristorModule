using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
        private ushort workingStatus;
        private ushort opredelenieFazRevers;


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
                    OnPropertyChanged("Voltage");
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

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
