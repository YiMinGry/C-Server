//MainWindow.xaml.cs
using System;
using System.Windows;
using WebSocketSharp.Server;

namespace websocketTest
{
    public partial class MainWindow : Window
    {
        private static MainWindow g_Main = null;
        public static MainWindow getWindow
        {
            get
            {
                return g_Main;
            }
        }
        public void addText(string text)
        {
            this.Dispatcher.Invoke
            (
                (Action)(() =>
                {
                    string myText = Console.Text;
                    myText += text;
                    Console.Text = myText;
                })
            );
        }
        public MainWindow()
        {
            InitializeComponent();
            g_Main = this;

            Start_Server();
        }

        private HttpServer webSocketServer = null;

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (webSocketServer != null)
                webSocketServer.Stop();
            webSocketServer = null;
            Console.Text += "서버종료\n";
        }
        private void Start_Server()
        {
            webSocketServer = new HttpServer(5641);
            webSocketServer.OnGet += (sen, ex) =>
            {
                var req = ex.Request;
                var res = ex.Response;

            };
            webSocketServer.AddWebSocketService<MGServer>("/MGServer");
            webSocketServer.Start();
            Console.Text += "서버시작\n";
        }
    }
}