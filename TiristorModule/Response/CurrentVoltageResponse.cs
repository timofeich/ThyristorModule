using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TiristorModule.Indicators;
using TiristorModule.Logging;
using TiristorModule.Model;

namespace TiristorModule.Response
{
    public class CurrentVoltageResponse
    {
        byte[] Response;

        public ushort[] GetCurrentVoltageResponse(byte[] Response)
        {
            this.Response = Response;
            ushort[] frame = new ushort[16];
            int j = 4;

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
            return frame;
        }
    }
}
