using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiristorModule.Setting
{
    public class SettingAttribute : Attribute
    {
        public string Description { get; set; }
        public bool Visible { get; set; }
    }
}
