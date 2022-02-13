using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SCHCINEMA
{
    public partial class ReservForm : Form
    {
        public ReservForm()
        {
            InitializeComponent();
        }

        Movie movie;
        User loginUser;
        Schedule schedule;
        ConnectDB myDB = new ConnectDB();

        List<CheckBox> T1_Abox = new List<CheckBox>();
        List<CheckBox> T1_Bbox = new List<CheckBox>();
        List<CheckBox> T2_Abox = new List<CheckBox>();
        List<CheckBox> T2_Bbox = new List<CheckBox>();
        List<CheckBox> T2_Cbox = new List<CheckBox>();
        List<CheckBox> T3_Abox = new List<CheckBox>();
        List<CheckBox> T3_Bbox = new List<CheckBox>();


        List<string> selectedList = new List<string>();
       
        public void setReservForm(Movie movie,User loginUser)
        {
            this.movie = movie;
            this.loginUser = loginUser;
            pictureBox1.Image = movie.Image;
        }

        public void setTimetable()
        {
            OracleDataReader result;
            ListViewItem item;
            try
            {
                listView4.Items.Clear();
                listView4.View = View.Details;
                string query = "select to_char(showtime,'YYYY/MM/DD HH24:MI'),theater_num from SCHEDULE where movie_num='" + movie.Num + "'";
                myDB.SelectQueryOracle(query, out result);
                while (result.Read())
                {
                    item = new ListViewItem(result.GetValue(0).ToString());
                    item.SubItems.Add(result.GetValue(1).ToString());
                    listView4.Items.Add(item);
                }
                result.Close();
                myDB.DisconnectionOracle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        void setboxList(List<CheckBox> CkBox, string zone, int column, int row)
        {
            CkBox.Clear();
            int i, j;
            for (i = 0; i < row; i++)
            {
                for (j = 0; j < column; j++)
                {
                    CheckBox box = new CheckBox();
                    int r = i + 1;
                    int c = j + 1;
                    box.Name = zone + r.ToString() + "-" + c.ToString();   //A1-1
                    box.Text = r.ToString() + "-" + c.ToString();   //1-1
                    box.Appearance = System.Windows.Forms.Appearance.Button;
                    box.AutoSize = true;
                    box.BackColor = System.Drawing.Color.LightSteelBlue;
                    box.Dock = System.Windows.Forms.DockStyle.Fill;
                    box.FlatAppearance.BorderSize = 0;
                    box.FlatAppearance.CheckedBackColor = System.Drawing.Color.Tomato;
                    box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    box.Size = new System.Drawing.Size(58, 36);
                    box.TabIndex = 0;
                    box.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    box.UseVisualStyleBackColor = false;
                    box.CheckedChanged += Common_CheckedChanged;
                    box.EnabledChanged += Common_EnabledChanged;

                    CkBox.Add(box);
                }
            }

        }
        
        void setChBoxTabel(TableLayoutPanel tablepanel, List<CheckBox> checkBoxlist)
        {
            tablepanel.Controls.Clear();
            int i, j, k = 0;
            int column = tablepanel.ColumnCount;
            int row = tablepanel.RowCount;

            while (k < checkBoxlist.Count)
            {
                for (i = 0; i < row; i++)
                {
                    for (j = 0; j < column; j++)
                    {
                        CheckBox box = checkBoxlist[k++];
                        tablepanel.Controls.Add(box, j, i);
                    }
                }
            }

        }
        //체크박스 이름으로 찾아 비활성화
        void boxEnable(List<CheckBox> boxList, List<string> boxnameList)
        {
            foreach (CheckBox box in boxList)
            {
                if (boxnameList.Contains(box.Name))
                {
                    box.Enabled = false;
                }
                else
                {
                    box.Enabled = true;
                }
            }
        }
        void setT1panel()
        {
            setboxList(T1_Abox, "A", 3, 10);
            setboxList(T1_Bbox, "B", 3, 10);
            setChBoxTabel(tableLayoutPanel1, T1_Abox);
            setChBoxTabel(tableLayoutPanel2, T1_Bbox);

            string query = "select seat_zone, seat_num from RESERVATIONSEAT where movie_num ='" + movie.Num 
                            + "' AND showtime=to_date('"+ schedule.Showtime +"','YYYY/MM/DD HH24:MI')" +  " AND theater_num='1'";
            OracleDataReader result;
            List<string> reservSeatList = new List<string>();
            try
            {
                myDB.SelectQueryOracle(query, out result);
                while (result.Read())
                {
                    reservSeatList.Add(result.GetValue(0).ToString() + result.GetValue(1).ToString());
                }
                result.Close();
                myDB.DisconnectionOracle();

                boxEnable(T1_Abox, reservSeatList);
                boxEnable(T1_Bbox, reservSeatList);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message); 
            }
            this.T1panel.Visible = true;
            this.T2panel.Visible = false;
            this.T3panel.Visible = false;
        }
        void setT2panel()
        {
            setboxList(T2_Abox, "A", 2, 10);
            setboxList(T2_Bbox, "B", 2, 10);
            setboxList(T2_Cbox, "C", 2, 10);
            setChBoxTabel(tableLayoutPanel3, T2_Abox);
            setChBoxTabel(tableLayoutPanel4, T2_Bbox);
            setChBoxTabel(tableLayoutPanel5, T2_Cbox);

            string query = "select seat_zone, seat_num from RESERVATIONSEAT where movie_num ='" + movie.Num
                            + "' AND showtime=to_date('" + schedule.Showtime + "','YYYY/MM/DD HH24:MI')" + " AND theater_num='2'";
            OracleDataReader result;
            List<string> reservSeatList = new List<string>();
            try
            {
                myDB.SelectQueryOracle(query, out result);
                while (result.Read())
                {
                    reservSeatList.Add(result.GetValue(0).ToString() + result.GetValue(1).ToString());
                }
                result.Close();
                myDB.DisconnectionOracle();

                boxEnable(T2_Abox, reservSeatList);
                boxEnable(T2_Bbox, reservSeatList);
                boxEnable(T2_Cbox, reservSeatList);
           }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            this.T1panel.Visible = false;
            this.T2panel.Visible = true;
            this.T3panel.Visible = false;
        }
        void setT3panel()
        {
            setboxList(T3_Abox, "A", 3, 8);
            setboxList(T3_Bbox, "B", 3, 8);
            setChBoxTabel(tableLayoutPanel6, T3_Abox);
            setChBoxTabel(tableLayoutPanel7, T3_Bbox);

            string query = "select seat_zone, seat_num from RESERVATIONSEAT where movie_num ='" + movie.Num
                            + "' AND showtime=to_date('" + schedule.Showtime + "','YYYY/MM/DD HH24:MI')" + " AND theater_num='3'";
            OracleDataReader result;
            List<string> reservSeatList = new List<string>();
            try
            {
                myDB.SelectQueryOracle(query, out result);
                while (result.Read())
                {
                    reservSeatList.Add(result.GetValue(0).ToString() + result.GetValue(1).ToString());
                }
                result.Close();
                myDB.DisconnectionOracle();

                boxEnable(T3_Abox, reservSeatList);
                boxEnable(T3_Bbox, reservSeatList);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            this.T1panel.Visible = false;
            this.T2panel.Visible = false;
            this.T3panel.Visible = true;
        }
        private void ReservForm_Load(object sender, EventArgs e)
        {
            setTimetable();
        }

        private void Common_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender; //캐스팅	
            string b = checkBox.Name;
            if (checkBox.Checked)   //선택되어 있으면 리스트에 저장
            {
                selectedList.Add(b);
            }
            else    //해제되면 리스트에서 삭제
            {
                selectedList.Remove(b);
            }
        }
        private void Common_EnabledChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender; //캐스팅	
            if(checkBox.Enabled)
                checkBox.BackColor= System.Drawing.Color.LightSteelBlue;
            else
                checkBox.BackColor = System.Drawing.Color.Gray;
        }

        private void listView4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            selectedList.Clear();
            OracleDataReader result;
            OracleDataReader result2;
            string showtime = listView4.FocusedItem.SubItems[0].Text;
            string theaternum = listView4.FocusedItem.SubItems[1].Text;
            double price=0.0;
            string theaterform=null;
            string query = "select price from MOVIEPRICE where theater_num='" + theaternum + "'";
            string query2 = "select theater_form from THEATER where theater_num='" + theaternum + "'";
            try
            {
                myDB.SelectQueryOracle(query, out result);
                while (result.Read())
                {
                    price = Convert.ToDouble(result.GetValue(0));
                }

                myDB.SelectQueryOracle(query2, out result2);
                while (result2.Read())
                {
                    theaterform = result2.GetValue(0).ToString();                  
                }

                schedule = new Schedule(movie.Num, showtime, theaternum, price, theaterform);

                result.Close();
                result2.Close();
                myDB.DisconnectionOracle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            label2.Text = movie.Title;
            label3.Text = schedule.Showtime;
            label4.Text = schedule.Theaternum + " 관("+ schedule.Theaterform+")";
            if (schedule.Theaternum.Equals("1"))
            {
                setT1panel();
            }
            else if (schedule.Theaternum.Equals("2"))
            {
                setT2panel();
            }
            else if (schedule.Theaternum.Equals("3"))
            {
                setT3panel();
            }


        }
        //예매하기
        private void button2_Click(object sender, EventArgs e)
        {
            selectedList.Sort();
            PaymentForm form4 = new PaymentForm();
            form4.setPaymentForm(movie, loginUser, schedule, selectedList);
            DialogResult result= form4.ShowDialog();
            if (result == DialogResult.OK)
                this.Close();
            
        }
    }
}
