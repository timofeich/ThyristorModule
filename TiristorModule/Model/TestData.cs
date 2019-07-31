using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiristorModule.Model
{
    public class TestData
    {
        public TestData() {  } 

        public ushort ApBn { get; set; }
        public ushort BpAn { get; set; }
        public ushort CpAn { get; set; }
        public ushort ApCn { get; set; }
        public ushort BpCn { get; set; }
        public ushort CpBn { get; set; }
        public ushort OpredelenieFazz { get; set; }
    }
}
