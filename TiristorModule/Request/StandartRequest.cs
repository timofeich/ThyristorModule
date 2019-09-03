using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiristorModule.Model;

namespace TiristorModule.Request
{
    public class StandartRequest : Request1
    {
        private byte CRC8
        {
            get { return CalculateCRC8(GetRequestWithoutCRC8()); }
        }

        public StandartRequest(byte AddressSlave, byte Command, byte TotalBytes)
        {
            this.AddressSlave = AddressSlave;
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
            List<byte> Request = new List<byte>() { AddressSlave, Command, TotalBytes };
            return Request;
        }
    }
}
