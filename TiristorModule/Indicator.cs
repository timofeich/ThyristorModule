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
        private double amperageA1;
        private double amperageB1;
        private double amperageC1;
        private double amperageA2;
        private double amperageB2;
        private double amperageC2;
        private double voltageA1;
        private double voltageA2;
        private double voltageA3;
        private int tiristorTemperature;
        private string tiristorStatus;

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
                amperageA1 = value;
                OnPropertyChanged("AmperageB1");
            }
        }

        public double AmperageC1
        {
            get { return amperageC1; }
            set
            {
                amperageA1 = value;
                OnPropertyChanged("AmperageC1");
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
