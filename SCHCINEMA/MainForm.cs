using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using System.Security.Cryptography;
using System.Threading;

namespace SCHCINEMA
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            setlistView2();
        }
        
        ConnectDB myDB=new ConnectDB();       
        string SW = "`~!?@$%^&*-_=+";                    // 비밀번호 조합 - 특수문자
        string eW = "abcdefghijklmnopqrstuvwxyz";       // 비밀번호 조합 - 영어 소문자
        string EW = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";       // 비밀번호 조합 - 영어 대문자
        string NW = "0123456789";                       // 비밀번호 조합 - 숫자
        bool idCheck = false;    //아이디 중복 체크

        User loginUser;
        List<Movie> movieList = new List<Movie>();
        List<Movie> notscreenmovieList = new List<Movie>();
        List<Movie> screenmovieList = new List<Movie>();

        //아이디 중복 확인 함수
        public bool CheckIDOverlapping(string id)
        {
            OracleDataReader result;
            try
            {
                string query = "select * from MEMBER where member_id ='"+id+"'";
                myDB.SelectQueryOracle(query,out result);

                if (!(result.Read())){
                    return false;   //중복 아님
                }
                result.Close();
                myDB.DisconnectionOracle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message); //에러 메세지 
            }
            return true;    //중복
        }
        //문자 조합 검사 함수
        private bool checkWord(string word, string text)
        {
            foreach (var item in word)
            {
                if (text.Contains(item))    // 특정 단어가 포함되어있는 지 검사
                    return true;            
            }
            return false;                   
        }

        //DB에 새 회원 저장 함수
        public void saveUser(string id, string pw, string name, string phone, string card )
        {
            try
            {
                string query = "INSERT INTO MEMBER VALUES('U'||SEQ_MEMBER_NUM.nextval,";
                query = query + "'" + id + "','" + pw + "','" + name + "','" + phone + "',DEFAULT,'"+ card + "',DEFAULT)";
                myDB.QueryOracle(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message); //에러 메세지 
            }


        }
        private byte[] imageToByteArray(Image img)
        {
            ImageConverter imageConverter = new ImageConverter();
            byte[] b = (byte[])imageConverter.ConvertTo(img, typeof(byte[]));
            return b;
        }

        private Image ByteArrayToImage(byte[] bytes)
        {
            ImageConverter imageConverter = new ImageConverter();
            Image img = (Image)imageConverter.ConvertFrom(bytes);
            return img;
        }
        public void setMoiveList(List<Movie> movieList)
        {
            movieList.Clear();
            try
            {
                String query = "SELECT * FROM MOVIE order by movie_num";
                OracleDataReader result;
                myDB.SelectQueryOracle(query, out result);

                while (result.Read())
                {
                    Image img;
                    string num = result.GetValue(0).ToString();
                    string title = result.GetValue(1).ToString();
                    string director = result.GetValue(2).ToString();
                    string actors = result.GetValue(3).ToString();
                    if (result.GetValue(4).ToString().Equals(""))
                    {
                        img = imageList1.Images[0]; //기본 이미지
                    }
                    else
                    {
                        img = ByteArrayToImage((byte[])result.GetValue(4));
                    }

                    Movie movie = new Movie(num, title, director, actors, img);
                    movieList.Add(movie);
                }
                result.Close();
                myDB.DisconnectionOracle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void setNotScreenList()
        {
            notscreenmovieList.Clear();
            try
            {
                String query = "SELECT * FROM MOVIE WHERE movie_num NOT IN (SELECT movie_num FROM SCHEDULE) order by movie_num";
                OracleDataReader result;
                myDB.SelectQueryOracle(query, out result);

                while (result.Read())
                {
                    Image img;
                    string num = result.GetValue(0).ToString();
                    string title = result.GetValue(1).ToString();
                    string director = result.GetValue(2).ToString();
                    string actors = result.GetValue(3).ToString();
                    if (result.GetValue(4).ToString().Equals(""))
                    {
                        img = imageList1.Images[0]; //기본 이미지
                    }
                    else
                    {
                        img = ByteArrayToImage((byte[])result.GetValue(4));
                    }

                    Movie movie = new Movie(num, title, director, actors,img);
                    notscreenmovieList.Add(movie);
                }
                result.Close();
                myDB.DisconnectionOracle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message); 
            }
        }
        private void setScreenList()
        {
            screenmovieList.Clear();
            try
            {
                String query = "SELECT * FROM MOVIE WHERE movie_num IN (SELECT movie_num FROM SCHEDULE) order by movie_num";
                OracleDataReader result;
                myDB.SelectQueryOracle(query, out result);

                while (result.Read())
                {
                    Image img;
                    string num = result.GetValue(0).ToString();
                    string title = result.GetValue(1).ToString();
                    string director = result.GetValue(2).ToString();
                    string actors = result.GetValue(3).ToString();
                    if (result.GetValue(4).ToString().Equals(""))
                    {
                        img = imageList1.Images[0]; //기본 이미지
                    }
                    else
                    {
                        img = ByteArrayToImage((byte[])result.GetValue(4));
                    }

                    Movie movie = new Movie(num, title, director, actors, img);
                    screenmovieList.Add(movie);
                }
                result.Close();
                myDB.DisconnectionOracle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void setlistView1()
        {
            NScreenM_imgList.Images.Clear();
            listView1.Clear();
            setNotScreenList(); //DB에서 영화 정보 받아 리스트 생성
            foreach (Movie movie in notscreenmovieList)
            {
                NScreenM_imgList.Images.Add(movie.Image);   //이미지 리스트
            }
            int index = 0;
            foreach (Movie movie in notscreenmovieList)
            {
                ListViewItem item = new ListViewItem(movie.Title, index);
                listView1.Items.Add(item);
                index++;
            }
        }
        private void setlistView2()
        {
            ScreenM_imgList.Images.Clear();
            listView2.Clear();
            setScreenList(); //DB에서 영화 정보 받아 리스트 생성
            foreach (Movie movie in screenmovieList)
            {
                ScreenM_imgList.Images.Add(movie.Image);   //이미지 리스트
            }
            int index = 0;
            foreach (Movie movie in screenmovieList)
            {
                ListViewItem item = new ListViewItem(movie.Title, index);
                listView2.Items.Add(item);
                index++;
            }
        }
        private void setlistView3()
        {
            ScreenM_imgList.Images.Clear();
            listView3.Clear();
            setScreenList(); //DB에서 영화 정보 받아 리스트 생성
            foreach (Movie movie in screenmovieList)
            {
                ScreenM_imgList.Images.Add(movie.Image);   //이미지 리스트
            }
            int index = 0;
            foreach (Movie movie in screenmovieList)
            {
                ListViewItem item = new ListViewItem(movie.Title, index);
                listView3.Items.Add(item);
                index++;
            }
        }
        void setlistView4()
        {
            sortColumn = -1;
            setMoiveList(movieList);
            OracleDataReader result;
            ListViewItem item;
            try
            {
                listView4.Items.Clear();
                string query1 = "select movie_num,to_char(showtime,'YYYY/MM/DD HH24:MI'),theater_num from SCHEDULE order by movie_num";
                myDB.SelectQueryOracle(query1, out result);
                while (result.Read())
                {
                    string movie_title = "";
                    foreach (Movie movie in movieList)
                    {
                        if (movie.Num.Equals(result.GetValue(0).ToString()))
                            movie_title = movie.Title;
                    }
                    item = new ListViewItem(result.GetValue(0).ToString());
                    item.SubItems.Add(movie_title);
                    item.SubItems.Add(result.GetValue(1).ToString());
                    item.SubItems.Add(result.GetValue(2).ToString());
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


        /*=======이벤트 관련==========================================================================================================================*/

        //로그인창의 로그인 버튼
        private void button5_Click(object sender, EventArgs e)
        {
            OracleDataReader result;
            try
            {
                string query = "select * from MEMBER where member_id ='" + textBox1.Text + "'";
                myDB.SelectQueryOracle(query, out result);
                if (!(result.Read()))
                {
                    MessageBox.Show("존재하지 않는 아이디입니다");
                    textBox1.Clear();
                    textBox2.Clear();
                }

                // SHA256 해시 생성
                SHA256 hash1 = new SHA256Managed();
                byte[] bytes1 = hash1.ComputeHash(Encoding.ASCII.GetBytes(textBox2.Text));

                // 16진수 형태로 문자열 결합
                StringBuilder sb1 = new StringBuilder();
                foreach (byte b1 in bytes1)
                    sb1.AppendFormat("{0:x2}", b1);

                // 입력값의 해시결과
                String hash_value = sb1.ToString();

                if (result.GetValue(2).ToString() != hash_value)
                {
                    MessageBox.Show("비밀번호가 일치하지 않습니다.");
                    textBox2.Clear();
                }
                else if (result.GetValue(7).ToString().Equals("N"))
                {
                    MessageBox.Show("관리자의 가입 승인이 필요합니다.");
                    textBox1.Clear();
                    textBox2.Clear();
                }
                else
                {
                    loginUser = new User(result.GetValue(0).ToString(), result.GetValue(1).ToString(), result.GetValue(2).ToString(), result.GetValue(3).ToString()
                                         , result.GetValue(4).ToString(), result.GetValue(5).ToString(), result.GetValue(6).ToString());
                    MessageBox.Show(" "+loginUser.Name + " 님 환영합니다!");
                    login_signup_panel.Visible = false;
                    if (loginUser.Id.Equals("admin")){
                        adminManu_panel.Visible = true;
                        userManu_panel.Visible = false; ;
                    }
                    else
                    {
                        userManu_panel.Visible = true;
                        adminManu_panel.Visible = false; ;
                        moviePanel.Visible = true;
                    }
                }
                result.Close();
                myDB.DisconnectionOracle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message); //에러 메세지 
            }
            

        }
        //로그인창의 회원가입 버튼
        private void button6_Click(object sender, EventArgs e)
        {
            loginPanel.Visible = false;
            signupPanel.Visible = true;
        }
        //회원가입창의 중복확인 버튼
        private void button7_Click(object sender, EventArgs e)
        {
            if (!textBox3.Text.Equals(""))
            {
                if (CheckIDOverlapping(textBox3.Text))
                {
                    idCheck = false;
                    MessageBox.Show("중복된 아이디 입니다.");
                }
                else
                {
                    idCheck = true;
                    MessageBox.Show("사용 가능한 아이디 입니다.");
                }
            }
            else
            {
                MessageBox.Show("아이디를 입력하세요 .");
            }
        }

        //회원가입창의 뒤로가기 버튼
        private void button8_Click(object sender, EventArgs e)
        {
            loginPanel.Visible = true;
            signupPanel.Visible = false;
        }
        //회원가입창의 회원가입 버튼
        private void button9_Click(object sender, EventArgs e)
        {
            string id, pw, name, phone, card;
            
            // 아이디 중복 체크 여부 검사
            if (!idCheck)
            {
                MessageBox.Show("아이디 중복확인을 해주십시오.");
            }
            //비밀번호 길이 검사
            else if (textBox4.Text.Length > 20)
            {     
                MessageBox.Show("비밀번호는 20자 이내로 입력해야합니다.");
                textBox4.Clear();
                textBox5.Clear();
            }
            else if (textBox4.Text.Length < 8)
            {
                MessageBox.Show("비밀번호는 8자 이상 입력해야합니다.");
                textBox4.Clear();
                textBox5.Clear();
            }
            //비밀번호 조합 검사
            else if (!(checkWord(SW, textBox4.Text) && checkWord(NW, textBox4.Text) && checkWord(eW, textBox4.Text) && checkWord(EW, textBox4.Text)))
            {
                MessageBox.Show("비밀번호는 대소문자, 숫자, 특수기호를 포함해야합니다.");
                textBox4.Clear();
                textBox5.Clear();
            }
            //비밀번호 확인 검사
            else if (!(textBox4.Text.Equals(textBox5.Text)))
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다 비밀번호를 다시 확인해주세요.");
                textBox5.Clear();
            }
            //이름 입력 검사
            else if (textBox6.Text.Equals(""))
            {
                MessageBox.Show("이름을 입력해주세요.");
            }
            else if(textBox6.Text.Length > 20)
            {
                MessageBox.Show("이름은 20자 이내로 입력해야합니다.");
                textBox6.Clear();
            }
            else if(maskedTextBox1.Text.Length <13)
            {
                MessageBox.Show("전화번호를 입력해주세요.");
                maskedTextBox1.Clear();
            }
            else if (comboBox1.Text.Equals(""))
            {
                MessageBox.Show("카드사를 선택해주세요.");
            }
            else if(maskedTextBox2.Text.Length < 19)
            {
                MessageBox.Show("카드번호를 입력해주세요.");
                maskedTextBox2.Clear();
            }
            else
            {
                id = textBox3.Text;
                
                // SHA256 해시 생성
                SHA256 hash = new SHA256Managed();
                byte[] bytes = hash.ComputeHash(Encoding.ASCII.GetBytes(textBox5.Text));

                // 16진수 형태로 문자열 결합
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.AppendFormat("{0:x2}", b);

                // 문자열 출력
                pw = sb.ToString();
         
                name = textBox6.Text;
                phone = maskedTextBox1.Text;
                card = comboBox1.Text + " " + maskedTextBox2.Text;
                saveUser(id, pw, name, phone, card);
                MessageBox.Show("회원 가입을 축하합니다! 관리자 승인 후 로그인 가능합니다.");
                loginPanel.Visible = true;
                signupPanel.Visible = false;
            }
        }

        private void maskedTextBox1_Click(object sender, EventArgs e)
        {
            maskedTextBox1.Select(0, 0);
        }

        private void maskedTextBox2_Click(object sender, EventArgs e)
        {
            maskedTextBox2.Select(0, 0);
        }

        //아이디 검사 후 다시 아이디 수정하는 경우를 위한 처리
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            idCheck = false;
        }

        //예매
        private void button1_Click(object sender, EventArgs e)
        {
            moviePanel.Visible = false;
            theaterPanel.Visible = false;

            reservPanel.Visible = true;
        }
        //영화
        private void button2_Click(object sender, EventArgs e)
        {
            reservPanel.Visible = false;
            theaterPanel.Visible = false;

            moviePanel.Visible = true;
        }
        //상영관
        private void button3_Click(object sender, EventArgs e)
        {
            moviePanel.Visible = false;
            reservPanel.Visible = false;

            theaterPanel.Visible = true;
        }
        //My Page
        private void button4_Click(object sender, EventArgs e)
        {

        }
        //로그아웃
        private void button21_Click(object sender, EventArgs e)
        {
            adminManu_panel.Visible = false;
            userManu_panel.Visible = false; ;
            moviePanel.Visible = false;
            reservPanel.Visible = false;
            theaterPanel.Visible = false;

            login_signup_panel.Visible = true;
        }
        //예매하기
        private void button10_Click(object sender, EventArgs e)
        {
            moviePanel.Visible = false;
            theaterPanel.Visible = false;

            reservPanel.Visible = true;
            reservationPanel.Visible = true;
        }
        //현재 상영작
        private void button12_Click(object sender, EventArgs e)
        {
            reservPanel.Visible = false;
            theaterPanel.Visible = false;

            moviePanel.Visible = true;
            screeningPanel.Visible = true;
            notscreeningPanel.Visible = false;
        }
        //상영예정작
        private void button13_Click(object sender, EventArgs e)
        {
            reservPanel.Visible = false;
            theaterPanel.Visible = false;

            moviePanel.Visible = true;
            screeningPanel.Visible = false;
            notscreeningPanel.Visible = true;
        }
        //1관
        private void button14_Click(object sender, EventArgs e)
        {
            moviePanel.Visible = false;
            reservPanel.Visible = false;

            theaterPanel.Visible = true;
            T1panel.Visible = true;
            T2panel.Visible = false;
            T3panel.Visible = false;
        }
        //2관
        private void button15_Click(object sender, EventArgs e)
        {
            moviePanel.Visible = false;
            reservPanel.Visible = false;

            theaterPanel.Visible = true;
            T1panel.Visible = false;
            T2panel.Visible = true;
            T3panel.Visible = false;
        }
        //3관
        private void button16_Click(object sender, EventArgs e)
        {
            moviePanel.Visible = false;
            reservPanel.Visible = false;

            theaterPanel.Visible = true;
            T1panel.Visible = false;
            T2panel.Visible = false;
            T3panel.Visible = true;
        }
      
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            int index = listView1.FocusedItem.Index;
            Movie movie = notscreenmovieList[index];
            MessageBox.Show("영화 제목: " + movie.Title + "\n"
                            + "감독: " + movie.Director + "\n"
                            + "주연: " + movie.Actors);
        }

        private void listView2_MouseClick(object sender, MouseEventArgs e)
        {
            int index = listView2.FocusedItem.Index;
            Movie movie = screenmovieList[index];
            MessageBox.Show("영화 제목: " + movie.Title + "\n"
                            + "감독: " + movie.Director + "\n"
                            + "주연: " + movie.Actors,"영화 정보");
        }
        private void listView3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listView3.FocusedItem.Index;
            Movie movie = screenmovieList[index];
            ReservForm form = new ReservForm();
            form.Text = "<" + movie.Title + ">" + " 예매창";
            form.setReservForm(movie, loginUser);
            form.ShowDialog();
        }

/*========슬라이딩 패널 동작 이벤트들================================================================================================*/
        public static Control FindControlAtPoint(Control container, Point pos)
        {
            Control child;
            foreach (Control c in container.Controls)
            {
                if (c.Visible && c.Bounds.Contains(pos))
                {
                    child = FindControlAtPoint(c, new Point(pos.X - c.Left, pos.Y - c.Top));
                    if (child == null) return c;
                    else return child;
                }
            }
            return null;
        }
        public static Control FindControlAtCursor(Form form)
        {
            Point pos = Cursor.Position;
            if (form.Bounds.Contains(pos))
                return FindControlAtPoint(form, form.PointToClient(Cursor.Position));
            return null;
        }
        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1Panel.Visible = true;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            Thread.Sleep(500);
            Control Cursor = FindControlAtCursor(this);
            if (Cursor != button1Panel && Cursor != button10 )
            {
                button1Panel.Visible = false;
            }
        }
        private void button1Panel_MouseLeave(object sender, EventArgs e)
        {
            Control Cursor = FindControlAtCursor(this);
            if (Cursor != button1Panel && Cursor != button10 )
                button1Panel.Visible = false;
        }

        private void button2_MouseMove(object sender, MouseEventArgs e)
        {
            button2Panel.Visible = true;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            Thread.Sleep(500);
            Control Cursor = FindControlAtCursor(this);
            if (Cursor != button2Panel && Cursor != button12 && Cursor != button13)
            {
                button2Panel.Visible = false;
            }
        }
        private void button2Panel_MouseLeave(object sender, EventArgs e)
        {
            Control Cursor = FindControlAtCursor(this);
            if (Cursor != button2Panel && Cursor != button12 && Cursor != button13)
                button2Panel.Visible = false;
        }

        private void button3_MouseMove(object sender, MouseEventArgs e)
        {
            button3Panel.Visible = true;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            Thread.Sleep(500);
            Control Cursor = FindControlAtCursor(this);
            if (Cursor != button3Panel && Cursor != button14 && Cursor != button15 && Cursor != button16)
            {
                button3Panel.Visible = false;
            }
        }
        private void button3Panel_MouseLeave(object sender, EventArgs e)
        {
            Control Cursor = FindControlAtCursor(this);
            if (Cursor != button3Panel && Cursor != button14 && Cursor != button15 && Cursor != button16)
                button3Panel.Visible = false;
        }

