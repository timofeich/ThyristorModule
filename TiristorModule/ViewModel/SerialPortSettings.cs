using System;
using System.IO.Ports;
using System.Windows;
using TiristorModule.Logging;

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
                if (!serialPort.IsOpen)
                {
                    serialPort.Close();
                    serialPort.Open();
                }
                else
                {
                    MessageBox.Show("Порт - " + serialPort.PortName + " закрыт.\n" +
                      "Чтобы отправлять запросы, укажите открытый порт.", "Предупреждение",
                      MessageBoxButton.OK, MessageBoxImage.Warning);
                    Logger.Log.Warn("Порт - " + serialPort.PortName + " закрыт.");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                Logger.Log.Error(ex.Message);
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
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log.Error(ex.Message);
            }
        }
    }
}
