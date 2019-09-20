using TiristorModule.Properties;

namespace TiristorModule.Model
{
    public class SettingsModel
    {
        public byte[] Times 
        {
            get { return BytesManipulating.ConvertStringCollectionToByte(Settings.Default.Time); }
        }

        public byte[] Capacities
        {
            get { return BytesManipulating.ConvertStringCollectionToByte(Settings.Default.Capacity); }
        }

        public byte SlaveAddress
        {
            get { return BytesManipulating.GetAddress(Settings.Default.SlaveAddress); }
        }

        public byte MasterAddress
        {
            get { return BytesManipulating.GetAddress(Settings.Default.MasterAddress); }
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

        public byte NominalTok1sk
        {
            get { return (byte)(Settings.Default.NominalTok1sk / 10); }
        }

        public byte NumberOfTest
        {
            get { return Settings.Default.NumberOfTest; }
        }


        public bool IsRequestSingle
        {
            get { return Settings.Default.IsRequestSingle; }
        }

        public bool IsPlavniiPusk
        {
            get { return Settings.Default.IsPlavniiPusk; }
        }
    }
}
