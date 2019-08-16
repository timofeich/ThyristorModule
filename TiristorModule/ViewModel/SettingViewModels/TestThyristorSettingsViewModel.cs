using System;
using System.Windows.Input;
using TiristorModule.Properties;

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
            Settings.Default.Save();
            OnRequestClose(this, new EventArgs());
        }

        private void CancelTestTiristorButtonClick()
        {
            Settings.Default.Reset();
            OnRequestClose(this, new EventArgs());
        }        
    }
}
