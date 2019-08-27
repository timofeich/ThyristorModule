using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using TiristorModule.ViewModel;
using System.Windows.Input;
using System.Windows;
using TiristorModule.View;
using System.Linq;
using TiristorModule.Properties;
using TiristorModule.Indicators;
using TiristorModule.Model;
using TiristorModule.Logging;

namespace TiristorModule
{
    class MainWindowViewModel
    {
        public static SerialPort serialPort1 = new SerialPort(Settings.Default.PortName,
            Convert.ToInt32(Settings.Default.BaudRate),
            SerialPortSettings.SetPortParity(Settings.Default.Parity),
            Convert.ToInt32(Settings.Default.DataBit),
            SerialPortSettings.SetStopBits(Settings.Default.StopBit));

        #region Fields
        private const byte AddressStartTiristorModuleCommand = 0x87;
        private const byte AddressStopTiristorModuleCommand = 0x88;
        private const byte AddressCurrentVoltageCommand = 0x90;
        private const byte AddressTestTiristorModuleCommand = 0x91;
        private const byte AddressResetAvariaTiristorCommand = 0x92;
        private const byte AddressAlarmStopCommand = 0x99;

        private const byte AlarmTemperatureTiristor = 85;

        private static Dictionary<int, string> WorkingStatus = new Dictionary<int, string>(4);

        private static int standartRequest = 0;
        private static int startRequest = 1;
        private static int testRequest = 2;

        private static bool IsCurrentVoltageRequestCyclical = true;

        #endregion

        #region Properties
        public static DataModel Data { get; set; }
        public static LedIndicatorModel LedIndicatorData { get; set; }
        public static SettingsModel SettingsModelData { get; set; }
        #endregion

        #region Commands

        public ICommand CurrentVoltageCommand { get; set; }
        public ICommand AlarmStopCommand { get; set; }
        public ICommand TestTerristorModuleCommand { get; set; }
        public ICommand StartTerristorModuleCommand { get; set; }
        public ICommand StopTerristorModuleCommand { get; set; }
        public ICommand ResetAvatiaTirristorCommand { get; set; }

        public ICommand ConnectionSettingsCommand { get; set; }
        public ICommand StartTiristorSettingsCommand { get; set; }
        public ICommand TestTiristorSettingsCommand { get; set; }
        public ICommand CycleModeCommand { get; set; }

        #endregion

        public MainWindowViewModel()
        {
            CurrentVoltageCommand = new Command(arg => CurrentVoltageClick());
            AlarmStopCommand = new Command(arg => AlarmStopClick());
            TestTerristorModuleCommand = new Command(arg => TestTerristorModuleClick());
            StartTerristorModuleCommand = new Command(arg => StartTerristorModuleClick());
            StopTerristorModuleCommand = new Command(arg => StopTerristorModuleClick());
            ResetAvatiaTirristorCommand = new Command(arg => ResetAvatiaTirristorClick());

            ConnectionSettingsCommand = new Command(arg => ConnectionSettingsClick());
            StartTiristorSettingsCommand = new Command(arg => StartTiristorSettingsClick());
            TestTiristorSettingsCommand = new Command(arg => TestTiristorSettingsClick());

            CycleModeCommand = new Command(arg => SetRequestMode());

            InitializeWorkingStatusData();

            Data = new DataModel
            {
                WorkingStatus = null
            };

            LedIndicatorData = new LedIndicatorModel
            {

            };

            SettingsModelData = new SettingsModel
            {

            };

            Logger.InitLogger();
        }

        ~MainWindowViewModel()
        {
            SerialPortSettings.CloseSerialPortConnection(serialPort1);
        }

        private static void InitializeWorkingStatusData()
        {
            WorkingStatus.Add(0, "Crach_ostanov");
            WorkingStatus.Add(1, "Tormoz");
            WorkingStatus.Add(2, "Baipass");
            WorkingStatus.Add(3, "Razgon");
            WorkingStatus.Add(4, "Дежурный режим" );
        }

