using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace TiristorModule
{
    class TiristorModuleViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Indicator selectedIndicator;

        public ObservableCollection<Indicator> Indicators { get; set; }

        public Indicator SelectedIndicator
        {
            get { return selectedIndicator; }
            set
            {
                selectedIndicator = value;
                OnPropertyChanged("SelectedIndicator");
            }
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
