using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TiristorModule.Model;

namespace TiristorModule.ViewModel
{
    public class TestThyristorWindowViewModel:INotifyPropertyChanged
    {
        private ObservableCollection<TestThyristorModel> testThyristorModels;
        public event EventHandler OnRequestClose;
        public event PropertyChangedEventHandler PropertyChanged;
        public static Dictionary<int, string> FazaName = new Dictionary<int, string>(3);

        public ObservableCollection<TestThyristorModel> TestDatas
        {
            get { return testThyristorModels; }
            set { testThyristorModels = value; OnPropertyChanged(); }
        }

        public TestThyristorWindowViewModel(ushort[] buff)
        {
            TestDatas = new ObservableCollection<TestThyristorModel>();
            InitializeFazzNameData();

            for(int i = 0; i < 3; i++)
            TestDatas.Add(new TestThyristorModel(FazaName[i], buff[i], buff[i + 3], buff[i + 6], buff[i + 9], 
                buff[i + 12], buff[i + 15], buff[17]));
        }

        private static void InitializeFazzNameData()
        {
            FazaName.Add(0, "Фаза A:");
            FazaName.Add(1, "Фаза B:");
            FazaName.Add(2, "Фаза C:");
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
