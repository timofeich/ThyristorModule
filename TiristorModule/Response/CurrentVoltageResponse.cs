using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TiristorModule.Indicators;
using TiristorModule.Logging;
using TiristorModule.Model;

namespace TiristorModule.Response
{
    public class CurrentVoltageResponse
    {
        public int CurrentVoltageResponseLength = 26;
        byte[] Response;

        private byte CRC8
        {
            get { return CalculateCRC8(); }
        }

        public CurrentVoltageResponse()
        {

        }

        public ushort[] ParseCurrentVoltageResponse(byte[] Response)
        {
            this.Response = Response;
            if (IsCRC8Correct())
            {
                ushort[] frame = new ushort[16];
                int j = 4;

                for (int i = 0; i < frame.Length; i++)
                {
                    if (i < 4)
                    {
                        frame[i] = Response[i];
                    }
                    else if(i < 13)
                    {
                        frame[i] = BytesManipulating.FromBytes(Response[j + 1], Response[j]);
                        j += 2;
                    }
                    else
                    {
                        frame[i] = Response[j];
                        j++;
                    }
                }
                return frame;
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
            ResponseList.RemoveAt(CurrentVoltageResponseLength - 1);
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
