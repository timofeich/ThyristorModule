﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using TiristorModule.ViewModel;
using System.Windows.Input;
using System.Windows;
using TiristorModule.View;

namespace TiristorModule
{
    class MainViewModel
    {
        private static SerialPort serialPort1 = new SerialPort("COM3", 57600, Parity.None, 8, StopBits.One);

        #region Fields
        private const byte slaveAddress = 0x67;
        private const byte MasterAddress = 0xFF;

        private const byte AddressStartTiristorModuleCommand = 0x87;
        private const byte AddressStopTiristorModuleCommand = 0x88;
        private const byte AddressRequestFotCurrentVoltageCommand = 0x90;
        private const byte AddressTestTiristorModuleCommand = 0x91;
        private const byte AddressResetAvariaTiristorCommand = 0x92;
        private const byte AddressAlarmStopCommand = 0x87;

        private static byte[] Times =      new byte[9] { 0,  5,  7,  9,  11, 13, 15, 17, 19 };
        private static byte[] Capacities = new byte[9] { 40, 30, 40, 50, 60, 70, 80, 90, 100 };
        private static ushort[] Buff;

        private const byte AlarmTemperatureTiristor = 85;

        private static byte VremiaKzMs1 = Properties.Settings.Default.VremiaKzMs1;
        private static byte VremiaKzMs2 = Properties.Settings.Default.VremiaKzMs2;

        private static ushort CurrentKz1_1 = Properties.Settings.Default.CurrentKz1;
        private static ushort CurrentKz2_1 = Properties.Settings.Default.CurrentKz2;

        private static byte PersentTestPower = Properties.Settings.Default.PersentTestPower;
        private static int NominalTok1sk = Properties.Settings.Default.NominalTok1sk / 10;
        private static byte NumberOfTest = Properties.Settings.Default.NumberOfTest;

        private static byte[] BuffTir = new byte[18];
        private static ushort[] BuffResponce;

        private static Dictionary<int, string> WorkingStatus = new Dictionary<int, string>(4);
        private static Dictionary<int, string> TestingStatus = new Dictionary<int, string>(2);

        private static int standartRequest = 0;
        private static int startRequest = 1;
        private static int testRequest = 2;

        private static bool flag = true;

        #endregion

        #region Properties
        public static DataModel Data { get; set; }
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

        #endregion

        public MainViewModel()
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

            InitializeWorkingStatusData();
            InitializeTestingStatusData();

            Data = new DataModel
            {
                AmperageA1 = 0,
                AmperageB1 = 0,
                AmperageC1 = 0,
                AmperageA2 = 0,
                AmperageB2 = 0,
                AmperageC2 = 0,
                VoltageA = 0,
                VoltageB = 0,
                VoltageC = 0,
                TemperatureOfTiristor = 0,
                WorkingStatus = null,
                TestingStatus = null
            };
        }

        private static void InitializeWorkingStatusData()
        {
            WorkingStatus.Add(0, "Crach_ostanov");
            WorkingStatus.Add(1, "Tormoz");
            WorkingStatus.Add(2, "Baipass");
            WorkingStatus.Add(3, "Razgon");
        }

        private static void InitializeTestingStatusData()
        {
            TestingStatus.Add(0, "Успешно");
            TestingStatus.Add(1, "Ошибочно");
        }

        #region ClickHandler

        private void StartTerristorModuleClick()
        {
            StartRequest(AddressStartTiristorModuleCommand, startRequest);
        }

        private void StopTerristorModuleClick()
        {
            StartRequest(AddressStopTiristorModuleCommand, standartRequest);
        }

        private void CurrentVoltageClick()
        {
            StartRequest(AddressRequestFotCurrentVoltageCommand, standartRequest);
        }

        private void AlarmStopClick()
        {
            StartRequest(AddressAlarmStopCommand, standartRequest);
        }

        private void TestTerristorModuleClick()
        {
            StartRequest(AddressTestTiristorModuleCommand, testRequest);
        }

        private void ResetAvatiaTirristorClick()
        {
            StartRequest(AddressResetAvariaTiristorCommand, standartRequest);
        }

        private void ConnectionSettingsClick()//switch window method
        {
            var connectSettingView = new ConnectSettingWindowView
            {
                DataContext = new ConnectSettingViewModel()
            };

            connectSettingView.ShowDialog();
        }

        private void StartTiristorSettingsClick()
        {
            var connectSettingView = new StartTiristorSettingsView
            {
                DataContext = new StartTiristorSettingsViewModel()
            };

            connectSettingView.ShowDialog(); 
        }

        private void TestTiristorSettingsClick()
        {
            var vm = new TestTiristorSettingsViewModel();
            var connectSettingView = new TestTiristorSettingsView
            {
                DataContext = vm
            };
            vm.OnRequestClose += (s, e) => connectSettingView.Close();
            connectSettingView.ShowDialog();
        }
        #endregion

