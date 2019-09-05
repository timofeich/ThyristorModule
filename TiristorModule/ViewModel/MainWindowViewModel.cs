﻿using System;
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

        private static List<string> WorkingStatus = new List<string>()
        {
            "Crach_ostanov", "Tormoz", "Baipass", "Razgon", "Дежурный режим"
        };

        private static byte[] Time = new byte[9] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        private static byte[] Capacities = new byte[9] { 10, 20, 30, 40, 50, 60, 70, 80, 90 };

        byte[] CurrentVoltageResponse = { 0xFF, 0x67, 22, 0x90, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 16, 1, 0x19 };
        byte[] TestThyristorResponse = { 0xFF, 0x67, 21, 0x91, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0xa0 };

        StandartRequest CurrentVoltage = new StandartRequest(
                BytesManipulating.GetAddress(Settings.Default.AddressSlave), 
                0x90, 
                0x00
            );

        StandartRequest StopThyristorModule = new StandartRequest(
                BytesManipulating.GetAddress(Settings.Default.AddressSlave),
                0x88, 
                0x00
            );

        StandartRequest ResetThyristorCrash = new StandartRequest(
                BytesManipulating.GetAddress(Settings.Default.AddressSlave),                                                 
                0x92, 
                0x00
            );

        StandartRequest AlarmStop = new StandartRequest(
                BytesManipulating.GetAddress(Settings.Default.AddressSlave), 
                0x99, 
                0x00
            );

        StartRequest StartThyristorModule = new StartRequest(
                BytesManipulating.GetAddress(Settings.Default.AddressSlave), 
                0x87, 
                28,
                BytesManipulating.ConvertStringCollectionToByte(Settings.Default.Time), 
                BytesManipulating.ConvertStringCollectionToByte(Settings.Default.Capacity), 
                Settings.Default.CurrentKz1, 
                Settings.Default.VremiaKzMs1,
                0, 
                Settings.Default.CurrentKz2,
                Settings.Default.VremiaKzMs2,
                0, 
                85,
                1//Convert.ToByte(Data.IsPlavniiPusk)
            );

        TestRequest TestThyristorModule = new TestRequest(
                BytesManipulating.GetAddress(Settings.Default.AddressSlave), 
                0x88, 
                7, 
                Settings.Default.PersentTestPower, 
                (byte)Settings.Default.NominalTok1sk, 
                Settings.Default.NumberOfTest, 
                Settings.Default.CurrentKz1, 
                Settings.Default.CurrentKz2
            );

        BaseResponse Response = new BaseResponse(
                BytesManipulating.GetAddress(Settings.Default.AddressMaster), 
                BytesManipulating.GetAddress(Settings.Default.AddressSlave)
            );
            
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
        }

        ~MainWindowViewModel()
        {
            //SerialPortSettings.CloseSerialPortConnection(serialPort1);
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
                    Response.GetResponse(TestThyristorResponse);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
                    {
                        while (!Data.IsRequestSingle)
                        {
                            SendRequest(request);
                            Response.GetResponse(CurrentVoltageResponse);
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
            serialPort1.Write(request, 0, request.Length);
            Thread.Sleep(SettingsModelData.RequestInterval);
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

                if (buff[14] == 128 || buff[14] == 1)
                {
                    LedIndicatorData.StartStatus = IndicatorColor.GetTestingStatusLEDColor(0);
                    LedIndicatorData.StopStatus = IndicatorColor.GetTestingStatusLEDColor(null);
                }
                else
                {
                    LedIndicatorData.StartStatus = IndicatorColor.GetTestingStatusLEDColor(null);
                    LedIndicatorData.StopStatus = IndicatorColor.GetTestingStatusLEDColor(0);
                }

                GetStatusFromCurrentVoltage(buff[15]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void GetStatusFromCurrentVoltage(ushort statusCrash)//try loop and list
        {
            switch (statusCrash)
            {
                case 0:
                    CurrentVoltageStatus(a1_kz: true, b1_kz: true, c1_kz: true, a2_kz: true, b2_kz: true, c2_kz: true);
                    break;

                case 1:
                    CurrentVoltageStatus(a1_kz: false, b1_kz: true, c1_kz: true, a2_kz: true, b2_kz: true, c2_kz: true);
                    break;

                case 2:
                    CurrentVoltageStatus(a1_kz: true, b1_kz: false, c1_kz: true, a2_kz: true, b2_kz: true, c2_kz: true);
                    break;

                case 4:
                    CurrentVoltageStatus(a1_kz: true, b1_kz: true, c1_kz: false, a2_kz: true, b2_kz: true, c2_kz: true);
                    break;

                case 8:
                    CurrentVoltageStatus(a1_kz: true, b1_kz: true, c1_kz: true, a2_kz: false, b2_kz: true, c2_kz: true);
                    break;

                case 16:
                    CurrentVoltageStatus(a1_kz: true, b1_kz: true, c1_kz: true, a2_kz: true, b2_kz: false, c2_kz: true);
                    break;

                case 32:
                    CurrentVoltageStatus(a1_kz: true, b1_kz: true, c1_kz: true, a2_kz: true, b2_kz: true, c2_kz: false);
                    break;

                case 64:
                    CurrentVoltageStatus(a1_kz: false, b1_kz: false, c1_kz: false, a2_kz: false, b2_kz: false, c2_kz: false);
                    break;
            }
        }

        private static void CurrentVoltageStatus(bool? a1_kz, bool? b1_kz, bool? c1_kz, bool? a2_kz, bool? b2_kz, bool? c2_kz)
        {
            LedIndicatorData.A1_kz = a1_kz;
            LedIndicatorData.B1_kz = b1_kz;
            LedIndicatorData.C1_kz = c1_kz;
            LedIndicatorData.A2_kz = a2_kz;
            LedIndicatorData.B2_kz = b2_kz;
            LedIndicatorData.C2_kz = c2_kz;
        }

        private static void CurrentStatus(ushort statusCrash)
        {
            List<bool?> LedIndicators = new List<bool?>();

            LedIndicators.Add(LedIndicatorData.A1_kz);
            LedIndicators.Add(LedIndicatorData.B1_kz);
            LedIndicators.Add(LedIndicatorData.C1_kz);
            LedIndicators.Add(LedIndicatorData.A2_kz);
            LedIndicators.Add(LedIndicatorData.B2_kz);
            LedIndicators.Add(LedIndicatorData.C2_kz);           
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
    }
    #endregion
}

