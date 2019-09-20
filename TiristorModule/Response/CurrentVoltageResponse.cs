using System.Windows;
using TiristorModule.Logging;

namespace TiristorModule.Response
{
    public class CurrentVoltageResponse
    {
        private byte MasterAddress { get; }
        private byte SlaveAddress { get; }

        public CurrentVoltageResponse()
        {
        }

        public CurrentVoltageResponse(byte MasterAddress, byte SlaveAddress)
        {
            this.MasterAddress = MasterAddress;
            this.SlaveAddress = SlaveAddress;
        }

        public void GetCurrentVoltageResponse(byte[] Response)
        {            
            ushort[] frame = new ushort[15];//16
            int j = 4;

            if (Response[0] == MasterAddress)
            {
                if (Response[1] == SlaveAddress)
                {
                    for (int i = 0; i < frame.Length; i++)
                    {
                        if (i < 4)
                        {
                            frame[i] = Response[i];
                        }
                        else if (i < 13)
                        {
                            frame[i] = BytesManipulating.FromBytes(Response[j + 1], Response[j]);
                            j += 2;
                        }
                        else
                        {
                            frame[i] = Response[j];
                            j++;
                        }
                    }
                    MainWindowViewModel.OutputDataFromArrayToDataModel(frame);
                }
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
