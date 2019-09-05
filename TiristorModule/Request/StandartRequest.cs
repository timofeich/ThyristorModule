using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiristorModule.Model;
using TiristorModule.Properties;

namespace TiristorModule.Request
{
    public class StandartRequest : BaseRequest
    {
        private byte SlaveAddress { get; set; }
        private byte Command { get; set; }
        private byte TotalBytes { get; set; }

        private byte CRC8
        {
            get { return CalculateCRC8(GetRequestWithoutCRC8()); }
        }

        public StandartRequest(byte Command, byte TotalBytes)
        {
            SlaveAddress = BytesManipulating.GetAddress(Settings.Default.SlaveAddress);
            this.Command = Command;
            this.TotalBytes = TotalBytes;
        }

        public byte[] GetRequestPackage()
        {
            List<byte> RequestWithoutCRC8 = GetRequestWithoutCRC8();
            RequestWithoutCRC8.Add(CRC8);
            return RequestWithoutCRC8.ToArray();
        }

        public List<byte> GetRequestWithoutCRC8()
        {
            List<byte> Request = new List<byte>() { SlaveAddress, Command, TotalBytes };
            return Request;
        }
    }
}
