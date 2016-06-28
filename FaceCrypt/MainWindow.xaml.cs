using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
namespace FaceCrypt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if(!File.Exists("key"))
            {
                var hwnd = new HandshakeWindow();
                hwnd.ShowDialog();
                if(hwnd.result == false)
                {
                    Close();
                    return;
                }
            }
            EncryptionWindow.encKey = File.ReadAllBytes("key");
            new EncryptionWindow().Show();
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Encrypt window
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Decrypt window
            
        }
    }
}
