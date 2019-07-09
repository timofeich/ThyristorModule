using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO.Ports;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace TiristorModule
{
    public class Protocol
    {
        // Declares variables
        private const byte slaveAddress = 1;
        public static List<Register> Registers = new List<Register>();
        private static SerialPort serialPort1 = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);

        private static byte[] Time = new byte[9] { 0, 5, 7, 9, 11, 13, 15, 17, 19 };
        private static byte[] Capacity = new byte[9] { 40, 30, 40, 50, 60, 70, 80, 90, 100 };
        private static byte AlarmTemperatureTiristor = 85;
        private static byte VremiaKzMs1 = 10;
        private static byte VremiaKzMs2 = 10;
        private static ushort CurrentKz1_1 = 300;
        private static ushort CurrentKz2_1 = 300;
        private static byte PersentTestPower = 15;
        private static byte NominalTok1sk = 54 / 10;
        private static byte NumberOfTest = 10;
        private static byte MasterAdress = 0xFF;
        private static byte[] BuffTir = new byte[18];
        private static byte FinishCheak;



        /// <summary>
        /// Starts Modbus RTU Service.
        /// </summary>
        public static void Start()
        {
            try
            {
                if (serialPort1.IsOpen) serialPort1.Close();
                serialPort1.Open();
                ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
                {
                    while (true)
                    {
                        ReadHoldingRegisters(40001, 1);//поменять на адрес слейва
                        Thread.Sleep(20); // Delay 20ms
                    }
                }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Stops Modbus RTU Service.
        /// </summary>
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

        #region Writes

        /// <summary>
        /// Writes a value into a single holding register.
        /// </summary>
        /// <param name="startAddress">Address of the register</param>
        /// <param name="value">Value of the register</param>
        public static void WriteSingleRegister(ushort startAddress, ushort value)
        {
            const byte function = 6;
            byte[] values = Word.ToByteArray(value);
            byte[] frame = WriteSingleRegisterMsg(slaveAddress, startAddress, function, values);
            serialPort1.Write(frame, 0, frame.Length);
        }

        /// <summary>
        /// Function 06 (06hex)  Write Single Register
        /// </summary>
        /// <param name="slaveAddress">Slave Address</param>
        /// <param name="startAddress">Starting Address</param>
        /// <param name="function">Function</param>
        /// <param name="values">Data</param>
        /// <returns>Byte Array</returns>
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
        #endregion

        #region Protocol Request
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

        private static byte[] StartTiristorModuleRequest(byte slaveAddress, bool plavniiPuskStart)
        {
            byte[] frame = new byte[31];
            frame[0] = slaveAddress;
            frame[1] = 0x87;//adressCommand
            frame[2] = 28;//quantityofbyte

            for( int i = 3; i < 11; i++ )
            {  
                frame[i] = Time[i]; 
            }

            for (int i = 11; i < 21; i++)
            {
                frame[i] = Capacity[i];
            }

            frame[21] = Convert.ToByte(CurrentKz1_1 >> 8);
            frame[22] = Convert.ToByte(CurrentKz1_1);
            frame[23] = VremiaKzMs1;
            frame[24] = 0;//kz_on_off_1
            frame[25] = Convert.ToByte(CurrentKz2_1 >> 8);
            frame[26] = Convert.ToByte(CurrentKz2_1);
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
        #endregion


        #region Protocol Response

        private static ushort[] ObrabotkaTestTirResponse(byte[] data)//вопрос по данным
        {
            ushort[] frame = new ushort[18];

            for(int i = 0; i < 17; i++)
            {
                frame[i] = data[i+4];
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
            frame[7] = Word.FromBytes(data[19],data[18]);
            frame[8] = Word.FromBytes(data[21], data[20]);
            frame[9] = data[22];
            frame[10] = data[23];
            frame[11] = data[24];
            return frame;
        }

        #endregion
        #region Reads

        private static byte[] ReadHoldingRegistersMsg(byte slaveAddress,  ushort startAddress, byte function, uint numberOfPoints)
        {
            byte[] frame = new byte[8];
            frame[0] = slaveAddress;			    // Slave Address
            frame[1] = function;				    // Function             
            frame[2] = (byte)(startAddress >> 8);	// Starting Address High
            frame[3] = (byte)startAddress;		    // Starting Address Low            
            frame[4] = (byte)(numberOfPoints >> 8);	// Quantity of Registers High
            frame[5] = (byte)numberOfPoints;		// Quantity of Registers Low
            byte[] crc = CalculateCRC(frame);  // Calculate CRC.
            frame[frame.Length - 2] = crc[0];       // Error Check Low
            frame[frame.Length - 1] = crc[1];       // Error Check High
            return frame;
        }

        public static List<Register> ReadHoldingRegistersFromProtocol(byte slaveAddress, bool plavniiPuskStart, byte commandNumber)
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

                Thread.Sleep(100); // Delay 100ms
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
                        Registers[i].Value = result[i];//несколько value в модель(токи напруги)
                        //11 полей
                    }
                }
            }
            return Registers;
        }


        public static List<Register> ReadHoldingRegisters(ushort startAddress, uint numberOfPoints)
        {
            const byte function = 3;
            if (serialPort1.IsOpen)
            {
                byte[] frame = ReadHoldingRegistersMsg(slaveAddress, startAddress, function, numberOfPoints);
                serialPort1.Write(frame, 0, frame.Length);
                Thread.Sleep(100); // Delay 100ms
                if (serialPort1.BytesToRead >= 5)
                {
                    byte[] bufferReceiver = new byte[serialPort1.BytesToRead];
                    serialPort1.Read(bufferReceiver, 0, serialPort1.BytesToRead);
                    serialPort1.DiscardInBuffer();

                    // Process data.
                    byte[] data = new byte[bufferReceiver.Length - 5];
                    Array.Copy(bufferReceiver, 3, data, 0, data.Length);
                    UInt16[] result = Word.ByteToUInt16(data);
                    for (int i = 0; i < result.Length; i++)
                    {
                        Registers[i].Value = result[i];
                    }
                }
            }
            return Registers;
        }

        private static void Registers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add: // если добавление
                    Register newUser = e.NewItems[0] as Register;
                    MessageBox.Show("Добавлен новый объект: {0}", Convert.ToString(newUser.Value));
                    break;
            }
        }

        #endregion

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
    }
}
