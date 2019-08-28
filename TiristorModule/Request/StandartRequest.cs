using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiristorModule.Model;

namespace TiristorModule.Request
{
    public class StandartRequest
    {
        private byte CommandNumber;
        private byte TotalBytes;

        //standart request - zapros_cur_voltage, stop_Tiristor_module, reset_avaria_tir, avarinii_stop
        private static SettingsModel SettingsModelData { get; set; }      

        private byte CRC8
        {
            get { return CalculateCRC8(); }
        }

        public StandartRequest(byte CommandNumber, byte TotalBytes)
        {
            this.CommandNumber = CommandNumber;
            this.TotalBytes = TotalBytes;
            SettingsModelData = new SettingsModel { };
        }

        public byte[] GetRequestPackage()
        {
            List<byte> RequestWithoutCRC8 = GetRequestWithoutCRC8();
            RequestWithoutCRC8.Add(CRC8);
            return RequestWithoutCRC8.ToArray<byte>();
        }

        private List<byte> GetRequestWithoutCRC8()
        {
            List<byte> Request = new List<byte>()
            {
                byte.Parse(SettingsModelData.AddressSlave, System.Globalization.NumberStyles.HexNumber),
                CommandNumber,
                TotalBytes
            };            

            return Request;
        }

        private byte CalculateCRC8()
        {
            byte crc = 0xFF;
            byte[] array = GetRequestWithoutCRC8().ToArray<byte>();

            foreach (byte b in array)
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
