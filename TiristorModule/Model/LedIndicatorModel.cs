using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TiristorModule.Model
{
    class LedIndicatorModel : INotifyPropertyChanged
    {
        public List<Color> ColorList { get; } = new List<Color>() { Colors.Red, Colors.Green, Colors.Gray };
        private bool? a1_kz;
        private bool? b1_kz;
        private bool? c1_kz;
        private bool? a2_kz;
        private bool? b2_kz;
        private bool? c2_kz;

        private bool? testingStatus;
        private bool? startStatus;
        private bool? stopStatus;

        private Color coloreOff;
        private bool flash;

        public Color ColorOn { get; } = Colors.Green;
        public Color ColorOff { get; } = Colors.Red;

        public bool? A1_kz
        {
            get { return a1_kz; }
            set
            {
                a1_kz = value;
                this.OnPropertyChanged("A1_kz");
            }
        }
        
        public bool? B1_kz
        {
            get { return b1_kz; }
            set
            {
                b1_kz = value;
                this.OnPropertyChanged("B1_kz");
            }
        }
        
        public bool? C1_kz
        {
            get { return c1_kz; }
            set
            {
                c1_kz = value;
                this.OnPropertyChanged("C1_kz");
            }
        }
        
        public bool? A2_kz
        {
            get { return a2_kz; }
            set
            {
                a2_kz = value;
                this.OnPropertyChanged("A2_kz");
            }
        }
        
        public bool? B2_kz
        {
            get { return b2_kz; }
            set
            {
                b2_kz = value;
                this.OnPropertyChanged("B2_kz");
            }
        }

        public bool? C2_kz
        {
            get { return c2_kz; }
            set
            {
                c2_kz = value;
                this.OnPropertyChanged("C2_kz");
            }
        }

        public bool? TestingStatus
        {
            get { return testingStatus; }
            set
            {
                testingStatus = value;
                this.OnPropertyChanged("TestingStatus");
            }
        }

        public bool? StartStatus
        {
            get { return startStatus; }
            set
            {
                if (startStatus != value)
                {
                    startStatus = value;
                    OnPropertyChanged("StartStatus");
                }
            }
        }

        public bool? StopStatus
        {
            get { return stopStatus; }
            set
            {
                if (stopStatus != value)
                {
                    stopStatus = value;
                    OnPropertyChanged("StopStatus");
                }
            }
        }

        public Color ColoreOff
        {
            get { return coloreOff; }
            set
            {
                coloreOff = value;
                this.OnPropertyChanged("ColoreOff");
            }
        }

        public bool Flash
        {
            get { return flash; }
            set
            {
                flash = value;
                this.OnPropertyChanged("Flash");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
