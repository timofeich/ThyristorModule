using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiristorModule.Model
{
    public class TestData : INotifyPropertyChanged
    {
        private List<TestData> _items;

        public TestData(ushort apBn, ushort bpAn, ushort cpAn)
        {
            ApBn = apBn;
            BpAn = bpAn;
            CpAn = cpAn;
        } 

        public ushort ApBn { get; set; }
        public ushort BpAn { get; set; }
        public ushort CpAn { get; set; }
        public ushort ApCn { get; set; }
        public ushort BpCn { get; set; }
        public ushort CpBn { get; set; }
        public ushort OpredelenieFazz { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }


    }
}
