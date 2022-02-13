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
    public partial class MovieInfoForm : Form
    {
        public MovieInfoForm()
        {
            InitializeComponent();
        }
        Image image = null;
        ConnectDB myDB = new ConnectDB();
       
        internal void setMovieInfoForm()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            image = null;
        }

        private byte[] imageToByteArray(Image img)
        {
            ImageConverter imageConverter = new ImageConverter();
            byte[] b = (byte[])imageConverter.ConvertTo(img, typeof(byte[]));
            return b;
        }
        public void saveMovie(string title, string director, string actors, Image image)
        {
            try
            {
                string query = "INSERT INTO MOVIE VALUES ('M'||SEQ_MOVIE_NUM.nextval,'" + title + "','" + director + "','" + actors + "',";
               
                if (image == null)
                {
                    query = query + "NULL)";
                    myDB.QueryOracle(query);
                }
                else
                {
                    query = query + ":image)";
                    byte[] bytes = imageToByteArray(image);
                    myDB.QueryOracle_WithImage(query, bytes);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message); //에러 메세지 
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = @"D:\";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string image_file = string.Empty;
                image_file = dialog.FileName;
                textBox4.Text = image_file;
                image = Bitmap.FromFile(image_file);
            }
            else return;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string title, director, actors;
            if (textBox1.Text.Equals(""))
            {
                MessageBox.Show("영화 제목을 입력하세요");
            }
            else if (textBox2.Text.Equals(""))
            {
                MessageBox.Show("감독을 입력하세요");
            }
            else if (textBox3.Text.Equals(""))
            {
                MessageBox.Show("주연 배우를 입력하세요");
            }
            else{
                title = textBox1.Text;
                director = textBox2.Text;
                actors = textBox3.Text;
                saveMovie(title, director, actors, image);
                setMovieInfoForm();
            }
        }
    }
}
