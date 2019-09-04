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
        byte[] Response;

        public TestThyristorModuleResponse()
        {
            
        }

        public ushort[] GetTestThyristorModuleResponse(byte[] Response)
        {
            this.Response = Response;
            return BytesManipulating.ConvertByteArrayIntoUshortArray(Response);
        }

    }
}
