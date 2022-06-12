using System;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;


namespace websocketTest
{
    public class MGServer : WebSocketBehavior
    {
        string _server = "localhost";
        int _port = 3306;
        string _database = "userinfo";
        string _infoTable = "info";

        string _id = "root";
        string _pw = "root";
        string _connectionAddress = "";


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
        // 유저생성
        public bool UserInsert(JObject _data)
        {
            bool ret = false;

            if (_data["ID"].ToString() == "")
            {
                //MessageBox.Show("공백입니다.");
                ret = false;
            }
            else
            {
                if (FindUserInfo(_data["ID"].ToString()) == false)
                {
                    try
                    {
                        using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                        {
                            mysql.Open();
                            string insertQuery = string.Format($"INSERT INTO {_infoTable} (ID, nickName, coin1, coin2) VALUES ('{_data["ID"].ToString()}', '{_data["nickName"].ToString()}','{1000}','{10}');");

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
                        //MessageBox.Show(exc.Message);

                        ret = false;
                    }
                }
                else
                {
                    //MessageBox.Show("중복된 id입니다.");
                    ret = false;
                }
            }
            return ret;
        }

        ////수정
        //private void buttonUpdate_Click()
        //{
        //    try
        //    {

        //        using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
        //        {
        //            mysql.Open();
        //            int pos = listViewPhoneBook.SelectedItems[0].Index;
        //            int index = Convert.ToInt32(listViewPhoneBook.Items[pos].Text);
        //            string updateQuery = string.Format($"UPDATE {_infoTable}  SET ID = '{textBoxNickName.Text}', nickName = '{textBoxID.Text}' WHERE idx={index};");

        //            MySqlCommand command = new MySqlCommand(updateQuery, mysql);

        //            if (command.ExecuteNonQuery() != 1)
        //            {
        //                MessageBox.Show("Failed to delete data.");
        //            }

        //            textBoxID.Text = "";
        //            textBoxNickName.Text = "";
        //            textBoxcoin1.Text = "";
        //            textBoxcoin2.Text = "";

        //            selectTable();
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        //MessageBox.Show(exc.Message);
        //    }
        //}

        ////삭제

        //private void buttonDelete_Click()
        //{
        //    try
        //    {
        //        using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
        //        {
        //            mysql.Open();
        //            int pos = listViewPhoneBook.SelectedItems[0].Index;
        //            int index = Convert.ToInt32(listViewPhoneBook.Items[pos].Text);

        //            string deleteQuery = string.Format($"DELETE FROM {_infoTable} WHERE idx={index};");

        //            MySqlCommand command = new MySqlCommand(deleteQuery, mysql);

        //            if (command.ExecuteNonQuery() != 1)
        //            {
        //                MessageBox.Show("Failed to delete data.");
        //            }

        //            textBoxID.Text = "";
        //            textBoxNickName.Text = "";
        //            textBoxcoin1.Text = "";
        //            textBoxcoin2.Text = "";

        //            selectTable();
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        MessageBox.Show(exc.Message);
        //    }
        //}

        ////조회
        //private void buttonSelect_Click()
        //{
        //    selectTable();
        //}

        ////private void selectTable()
        ////{
        ////    try
        ////    {
        ////        using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
        ////        {
        ////            mysql.Open();
        ////            string selectQuery = string.Format($"SELECT * FROM {_infoTable}");

        ////            MySqlCommand command = new MySqlCommand(selectQuery, mysql);
        ////            MySqlDataReader table = command.ExecuteReader();

        ////            listViewPhoneBook.Items.Clear();

        ////            while (table.Read())
        ////            {
        ////                ListViewItem item = new ListViewItem();
        ////                item.Text = table["idx"].ToString();
        ////                item.SubItems.Add(table["ID"].ToString());
        ////                item.SubItems.Add(table["nickName"].ToString());
        ////                item.SubItems.Add(table["coin1"].ToString());
        ////                item.SubItems.Add(table["coin2"].ToString());

        ////                listViewPhoneBook.Items.Add(item);
        ////            }

        ////            table.Close();
        ////        }
        ////    }
        ////    catch (Exception exc)
        ////    {
        ////        //MessageBox.Show(exc.Message);
        ////    }
        ////}



        //private void listViewPhoneBook_SelectedIndexChanged()
        //{
        //    ListView listview = sender as ListView;

        //    int index = listview.FocusedItem.Index;
        //    textBoxID.Text = listview.Items[index].SubItems[1].Text;
        //    textBoxNickName.Text = listview.Items[index].SubItems[2].Text;
        //    textBoxcoin1.Text = listview.Items[index].SubItems[3].Text;
        //    textBoxcoin2.Text = listview.Items[index].SubItems[4].Text;
        //}

        //private void buttonReset_Click()
        //{
        //    textBoxID.Text = "";
        //    textBoxNickName.Text = "";
        //    textBoxcoin1.Text = "";
        //    textBoxcoin2.Text = "";
        //}

        //private void Form1_Load()
        //{

        //}





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