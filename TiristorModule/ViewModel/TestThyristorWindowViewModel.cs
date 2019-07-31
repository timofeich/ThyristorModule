using System;
using System.Collections.ObjectModel;
using TiristorModule.Model;

namespace TiristorModule.ViewModel
{
    public class TestThyristorWindowViewModel
    {
        public event EventHandler OnRequestClose;

        public ObservableCollection<TestData> Testdata { get; set; } = new ObservableCollection<TestData>();

        public TestThyristorWindowViewModel()
        {
            var tmp = GetAllTestData();
            foreach (var item in tmp)
            {
                Testdata.Add(item);
            }
        }

        private ObservableCollection<TestData> GetAllTestData()
        {
            ObservableCollection<TestData> result = new ObservableCollection<TestData>();

            for (int i = 0; i < 3; i++)
            {
                result.Add(
                    new TestData
                    {
                        ApBn = 10,
                        BpAn = 20,
                        CpAn = 30,
                        ApCn = 40,
                        BpCn = 50,
                        CpBn = 60

                    });
            }

            return result;
        }
    }
}
