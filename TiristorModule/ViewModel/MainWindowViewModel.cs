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
        private static SerialPort serialPort1 = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);

        private static byte[] Times = new byte[9] { 0, 5, 7, 9, 11, 13, 15, 17, 19 };
        private static byte[] Capacities = new byte[9] { 40, 30, 40, 50, 60, 70, 80, 90, 100 };

        private static byte AlarmTemperatureTiristor = 85;

        private static byte VremiaKzMs1 = 10;
        private static byte VremiaKzMs2 = 10;

        private static ushort CurrentKz1_1 = 300;
        private static ushort CurrentKz2_1 = 300;

        private static byte PersentTestPower = 15;
        private static byte NominalTok1sk = 54 / 10;
        private static byte NumberOfTest = 10;
        private static byte MasterAddress = 0xFF;
        private static byte[] BuffTir = new byte[18];
        private static ushort[] BuffResponce;
        private static byte FinishCheak;
        //public enum Status { Crach_ostanov = 16, Tormoz = 32, Baipass = 64, Razgon = 128 }//16, 32, 64, 128
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
            MessageBox.Show("StopTerristorModuleClick");
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
                //Data.Amperage = ReadHoldingRegisters(40001, 1)[0];
                ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
                {
                    while (true)
                    {
                        Data.VoltageA = ReadHoldingRegistersProtocol(40001, 1)[0];//поменять на адрес слейва
                        Thread.Sleep(20); // Delay 20ms
                    }
                }));
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

        public static void WriteSingleRegister(ushort startAddress, ushort value)
        {
            const byte function = 6;
            byte[] values = Word.ToByteArray(value);
            byte[] frame = WriteSingleRegisterMsg(slaveAddress, startAddress, function, values);
            serialPort1.Write(frame, 0, frame.Length);
        }

        private static byte[] WriteSingleRegisterMsg(byte slaveAddress, ushort startAddress, byte function, byte[] values)
        {
            byte[] frame = new byte[8];                     // Message size
            frame[0] = slaveAddress;                        // Slave address
            frame[1] = function;                            // Function code            
            frame[2] = (byte)(startAddress >> 8);           // Register Address Hi
            frame[3] = (byte)startAddress;                  // Register Address lo
            Array.Copy(values, 0, frame, 4, values.Length); // Write Data
            byte[] crc = CalculateCRC(frame);          // Calculate CRC
            frame[frame.Length - 2] = crc[0];               //Error Check Lo
            frame[frame.Length - 1] = crc[1];               //Error Check Hi
            return frame;
        }

        private static byte[] StandartRequest(byte slaveAddress, byte commandNumber)
        {
            byte[] frame = new byte[8];
            frame[0] = slaveAddress;
            frame[1] = commandNumber;
            frame[2] = 0x00;
            byte crc = CalculateCRC8(frame);
            frame[3] = crc;
            return frame;
        }

        private static byte[] StartTiristorModuleRequest(byte slaveAddress)
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

        private static byte[] ReadHoldingRegistersMsg(byte slaveAddress, ushort startAddress, byte function, uint numberOfPoints)
        {
            byte[] frame = new byte[8];

            frame[0] = slaveAddress;                // Slave Address
            frame[1] = function;                    // Function             
            frame[2] = (byte)(startAddress >> 8);   // Starting Address High
            frame[3] = (byte)startAddress;          // Starting Address Low            
            frame[4] = (byte)(numberOfPoints >> 8); // Quantity of Registers High
            frame[5] = (byte)numberOfPoints;        // Quantity of Registers Low

            byte[] crc = CalculateCRC(frame);  // Calculate CRC.
            frame[frame.Length - 2] = crc[0];       // Error Check Low
            frame[frame.Length - 1] = crc[1];       // Error Check High

            return frame;
        }

        /*public static List<DataModel> ReadHoldingRegistersFromProtocol(byte slaveAddress, bool plavniiPuskStart, byte commandNumber)
        {
            if (serialPort1.IsOpen)
            {
                byte[] frame;
                ushort[] result;

                switch (commandNumber)
                {
                    case 0x88:
                    case 0x90:
                    case 0x92:
                    case 0x99:
                        frame = StandartRequest(slaveAddress, commandNumber);
                        serialPort1.Write(frame, 0, frame.Length);
                        break;

                    case 0x87:
                        frame = StartTiristorModuleRequest(slaveAddress, plavniiPuskStart);
                        serialPort1.Write(frame, 0, frame.Length);
                        break;

                    case 0x91:
                        frame = TestTiristorModuleRequest(slaveAddress, plavniiPuskStart);
                        serialPort1.Write(frame, 0, frame.Length);
                        break;
                }

                Thread.Sleep(300); // Delay 300ms
                if (serialPort1.BytesToRead >= 20)
                {
                    byte[] bufferReceiver = new byte[serialPort1.BytesToRead];

                    serialPort1.Read(bufferReceiver, 0, serialPort1.BytesToRead);
                    serialPort1.DiscardInBuffer();

                    byte[] data = new byte[bufferReceiver.Length];
                    Array.Copy(bufferReceiver, 0, data, 0, data.Length);

                    if (data[3] == 0x90)
                    {
                        result = ObrabotkaTestTirResponse(data);//добавлить проверку CRC8
                    }
                    else if (data[3] == 0x91)
                    {
                        result = CurrentVoltageResponse(data);
                    }
                    else
                    {
                        result = null;
                    }

                    for (int i = 0; i < result.Length; i++)
                    {
                        Datas[i].Voltage = result[i];//несколько value в модель(токи напруги)
                        //Registers[i].VoltageA = result[i + 1];
                        //Registers[i].VoltageB = result[i + 2];
                        //Registers[i].VoltageC = result[i + 3];
                        //Registers[i].AmperageA1 = result[i + 4];
                        //Registers[i].AmperageA2 = result[i + 5];
                        //Registers[i].AmperageB1 = result[i + 6];
                        //Registers[i].AmperageB2 = result[i + 7];
                        //11 полей

                    }
                }
            }
            return Datas;
        }*/

        public static ushort[] ReadHoldingRegisters(ushort startAddress, uint numberOfPoints)
        {
            const byte function = 3;
            if (serialPort1.IsOpen)
            {
                byte[] frame = ReadHoldingRegistersMsg(slaveAddress, startAddress, function, numberOfPoints);               
                serialPort1.Write(frame, 0, frame.Length);
                Thread.Sleep(300); // Delay 100ms
                if (serialPort1.BytesToRead >= 5)
                {
                    byte[] bufferReceiver = new byte[serialPort1.BytesToRead];
                    serialPort1.Read(bufferReceiver, 0, serialPort1.BytesToRead);
                    serialPort1.DiscardInBuffer();

                    // Process data.
                    byte[] data = new byte[bufferReceiver.Length - 5];
                    Array.Copy(bufferReceiver, 3, data, 0, data.Length);
                    ushort[] result = Word.ByteToUInt16(data);
                    BuffResponce = result;
                }
            }
            return BuffResponce;
        }

        public static ushort[] ReadHoldingRegistersProtocol(ushort startAddress, uint numberOfPoints)
        {
            if (serialPort1.IsOpen)
            {
                byte[] frame = StartTiristorModuleRequest(slaveAddress);
                serialPort1.Write(frame, 0, frame.Length);
                Thread.Sleep(300); // Delay 100ms
                if (serialPort1.BytesToRead >= 20)
                {
                    byte[] bufferReceiver = new byte[serialPort1.BytesToRead];
                    serialPort1.Read(bufferReceiver, 0, serialPort1.BytesToRead);
                    serialPort1.DiscardInBuffer();

                    // Process data.
                    byte[] data = new byte[bufferReceiver.Length - 5];
                    Array.Copy(bufferReceiver, 3, data, 0, data.Length);
                    ushort[] result = Word.ByteToUInt16(data);
                    BuffResponce = result;
                }
            }
            return BuffResponce;
        }

        private static byte[] CalculateCRC(byte[] data)
        {
            ushort CRCFull = 0xFFFF; // Set the 16-bit register (CRC register) = FFFFH.
            char CRCLSB;
            byte[] CRC = new byte[2];
            for (int i = 0; i < (data.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ data[i]); // 

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = (byte)(CRCFull & 0xFF);
            return CRC;
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
