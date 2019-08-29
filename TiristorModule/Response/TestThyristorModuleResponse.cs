using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TiristorModule.Indicators;
using TiristorModule.Logging;
using TiristorModule.Model;
using TiristorModule.View;
using TiristorModule.ViewModel;

namespace TiristorModule.Response
{
    class TestThyristorModuleResponse
    {
        public int TestThyristorModuleResponseLength = 25;
        byte[] Response;

        private byte CRC8
        {
            get { return CalculateCRC8(); }
        }

        public TestThyristorModuleResponse()
        {

        }

        public TestThyristorModuleResponse(byte[] Response)
        {
            this.Response = Response;
        }

        public ushort[] ParseTestThyristorModuleResponse()
        {
            if (IsCRC8Correct())
            {
                return BytesManipulating.ConvertByteArrayIntoUshortArray(Response);
            }
            else
            {
                MessageBox.Show("Нарушена целостность пакета.");
                return null;
            }
        }

        private List<byte> GetRequestWithoutCRC8()
        {
            List<byte> ResponseList = new List<byte>();
            ResponseList.AddRange(Response);
            ResponseList.RemoveAt(TestThyristorModuleResponseLength - 1);
            return ResponseList;
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

        private bool IsCRC8Correct()
        {
            if (Response[Response.Length - 1] == CRC8) return true;
            else return false;
        }


    }
}
