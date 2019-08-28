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

            SettingsModelData = new SettingsModel { };

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
                byte[] response = {0xFF, 0x67, 21, 0x91, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0x08};
                //byte[] response = { };
                //serialPort1.Read(response, 0, serialPort1.BytesToRead);
                //serialPort1.DiscardInBuffer();

                //Logger.Log.Debug("Ответ " + BitConverter.ToString(response));

                if (response.Length == TestThyristorModuleResponse.TestThyristorModuleResponseLength)
                {
                    TestThyristorModuleResponse = new TestThyristorModuleResponse(response);
                    OutputDataFromArrayToTestModel(TestThyristorModuleResponse.ParseTestThyristorModuleResponse());
                }
                else if (response.Length == CurrentVoltageResponse.CurrentVoltageResponseLength)
                {
                    CurrentVoltageResponse = new CurrentVoltageResponse(response);
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


        public void OutputDataFromArrayToTestModel(ushort[] buff)//wich status will open thyristor module
        {
            try
            {
                LedIndicatorData.TestingStatus = IndicatorColor.GetTestingStatusLEDColor(buff[23]);
                TestThyristorWindowShow(buff);
            }
            catch (Exception ex)
            {
                //Logger.Log.Error("Невозможно отобразить тестовые данные." + "Пришёл неверный статус.");
                //MessageBox.Show("Невозможно отобразить тестовые данные." + "\n" + "Пришёл неверный статус.", "Ошибка!");
                MessageBox.Show(ex.Message);
            }
        }
    }
    #endregion
}

