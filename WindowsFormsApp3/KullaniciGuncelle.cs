using System;
using System.Windows.Forms;
using Sistem.DB.Service;
using Sistem.DB.Model;

namespace WindowsFormsApp3
{
    public partial class KullaniciGuncelle : Form
    {
        private readonly hazaluserservice _userService;
        private readonly int _userId;
        private readonly string _username;
        private readonly string _password;
        private readonly string _email;

        public KullaniciGuncelle(int userId, string username, string email, string password)
        {
            InitializeComponent();
            string connectionString = @"Data source=HAZAL;Initial Catalog=hazal;Integrated Security=True";
            _userService = new hazaluserservice(connectionString);

            _userId = userId;
            _username = username;
            _password = password;
            _email = email;
            textBox1.Text = _username;
            textBox2.Text = _email;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _userService.Guncelle(new hazaluser
            {
                Id = _userId,
                username = textBox1.Text,
                email = textBox2.Text,
                password = textBox3.Text 
            });
            MessageBox.Show("Güncelleme Başarılı");
            this.Close();
        }

        private void KullaniciGuncelle_Load(object sender, EventArgs e)
        {

        }
    }
}
