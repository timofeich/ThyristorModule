using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TiristorModule.Indicators;
using TiristorModule.Logging;
using TiristorModule.Model;
using TiristorModule.View;
using TiristorModule.ViewModel;

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
                {
                    MainWindowViewModel.TestThyristorWindowShow(BytesManipulating.ConvertByteArrayIntoUshortArray(Response));
                }
                else
                {
                    MessageBox.Show("Ошибка. Пришел неверный адрес слейва.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Ошибка. Пришел неверный адрес мастера.");
                return;
            }

        }

    }
}
