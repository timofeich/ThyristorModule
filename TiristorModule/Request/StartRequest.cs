using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiristorModule.Model;

namespace TiristorModule.Request
{
    public class StartRequest : Request1
    {
        private byte[] Times = new byte[9];
        private byte[] Capacities = new byte[9];

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

        public StartRequest(byte AddressSlave, byte Command, byte TotalBytes, byte[] Times, byte[] Capacities,
                            ushort CurrentKz1, byte KzTimeMs1, byte KzOnOff1, ushort CurrentKz2, byte KzTimeMs2, byte KzOnOff2,
                            byte AlarmThyristorTemperature, byte PlavniiPuskStart)
        {
            this.AddressSlave = AddressSlave;
            this.Command = Command;
            this.TotalBytes = TotalBytes;

            this.Times = Times;
            this.Capacities = Capacities;

            this.CurrentKz1 = CurrentKz1;
            this.KzTimeMs1 = KzTimeMs1;
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
            List<byte> Request = new List<byte>() { AddressSlave, Command, TotalBytes };

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
