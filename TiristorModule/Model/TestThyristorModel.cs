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
        public string FazaName { get; set; }
        public ushort ApBn { get; set; }
        public ushort BpAn { get; set; }
        public ushort CpAn { get; set; }
        public ushort ApCn { get; set; }
        public ushort BpCn { get; set; }
        public ushort CpBn { get; set; }
        public ushort OpredelenieFazz { get; set; }

        public TestThyristorModel(string fazaName, ushort apBn, ushort bpAn, ushort cpAn, ushort apCn, ushort bpCn, ushort cpBn, ushort opredelenieFazz)
        {
            FazaName = fazaName;
            ApBn = apBn;
            BpAn = bpAn;
            CpAn = cpAn;
            ApCn = apCn;
            BpCn = bpCn;
            CpBn = cpBn;
            OpredelenieFazz = opredelenieFazz;
        }
    }
}
