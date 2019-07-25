using System;
using System.Windows.Input;

namespace TiristorModule.ViewModel
{
    public class TestTiristorSettingsViewModel
     {
        public ICommand SaveTestTiristorSettingsCommand { get; set; }
        public ICommand CancelTestTiristorSettingsCommand { get; set; }
        public event EventHandler OnRequestClose;

        public TestTiristorSettingsViewModel()
        {
            SaveTestTiristorSettingsCommand = new Command(arg => OkTestTiristorButtonClick());
            CancelTestTiristorSettingsCommand = new Command(arg => CancelTestTiristorButtonClick());
        }

        private void OkTestTiristorButtonClick()
        {
            Properties.Settings.Default.Save();
            OnRequestClose(this, new EventArgs());
        }

        private void CancelTestTiristorButtonClick()
        {
            Properties.Settings.Default.Reset();
            OnRequestClose(this, new EventArgs());
        }        
    }
}
