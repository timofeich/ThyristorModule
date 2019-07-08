using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TiristorModule
{
    public class Indicator: INotifyPropertyChanged
    {
        #region Indicator items
        private double amperageA1;
        private double amperageB1;
        private double amperageC1;
        private double amperageA2;
        private double amperageB2;
        private double amperageC2;
        private double voltageA;
        private double voltageB;
        private double voltageC;
        private int tiristorTemperature;
        private string tiristorStatus;
        #endregion

        public double AmperageA1
        {
            get { return amperageA1; }
            set
            {
                amperageA1 = value;
                OnPropertyChanged("AmperageA1");
            }
        }

        public double AmperageB1
        {
            get { return amperageB1; }
            set
            {
                amperageB1 = value;
                OnPropertyChanged("AmperageB1");
            }
        }

        public double AmperageC1
        {
            get { return amperageC1; }
            set
            {
                amperageC1 = value;
                OnPropertyChanged("AmperageC1");
            }
        }

        public double AmperageA2
        {
            get { return amperageA2; }
            set
            {
                amperageA2 = value;
                OnPropertyChanged("AmperageA2");
            }
        }

        public double AmperageB2
        {
            get { return amperageB2; }
            set
            {
                amperageB2 = value;
                OnPropertyChanged("AmperageB2");
            }
        }

        public double AmperageC2
        {
            get { return amperageC2; }
            set
            {
                amperageC2 = value;
                OnPropertyChanged("AmperageC2");
            }
        }

        public double VoltageA
        {
            get { return voltageA; }
            set
            {
                voltageA = value;
                OnPropertyChanged("VoltageA");
            }
        }

        public double VoltageB
        {
            get { return voltageB; }
            set
            {
                voltageB = value;
                OnPropertyChanged("VoltageB");
            }
        }

        public double VoltageC
        {
            get { return voltageC; }
            set
            {
                voltageC = value;
                OnPropertyChanged("VoltageC");
            }
        }

        public int TiristorTemperature
        {
            get { return tiristorTemperature; }
            set
            {
                tiristorTemperature = value;
                OnPropertyChanged("TiristorTemperature");
            }
        }

        public string TiristorStatus
        {
            get { return tiristorStatus; }
            set
            {
                tiristorStatus = value;
                OnPropertyChanged("TiristorStatus");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
