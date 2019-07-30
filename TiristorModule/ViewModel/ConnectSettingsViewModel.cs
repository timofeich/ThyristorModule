using System;
using System.Windows.Input;

namespace TiristorModule.ViewModel
{
    public class ConnectSettingsViewModel
    {
        //списки для заполнения комбо боксов как в ModBusSlave
        //отображение текущих данных
        //сохранение настроек, даже при перезапуске
        //подсказки при наведении
        //обработка Ок\Отмены в соответствии с патерном MVVM

        public ICommand SaveConnectSettingsCommand { get; set; }
        public ICommand CancelConnectSettingsCommand { get; set; }
       //public ICommand SelectPortNameCommand { set; get; }

        public event EventHandler OnRequestClose;

        public ConnectSettingsViewModel()
        {
            SaveConnectSettingsCommand = new Command(arg => OkStartTiristorButtonClick());
            CancelConnectSettingsCommand = new Command(arg => CancelStartTiristorButtonClick());
           // SelectPortNameCommand = new Command(arg => SelectPortNameComboboxItem());
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

        //private string SelectPortNameComboboxItem()
        //{
        //   //return Properties.Settings.Default.PortName.SelectedItem;
        //}
    }
}
