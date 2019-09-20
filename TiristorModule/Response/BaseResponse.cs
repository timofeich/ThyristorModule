using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TiristorModule.Response
{
    public class BaseResponse
    {
        byte[] Response;

        public byte MasterAddress { get; set; }
        public byte SlaveAddress { get; set; }

        CurrentVoltageResponse CurrentVoltageResponse;
        TestThyristorModuleResponse TestThyristorModuleResponse;

        private byte CRC8
        {
            get { return CalculateCRC8(); }
        }

        public BaseResponse(byte MasterAddress, byte SlaveAddress)
        {
            this.MasterAddress = MasterAddress;
            this.SlaveAddress = SlaveAddress;
            CurrentVoltageResponse = new CurrentVoltageResponse(MasterAddress, SlaveAddress);
            TestThyristorModuleResponse = new TestThyristorModuleResponse(MasterAddress, SlaveAddress);
        }

        public BaseResponse()
        {
        }

        public void GetResponse(byte[] Response)
        {
            this.Response = Response;

            if (IsCRC8Correct()) IdentifyRequestType(); 
            else
                MessageBox.Show("Нарушена целостность пакета.", "Предупреждение", MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
        }

        private void IdentifyRequestType()
        {
            byte CommandTypeByte = 3;

            if (Response[CommandTypeByte] == 0x90)
                CurrentVoltageResponse.GetCurrentVoltageResponse(Response);
            else if(Response[CommandTypeByte] == 0x91)
                TestThyristorModuleResponse.GetTestThyristorModuleResponse(Response);
            else MessageBox.Show("Пришел неверный адрес команды.", "Предупреждение", MessageBoxButton.OK, 
                MessageBoxImage.Warning);
        }

        private List<byte> GetRequestWithoutCRC8()
        {
            List<byte> ResponseList = new List<byte>();
            ResponseList.AddRange(Response);
            ResponseList.RemoveAt(Response.Length - 1);
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
