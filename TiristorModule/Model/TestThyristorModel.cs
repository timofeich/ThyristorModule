namespace TiristorModule.Model
{
    public class TestThyristorModel
    {
        private string FazaName { get; set; }
        private ushort ApBn { get; set; }
        private ushort BpAn { get; set; }
        private ushort CpAn { get; set; }
        private ushort ApCn { get; set; }
        private ushort BpCn { get; set; }
        private ushort CpBn { get; set; }
        private ushort OpredelenieFazz { get; set; }

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
