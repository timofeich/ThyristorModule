using System;
using System.Windows.Input;

namespace TiristorModule.ViewModel
{
    class StartTiristorSettingsViewModel
    {
        public ICommand SaveStartTiristorSettingsCommand { get; set; }
        public ICommand CancelStartTiristorSettingsCommand { get; set; }
        public event EventHandler OnRequestClose;

        public StartTiristorSettingsViewModel()
        {
            SaveStartTiristorSettingsCommand = new Command(arg => OkStartTiristorButtonClick());
            CancelStartTiristorSettingsCommand = new Command(arg => CancelStartTiristorButtonClick());
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