        public static byte[] ConvertStringCollectionToByte(System.Collections.Specialized.StringCollection stringCollection)
        {
            string[] stringArray = new string[stringCollection.Count];
            stringCollection.CopyTo(stringArray, 0);

            return stringArray.Select(byte.Parse).ToArray();
        }

        private static byte GetAddress(string address)
        {
            return byte.Parse(address, System.Globalization.NumberStyles.HexNumber);
        }
        #region ClickHandler

        private void StartTerristorModuleClick()
        {
            ChooseRequestMode(AddressStartTiristorModuleCommand, startRequest);
        }

        private void StopTerristorModuleClick()
        {
            ChooseRequestMode(AddressStopTiristorModuleCommand, standartRequest);
        }

        private void CurrentVoltageClick()
        {
            ChooseRequestMode(AddressCurrentVoltageCommand, standartRequest);
        }

        private void AlarmStopClick()
        {
            ChooseRequestMode(AddressAlarmStopCommand, standartRequest);
        }

        private void TestTerristorModuleClick()
        {
            ChooseRequestMode(AddressTestTiristorModuleCommand, testRequest);
        }

        private void ResetAvatiaTirristorClick()
        {
            ChooseRequestMode(AddressResetAvariaTiristorCommand, standartRequest);
        }

        private void SetRequestMode()
        {
            ChooseRequestMode(AddressResetAvariaTiristorCommand, standartRequest);
        }

        private static void TestThyristorWindowShow(ushort[] buff)
        {
            var vm = new TestThyristorWindowViewModel(buff);
            var connectSettingView = new TestThyristorWindowView
            {
                DataContext = vm
            };
            vm.OnRequestClose += (s, e) => connectSettingView.Close();
            connectSettingView.ShowDialog();
        }

        private void ConnectionSettingsClick()
        {
            var vm = new ConnectSettingsViewModel();
            var connectSettingView = new ConnectSettingsView
            {
                DataContext = vm
            };
            vm.OnRequestClose += (s, e) => connectSettingView.Close();
            connectSettingView.ShowDialog();
        }

        private void StartTiristorSettingsClick()
        {
            var vm = new StartTiristorSettingsViewModel();
            var startSettingView = new StartTiristorSettingsView
            {
                DataContext = vm
            };
            vm.OnRequestClose += (s, e) => startSettingView.Close();
            startSettingView.ShowDialog();
        }

        private void TestTiristorSettingsClick()
        {
            var vm = new TestTiristorSettingsViewModel();
            var testSettingView = new TestTiristorSettingsView
            {
                DataContext = vm
            };
            vm.OnRequestClose += (s, e) => testSettingView.Close();
            testSettingView.ShowDialog();
        }
        #endregion

        #region Methods

        public static void ChooseRequestMode(byte AddressCommand, int RequestType)
        {
            if (Data.IsRequestSingle) StartSingleRequest(AddressCommand, RequestType);
            else
            {            
                StartCycleRequest(AddressCommand, RequestType);
                IsCurrentVoltageRequestCyclical = true;
            }
        }

        public static void StartSingleRequest(byte AddressCommand, int RequestType)
        {
            try
            {
                if (AddressCommand == AddressCurrentVoltageCommand)
                {
                    OutputResponceData(AddressCommand, RequestType);
                }
                else
                {
                    OutputResponceData(AddressCommand, RequestType);
                    AddressCommand = AddressCurrentVoltageCommand;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message);
                MessageBox.Show(ex.Message, "Ошибка!");
            }
        }

