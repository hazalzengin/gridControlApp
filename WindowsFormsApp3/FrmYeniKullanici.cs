using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sistem.DB.Model;
using Sistem.DB.Service;

namespace WindowsFormsApp3
{
    public partial class NewUser : Form
    {

        private readonly hazaluserservice _userService;

        public NewUser()
        {
            InitializeComponent();
            string connectionString = @"Data source=HAZAL;Initial Catalog=hazal;Integrated Security=True";
            _userService = new hazaluserservice(connectionString);
        }

        private void button1_Click(object sender, EventArgs e)
        {
           int sonuc =  _userService.Ekle(new hazaluser
            {
                username = textBox1.Text,
                email = textBox2.Text,
                password = textBox3.Text
            });
            if(sonuc > 0)
            {
                MessageBox.Show("Yeni kullanıcı eklendi");
            }
            else
            {

            }
     
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }

        private void NewUser_Load(object sender, EventArgs e)
        {

        }
    }
}
