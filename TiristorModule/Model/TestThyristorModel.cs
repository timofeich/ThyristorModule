using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiristorModule.Model
{
    public class TestThyristorModel
    {
        public ObservableCollection<TestData> Testdata { get; set; } = new ObservableCollection<TestData>();

        
    }
}
