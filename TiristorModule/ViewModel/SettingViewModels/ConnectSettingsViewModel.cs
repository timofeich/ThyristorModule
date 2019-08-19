using System;
using System.Windows.Input;
using TiristorModule.Properties;

namespace TiristorModule.ViewModel
{
    public class ConnectSettingsViewModel
    {
        public ICommand SaveConnectSettingsCommand { get; set; }
        public ICommand CancelConnectSettingsCommand { get; set; }

        public event EventHandler OnRequestClose;

        public ConnectSettingsViewModel()
        {
            SaveConnectSettingsCommand = new Command(arg => OkStartTiristorButtonClick());
            CancelConnectSettingsCommand = new Command(arg => CancelStartTiristorButtonClick());
        }

        private void OkStartTiristorButtonClick()
        {
            MainWindowViewModel.serialPort1 = new System.IO.Ports.SerialPort(Settings.Default.PortName,
            Convert.ToInt32(Settings.Default.BaudRate),
            SerialPortSettings.SetPortParity(Settings.Default.Parity),
            Convert.ToInt32(Settings.Default.DataBit),
            SerialPortSettings.SetStopBits(Settings.Default.StopBit));

            Settings.Default.Save();
            OnRequestClose(this, new EventArgs());
        }

        private void CancelStartTiristorButtonClick()
        {
            Settings.Default.Reload();
            OnRequestClose(this, new EventArgs());
        }
    }
}
