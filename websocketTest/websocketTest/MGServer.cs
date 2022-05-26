using System;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace websocketTest
{
    public class MGServer : WebSocketBehavior
    {
        private string _suffix;
        public MGServer() : this(null)
        {

        }
        public MGServer(string suffix)
        {
            _suffix = suffix ?? String.Empty;
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            MainWindow win = MainWindow.getWindow;

            var text = e.Data;
            win.addText("server recv : " + text + "\n");
            JObject json2 = new JObject();
            json2 = JObject.Parse(text);

            string _cmd = json2["cmd"].ToString();

            switch (_cmd)
            {
                case "userEnter":
                    JObject retJson = new JObject();
                    retJson.Add("cmd", "userEnter");
                    retJson.Add("retMsg", "입장 성공");
                    retJson.Add("ssid", ID);

                    Sessions.SendTo(retJson.ToString(), ID);
                    //Sessions.Broadcast(e.Data);
                    break;
                case "원하는기능":

                    break;
            }
        }
    }
}