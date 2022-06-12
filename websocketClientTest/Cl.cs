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
            //mWebSocket = new WebSocket("ws://192.168.219.114:5641/MGServer");
            mWebSocket = new WebSocket("ws://localhost:5641/MGServer");

            mWebSocket.OnOpen += (sender, e) =>
            {
                JObject json = new JObject();
                json.Add("cmd", "userEnter");
                json.Add("ID", "1");
                json.Add("nickName", "user");

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