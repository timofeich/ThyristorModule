using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TiristorModule.Indicators;
using TiristorModule.Model;

namespace TiristorModule.ViewModel
{
    public class TestThyristorWindowViewModel
    {
        public event EventHandler OnRequestClose;
        private static Dictionary<int, string> FazaName = new Dictionary<int, string>(3);
        public LedIndicatorModel LedIndicatorData = new LedIndicatorModel();
      
        public ObservableCollection<TestThyristorModel> TestThyristorModels { get; set; }

        public TestThyristorWindowViewModel()
        {

        }

        public TestThyristorWindowViewModel(ushort[] buff)
        {
            TestThyristorModels = new ObservableCollection<TestThyristorModel>();

            InitializeFazzNameData();

            for (int i = 4; i < 7; i++)
                TestThyristorModels.Add(new TestThyristorModel()
                {
                    FazaName = FazaName[i - 4],
                    ApBn = buff[i],
                    BpAn = buff[i + 3],
                    CpAn = buff[i + 6],
                    ApCn = buff[i + 9],
                    BpCn = buff[i + 12],
                    CpBn = buff[i + 15],
                    OpredelenieFazz = buff[23]
                });

             DeleteFazzNameData();
        }

        private static void InitializeFazzNameData()
        {
            FazaName.Add(0, "Фаза A");
            FazaName.Add(1, "Фаза B");
            FazaName.Add(2, "Фаза C");
        }

        private static void DeleteFazzNameData()
        {
            FazaName.Remove(0);
            FazaName.Remove(1);
            FazaName.Remove(2);
        }

        public ushort[] OutputDataFromArrayToTestModel(ushort[] buff)//wich status will open thyristor module
        {
            try
            {
                LedIndicatorData.TestingStatus = IndicatorColor.GetTestingStatusLEDColor(buff[23]);
                return buff;
            }
            catch (Exception ex)
            {
                //Logger.Log.Error("Невозможно отобразить тестовые данные." + "Пришёл неверный статус.");
                //MessageBox.Show("Невозможно отобразить тестовые данные." + "\n" + "Пришёл неверный статус.", "Ошибка!");
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
