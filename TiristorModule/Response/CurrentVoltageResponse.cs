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
        private static LedIndicatorModel LedIndicatorData { get; set; }
        private static Dictionary<int, string> WorkingStatus = new Dictionary<int, string>(4);

        private byte CRC8
        {
            get { return CalculateCRC8(); }
        }

        public CurrentVoltageResponse()
        {
            LedIndicatorData = new LedIndicatorModel { };
            InitializeWorkingStatusData();
        }

        public CurrentVoltageResponse(byte[] Response)
        {
            this.Response = Response;
            ParseCurrentVoltageResponse();
        }

        private static void InitializeWorkingStatusData()
        {
            WorkingStatus.Add(0, "Crach_ostanov");
            WorkingStatus.Add(1, "Tormoz");
            WorkingStatus.Add(2, "Baipass");
            WorkingStatus.Add(3, "Razgon");
            WorkingStatus.Add(4, "Дежурный режим");
        }

        private void ParseCurrentVoltageResponse()
        {
            if (IsCRC8Correct())
            {
                ushort[] frame = new ushort[16];
                int j = 4;

                for (int i = 0; i < frame.Length; i++)
                {
                    if (i < 4 || i >= 13)
                    {
                        frame[i] = Response[i];
                    }
                    else
                    {
                        frame[i] = BytesManipulating.FromBytes(Response[j + 1], Response[j]);
                        j += 2;
                    }
                }
                OutputDataFromArrayToDataModel(frame);
            }
            else
            {
                MessageBox.Show("Нарушена целостность пакета.");
                return ;
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

        private static void OutputDataFromArrayToDataModel(ushort[] buff)//wich status will open thyristor module
        {

            try
            {
                DataModel Data = new DataModel();
                Data.VoltageA = buff[4];
                Data.VoltageB = buff[5];
                Data.VoltageC = buff[6];
                Data.AmperageA1 = buff[7];
                Data.AmperageB1 = buff[8];
                Data.AmperageC1 = buff[9];
                Data.AmperageA2 = buff[10];
                Data.AmperageB2 = buff[11];
                Data.AmperageC2 = buff[12];
                Data.TemperatureOfTiristor = buff[13];
                Data.WorkingStatus = GetWorkingStatus(buff[14]);
                if (buff[14] == 128 || buff[14] == 1)
                {
                    LedIndicatorData.StartStatus = IndicatorColor.GetTestingStatusLEDColor(1);
                    LedIndicatorData.StopStatus = IndicatorColor.GetTestingStatusLEDColor(0);
                }
                else
                {
                    LedIndicatorData.StartStatus = IndicatorColor.GetTestingStatusLEDColor(0);
                    LedIndicatorData.StopStatus = IndicatorColor.GetTestingStatusLEDColor(1);
                }

                GetStatusFromCurrentVoltage(buff[15]);

                Logger.Log.Info(buff.Skip(4).Take(buff.Length - 2).ToArray());
            }
            catch
            {
                Logger.Log.Error("Нет данных для вывода, отправка запросов остановлена.");
                MessageBox.Show("Нет данных для вывода, отправка запросов остановлена.", "Ошибка!");
            }
        }

        private static string GetWorkingStatus(ushort statusByte)
        {
            if (statusByte % 16 == 0 && statusByte != 0)
            {
                int i = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(statusByte)) - 4);
                return WorkingStatus[i];
            }
            else return WorkingStatus[4];
        }

        private static void GetStatusFromCurrentVoltage(ushort statusCrash)//try loop and list
        {
            switch (statusCrash)
            {
                case 0:
                    LedIndicatorData.A1_kz = true;
                    LedIndicatorData.B1_kz = true;
                    LedIndicatorData.C1_kz = true;
                    LedIndicatorData.A2_kz = true;
                    LedIndicatorData.B2_kz = true;
                    LedIndicatorData.C2_kz = true;
                    break;

                case 1:
                    LedIndicatorData.A1_kz = false;
                    LedIndicatorData.B1_kz = true;
                    LedIndicatorData.C1_kz = true;
                    LedIndicatorData.A2_kz = true;
                    LedIndicatorData.B2_kz = true;
                    LedIndicatorData.C2_kz = true;
                    break;

                case 2:
                    LedIndicatorData.A1_kz = true;
                    LedIndicatorData.B1_kz = false;
                    LedIndicatorData.C1_kz = true;
                    LedIndicatorData.A2_kz = true;
                    LedIndicatorData.B2_kz = true;
                    LedIndicatorData.C2_kz = true;
                    break;

                case 4:
                    LedIndicatorData.A1_kz = true;
                    LedIndicatorData.B1_kz = true;
                    LedIndicatorData.C1_kz = false;
                    LedIndicatorData.A2_kz = true;
                    LedIndicatorData.B2_kz = true;
                    LedIndicatorData.C2_kz = true;
                    break;

                case 8:
                    LedIndicatorData.A1_kz = true;
                    LedIndicatorData.B1_kz = true;
                    LedIndicatorData.C1_kz = true;
                    LedIndicatorData.A2_kz = false;
                    LedIndicatorData.B2_kz = true;
                    LedIndicatorData.C2_kz = true;
                    break;

                case 16:
                    LedIndicatorData.A1_kz = true;
                    LedIndicatorData.B1_kz = true;
                    LedIndicatorData.C1_kz = true;
                    LedIndicatorData.A2_kz = true;
                    LedIndicatorData.B2_kz = false;
                    LedIndicatorData.C2_kz = true;
                    break;

                case 32:
                    LedIndicatorData.A1_kz = true;
                    LedIndicatorData.B1_kz = true;
                    LedIndicatorData.C1_kz = true;
                    LedIndicatorData.A2_kz = true;
                    LedIndicatorData.B2_kz = true;
                    LedIndicatorData.C2_kz = false;
                    break;

                case 64:
                    LedIndicatorData.A1_kz = false;
                    LedIndicatorData.B1_kz = false;
                    LedIndicatorData.C1_kz = false;
                    LedIndicatorData.A2_kz = false;
                    LedIndicatorData.B2_kz = false;
                    LedIndicatorData.C2_kz = false;
                    break;
            }
            //        if (bufferReceiver[3] == 0x91 && bufferReceiver[0] == GetAddress(SettingsModelData.AddressMaster) &&
            //                                         bufferReceiver[1] == GetAddress(SettingsModelData.AddressSlave))//уведомить пользователя о неверном адресе
            //            BuffResponce = ParseTestTirResponse(bufferReceiver);

            //        else if (bufferReceiver[3] == 0x90 && bufferReceiver[0] == GetAddress(SettingsModelData.AddressMaster) &&
            //                                              bufferReceiver[1] == GetAddress(SettingsModelData.AddressSlave))
            //            BuffResponce = ParseCurrentVoltageResponse(bufferReceiver);
            //        if (BuffResponce == null) MessageBox.Show("Модуль тиристора отправил нулевой ответ.", "Ошибка!");
            //    }
            //    else
            //    {
            //        Logger.Log.Debug("Ответ отсутствует ");
            //        MessageBox.Show("Модуль тиристора ответа не дал.", "Ошибка!");
            //    }
            //}
            //return BuffResponce;
        }
    }
}
