using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SCHCINEMA
{
    public partial class PaymentForm : Form
    {
        public PaymentForm()
        {
            InitializeComponent();
        }
        Movie movie;
        User loginUser;
        Schedule schedule;
        List<string> selectedList = new List<string>();
        ConnectDB myDB = new ConnectDB();
        private double payment =0.0;
       
        public void setPaymentForm(Movie moive, User loginUser, Schedule schedule, List<string> selectedList)
        {
            this.movie = moive;
            this.loginUser = loginUser;
            this.schedule = schedule;
            this.selectedList = selectedList;
        }
        double getDiscount()
        {
            double discount = 1;
            string query = "select rate from discountrate where member_level='" + loginUser.Level + "'";
            OracleDataReader result;
            try
            {
                myDB.SelectQueryOracle(query, out result);
                while (result.Read())
                {
                    discount = Convert.ToDouble(result.GetValue(0));
                }
                result.Close();
                myDB.DisconnectionOracle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return discount;
        }
        private void PaymentForm_Load(object sender, EventArgs e)
        {
            int count=0;
            string seatlist=null;
            double discount = getDiscount();
            
            label6.Text = movie.Title;
            label7.Text = schedule.Showtime;
            label8.Text = schedule.Theaternum+"관("+ schedule.Theaterform+")";
            
            foreach(string seat in selectedList)
            {
                if (count == 0)
                    seatlist += seat;
                else
                    seatlist +=(","+seat);
                count++;
            }
            label9.Text = seatlist;
            label10.Text = "총 "+count.ToString()+"석";
            label11.Text = loginUser.Level + " 등급 할인 적용";
            payment = Convert.ToInt32((schedule.Price - schedule.Price * (discount / 100)) *count);
            label12.Text="합계 "+payment.ToString() + "원";

        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string date = DateTime.Now.ToString("yyyyMMdd");
                string time = DateTime.Now.ToString("HHmm");
                foreach (string seat in selectedList)
                {
                    string zone = seat.Substring(0, 1);
                    string seatnum = seat.Substring(1);
                    string query = "INSERT INTO RESERVATIONSEAT VALUES('" + movie.Num + "',to_date('" + schedule.Showtime
                            + "','YYYY/MM/DD HH24:MI'),'" + schedule.Theaternum + "','" + zone + "','" + seatnum + "','" + loginUser.Num
                            + "',to_date(sysdate,'YYYY/MM/DD HH24:MI'),'" + payment.ToString() + "','" + schedule.Theaternum + "-" +
                            date + "-" + time + "-'||SEQ_RESERVATION_NUM.nextval)";
                    myDB.QueryOracle(query);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            MessageBox.Show("예매가 완료되었습니다. \r 즐거운 관람되세요!");
            this.Close();
            

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                textBox1.Text = loginUser.Card;
            }
            else
            {
                textBox1.Text = " ";
            }
        }

        private void PaymentForm_Closed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
