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
        public StringCollection Times 
        {
            get { return Settings.Default.Time; }
        }

        public StringCollection Capacities
        {
            get { return Settings.Default.Capacity; }
        }

        public string AddressSlave
        {
            get { return Settings.Default.AddressSlave; }
        }

        public string AddressMaster
        {
            get { return Settings.Default.AddressMaster; }
        }

        public int RequestInterval
        {
            get { return Settings.Default.RequestInterval; }
        }

        public byte VremiaKzMs1
        {
            get { return Settings.Default.VremiaKzMs1; }
        }

        public byte VremiaKzMs2
        {
            get { return Settings.Default.VremiaKzMs2; }
        }

        public ushort CurrentKz1
        {
            get { return Settings.Default.CurrentKz1; }
        }

        public ushort CurrentKz2
        {
            get { return Settings.Default.CurrentKz2; }
        }

        public byte PersentTestPower
        {
            get { return Settings.Default.PersentTestPower; }
        }

        public int NominalTok1sk
        {
            get { return Settings.Default.NominalTok1sk / 10; }
        }

        public byte NumberOfTest
        {
            get { return Settings.Default.NumberOfTest; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
