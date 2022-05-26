using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace websocketClientTest
{
    class Cl
    {
        WebSocket mWebSocket;
        public Cl()
        {
            StartConnect();

        }
        public void StartConnect()
        {
            mWebSocket = new WebSocket("ws://localhost/MGServer");
            mWebSocket.OnOpen += (sender, e) =>
            {
                JObject json = new JObject();
                json.Add("cmd", "userEnter");
                json.Add("userName", "나는 사람이다");
                json.Add("userID", "1");

                SendMessage(json.ToString());
            };
            mWebSocket.OnClose += (sender, e) =>
            {
                SendMessage("퇴장");
            };

            mWebSocket.OnMessage += (sender, e) =>
            {
                MainWindow.mainWindow.recvMsg(e.Data);
            };

            mWebSocket.Connect();
        }
        public void SendMessage(string msg)
        {
            mWebSocket.Send(msg);
        }
    }
}