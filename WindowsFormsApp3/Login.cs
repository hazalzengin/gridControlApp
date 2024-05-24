using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Sistem.DB.Model;
using Sistem.DB.Service;

namespace WindowsFormsApp3
{
    public partial class Login : Form
    {

        private readonly hazaluserservice _userService;
        public Login()
        {
            InitializeComponent();
            _userService = new hazaluserservice(connectionString);
        }
        public string connectionString = @"Data source=HAZAL;Initial Catalog=hazal;Integrated Security=True";

        private void button1_Click(object sender, EventArgs e)
        {
            var user = _userService.GetAllUsers().FirstOrDefault(x => x.email == textBox1.Text && x.password == textBox2.Text);
            if (user != null)
            {
                int userId = user.Id;
                FrmKullanıcılar frm = new FrmKullanıcılar(userId);
                frm.Show();
                this.Hide();
            }
            else
            {
                XtraMessageBox.Show("HATALI GİRİŞ YAPTINIZ");
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
