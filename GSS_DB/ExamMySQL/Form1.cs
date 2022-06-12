using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ExamMySQL
{
    public partial class Form1 : Form
    {
        string _server = "localhost";
        int _port = 3306;
        string _database = "userinfo";
        string _infoTable = "info";

        string _id = "root";
        string _pw = "root";
        string _connectionAddress = "";

        public Form1()
        {
            InitializeComponent();

            _connectionAddress = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", _server, _port, _database, _id, _pw);
        }

        //정보 입력
        private void buttonInsert_Click(object sender, EventArgs e)
        {
            if (textBoxID.Text == "")
            {
                MessageBox.Show("공백입니다.");
            }
            else
            {
                if (FindUserInfo(textBoxID.Text) == true)
                {
                    try
                    {
                        using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                        {
                            mysql.Open();
                            string insertQuery = string.Format($"INSERT INTO {_infoTable} (ID, nickName, coin1, coin2) VALUES ('{textBoxID.Text}', '{textBoxNickName.Text}','{1000}','{10}');");

                            MySqlCommand command = new MySqlCommand(insertQuery, mysql);

                            if (command.ExecuteNonQuery() != 1)
                            {
                                MessageBox.Show("Failed to insert data.");
                            }

                            textBoxID.Text = "";
                            textBoxNickName.Text = "";
                            textBoxcoin1.Text = "";
                            textBoxcoin2.Text = "";

                            selectTable();
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }
                }
                else 
                {
                    MessageBox.Show("중복된 id입니다.");
                }
            }
        }

        //수정
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            try
            {

                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    int pos = listViewPhoneBook.SelectedItems[0].Index;
                    int index = Convert.ToInt32(listViewPhoneBook.Items[pos].Text);
                    string updateQuery = string.Format($"UPDATE {_infoTable}  SET ID = '{textBoxNickName.Text}', nickName = '{textBoxID.Text}' WHERE idx={index};");

                    MySqlCommand command = new MySqlCommand(updateQuery, mysql);

                    if (command.ExecuteNonQuery() != 1)
                    {
                        MessageBox.Show("Failed to delete data.");
                    }

                    textBoxID.Text = "";
                    textBoxNickName.Text = "";
                    textBoxcoin1.Text = "";
                    textBoxcoin2.Text = "";

                    selectTable();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        //삭제

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    int pos = listViewPhoneBook.SelectedItems[0].Index;
                    int index = Convert.ToInt32(listViewPhoneBook.Items[pos].Text);

                    string deleteQuery = string.Format($"DELETE FROM {_infoTable} WHERE idx={index};");

                    MySqlCommand command = new MySqlCommand(deleteQuery, mysql);

                    if (command.ExecuteNonQuery() != 1)
                    {
                        MessageBox.Show("Failed to delete data.");
                    }

                    textBoxID.Text = "";
                    textBoxNickName.Text = "";
                    textBoxcoin1.Text = "";
                    textBoxcoin2.Text = "";

                    selectTable();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        //조회
        private void buttonSelect_Click(object sender, EventArgs e)
        {
            selectTable();
        }

        private void selectTable()
        {
            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    string selectQuery = string.Format($"SELECT * FROM {_infoTable}");

                    MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                    MySqlDataReader table = command.ExecuteReader();

                    listViewPhoneBook.Items.Clear();

                    while (table.Read())
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = table["idx"].ToString();
                        item.SubItems.Add(table["ID"].ToString());
                        item.SubItems.Add(table["nickName"].ToString());
                        item.SubItems.Add(table["coin1"].ToString());
                        item.SubItems.Add(table["coin2"].ToString());

                        listViewPhoneBook.Items.Add(item);
                    }

                    table.Close();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private bool FindUserInfo(string _id)
        {
            bool ret = true;
            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    string selectQuery = string.Format($"SELECT * FROM {_infoTable}");

                    MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                    MySqlDataReader table = command.ExecuteReader();

                    listViewPhoneBook.Items.Clear();

                    while (table.Read())
                    {
                        if (_id.Equals(table["ID"].ToString()))
                        {
                            ret = false;
                            break;
                        }
                    }

                    table.Close();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            return ret;
        }


        private void listViewPhoneBook_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView listview = sender as ListView;

            int index = listview.FocusedItem.Index;
            textBoxID.Text = listview.Items[index].SubItems[1].Text;
            textBoxNickName.Text = listview.Items[index].SubItems[2].Text;
            textBoxcoin1.Text = listview.Items[index].SubItems[3].Text;
            textBoxcoin2.Text = listview.Items[index].SubItems[4].Text;
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            textBoxID.Text = "";
            textBoxNickName.Text = "";
            textBoxcoin1.Text = "";
            textBoxcoin2.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