        #region Methods
        public static void StartRequest(byte AddressCommand, int RequestType)
        {
            try
            {
                if (AddressCommand == AddressRequestFotCurrentVoltageCommand)
                {
                    flag = true;
                    OpenSerialPortConnection(serialPort1);
                    ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
                    {
                        while (flag)
                        {
                            Buff = ReadHoldingResponcesFromBuffer(AddressCommand, RequestType);
                            OutputDataFromArrayToModel(Buff, AddressCommand);
                            Buff = null;
                            Thread.Sleep(20); // Delay 20ms
                        }
                    }));
                }
                else
                {
                    flag = false;
                    OpenSerialPortConnection(serialPort1);
                    Buff = ReadHoldingResponcesFromBuffer(AddressCommand, RequestType);
                    OutputDataFromArrayToModel(Buff, AddressCommand);
                    Buff = null;
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex);
            }
        }

        public static void StopRequest()
        {
            try
            {
                if (serialPort1.IsOpen) serialPort1.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void OutputDataFromArrayToModel(ushort[] buff, byte AdressCommand)
        {
            Data.VoltageA = buff[0];
            Data.VoltageB = buff[1];
            Data.VoltageC = buff[2];
            Data.AmperageA1 = buff[3];
            Data.AmperageB1 = buff[4];
            Data.AmperageC1 = buff[5];
            Data.AmperageA2 = buff[6];
            Data.AmperageB2 = buff[7];
            Data.AmperageC2 = buff[8];
            Data.TemperatureOfTiristor = buff[9];
            if(AdressCommand != AddressTestTiristorModuleCommand) { Data.WorkingStatus = GetWorkingStatus(buff[10]); }
            else { Data.TestingStatus = GetStatusFromTestTiristor(buff[22]); }
        }

        private static string GetWorkingStatus(ushort statusByte)
        {
            if (statusByte % 16 == 0 && statusByte != 0)
            {
                int i = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(statusByte)) - 4);
                return WorkingStatus[1];
            }     
            else
            {
                return "Дежурный р-м";
            }
        }

        //standart request - zapros_cur_voltage, stop_tiristor_module, reset_avaria_tir, avarinii_stop
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

        private static byte[] CreateStartTiristorModuleRequest(byte slaveAddress, bool plavniiPuskStart)
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
            frame[30] = Convert.ToByte(plavniiPuskStart);//flag

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

        private static ushort[] ParseTestTirResponse(byte[] data)//output as a table
        {
            ushort[] frame = new ushort[18];
            if (data[23] == CalculateCRC8(data))
            {
                for (int i = 0; i < 17; i++)
                {
                    frame[i] = data[i + 4];
                }

                frame[17] = data[22];
                return frame;
            }
            else
            {
                frame[2] = 0;
                return frame;
            }
        }

        private static ushort[] ParseCurrentVoltageResponse(byte[] data)//status_crash(0-suc-s/list of errors)
        {
            ushort[] frame = new ushort[12];
            int j = 4;
            if (data[24] == CalculateCRC8(data))
            {
                for (int i = 0; i <= 8; i++)
                {
                    frame[i] = Word.FromBytes(data[j + 1], data[j]);
                    j += 2;
                }

                for (int i = 9; i < frame.Length; i++)
                {
                    frame[i] = data[i + 13];
                }

                return frame;
            }
            else
            {
                frame[2] = 0;
                return frame;
            }

        }

        public static ushort[] ReadHoldingResponcesFromBuffer(byte commandNumber, int requestType)
        {
            byte[] frame;
            ushort[] result;

            if (serialPort1.IsOpen)
            {
                if (requestType == standartRequest) frame = CreateStandartRequest(slaveAddress, commandNumber);
                else if (requestType == startRequest) frame = CreateStartTiristorModuleRequest(slaveAddress, true);
                else frame = CreateTestTiristorModuleRequest(slaveAddress);
                 
                serialPort1.Write(frame, 0, frame.Length);
               
                Thread.Sleep(300); // Delay 300ms
                if (serialPort1.BytesToRead >= 20)
                {
                    byte[] bufferReceiver = new byte[serialPort1.BytesToRead];
                    serialPort1.Read(bufferReceiver, 0, serialPort1.BytesToRead);
                    serialPort1.DiscardInBuffer();

                    if (bufferReceiver[2] == 21) result = ParseTestTirResponse(bufferReceiver);
                    else result = ParseCurrentVoltageResponse(bufferReceiver);

                    if (result[2] == 0) MessageBox.Show("Нарушена целостность ответа.");
                    else BuffResponce = result;
                }
            }
            return BuffResponce;
        }

        private static void GetStatusFromCurrentVoltage(ushort statusCrash) 
        {
            //массив эл. управления
            //перебор 
            //логика вывода ошибок
            //    int i = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(statusCrash)));

            switch (statusCrash)
            {
                case 0:

                    break;
                case 1:

                    break;

               

            }
        }

        private static string GetStatusFromTestTiristor(ushort statusTest)
        {
            return TestingStatus[statusTest];
        }

        private static void OpenSerialPortConnection(SerialPort serialPort)
        {
            if (serialPort.IsOpen) serialPort.Close();
            serialPort.Open();
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
