using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TiristorModule.ViewModel;
using System.Windows.Input;
using System.Windows;

namespace TiristorModule
{
    class MainViewModel
    {
        #region Fields
        private const byte slaveAddress = 0x67;
        private static byte MasterAddress = 0xFF;

        private static byte AddressStartTiristorModuleCommand = 0x87;
        private static byte AddressStopTiristorModuleCommand = 0x88;
        private static byte AddressRequestFotCurrentVolumeCommand = 0x90;
        private static byte AddressTestingOfTiristorModuleCommand = 0x91;
        private static byte AddressResetAvariaTiristorCommand = 0x92;
        private static byte AddressAlarmStopCommand = 0x87;

        private static SerialPort serialPort1 = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);

        private static byte[] Times = new byte[9] { 0, 5, 7, 9, 11, 13, 15, 17, 19 };
        private static byte[] Capacities = new byte[9] { 40, 30, 40, 50, 60, 70, 80, 90, 100 };
        private static ushort[] Buff;

        private static byte AlarmTemperatureTiristor = 85;

        private static byte VremiaKzMs1 = 10;
        private static byte VremiaKzMs2 = 10;

        private static ushort CurrentKz1_1 = 300;
        private static ushort CurrentKz2_1 = 300;

        private static byte PersentTestPower = 15;
        private static byte NominalTok1sk = 54 / 10;
        private static byte NumberOfTest = 10;
        private static byte[] BuffTir = new byte[18];
        private static ushort[] BuffResponce;
        private static byte FinishCheak;
        //public enum Status { Crach_ostanov = 16, Tormoz = 32, Baipass = 64, Razgon = 128 }//16, 32, 64, 128
        private static int standartRequest = 0;
        private static int startRequest = 1;
        private static int testRequest = 2;


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

        #endregion

        public MainViewModel()
        {
            CurrentVoltageCommand = new Command(arg => CurrentVoltageClick());
            AlarmStopCommand = new Command(arg => AlarmStopClick());
            TestTerristorModuleCommand = new Command(arg => TestTerristorModuleClick());
            StartTerristorModuleCommand = new Command(arg => StartTerristorModuleClick());
            StopTerristorModuleCommand = new Command(arg => StopTerristorModuleClick());
            ResetAvatiaTirristorCommand = new Command(arg => ResetAvatiaTirristorClick());

            Data = new DataModel
            {
                AmperageA1 = 0,
                VoltageA = 0
            };
        }

        #region ClickHandler

        private void CurrentVoltageClick()
        {
            Start();
        }

        private void AlarmStopClick()
        {
            MessageBox.Show("AlarmStopClick");
        }

        private void TestTerristorModuleClick()
        {
            MessageBox.Show("TestTerritorModuleClick");
        }

        private void StartTerristorModuleClick()
        {
            MessageBox.Show("StartTerristorModuleClick");
        }

        private void StopTerristorModuleClick()
        {
            //MessageBox.Show("StopTerristorModuleClick");
            Start();
        }

        private void ResetAvatiaTirristorClick()
        {
            MessageBox.Show("ResetAvatiaTirristorClick");
        }
        #endregion