        public static void StartCycleRequest(byte AddressCommand, int RequestType)
        {
            try
            {
                SerialPortSettings.OpenSerialPortConnection(serialPort1);
                ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
                {
                    while (IsCurrentVoltageRequestCyclical && !Data.IsRequestSingle)
                    {
                        if (AddressCommand == AddressCurrentVoltageCommand) OutputResponceData(AddressCommand, RequestType);
                        else
                        {
                            OutputResponceData(AddressCommand, RequestType);
                            AddressCommand = AddressCurrentVoltageCommand;
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message);
                MessageBox.Show(ex.Message, "Ошибка!");
            }
        }

        public static void OutputResponceData(byte AddressCommand, int RequestType)
        {
            ushort[] buffer = ReadHoldingResponcesFromBuffer(AddressCommand, RequestType);

            if (AddressCommand == AddressTestTiristorModuleCommand)
            {                
                OutputDataFromArrayToTestModel(buffer);
            }
            else
            {
                OutputDataFromArrayToDataModel(buffer);
            }
        }

        private static void OutputDataFromArrayToTestModel(ushort[] buff)//wich status will open thyristor module
        {
            try
            {
                IsCurrentVoltageRequestCyclical = false;
                LedIndicatorData.TestingStatus = IndicatorColor.GetTestingStatusLEDColor(buff[23]);
                TestThyristorWindowShow(buff);
            }
            catch (Exception)
            {
                Logger.Log.Error("Невозможно отобразить тестовые данные." + "Пришёл неверный статус.");
                MessageBox.Show("Невозможно отобразить тестовые данные." + "\n" + "Пришёл неверный статус.", "Ошибка!");
            }
        }

        private static void OutputDataFromArrayToDataModel(ushort[] buff)//wich status will open thyristor module
        {
            try
            {
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
                IsCurrentVoltageRequestCyclical = false;
                Logger.Log.Error("Нет данных для вывода, отправка запросов остановлена.");
                MessageBox.Show("Нет данных для вывода, отправка запросов остановлена.", "Ошибка!");
            }
        }

        //standart request - zapros_cur_voltage, stop_Tiristor_module, reset_avaria_tir, avarinii_stop
        private static byte[] CreateStandartRequest(byte slaveAddress, byte commandNumber)
        {
            byte[] frame = new byte[3];
            byte[] frameout = new byte[4];
            frame[0] = slaveAddress;
            frame[1] = commandNumber;
            frame[2] = 0x00;
            Array.Copy(frame, frameout, frame.Length);
            frameout[3] = CalculateCRC8(frame);

            return frameout;
        }

        private static byte[] CreateStartTiristorModuleRequest(byte slaveAddress)
        {
            byte[] frame = new byte[31];
            byte[] frameout = new byte[32];
            byte[] times = ConvertStringCollectionToByte(SettingsModelData.Times);
            byte[] capacities = ConvertStringCollectionToByte(SettingsModelData.Capacities);


            frame[0] = slaveAddress;
            frame[1] = AddressStartTiristorModuleCommand;
            frame[2] = 28;//quantityofbyte

            for (int i = 0; i < times.Length; i++)
            {
                frame[i + 3] = times[i];
            }

            for (int i = 0; i < times.Length; i++)
            {
                frame[i + 12] = capacities[i];
            }

            frame[21] = Convert.ToByte(SettingsModelData.CurrentKz1 >> 8);
            frame[22] = Convert.ToByte(SettingsModelData.CurrentKz1 ^ 0x100);
            frame[23] = SettingsModelData.VremiaKzMs1;
            frame[24] = 0;//kz_on_off_1
            frame[25] = Convert.ToByte(SettingsModelData.CurrentKz2 >> 8);
            frame[26] = Convert.ToByte(SettingsModelData.CurrentKz2 ^ 0x100);
            frame[27] = SettingsModelData.VremiaKzMs2;
            frame[28] = 0;//kz_on_off_2
            frame[29] = AlarmTemperatureTiristor;
            frame[30] = Convert.ToByte(Data.IsPlavniiPusk);

            Array.Copy(frame, frameout, frame.Length);
            frameout[frameout.Length - 1] = CalculateCRC8(frame);

            return frameout;
        }

        private static byte[] CreateTestTiristorModuleRequest(byte slaveAddress)
        {
            byte[] frame = new byte[10];
            byte[] frameout = new byte[11];

            frame[0] = slaveAddress;
            frame[1] = AddressTestTiristorModuleCommand;//adressCommand
            frame[2] = 7;//quantityofbyte
            frame[3] = SettingsModelData.PersentTestPower;
            frame[4] = Convert.ToByte(SettingsModelData.NominalTok1sk);
            frame[5] = SettingsModelData.NumberOfTest;
            frame[6] = Convert.ToByte(SettingsModelData.CurrentKz1 >> 8);
            frame[7] = Convert.ToByte(SettingsModelData.CurrentKz1 ^ 0x100);
            frame[8] = Convert.ToByte(SettingsModelData.CurrentKz2 >> 8);
            frame[9] = Convert.ToByte(SettingsModelData.CurrentKz2 ^ 0x100);

            Array.Copy(frame, frameout, frame.Length);
            frameout[frameout.Length - 1] = CalculateCRC8(frame);

            return frameout;
        }

        private static ushort[] ParseTestTirResponse(byte[] data)
        {
            if (data[24] == CalculateCRC8(data)) return BytesManipulating.ConvertByteArrayIntoUshortArray(data);
            else
            {
                Logger.Log.Error("Нарушена целостность пакета.");
                MessageBox.Show("Нарушена целостность пакета.");
                return null;
            }
        }

        private static ushort[] ParseCurrentVoltageResponse(byte[] data)
        {            
            if (data[25] == CalculateCRC8(data))
            {
                ushort[] frame = new ushort[16];
                int j = 4;

                for (int i = 0; i <= frame.Length; i++)
                {
                    if (i < 4 && i >= 13)
                    {
                        frame[i] = data[i];
                    }
                    else 
                    {
                        frame[i] = BytesManipulating.FromBytes(data[j + 1], data[j]);
                        j += 2;
                    }
                }
                return frame;
            } 
            else
            {
                Logger.Log.Error("Нарушена целостность пакета.");
                MessageBox.Show("Нарушена целостность пакета.");
                return null;
            }
        }

        private static ushort[] ReadHoldingResponcesFromBuffer(byte commandNumber, int requestType)
        {
            ushort[] BuffResponce = null;
            byte SlaveAddress = byte.Parse(Settings.Default.AddressSlave, System.Globalization.NumberStyles.HexNumber);
            byte[] writebuffer;

            if (serialPort1.IsOpen)
            {
                writebuffer = GetFrameDependentOnTypeOfRequest(requestType, commandNumber);
                Logger.Log.Debug("Запрос " + BitConverter.ToString(writebuffer));
                serialPort1.Write(writebuffer, 0, writebuffer.Length);

                Thread.Sleep(SettingsModelData.RequestInterval);

                if (serialPort1.BytesToRead >= 20)
                {
                    byte[] bufferReceiver = new byte[serialPort1.BytesToRead];
                    serialPort1.Read(bufferReceiver, 0, serialPort1.BytesToRead);
                    serialPort1.DiscardInBuffer();
                    Logger.Log.Debug("Ответ " + BitConverter.ToString(bufferReceiver));

                    if (bufferReceiver[3] == 0x91 && bufferReceiver[0] == GetAddress(SettingsModelData.AddressMaster) &&
                                                     bufferReceiver[1] == GetAddress(SettingsModelData.AddressSlave))//уведомить пользователя о неверном адресе
                        BuffResponce = ParseTestTirResponse(bufferReceiver);

                    else if (bufferReceiver[3] == 0x90 && bufferReceiver[0] == GetAddress(SettingsModelData.AddressMaster) &&
                                                          bufferReceiver[1] == GetAddress(SettingsModelData.AddressSlave))
                        BuffResponce = ParseCurrentVoltageResponse(bufferReceiver);
                    if (BuffResponce == null) MessageBox.Show("Модуль тиристора отправил нулевой ответ.", "Ошибка!");
                }
                else
                {
                    Logger.Log.Debug("Ответ отсутствует ");
                    MessageBox.Show("Модуль тиристора ответа не дал.", "Ошибка!");
                }
            }
            return BuffResponce;
        }

        private static byte[] GetFrameDependentOnTypeOfRequest(int requestType, byte commandNumber)
        {
            byte addressSlave = GetAddress(SettingsModelData.AddressSlave);

            if (requestType == standartRequest) return CreateStandartRequest(addressSlave, commandNumber);
            else if (requestType == startRequest) return CreateStartTiristorModuleRequest(addressSlave);
            else return CreateTestTiristorModuleRequest(addressSlave);
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

        private static byte CalculateCRC8(byte[] array)
        {
            byte crc = 0xFF;

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
        #endregion
    }
}
