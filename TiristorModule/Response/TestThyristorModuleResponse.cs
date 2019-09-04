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

        public TestThyristorModuleResponse()
        {
            
        }

        public void GetTestThyristorModuleResponse(byte[] Response)
        {
            MainWindowViewModel.TestThyristorWindowShow(BytesManipulating.ConvertByteArrayIntoUshortArray(Response));
        }

    }
}