        #region Methods
        public static void Start()
        {
            try
            {
                if (serialPort1.IsOpen) serialPort1.Close();
                serialPort1.Open();
                Array.Copy(Buff, 0, ReadHoldingRegistersFromResponce(AddressStartTiristorModuleCommand, 0), 0, 
                    ReadHoldingRegistersFromResponce(AddressStartTiristorModuleCommand, 0).Length);
                Data.VoltageA = Buff[0];
                Data.VoltageB = Buff[1];
                Data.VoltageC = Buff[2];
                Data.AmperageA1 = Buff[3];
                Data.AmperageB1 = Buff[4];
                Data.AmperageC1 = Buff[5];
                Data.AmperageA2 = Buff[6];
                Data.AmperageB2 = Buff[7];
                Data.AmperageC2 = Buff[8];
                Data.TemperatureOfTiristor = Buff[9];
                Data.WorkingStatus = Buff[10];//чекнуть приходящий массив много байт приходит
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Stop()
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

        private static byte[] StandartRequest(byte slaveAddress, byte commandNumber)//maybe correct
        {
            byte[] frame = new byte[4];
            frame[0] = slaveAddress;
            frame[1] = commandNumber;
            frame[2] = 0x00;
            byte crc = CalculateCRC8(frame);
            frame[3] = crc;
            return frame;
        }

        private static byte[] StartTiristorModuleRequest(byte slaveAddress)//maybe correct
        {
            byte[] frame = new byte[32];
            bool plavniiPuskStart = true;

            frame[0] = slaveAddress;
            frame[1] = 0x87;//adressCommand
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

            byte crc = CalculateCRC8(frame);
            frame[frame.Length - 1] = crc;

            return frame;
        }

        private static byte[] TestTiristorModuleRequest(byte slaveAddress, bool plavniiPuskStart)
        {
            byte[] frame = new byte[10];

            frame[0] = slaveAddress;
            frame[1] = 0x88;//adressCommand
            frame[2] = 7;//quantityofbyte
            frame[3] = PersentTestPower;
            frame[4] = NominalTok1sk;
            frame[5] = NumberOfTest;
            frame[6] = Convert.ToByte(CurrentKz1_1 >> 8);
            frame[7] = Convert.ToByte(CurrentKz1_1);
            frame[8] = Convert.ToByte(CurrentKz2_1 >> 8);
            frame[9] = Convert.ToByte(CurrentKz2_1);

            byte crc = CalculateCRC8(frame);
            frame[frame.Length - 1] = crc;

            return frame;
        }

        private static ushort[] ObrabotkaTestTirResponse(byte[] data)//вопрос по данным
        {
            ushort[] frame = new ushort[18];

            for (int i = 0; i < 17; i++)
            {
                frame[i] = data[i + 4];
            }

            frame[17] = data[22];
            frame[18] = data[23];


            return frame;
        }

        private static ushort[] CurrentVoltageResponse(byte[] data)//перечисление вместо массивов?
        {
            ushort[] frame = new ushort[12];

            frame[0] = Word.FromBytes(data[5], data[4]);
            frame[1] = Word.FromBytes(data[7], data[6]);
            frame[2] = Word.FromBytes(data[9], data[8]);
            frame[3] = Word.FromBytes(data[11], data[10]);
            frame[4] = Word.FromBytes(data[13], data[12]);
            frame[5] = Word.FromBytes(data[15], data[14]);
            frame[6] = Word.FromBytes(data[17], data[16]);
            frame[7] = Word.FromBytes(data[19], data[18]);
            frame[8] = Word.FromBytes(data[21], data[20]);
            frame[9] = data[22];
            frame[10] = data[23];
            frame[11] = data[24];

            return frame;
        }

        public static ushort[] ReadHoldingRegistersFromResponce(byte commandNumber, int requestType)
        {
            byte[] frame;
            ushort[] result;

            if (serialPort1.IsOpen)
            {
                if(requestType == standartRequest)
                {
                    frame = StandartRequest(slaveAddress, commandNumber);
                }
                else if(requestType == startRequest)
                {
                    frame = StartTiristorModuleRequest(slaveAddress);
                }
                else
                {
                    frame = TestTiristorModuleRequest(slaveAddress, true);//control plavnii pusk
                }
                 
                serialPort1.Write(frame, 0, frame.Length);
                Thread.Sleep(300); // Delay 100ms
                if (serialPort1.BytesToRead >= 20)
                {
                    byte[] bufferReceiver = new byte[serialPort1.BytesToRead];
                    serialPort1.Read(bufferReceiver, 0, serialPort1.BytesToRead);
                    serialPort1.DiscardInBuffer();
                    if(bufferReceiver[2] == 21)
                    {
                        result = ObrabotkaTestTirResponse(bufferReceiver);
                    }
                    else
                    {
                        result = CurrentVoltageResponse(bufferReceiver);
                    }
                    BuffResponce = result;
                }
            }
            return BuffResponce;
        }

        private static byte CalculateCRC8(byte[] array)//crc-8/cdma2000
        {
            byte crc = 0xFF;

            foreach (byte b in array)
            {
                crc ^= b;

                for (int i = 0; i < 8; i++)
                {
                    crc = (crc & 0x80) != 0 ? (byte)((crc << 1) ^ 0x9B) : (byte)(crc << 1);
                }
            }

            return crc;
        }
        #endregion
    }
}
