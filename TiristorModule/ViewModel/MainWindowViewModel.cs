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
using TiristorModule.Request;
using TiristorModule.Response;

namespace TiristorModule
{
    class MainWindowViewModel
    {
        public static SerialPort serialPort1 = new SerialPort(Settings.Default.PortName,
            Convert.ToInt32(Settings.Default.BaudRate),
            SerialPortSettings.SetPortParity(Settings.Default.Parity),
            Convert.ToInt32(Settings.Default.DataBit),
            SerialPortSettings.SetStopBits(Settings.Default.StopBit));

        StandartRequest CurrentVoltage = new StandartRequest(0x90, 0x00);
        StandartRequest StopThyristorModule = new StandartRequest(0x88, 0x00);
        StandartRequest ResetThyristorCrash = new StandartRequest(0x92, 0x00);
        StandartRequest AlarmStop = new StandartRequest(0x99, 0x00);
        StartRequest StartThyristorModule = new StartRequest(0x87, 28);
        TestRequest TestThyristorModule = new TestRequest(0x88, 7);

        CurrentVoltageResponse CurrentVoltageResponse = new CurrentVoltageResponse();
        TestThyristorModuleResponse TestThyristorModuleResponse = new TestThyristorModuleResponse();

        private static Dictionary<int, string> WorkingStatus = new Dictionary<int, string>(4);

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

            Data = new DataModel { WorkingStatus = null };
            LedIndicatorData = new LedIndicatorModel { };

            SettingsModelData = new SettingsModel { };

            InitializeWorkingStatusData();

            Logger.InitLogger();
        }

        ~MainWindowViewModel()
        {
            SerialPortSettings.CloseSerialPortConnection(serialPort1);
        }

        #region ClickHandler

        private void StartTerristorModuleClick()
        {
            CommunicateWithThyristorModule(StartThyristorModule.GetRequestPackage());

        }

        private void StopTerristorModuleClick()
        {
            CommunicateWithThyristorModule(StopThyristorModule.GetRequestPackage());
        }

        private void CurrentVoltageClick()
        {
            CommunicateWithThyristorModule(CurrentVoltage.GetRequestPackage());
        }

        private void AlarmStopClick()
        {
            CommunicateWithThyristorModule(AlarmStop.GetRequestPackage());
        }

        private void TestTerristorModuleClick()
        {
            CommunicateWithThyristorModule(TestThyristorModule.GetRequestPackage());
        }

        private void ResetAvatiaTirristorClick()
        {
            CommunicateWithThyristorModule(ResetThyristorCrash.GetRequestPackage());
        }

        public static void TestThyristorWindowShow(ushort[] buff)
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
        private void CommunicateWithThyristorModule(byte[] request)
        {
            SerialPortSettings.OpenSerialPortConnection(serialPort1);
            if (serialPort1.IsOpen)
            {
                if (Data.IsRequestSingle)
                {
                    SendRequest(request);
                    ReceiveResponse();
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
                    {
                        while (!Data.IsRequestSingle)
                        {
                            SendRequest(request);
                            ReceiveResponse();
                        }
                    }));
                }
            }
            else
            {
                MessageBox.Show("Модуль тиристора ответа не дал.", "Ошибка!");
                return;
            }
        }

        private void SendRequest(byte[] request)
        {
            Logger.Log.Debug("Запрос " + BitConverter.ToString(request));
            serialPort1.Write(request, 0, request.Length);
            Thread.Sleep(SettingsModelData.RequestInterval);
        }

        private void ReceiveResponse()
        {
            //if (serialPort1.BytesToRead > 20)
            //{
            //byte[] response = new byte[serialPort1.BytesToRead];

            //byte[] response =   { 0xFF, 0x67, 21, 0x91, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 16, 1, 0xa3};
            byte[] response = { 0xFF, 0x67, 21, 0x91, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0xa0 };


            //byte[] response = { };
            //serialPort1.Read(response, 0, serialPort1.BytesToRead);
            //serialPort1.DiscardInBuffer();

            //Logger.Log.Debug("Ответ " + BitConverter.ToString(response));

            if (response.Length == TestThyristorModuleResponse.TestThyristorModuleResponseLength)
            {
                //TestThyristorModuleResponse = new TestThyristorModuleResponse(response);
                //ushort[] buff = TestThyristorModuleResponse.ParseTestThyristorModuleResponse();
                ushort[] response1 = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
                TestThyristorWindowShow(response1);
            }
            else if (response.Length == CurrentVoltageResponse.CurrentVoltageResponseLength)
            {
                CurrentVoltageResponse = new CurrentVoltageResponse(response);
                OutputDataFromArrayToDataModel(CurrentVoltageResponse.ParseCurrentVoltageResponse());
            }
            else
            {
                MessageBox.Show("Модуль тиристора дал неполный ответ", "Ошибка!");
            }
            //}
            //else
            //{
            //    MessageBox.Show("Модуль тиристора ответа не дал.", "Ошибка!");
            //}
        }

        private static void InitializeWorkingStatusData()
        {
            WorkingStatus.Add(0, "Crach_ostanov");
            WorkingStatus.Add(1, "Tormoz");
            WorkingStatus.Add(2, "Baipass");
            WorkingStatus.Add(3, "Razgon");
            WorkingStatus.Add(4, "Дежурный режим");
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
                   
                    LedIndicatorData.StartStatus = IndicatorColor.GetTestingStatusLEDColor(1);//здесь искать
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
            catch(Exception ex)
            {
                Logger.Log.Error("Нет данных для вывода, отправка запросов остановлена.");
                MessageBox.Show(ex.Message);
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
        }

        public ushort[] OutputDataFromArrayToTestModel(ushort[] buff)//wich status will open thyristor module
        {
            try
            {
                LedIndicatorData.TestingStatus = IndicatorColor.GetTestingStatusLEDColor(buff[23]);
                return buff;
                //TestThyristorWindowShow(buff);
            }
            catch (Exception ex)
            {
                //Logger.Log.Error("Невозможно отобразить тестовые данные." + "Пришёл неверный статус.");
                //MessageBox.Show("Невозможно отобразить тестовые данные." + "\n" + "Пришёл неверный статус.", "Ошибка!");
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
    #endregion
}

