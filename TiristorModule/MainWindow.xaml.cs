using System;
using System.Windows;

namespace TiristorModule
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeRegisters();
            try
            {
                Protocol.Start();
              //VoltageATextBlock.Text = Protocol.Registers[0]; //new Register() { Address = 40001 };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void InitializeRegisters()
        {
            Protocol.Registers.Clear();
            Protocol.Registers.Add(new Register() { Address = 40001 });

        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Protocol.Stop();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
