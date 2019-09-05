using System;
using System.IO.Ports;
using System.Windows;

namespace TiristorModule.ViewModel
{
    public class SerialPortSettings
    {
        public static Parity SetPortParity(string stringCollection)
        {
            string[] array = new string[] { };

            array = Enum.GetNames(typeof(Parity));

            switch (stringCollection)
            {
                case "Нет проверки":
                    return (Parity)Enum.Parse(typeof(Parity), array[0], true);
                case "Нечетный":
                    return (Parity)Enum.Parse(typeof(Parity), array[1], true);
                case "Четный":
                    return (Parity)Enum.Parse(typeof(Parity), array[2], true);
            }

            return (Parity)Enum.Parse(typeof(Parity), array[0], true);
        }

        public static StopBits SetStopBits(string stringCollection)
        {
            string[] array = new string[] { };

            array = Enum.GetNames(typeof(StopBits));

            switch (stringCollection)
            {
                case "0":
                    return (StopBits)Enum.Parse(typeof(StopBits), array[0], true);
                case "1":
                    return (StopBits)Enum.Parse(typeof(StopBits), array[1], true);
                case "1.5":
                    return (StopBits)Enum.Parse(typeof(StopBits), array[3], true);
                case "2":
                    return (StopBits)Enum.Parse(typeof(StopBits), array[2], true);

            }

            return (StopBits)Enum.Parse(typeof(StopBits), array[1], true);
        }

        public static void OpenSerialPortConnection(SerialPort serialPort)
        {
            try
            {
                if (serialPort.IsOpen) serialPort.Close();
                serialPort.Open();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void CloseSerialPortConnection(SerialPort serialPort)
        {
            try
            {
                if (serialPort.IsOpen) serialPort.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
