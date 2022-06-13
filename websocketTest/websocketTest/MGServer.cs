using System;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;
using System.Text;

namespace websocketTest
{
    public struct userData
    {
        public string idx;
        public string ssID;
        public string ID;
        public string nickName;
        public string coin1;
        public string coin2;
    }

    public class MGServer : WebSocketBehavior
    {
        string _server = "localhost";
        int _port = 3306;
        string _database = "userinfo";
        string _infoTable = "info";

        string _id = "root";
        string _pw = "root";
        string _connectionAddress = "";

        MainWindow win = MainWindow.getWindow;


        //유저찾기
        private bool FindUserInfo(string _id)
        {
            bool ret = false;
            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    string selectQuery = string.Format($"SELECT * FROM {_infoTable}");

                    MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                    MySqlDataReader table = command.ExecuteReader();

                    while (table.Read())
                    {
                        if (_id.Equals(table["ID"].ToString()))
                        {
                            ret = true;
                            break;
                        }
                    }

                    table.Close();
                }
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }

            return ret;
        }
        //유저 데이터 받아오기
        private userData GetUserInfo(string _id)
        {
            userData _info = new userData();

            _info.idx = "";


            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    string selectQuery = string.Format($"SELECT * FROM {_infoTable}");

                    MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                    MySqlDataReader table = command.ExecuteReader();

                    while (table.Read())
                    {
                        if (_id.Equals(table["ID"].ToString()))
                        {
                            _info.idx = table["idx"].ToString();
                            _info.ssID = table["ssID"].ToString();
                            _info.ID = table["ID"].ToString();
                            _info.nickName = table["nickName"].ToString();
                            _info.coin1 = table["coin1"].ToString();
                            _info.coin2 = table["coin2"].ToString();

                            return _info;
                        }
                    }

                    table.Close();
                }
            }
            catch (Exception exc)
            {
                win.addText("!!!!!" + exc.Message + "\n");
            }

            return _info;
        }

        // 유저생성
        public bool UserInsert(JObject _data)
        {
            bool ret = false;

            if (_data["ID"].ToString() == "")
            {
                ret = false;

                win.addText("아이디 공백!!!!!" + "\n");
            }
            else
            {
                try
                {
                    using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                    {
                        mysql.Open();
                        string insertQuery = string.Format($"INSERT INTO {_infoTable} (ssID, ID, nickName,coin1, coin2) VALUES ('{_data["ssID"].ToString()}','{_data["ID"].ToString()}','{""}','{1000}','{10}');");

                        MySqlCommand command = new MySqlCommand(insertQuery, mysql);

                        if (command.ExecuteNonQuery() != 1)
                        {
                            //MessageBox.Show("Failed to insert data.");
                            ret = false;
                        }

                        //selectTable();

                        ret = true;
                    }
                }
                catch (Exception exc)
                {

                    win.addText("!!!!!" + exc.Message + "\n");
                    ret = false;
                }
            }
            return ret;
        }
        // 유저닉네임 체크
        private bool CheckUserNickName(string _id)
        {
            bool ret = false;
            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    string selectQuery = string.Format($"SELECT * FROM {_infoTable}");

                    MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                    MySqlDataReader table = command.ExecuteReader();

                    while (table.Read())
                    {
                        if (_id.Equals(table["ID"].ToString()) && table["nickName"].ToString() != "")
                        {
                            ret = true;
                            break;
                        }
                    }

                    table.Close();
                }
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }

            return ret;
        }

        //닉네임 수정
        private void UserNinameUpdate(string _id, string _nName)
        {
            try
            {

                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();

                    string updateQuery = string.Format($"UPDATE {_infoTable}  SET nickName = '{_nName}' WHERE ID={_id};");

                    MySqlCommand command = new MySqlCommand(updateQuery, mysql);

                    if (command.ExecuteNonQuery() != 1)
                    {
                    }

                }
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }
        }
        //세션 업데이트
        private void UserSSIDUpdate(string _id, string _ssID)
        {
            try
            {

                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();

                    string updateQuery = string.Format($"UPDATE {_infoTable}  SET ssID = '{_ssID}' WHERE ID={_id};");

                    MySqlCommand command = new MySqlCommand(updateQuery, mysql);

                    if (command.ExecuteNonQuery() != 1)
                    {
                    }

                }
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }
        }


        private string _suffix;
        public MGServer() : this(null)
        {

        }
        public MGServer(string suffix)
        {
            _suffix = suffix ?? String.Empty;

            _connectionAddress = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", _server, _port, _database, _id, _pw);
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            var text = e.Data;

            win.addText("server recv : " + text + "\n");
            JObject json2 = new JObject();
            json2 = JObject.Parse(text);

            string _cmd = json2["cmd"].ToString();

            switch (_cmd)
            {
                case "ssEnter":

                    if (FindUserInfo(json2["ID"].ToString()) == true)
                    { //기존 유저인지 체크

                        win.addText("기존 유저 접속 " + json2["ID"].ToString() + "\n");
                        UserSSIDUpdate(json2["ID"].ToString(), ID);

                    }
                    else
                    { //신규 유저일경우
                      //유저 디비에 추가하고
                        win.addText("신규 유저 접속 " + json2["ID"].ToString() + "\n");

                        json2.Add("ssID", ID);
                        UserInsert(json2);

                    }


                    ////닉네임 설정여부 체크 
                    //if (CheckUserNickName(json2["ID"].ToString()) == false)
                    //{//닉네임 설정 안됨 //클라한테 닉네임 설정하라고 패킷 보내줘야함

                    //    JObject SetUserNickNameData = new JObject();
                    //    SetUserNickNameData.Add("cmd", "SetUserNickName");
                    //    SetUserNickNameData.Add("retMsg", "닉네임을 설정해주세요");
                    //    SetUserNickNameData.Add("ssID", ID);
                    //    SetUserNickNameData.Add("ID", json2["ID"].ToString());
                    //    Sessions.SendTo(SetUserNickNameData.ToString(), ID);

                    //    return;
                    //}


                    //닉네임까지 이상 없을경우 유저 데이터 클라로 넘겨주기
                    userData _info = new userData();
                    _info = GetUserInfo(json2["ID"].ToString());

                    JObject _userData = new JObject();
                    _userData.Add("cmd", "LoginOK");
                    _userData.Add("retMsg", "로그인에 성공했습니다.");
                    _userData.Add("idx", _info.idx);
                    _userData.Add("ssID", _info.ssID);
                    _userData.Add("ID", _info.ID);
                    _userData.Add("nickName", _info.nickName);
                    _userData.Add("coin1", _info.coin1);
                    _userData.Add("coin2", _info.coin2);
                    win.addText("기존 유저 접속 " + _userData.ToString() + "\n");

                    Sessions.SendTo(_userData.ToString(), ID);

                    break;


                case "userEnter":

                    JObject retJson = new JObject();

                    if (FindUserInfo(json2["ID"].ToString()) == true)
                    {
                        retJson.Add("cmd", "userEnter");
                        retJson.Add("retMsg", "입장 성공");
                        retJson.Add("ssid", ID);
                    }
                    else
                    {
                        if (UserInsert(json2) == true)
                        {
                            retJson.Add("cmd", "userEnter");
                            retJson.Add("retMsg", "가입 성공");
                            retJson.Add("ssid", ID);

                        }
                        else
                        {
                            retJson.Add("cmd", "userEnter");
                            retJson.Add("retMsg", "가입 실패");
                            retJson.Add("ssid", ID);

                        }
                    }

                    Sessions.SendTo(retJson.ToString(), ID);



                    //Sessions.Broadcast(e.Data);
                    break;
                case "원하는기능":

                    break;
            }
        }
    }
}