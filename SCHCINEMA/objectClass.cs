using System.Drawing;

namespace SCHCINEMA
{
    class objectClass
    {

    }
    public class Movie
    {
        string num;
        string title;
        string director;
        string actors;
        Image image;
        public string Num
        {
            set { num = value; }
            get { return num; }
        }
        public string Title
        {
            set { title = value; }
            get { return title; }
        }
        public string Director
        {
            set { director = value; }
            get { return director; }
        }
        public string Actors
        {
            set { actors = value; }
            get { return actors; }
        }
        public Image Image
        {
            set { image = value; }
            get { return image; }
        }
       
        public Movie(string num, string title, string director, string actors, Image image)
        {
            this.num = num;
            this.title = title;
            this.director = director;
            this.actors = actors;
            this.image = image;
        }
    }
    public class User
    {
        string num;
        string id;
        string pw;
        string name;
        string phone;
        string level;
        string card;
        public string Num
        {
            set { num = value; }
            get { return num; }
        }
        public string Id
        {
            set { id = value; }
            get { return id; }
        }
        public string Pw
        {
            set { pw = value; }
            get { return pw; }
        }
        public string Name
        {
            set { name = value; }
            get { return name; }
        }
        public string Phone
        {
            set { phone = value; }
            get { return phone; }
        }
        public string Level
        {
            set { level = value; }
            get { return level; }
        }
        public string Card
        {
            set { card = value; }
            get { return card; }
        }
        public User(string num, string id, string pw, string name, string phone, string level, string card)
        {
            this.num = num;
            this.id = id;
            this.pw = pw;
            this.name = name;
            this.phone = phone;
            this.level = level;
            this.card = card;
        }
    }
    public class Schedule
    {
        string movienum;
        string showtime;
        string theaternum;
        double price;
        string theaterform;

        public string Movienum
        {
            set { movienum = value; }
            get { return movienum; }
        }
        public string Showtime
        {
            set { showtime = value; }
            get { return showtime; }
        }
        public string Theaternum
        {
            set { theaternum = value; }
            get { return theaternum; }
        }
        public double Price
        {
            set { price = value; }
            get { return price; }
        }
        public string Theaterform
        {
            set { theaterform = value; }
            get { return theaterform; }
        }
        public Schedule(string movienum, string showtime, string theaternum, double price,string theaterform)
        {
            this.movienum = movienum;
            this.showtime = showtime;
            this.theaternum = theaternum;
            this.price = price;
            this.theaterform = theaterform;
        }
    }
}