using System.Windows;
using TiristorModule.Logging;

namespace TiristorModule.Response
{
    public class TestThyristorModuleResponse
    {
        private byte MasterAddress { get; }
        private byte SlaveAddress { get; }

        public TestThyristorModuleResponse()
        {
        }

        public TestThyristorModuleResponse(byte MasterAddress, byte SlaveAddress)
        {
            this.MasterAddress = MasterAddress;
            this.SlaveAddress = SlaveAddress;
        }

        public void GetTestThyristorModuleResponse(byte[] Response)
        {
            if (Response[0] == MasterAddress)
            {
                if (Response[1] == SlaveAddress)
                    MainWindowViewModel.TestThyristorWindowShow(BytesManipulating.ConvertByteArrayIntoUshortArray(Response));

                else
                {
                    MessageBox.Show("Пришел неверный адрес слейва.", "Предупреждение", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    Logger.Log.Warn("Пришел неверный адрес слейва.");
                }
            }
            else
            {
                MessageBox.Show("Пришел неверный адрес мастера.", "Предупреждение", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                Logger.Log.Warn("Пришел неверный адрес мастера.");
            }
        }
    }
}
