using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiristorModule.Request
{
    public class BaseRequest
    {
        private byte RequestAddress { get; set; }

        private StandartRequest CurrentVoltage;
        private StandartRequest StopThyristorModule;
        private StandartRequest ResetThyristorCrash;
        private StandartRequest AlarmStop;
        private StartRequest StartThyristorModule;
        private TestRequest TestThyristorModule;

        public BaseRequest(byte RequestAddress)
        {
            this.RequestAddress = RequestAddress;
        }

        private void IdentifyRequestbyAddress()
        {
            switch (RequestAddress)
            {
                case MainWindowViewModel.CurrentVoltageRequestID:
                    CurrentVoltage.StandartRequest();
                    break;
                case MainWindowViewModel.StopThyristorModuleRequestID:
                    StopThyristorModule.StandartRequest();
                    break;
                case MainWindowViewModel.ResetThyristorCrashRequestID:
                    ResetThyristorCrash.StandartRequest();
                    break;
                case MainWindowViewModel.AlarmStopRequestID:
                    AlarmStop.StandartRequest();
                    break;
                case MainWindowViewModel.StartThyistorModuleRequestID:
                    StartThyristorModule.StartThyristorModule();
                    break;
                case MainWindowViewModel.TestThyristorModuleRequestID:
                    TestThyristorModule.TestThyristorModule();
                    break;
            }
        }

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
