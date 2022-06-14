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
        //string _mg1RankTable = "mg1rank";
        //string _mg2RankTable = "mg2rank";
        //string _mg3RankTable = "mg3rank";
        //string _mg4RankTable = "mg4rank";
        //string _mg5RankTable = "mg5rank";

        string _id = "root";
        string _pw = "root";
        string _connectionAddress = "";

        MainWindow win = MainWindow.getWindow;

        //커맨드 리턴
        public MySqlCommand GetCommand(string _query)
        {
            MySqlConnection conn = new MySqlConnection(_connectionAddress);
            conn.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = _query;
            cmd.ExecuteNonQuery();
            return cmd;
        }
        //데이터리더 리턴
        public MySqlDataReader GetDataReader(string _query)
        {
            MySqlConnection conn = new MySqlConnection(_connectionAddress);
            conn.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = _query;
            MySqlDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

            return rdr;
        }

        //유저찾기
        private bool FindUserInfo(string _id)
        {
            bool ret = false;

            try
            {
                string _query = string.Format($"SELECT * FROM {_infoTable}");

                MySqlDataReader table = GetDataReader(_query);

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
            catch (Exception exc)
            {
                win.addText("FindUserInfo !!!!!" + exc.Message + "\n");
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
                string _query = string.Format($"SELECT * FROM {_infoTable}");

                MySqlDataReader table = GetDataReader(_query);

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
            catch (Exception exc)
            {
                win.addText("GetUserInfo !!!!!" + exc.Message + "\n");
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
                    string _query = string.Format($"INSERT INTO {_infoTable} (ssID, ID, nickName,coin1, coin2) VALUES ('{_data["ssID"].ToString()}','{_data["ID"].ToString()}','{""}','{1000}','{10}');");
                    MySqlCommand command = GetCommand(_query);

                    ret = true;
                }
                catch (Exception exc)
                {

                    win.addText("UserInsert !!!!!" + exc.Message + "\n");
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
                string _query = string.Format($"SELECT * FROM {_infoTable}");

                MySqlDataReader table = GetDataReader(_query);

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
            catch (Exception exc)
            {
                win.addText("CheckUserNickName !!!!!" + exc.Message + "\n");
            }

            return ret;
        }
        //닉네임 수정
        private void UserNinameUpdate(string _id, string _nName)
        {

            try
            {
                string _query = string.Format($"UPDATE {_infoTable} SET nickName='{_nName}' WHERE ID='{_id}';");
                MySqlCommand command = GetCommand(_query);


            }
            catch (Exception exc)
            {
                win.addText("UserNinameUpdate !!!!!" + exc.Message + "\n");
            }
        }
        //세션 업데이트
        private void UserSSIDUpdate(string _id, string _ssID)
        {
            try
            {
                string _query = string.Format($"UPDATE {_infoTable} SET ssID='{_ssID}' WHERE ID='{_id}';");

                MySqlCommand command = GetCommand(_query);
            }
            catch (Exception exc)
            {
                win.addText("UserSSIDUpdate !!!!!" + exc.Message + "\n");
            }

        }

        //미니게임 랭킹 가져오기
        private JArray GetMG1TopTRank(string _tableName)
        {

            JArray ret = new JArray();

            try
            {
                string _query = string.Format($"select ID, nickName, Score, dense_rank() over (order by Score desc) as ranking from {_tableName};");

                MySqlDataReader table = GetDataReader(_query);

                JArray rankArr = new JArray();

                while (table.Read())
                {
                    if (rankArr.Count < 10)//최대 열개만 나오게
                    {
                        JObject rkData = new JObject();
                        rkData.Add("ID", table["ID"].ToString());
                        rkData.Add("nickName", table["nickName"].ToString());
                        rkData.Add("Score", table["Score"].ToString());
                        rkData.Add("ranking", table["ranking"].ToString());

                        rankArr.Add(rkData);
                    }
                }

                ret = rankArr;

                table.Close();
            }
            catch (Exception exc)
            {
                win.addText("GetMG1TopTRank !!!!!" + exc.Message + "\n");
            }

            return ret;
        }
        //내 미니게임 랭킹 가져오기
        private JObject GetMG1MyRank(string _tableName, string _id)
        {

            JObject ret = new JObject();

            try
            {
                string _query = string.Format($"SELECT * FROM {_tableName};");
                MySqlDataReader table = GetDataReader(_query);

                while (table.Read())
                {
                    if (_id.Equals(table["ID"].ToString()))
                    {
                        ret.Add("msg", "요청한 유저의 랭킹");
                        ret.Add("ID", table["ID"].ToString());
                        ret.Add("nickName", table["nickName"].ToString());
                        ret.Add("Score", table["Score"].ToString());
                        break;
                    }
                }

                table.Close();
            }
            catch (Exception exc)
            {
                win.addText("GetMG1MyRank !!!!!" + exc.Message + "\n");
            }

            return ret;
        }
        //미니게임 랭킹 추가
        public void MGRankInsert(string _tableName, JObject _data)
        {
            try
            {
                string _query = string.Format($"INSERT INTO {_tableName} (ID, nickName, Score) VALUES ('{_data["ID"].ToString()}','{_data["nickName"].ToString()}','{_data["Score"].ToString()}');");

                MySqlCommand command = GetCommand(_query);
            }
            catch (Exception exc)
            {

                win.addText("MGRankInsert !!!!!" + exc.Message + "\n");
            }

        }
        //미니게임 랭킹 업데이트
        private void MGRankUpdate(string _tableName, JObject _data)
        {
            try
            {
                string _query = string.Format($"UPDATE {_tableName} SET Score='{_data["Score"].ToString()}' WHERE ID='{_data["ID"].ToString()}';");

                MySqlCommand command = GetCommand(_query);
            }
            catch (Exception exc)
            {
                win.addText("MGRankUpdate !!!!!" + exc.Message + "\n");
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
                    break;

                case "ReadRanking":
                    JArray rktmp = GetMG1TopTRank(json2["MG_NAME"].ToString());
                    rktmp.Add(GetMG1MyRank(json2["MG_NAME"].ToString(), json2["ID"].ToString()));

                    Sessions.SendTo(rktmp.ToString(), ID);

                    break;
                case "UpdateRanking":

                    if (GetMG1MyRank(json2["MG_NAME"].ToString(), json2["ID"].ToString()).Count > 0) //내 정보가 이미 있을때
                    { //랭킹 점수 업데이트
                        MGRankUpdate(json2["MG_NAME"].ToString(), json2);
                    }
                    else
                    {//랭킹 추가
                        MGRankInsert(json2["MG_NAME"].ToString(), json2);
                    }

                    JArray allRankArr = GetMG1TopTRank(json2["MG_NAME"].ToString());
                    JObject myRkData = GetMG1MyRank(json2["MG_NAME"].ToString(), json2["ID"].ToString());
                    int rankIdx = -1;

                    for (int i = 0; i < allRankArr.Count; i++)
                    {
                        if (allRankArr[i]["ID"].ToString().Equals(json2["ID"].ToString()))
                        {
                            rankIdx = i + 1;
                        }
                    }

                    myRkData.Add("ranking", rankIdx);

                    allRankArr.Add(myRkData);

                    Sessions.SendTo(allRankArr.ToString(), ID);

                    break;
                case "원하는기능":

                    break;
            }
        }
    }
}