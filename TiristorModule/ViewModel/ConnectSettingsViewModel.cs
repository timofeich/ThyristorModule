using System;
using System.Windows.Input;

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
            Properties.Settings.Default.Save();
            OnRequestClose(this, new EventArgs());
        }

        private void CancelStartTiristorButtonClick()
        {
            Properties.Settings.Default.Reset();
            OnRequestClose(this, new EventArgs());
        }
    }
}
