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
        private static byte SlaveAddress = byte.Parse(Settings.Default.AddressSlave, System.Globalization.NumberStyles.HexNumber);
        private static byte MasterAddress = byte.Parse(Settings.Default.AddressMaster, System.Globalization.NumberStyles.HexNumber);

        private const byte AddressStartTiristorModuleCommand = 0x87;
        private const byte AddressStopTiristorModuleCommand = 0x88;
        private const byte AddressCurrentVoltageCommand = 0x90;
        private const byte AddressTestTiristorModuleCommand = 0x91;
        private const byte AddressResetAvariaTiristorCommand = 0x92;
        private const byte AddressAlarmStopCommand = 0x87;

        private static byte[] Times = ConvertStringCollectionToByte(Settings.Default.Time);
        private static byte[] Capacities = ConvertStringCollectionToByte(Settings.Default.Capacity);

        private const byte AlarmTemperatureTiristor = 85;

        private static byte VremiaKzMs1 = Settings.Default.VremiaKzMs1;
        private static byte VremiaKzMs2 = Settings.Default.VremiaKzMs2;

        private static ushort CurrentKz1_1 = Settings.Default.CurrentKz1;
        private static ushort CurrentKz2_1 = Settings.Default.CurrentKz2;

        private static byte PersentTestPower = Settings.Default.PersentTestPower;
        private static int NominalTok1sk = Settings.Default.NominalTok1sk / 10;
        private static byte NumberOfTest = Settings.Default.NumberOfTest;

        private static int RequestInterval = Settings.Default.RequestInterval;

        private static Dictionary<int, string> WorkingStatus = new Dictionary<int, string>(4);
        private static Dictionary<int, string> LedIndicators = new Dictionary<int, string>(4);
        public static Dictionary<int, bool?> LedIndicatorsList = new Dictionary<int, bool?>(6);

        private static int standartRequest = 0;
        private static int startRequest = 1;
        private static int testRequest = 2;

        private static bool IsCurrentVoltageRequestCyclical = true;
        #endregion

        #region Properties
        public static DataModel Data { get; set; }
        public static LedIndicatorModel LedIndicatorData { get; set; }
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

        #region ClickHandler

        private void StartTerristorModuleClick()
        {
            IsCurrentVoltageRequestCyclical = true;
            ChooseRequestMode(AddressStartTiristorModuleCommand, startRequest);
        }

        private void StopTerristorModuleClick()
        {
            IsCurrentVoltageRequestCyclical = true;
            ChooseRequestMode(AddressStopTiristorModuleCommand, standartRequest);
        }

        private void CurrentVoltageClick()
        {
            IsCurrentVoltageRequestCyclical = true;
            ChooseRequestMode(AddressCurrentVoltageCommand, standartRequest);
        }

        private void AlarmStopClick()
        {
            IsCurrentVoltageRequestCyclical = true;
            ChooseRequestMode(AddressAlarmStopCommand, standartRequest);
        }

        private void TestTerristorModuleClick()
        {
            IsCurrentVoltageRequestCyclical = true;
            ChooseRequestMode(AddressTestTiristorModuleCommand, testRequest);
        }

        private void ResetAvatiaTirristorClick()
        {
            IsCurrentVoltageRequestCyclical = true;
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
            else StartCycleRequest(AddressCommand, RequestType);
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
                    while (IsCurrentVoltageRequestCyclical)
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
                MessageBox.Show(ex.Message, "Ошибка!");
            }
        }

        public static void OutputResponceData(byte AddressCommand, int RequestType)
        {
            if (AddressCommand == AddressTestTiristorModuleCommand)
            {
                ushort[] buffer = ReadHoldingResponcesFromBuffer(AddressCommand, RequestType);
                OutputDataFromArrayToTestModel(buffer, AddressCommand);
            }
            else
            {
                ushort[] buffer = ReadHoldingResponcesFromBuffer(AddressCommand, RequestType);
                OutputDataFromArrayToDataModel(buffer, AddressCommand);
            }
        }

        private static void OutputDataFromArrayToTestModel(ushort[] buff, byte AddressCommand)//wich status will open thyristor module
        {
            try
            {
                LedIndicatorData.TestingStatus = IndicatorColor.GetTestingStatusLEDColor(buff[23]);//исключение на ноль
                TestThyristorWindowShow(buff);
            }
            catch (Exception)
            {
                MessageBox.Show("Невозможно отобразить тестовые данные." + "\n" + "Пришёл неверный статус.", "Ошибка!");
            }
        }

        private static void OutputDataFromArrayToDataModel(ushort[] buff, byte AddressCommand)//wich status will open thyristor module
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
            }
            catch 
            {
                IsCurrentVoltageRequestCyclical = false;
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
            frameout[3] = CalculateCRC8(frame);//0x6e

            return frameout;
        }

        private static byte[] CreateStartTiristorModuleRequest(byte slaveAddress)
        {
            byte[] frame = new byte[31];
            byte[] frameout = new byte[32];

            frame[0] = slaveAddress;
            frame[1] = AddressStartTiristorModuleCommand;//adressCommand
            frame[2] = 28;//quantityofbyte

            for (int i = 0; i < Times.Length; i++)
            {
                frame[i + 3] = Times[i];
            }

            for (int i = 0; i < Times.Length; i++)
            {
                frame[i + 12] = Capacities[i];
            }

            frame[21] = Convert.ToByte(CurrentKz1_1 >> 8);
            frame[22] = Convert.ToByte(CurrentKz1_1 ^ 0x100);
            frame[23] = VremiaKzMs1;
            frame[24] = 0;//kz_on_off_1
            frame[25] = Convert.ToByte(CurrentKz2_1 >> 8);
            frame[26] = Convert.ToByte(CurrentKz2_1 ^ 0x100);
            frame[27] = VremiaKzMs2;
            frame[28] = 0;//kz_on_off_2
            frame[29] = AlarmTemperatureTiristor;
            frame[30] = Convert.ToByte(Data.IsPlavniiPusk);//flag

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
            frame[3] = PersentTestPower;
            frame[4] = Convert.ToByte(NominalTok1sk);
            frame[5] = NumberOfTest;
            frame[6] = Convert.ToByte(CurrentKz1_1 >> 8);
            frame[7] = Convert.ToByte(CurrentKz1_1 ^ 0x100);
            frame[8] = Convert.ToByte(CurrentKz2_1 >> 8);
            frame[9] = Convert.ToByte(CurrentKz2_1 ^ 0x100);

            Array.Copy(frame, frameout, frame.Length);
            frameout[frameout.Length - 1] = CalculateCRC8(frame);

            return frameout;
        }

        private static ushort[] ParseTestTirResponse(byte[] data)
        {
            if (data[24] == CalculateCRC8(data)) return BytesManipulating.ConvertByteArrayIntoUshortArray(data);
            else throw new ArgumentException("Нарушена целостность пакета.");
        }

        private static ushort[] ParseCurrentVoltageResponse(byte[] data)
        {
            if (data[25] == CalculateCRC8(data)) return BytesManipulating.ConvertByteArrayIntoUshortArray(data);
            else throw new ArgumentException("Нарушена целостность пакета.");
        }

        public static ushort[] ReadHoldingResponcesFromBuffer(byte commandNumber, int requestType)
        {
            ushort[] BuffResponce = null;
            byte[] writebuffer;
            if (serialPort1.IsOpen)
            {
                writebuffer = GetFrameDependentOnTypeOfRequest(requestType, commandNumber);
                serialPort1.Write(writebuffer, 0, writebuffer.Length);

                Thread.Sleep(RequestInterval);

                if (serialPort1.BytesToRead >= 20)
                {
                    int i = serialPort1.BytesToRead;// для отладки
                    byte[] bufferReceiver = new byte[serialPort1.BytesToRead];
                    serialPort1.Read(bufferReceiver, 0, serialPort1.BytesToRead);
                    serialPort1.DiscardInBuffer();

                    if (bufferReceiver[3] == 0x91 && bufferReceiver[0] == MasterAddress && bufferReceiver[1] == SlaveAddress)//уведомить пользователя о неверном адресе
                        BuffResponce = ParseTestTirResponse(bufferReceiver);
                    else if (bufferReceiver[3] == 0x90 && bufferReceiver[0] == MasterAddress && bufferReceiver[1] == SlaveAddress)
                        BuffResponce = ParseCurrentVoltageResponse(bufferReceiver);
                    if (BuffResponce == null) MessageBox.Show("Модуль тиристора отправил нулевой ответ.", "Ошибка!");
                }
                else MessageBox.Show("Модуль тиристора ответа не дал.", "Ошибка!");//чекнуть при отладке           
            }
            return BuffResponce;
        }

        private static byte[] GetFrameDependentOnTypeOfRequest(int requestType, byte commandNumber)
        {
            if (requestType == standartRequest) return CreateStandartRequest(SlaveAddress, commandNumber);
            else if (requestType == startRequest) return CreateStartTiristorModuleRequest(SlaveAddress);
            else return CreateTestTiristorModuleRequest(SlaveAddress);
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