/*====관리자 패널 이벤트=====================================================================================================*/
        //로그아웃
        private void button17_Click(object sender, EventArgs e)
        {
            adminManu_panel.Visible = false;
            userManu_panel.Visible = false; ;
            adm_menberpanel.Visible = false;
            adm_schedulepanel.Visible = false;

            login_signup_panel.Visible = true;
        }
        //회원관리
        private void button18_Click(object sender, EventArgs e)
        {
            adm_menberpanel.Visible = true;
            adm_schedulepanel.Visible = false;
        }
        //영화
        private void button19_Click(object sender, EventArgs e)
        {
            MovieInfoForm form = new MovieInfoForm();        
            form.ShowDialog();
        }
        //시간표
        private void button20_Click(object sender, EventArgs e)
        {
            adm_schedulepanel.Visible = true;
            adm_menberpanel.Visible = false;
        }
        void addLevelComboBox(DataSet DS) {
            dataGridView1.Columns.Remove("member_level");
            DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
            column.DataPropertyName = "MEMBER_LEVEL";
            column.HeaderText = "회원 등급";
            column.MinimumWidth = 6;
            column.Name = "member_level";
            column.Width = 130;
            column.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            column.DataSource = DS.Tables["Level"];
            column.DisplayMember = "MEMBER_LEVEL";
            column.ValueMember = "MEMBER_LEVEL";
            dataGridView1.Columns.Insert(3, column);
        }
        void addApprovedComboBox(DataSet DS)
        {
            dataGridView1.Columns.Remove("member_approved");
            DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
            column.DataPropertyName = "MEMBER_APPROVED";
            column.HeaderText = "승인 여부";
            column.MinimumWidth = 6;
            column.Name = "member_approved";
            column.Width = 130;
            column.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            column.DataSource = DS.Tables["Approved"];
            column.DisplayMember = "MEMBER_APPROVED";
            column.ValueMember = "MEMBER_APPROVED";
            dataGridView1.Columns.Insert(4, column);
        }
        //회원 관리 조회
        private void button24_Click(object sender, EventArgs e)
        {
            try {

                //회원 정보 불러옴
                DataSet DS = new DataSet();
                string query1 = "select member_num, member_id, member_name, member_level,member_approved from MEMBER order by member_num";
                string query2 = "select member_level from DISCOUNTRATE";
                string query3 = "select distinct member_approved from MEMBER";
                myDB.getDataset(query1, DS, "Member");
                myDB.getDataset(query2, DS, "Level");
                myDB.getDataset(query3, DS, "Approved");

                dataGridView1.DataSource = DS.Tables["Member"];
                addLevelComboBox(DS);
                addApprovedComboBox(DS);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);  
            }

        }
        //회원 관리 저장
        private void button25_Click(object sender, EventArgs e)
        {
            try
            {
                string query = null;
                DataTable changedDT;
                DataTable gridviewDT = (DataTable)dataGridView1.DataSource;
                changedDT = gridviewDT.GetChanges(DataRowState.Modified);   //수정된 열만 받아온다 
                if (changedDT != null)
                {
                    for (int i = 0; i < changedDT.Rows.Count; i++)
                    {
                        query = "update MEMBER set member_level= '" + changedDT.Rows[i]["MEMBER_LEVEL"].ToString() + "',"
                                + "member_approved= '" + changedDT.Rows[i]["MEMBER_APPROVED"].ToString() + "' where member_num= '"
                                + changedDT.Rows[i]["MEMBER_NUM"] + "'";
                        myDB.QueryOracle(query);
                    }
                }
                MessageBox.Show("DB에 저장 완료!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
       //시간표 조회
        private void button23_Click(object sender, EventArgs e)
        {
            setlistView4();
        }
        //시간표 등록
        private void button22_Click(object sender, EventArgs e)
        {
            ScheduleForm form= new ScheduleForm(movieList);
            DialogResult result = form.ShowDialog();
            if (result == DialogResult.OK)
                setlistView4();
        }
        //리스트뷰 정렬
        private int sortColumn = -1;
        private void listView4_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != sortColumn)
            {
                sortColumn = e.Column;
                listView4.Sorting = SortOrder.Ascending;
            }
            else
            {
                if (listView4.Sorting == SortOrder.Ascending)
                {
                    listView4.Sorting = SortOrder.Descending;
                }
                else
                {
                    listView4.Sorting = SortOrder.Ascending;
                }
            }
            listView4.Sort();
        }
/*====패널 전환 처리==========================================================================================================*/
        private void adm_menberpanel_VisibleChanged(object sender, EventArgs e)
        {
            if (!adm_menberpanel.Visible && dataGridView1.DataSource != null)
            {
                dataGridView1.DataSource = (dataGridView1.DataSource as DataTable).Clone();
            }
        }
        private void adm_schedulepanel_VisibleChanged(object sender, EventArgs e)
        {
            if (!adm_schedulepanel.Visible)
            {
                listView4.Items.Clear();
            }
        }

        private void login_signup_panel_VisibleChanged(object sender, EventArgs e)
        {
            if (login_signup_panel.Visible)
            {
                loginPanel.Visible = true;
                signupPanel.Visible = false;
            }
            else
            {
                loginPanel.Visible = false;
                signupPanel.Visible = false;
            }
        }

        private void loginPanel_VisibleChanged(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
        }

        private void signupPanel_VisibleChanged(object sender, EventArgs e)
        {
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            maskedTextBox1.Clear();
            maskedTextBox2.Clear();
            comboBox1.Items.Clear();
        }

        private void reservPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (reservPanel.Visible)
            {
                reservationPanel.Visible = true;
            }
            else
            {
                reservationPanel.Visible = false;
            }
        }

        private void reservationPanel_VisibleChanged(object sender, EventArgs e)
        {
            if(reservationPanel.Visible)
                setlistView3();
        }

        private void moviePanel_VisibleChanged(object sender, EventArgs e)
        {
            if (moviePanel.Visible)
            {
                notscreeningPanel.Visible = false;
                screeningPanel.Visible = true;
            }
            else
            {
                notscreeningPanel.Visible = false;
                screeningPanel.Visible = false;
            }
           
        }
        private void notscreeningPanel_VisibleChanged(object sender, EventArgs e)
        {
            if(notscreeningPanel.Visible)
                setlistView1();
        }

        private void screeningPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (screeningPanel.Visible)
                setlistView2();
        }

        private void theaterPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (theaterPanel.Visible)
            {
                T1panel.Visible = true;
                T2panel.Visible = false;
                T3panel.Visible = false;
            }
            else
            {
                T1panel.Visible = false;
                T2panel.Visible = false;
                T3panel.Visible = false;
            }
        }
    
    }
}
