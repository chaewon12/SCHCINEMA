using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCHCINEMA
{
    public partial class ScheduleForm : Form
    {
        public ScheduleForm()
        {
            InitializeComponent();
        }
        public ScheduleForm(List<Movie> movieList)
        {
            InitializeComponent();
            this.movieList = movieList;
        }
        List<Movie> movieList = new List<Movie>();
        ConnectDB myDB = new ConnectDB();
        string movie_num = "";
        string movie_title = "";
        string date = "";
        string hour = "";
        string min = "";
        string showtime="";
        string theater_num = "";
        double price = 0.0;
        private void ScheduleForm_Load(object sender, EventArgs e)
        {
            setTheaterCombo();
            setlistView1();
            dateTimePicker1.Value = new System.DateTime(int.Parse(System.DateTime.Now.ToString("yyyy")), int.Parse(System.DateTime.Now.ToString("MM")), int.Parse(System.DateTime.Now.ToString("dd")));
            dateTimePicker1.MinDate = new System.DateTime(int.Parse(System.DateTime.Now.ToString("yyyy")), int.Parse(System.DateTime.Now.ToString("MM")), int.Parse(System.DateTime.Now.ToString("dd")));
        }
        void setPanel()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
        }
        void setTheaterCombo()
        {
            BindingList<object> theaterTypeList = new BindingList<object>();
            theaterTypeList.Add(new { Text = "1관(2D)", Value = "1" });
            theaterTypeList.Add(new { Text = "2관(3D)", Value = "2" });
            theaterTypeList.Add(new { Text = "3관(X-Screen)", Value = "3" }); 
            comboBox3.DataSource = theaterTypeList;
            comboBox3.DisplayMember = "Text";
            comboBox3.ValueMember = "Value";
            comboBox3.SelectedIndex = -1;
        }
        
        public void setlistView1()
        {
            try{
                listView1.Items.Clear();
               
                foreach (Movie movie in movieList)
                {
                    ListViewItem item = new ListViewItem(movie.Num);
                    item.SubItems.Add(movie.Title);
                    item.SubItems.Add(movie.Director);
                    item.SubItems.Add(movie.Actors);
                    listView1.Items.Add(item); 
                }
            }
             catch (Exception ex)
            {
                MessageBox.Show("Error2: " + ex.Message);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            movie_num = listView1.FocusedItem.SubItems[0].Text;
            movie_title = listView1.FocusedItem.SubItems[1].Text;
            textBox1.Text = movie_num;
            textBox2.Text = movie_title;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                if (comboBox3.SelectedIndex>=0)
                {
                    theater_num = comboBox3.SelectedValue.ToString();
                    OracleDataReader result;
                    string query = "select price from MOVIEPRICE where theater_num='" + theater_num + "'";
                    myDB.SelectQueryOracle(query, out result);
                    while (result.Read())
                    {
                        price = Convert.ToDouble(result.GetValue(0));
                    }
                    result.Close();
                    myDB.DisconnectionOracle();
                    textBox3.Text = price.ToString();
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error3: " + ex.Message);
            }
        }
        bool checkSchedule(string showtime)
        {
            DateTime startdt=new DateTime(), enddt = new DateTime();
            startdt = Convert.ToDateTime(showtime).AddHours(-1);  //영화 상영시간 1시간
            enddt = Convert.ToDateTime(showtime).AddHours(1);
            enddt = enddt.AddMinutes(10); //상영관 정리 시간

            string startShowtime = startdt.ToString("yyyy/MM/dd HH:mm");
            string endShowtime=enddt.ToString("yyyy/MM/dd HH:mm");

            string query = "select movie_num from SCHEDULE where (showtime between to_date('" + startShowtime + "','YYYY/MM/DD HH24:MI')" +
                           " AND to_date('" + endShowtime + "','YYYY/MM/DD HH24:MI')) AND theater_num='" + theater_num + "'";

            OracleDataReader result;
            try
            {
                myDB.SelectQueryOracle(query, out result);

                if (!(result.Read()))
                {
                    return false;   //중복 아님
                }
                result.Close();
                myDB.DisconnectionOracle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error4: " + ex.Message);

            }
            return true;    //중복
        }
        private void button1_Click(object sender, EventArgs e)
        {
            movie_num = textBox1.Text;
            date = dateTimePicker1.Value.ToString("yyyy/MM/dd");
            hour = comboBox1.SelectedItem.ToString();
            min = comboBox2.SelectedItem.ToString();
            showtime = date + " " + hour + ":" + min;

            try
            {
                if (checkSchedule(showtime))
                {
                    MessageBox.Show("이미 상영 스케쥴이 존재합니다. 다시 설정해주세요");
                }
                else
                {
                    string query = "insert into SCHEDULE values('" + movie_num + "',to_date('" + showtime
                                + "','YYYY/MM/DD HH24:MI'),'" + theater_num + "','" + price + "')";
                    myDB.QueryOracle(query);
                    MessageBox.Show("시간표 등록 완료.");
                    setPanel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error5: " + ex.Message);
            }
        }

        private void ScheduleForm_Closed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
