using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiristorModule.Setting
{
    public class AppSettings
    {
        [Setting(Description = "Значение тока КЗ 1")]
        public ushort CurrentKz1_1 { get; set; }

        [Setting(Description = "Значение тока КЗ 2")]
        public ushort CurrentKz2_1 { get; set; }

        [Setting(Description = "Значенние мощности для теста")]
        public byte PersentTestPower { get; set; }

        [Setting(Description = "Значение номинального тока")]
        public int NominalTok1sk { get; set; }

        [Setting(Description = "Количество тестов")]
        public byte NumberOfTest { get; set; }
    }
}
