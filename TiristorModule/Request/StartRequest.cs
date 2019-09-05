using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiristorModule.Model;
using TiristorModule.Properties;

namespace TiristorModule.Request
{
    public class StartRequest : BaseRequest
    {
        private byte[] Times { get; }
        private byte[] Capacities { get; }

        private byte SlaveAddress { get; }
        private byte Command { get; }
        private byte TotalBytes { get; }

        private ushort CurrentKz1;
        private ushort CurrentKz2;

        private byte KzTimeMs1;
        private byte KzTimeMs2;

        private byte KzOnOff1;
        private byte KzOnOff2;

        private byte AlarmThyristorTemperature;

        private byte PlavniiPuskStart;

        private byte CRC8
        {
            get { return CalculateCRC8(GetRequestWithoutCRC8()); }
        }

        public StartRequest(byte Command, byte TotalBytes,
                            byte KzOnOff1, ushort CurrentKz2, byte KzTimeMs2, byte KzOnOff2,
                            byte AlarmThyristorTemperature, byte PlavniiPuskStart)
        {
            SlaveAddress = BytesManipulating.GetAddress(Settings.Default.SlaveAddress); 
            this.Command = Command;
            this.TotalBytes = TotalBytes;

            Times = BytesManipulating.ConvertStringCollectionToByte(Settings.Default.Time);
            Capacities = BytesManipulating.ConvertStringCollectionToByte(Settings.Default.Capacity);

            CurrentKz1 = Settings.Default.CurrentKz1;
            KzTimeMs1 = Settings.Default.VremiaKzMs1;
            this.KzOnOff1 = KzOnOff1;

            this.CurrentKz2 = CurrentKz2;
            this.KzTimeMs2 = KzTimeMs2;
            this.KzOnOff2 = KzOnOff2;

            this.AlarmThyristorTemperature = AlarmThyristorTemperature;
            this.PlavniiPuskStart = PlavniiPuskStart;
        }

        public byte[] GetRequestPackage()
        {
            List<byte> RequestWithoutCRC8 = GetRequestWithoutCRC8();
            RequestWithoutCRC8.Add(CRC8);
            return RequestWithoutCRC8.ToArray();
        }

        private List<byte> GetRequestWithoutCRC8()
        {
            List<byte> Request = new List<byte>() { SlaveAddress, Command, TotalBytes };

            Request.AddRange(Times);
            Request.AddRange(Capacities);

            Request.Add(Convert.ToByte(CurrentKz1 >> 8));
            Request.Add(Convert.ToByte(CurrentKz1 ^ 0x100));

            Request.Add(KzTimeMs1);
            Request.Add(KzOnOff1);

            Request.Add(Convert.ToByte(CurrentKz2 >> 8));
            Request.Add(Convert.ToByte(CurrentKz2 ^ 0x100));

            Request.Add(KzTimeMs2);
            Request.Add(KzOnOff2);

            Request.Add(AlarmThyristorTemperature);
            Request.Add(PlavniiPuskStart);

            return Request;
        }
    }
}
