//MainWindow.xaml.cs
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace websocketClientTest
{
    public partial class MainWindow : Window
    {
        Cl c;
        private static MainWindow _mainWindow = null;
        public static MainWindow mainWindow
        {
            get
            {
                return _mainWindow;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            _mainWindow = this;
            c = new Cl();
        }
        public void recvMsg(string text)
        {
            JObject json2 = new JObject();

            this.Dispatcher.Invoke(() =>
            {
                json2 = JObject.Parse(text);
                //string tmp = json2["retMsg"].ToString();

                RecvBox.Text += "cl recv " + text + "\n";
            }
            );
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            c.SendMessage(sendBox.Text);
            sendBox.Text = "";
        }
    }
}