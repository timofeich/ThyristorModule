using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using TiristorModule.ViewModel;
using System.Windows.Input;
using System.Windows;
using TiristorModule.View;
using TiristorModule.Properties;
using TiristorModule.Model;
using TiristorModule.Request;
using TiristorModule.Response;
using TiristorModule.Logging;
using System.Linq;

namespace TiristorModule
{
    class MainWindowViewModel
    {
        public static SerialPort serialPort1 = new SerialPort(Settings.Default.PortName,
            Convert.ToInt32(Settings.Default.BaudRate),
            SerialPortSettings.SetPortParity(Settings.Default.Parity),
            Convert.ToInt32(Settings.Default.DataBit),
            SerialPortSettings.SetStopBits(Settings.Default.StopBit));

        private static List<string> WorkingStatus = new List<string>()
        {
            "Авария", "Тормоз", "Байпасс", "Разгон", "Дежурный режим"
        };

        //byte[] CurrentVoltageResponse = { 0xFF, 0x67, 22, 0x90, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 16, 1, 0x19 };
        //byte[] TestThyristorResponse = { 0xFF, 0x67, 21, 0x91, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0xa0 };

        bool IsRequestIsTest;

        private static StandartRequest CurrentVoltage;
        private static StandartRequest StopThyristorModule;
        private static StandartRequest ResetThyristorCrash;
        private static StandartRequest AlarmStop;
        private static StartRequest StartThyristorModule;
        private static TestRequest TestThyristorModule;
        private static BaseResponse Response;
            
        public static DataModel Data { get; set; }
        public static LedIndicatorModel LedIndicatorData { get; set; }
        public static SettingsModel SettingsModelData { get; set; }

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
            BindingCommandsToClickMethods();
            InitializeDataModels();
            InitializeRequests();
            InitializeResponses();
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
            //GetStatusFromCurrentVoltage(127);
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
            IsRequestIsTest = true;
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
        private void BindingCommandsToClickMethods()
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
        }

        private void InitializeDataModels()
        {
            Data = new DataModel { WorkingStatus = null };
            LedIndicatorData = new LedIndicatorModel { };
            SettingsModelData = new SettingsModel { };
        }

        public static void InitializeRequests()
        {
            StopThyristorModule = new StandartRequest(SettingsModelData.SlaveAddress, 0x88, 0x00);
            CurrentVoltage = new StandartRequest(SettingsModelData.SlaveAddress, 0x90, 0x00);
            ResetThyristorCrash = new StandartRequest(SettingsModelData.SlaveAddress, 0x92, 0x00);
            AlarmStop = new StandartRequest(SettingsModelData.SlaveAddress, 0x99, 0x00);

            StartThyristorModule = new StartRequest(SettingsModelData.SlaveAddress, 0x87, 28, SettingsModelData.Times,
                SettingsModelData.Capacities, SettingsModelData.CurrentKz1, SettingsModelData.VremiaKzMs1, 0,
                SettingsModelData.CurrentKz2, SettingsModelData.VremiaKzMs2, 0, 85, Convert.ToByte(Data.IsPlavniiPusk));

            TestThyristorModule = new TestRequest(SettingsModelData.SlaveAddress, 0x88, 7, SettingsModelData.PersentTestPower,
                SettingsModelData.NominalTok1sk, SettingsModelData.NumberOfTest, SettingsModelData.CurrentKz1, 
                SettingsModelData.CurrentKz2);
        }

        public static void InitializeResponses()
        {
            Response = new BaseResponse(
                SettingsModelData.MasterAddress,
                SettingsModelData.SlaveAddress
            );
        }

        public void CommunicateWithThyristorModule(byte[] request)
        {
            SerialPortSettings.OpenSerialPortConnection(serialPort1);
            if (serialPort1.IsOpen)
            {
                if (Data.IsRequestSingle)
                {
                    if (!IsRequestIsTest)
                    {
                        SendRequest(request);
                        ReceiveResponse();
                    }
                    else
                    {
                        TestModeOfCommunicateWithThyristorModule(request);
                    }
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
                    {
                        while (!Data.IsRequestSingle)
                        {
                            if (!IsRequestIsTest)
                            {
                                SendRequest(request);
                                ReceiveResponse();
                            }
                            else
                            {
                                TestModeOfCommunicateWithThyristorModule(request);
                            }
                        }
                    }));
                }
            }
        }

        private void TestModeOfCommunicateWithThyristorModule(byte[] request)
        {
            SendRequest(request);
            ReceiveResponse();
            Thread.Sleep(100);
            ReceiveResponse();
            IsRequestIsTest = false;
        }

        private void SendRequest(byte[] request)
        {
            try
            {
                serialPort1.Write(request, 0, request.Length);
                Logger.Log.Debug("Запрос - " + BitConverter.ToString(request));
                Thread.Sleep(SettingsModelData.RequestInterval);                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log.Error(ex.Message);
            }
        }

        public static void ReceiveResponse()
        {
            try
            {
                if (serialPort1.BytesToRead >= 20)
                {
                    byte[] bufferReceiver = new byte[serialPort1.BytesToRead];
                    serialPort1.Read(bufferReceiver, 0, serialPort1.BytesToRead);
                    serialPort1.DiscardInBuffer();
                    Logger.Log.Debug("Ответ - " + BitConverter.ToString(bufferReceiver));
                    Response.GetResponse(bufferReceiver);
                }
                else
                {
                    MessageBox.Show("Модуль тиристора ответа не дал.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Logger.Log.Warn("Модуль тиристора ответа не дал.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log.Error(ex.Message);
            }
        }
    
        public static void OutputDataFromArrayToDataModel(ushort[] buff)//wich status will open thyristor module
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
                StartStopStatus(buff[14]);
               
                GetStatusFromCurrentVoltage(buff[15]);
                Logger.Log.Info(buff.Skip(4).Take(buff.Length - 2).ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log.Error(ex.Message);
            }
        }

        private static void CurrentVoltageStatus(bool?[] Status)
        {
            LedIndicatorData.A1_kz = !Status[0];
            LedIndicatorData.B1_kz = !Status[1];
            LedIndicatorData.C1_kz = !Status[2];
            LedIndicatorData.A2_kz = !Status[3];
            LedIndicatorData.B2_kz = !Status[4];
            LedIndicatorData.C2_kz = !Status[5];
            LedIndicatorData.ReferenceVoltage = !Status[6];
        }

        private static void GetStatusFromCurrentVoltage(ushort statusCrash)
        {
            bool?[] LedLightRegim = new bool?[7];

            for (int i = 0; i < LedLightRegim.Length; i++) 
                LedLightRegim[i] = Convert.ToBoolean((statusCrash >> i) & 0x01);
            
            CurrentVoltageStatus(LedLightRegim);
        }

        private static void StartStopStatus(ushort Status)
        {
            if (Status == 128 || Status == 1)
            {
                LedIndicatorData.StartStatus = true;
                LedIndicatorData.StopStatus = null;
            }
            else
            {
                LedIndicatorData.StartStatus = null;
                LedIndicatorData.StopStatus = true;  
            }
        }

        private static string GetWorkingStatus(ushort CurrentVoltageWorkStatus)
        {
            if (CurrentVoltageWorkStatus % 16 == 0 && CurrentVoltageWorkStatus != 0)
            {
                int i = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(CurrentVoltageWorkStatus)) - 4);
                return WorkingStatus[i];
            }
            else return WorkingStatus[4];
        }
    }
    #endregion
}

