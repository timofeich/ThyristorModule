using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TiristorModule.Model;

namespace TiristorModule.ViewModel
{
    public class TestThyristorWindowViewModel
    {
        public event EventHandler OnRequestClose;
        public static Dictionary<int, string> FazaName = new Dictionary<int, string>(3)
        {
            [0] = "Фаза A",
            [1] = "Фаза B",
            [2] = "Фаза C"
        };
        public ObservableCollection<TestThyristorModel> TestThyristorModels { get; set; }

        public TestThyristorWindowViewModel(ushort[] buff)
        {
            TestThyristorModels = new ObservableCollection<TestThyristorModel>();

            for (int i = 4; i < 7; i++)
                TestThyristorModels.Add(new TestThyristorModel()
                {
                    FazaName = FazaName[i - 4],
                    ApBn =  buff[i],
                    BpAn = buff[i + 3],
                    CpAn = buff[i + 6],
                    ApCn = buff[i + 9],
                    BpCn = buff[i + 12],
                    CpBn = buff[i + 15],
                    OpredelenieFazz = buff[23]
                });           
        }
    }
}
