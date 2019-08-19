using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiristorModule.Properties;

namespace TiristorModule.Model
{
    public class SettingsModel
    {
        private static StringCollection times;
        public StringCollection Times 
        {
            get { return Settings.Default.Time; }
            set
            {
                Settings.Default.Time = times;
                OnPropertyChanged("Times");
            }
        }

        private static StringCollection capacities;
        public StringCollection Capacities
        {
            get { return Settings.Default.Capacity; }
            set
            {
                Settings.Default.Capacity = capacities;
                OnPropertyChanged("Capacities");
            }
        }

        private static string addressSlave;
        public string AddressSlave
        {
            get { return Settings.Default.AddressSlave; }
            set
            {
                Settings.Default.AddressSlave = addressSlave;
                OnPropertyChanged("AddressSlave");
            }
        }

        private static string addressMaster;
        public string AddressMaster
        {
            get { return Settings.Default.AddressMaster; }
            set
            {
                Settings.Default.AddressMaster = addressMaster;
                OnPropertyChanged("AddressMaster");
            }
        }

        private static int requestInterval;
        public int RequestInterval
        {
            get { return Settings.Default.RequestInterval; }
            set
            {
                Settings.Default.RequestInterval = requestInterval;
                OnPropertyChanged("RequestInterval");
            }
        }

        private static byte vremiaKzMs1;
        public byte VremiaKzMs1
        {
            get { return Settings.Default.VremiaKzMs1; }
            set
            {
                Settings.Default.VremiaKzMs1 = vremiaKzMs1;
                OnPropertyChanged("VremiaKzMs1");
            }
        }

        private static byte vremiaKzMs2;
        public byte VremiaKzMs2
        {
            get { return Settings.Default.VremiaKzMs2; }
            set
            {
                Settings.Default.VremiaKzMs2 = vremiaKzMs2;
                OnPropertyChanged("VremiaKzMs2");
            }
        }

        private static ushort currentKz1_1;
        public ushort CurrentKz1
        {
            get { return Settings.Default.CurrentKz1; }
            set
            {
                Settings.Default.CurrentKz1 = currentKz1_1;
                OnPropertyChanged("CurrentKz1");
            }
        }

        private static ushort currentKz2_1;
        public ushort CurrentKz2
        {
            get { return Settings.Default.CurrentKz2; }
            set
            {
                Settings.Default.CurrentKz2 = currentKz2_1;
                OnPropertyChanged("CurrentKz2");
            }
        }

        private static byte persentTestPower;
        public byte PersentTestPower
        {
            get { return Settings.Default.PersentTestPower; }
            set
            {
                Settings.Default.PersentTestPower = persentTestPower;
                OnPropertyChanged("PersentTestPower");
            }
        }

        private static int nominalTok1sk;
        public int NominalTok1sk
        {
            get { return Settings.Default.NominalTok1sk / 10; }
            set
            {
                Settings.Default.NominalTok1sk = nominalTok1sk / 10;
                OnPropertyChanged("NominalTok1sk");
            }
        }

        private static byte numberOfTest;
        public byte NumberOfTest
        {
            get { return Settings.Default.NumberOfTest; }
            set
            {
                Settings.Default.NumberOfTest = numberOfTest;
                OnPropertyChanged("NumberOfTest");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
