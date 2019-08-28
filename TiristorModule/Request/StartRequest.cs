using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiristorModule.Model;

namespace TiristorModule.Request
{
    public class StartRequest
    {
        private byte CommandNumber;
        private byte TotalBytes;
        private byte KzOnOff1 = 0;
        private byte KzOnOff2 = 0;
        private byte AlarmTemperatureThyristor = 85;

        private static SettingsModel SettingsModelData { get; set; }
        private static DataModel Data { get; set; }

        private byte CRC8
        {
            get { return CalculateCRC8(); }
        }

        public StartRequest(byte CommandNumber, byte TotalBytes)
        {
            this.CommandNumber = CommandNumber;
            this.TotalBytes = TotalBytes;

            SettingsModelData = new SettingsModel { };
            Data = new DataModel { };
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

            Request.AddRange(ConvertStringCollectionToByte(SettingsModelData.Times));
            Request.AddRange(ConvertStringCollectionToByte(SettingsModelData.Capacities));

            Request.Add(Convert.ToByte(SettingsModelData.CurrentKz1 >> 8));
            Request.Add(Convert.ToByte(SettingsModelData.CurrentKz1 ^ 0x100));

            Request.Add(SettingsModelData.VremiaKzMs1);
            Request.Add(KzOnOff1);

            Request.Add(Convert.ToByte(SettingsModelData.CurrentKz2 >> 8));
            Request.Add(Convert.ToByte(SettingsModelData.CurrentKz2 ^ 0x100));

            Request.Add(SettingsModelData.VremiaKzMs1);
            Request.Add(KzOnOff2);

            Request.Add(AlarmTemperatureThyristor);
            Request.Add(Convert.ToByte(Data.IsPlavniiPusk));

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

        public byte[] ConvertStringCollectionToByte(System.Collections.Specialized.StringCollection stringCollection)
        {
            string[] stringArray = new string[stringCollection.Count];
            stringCollection.CopyTo(stringArray, 0);

            return stringArray.Select(byte.Parse).ToArray();
        }
    }
}
