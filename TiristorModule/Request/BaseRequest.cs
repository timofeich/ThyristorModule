using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiristorModule.Request
{
    public class BaseRequest
    {
        protected byte AddressSlave { get; set; }
        protected byte Command { get; set; }
        protected byte TotalBytes { get; set; }

        protected byte CalculateCRC8(List<byte> requestWithoutCRC8)
        {
            byte crc = 0xFF;

            foreach (byte b in requestWithoutCRC8)
            {
                crc ^= b;

                for (int i = 0; i < 8; i++)
                {
                    crc = (crc & 0x80) != 0 ? (byte)((crc << 1) ^ 0x31) : (byte)(crc << 1);
                }
            }

            return crc;
        }
    }
}
