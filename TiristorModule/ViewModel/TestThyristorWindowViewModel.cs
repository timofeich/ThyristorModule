using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TiristorModule.Model;

namespace TiristorModule.ViewModel
{
    public class TestThyristorWindowViewModel : INotifyPropertyChanged
    {
        private bool _firstColumnChecked;
        private List<TestData> _items;
        public event EventHandler OnRequestClose;

        public TestThyristorWindowViewModel()
        {
            _items = new List<TestData>
         {
            new TestData(1, 2, 3),
            new TestData(4, 5, 6),
         };
        }

        public List<TestData> TestDatas
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged(nameof(TestDatas));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {

            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
