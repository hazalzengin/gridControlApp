using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sistem.DB.Service;
using Sistem.DB.Model;
using DevExpress.XtraGrid.Views.Grid;
using System.Data.SqlClient;

namespace WindowsFormsApp3
{
    public partial class Yetkilendirme : Form
    {

        private readonly hazalmenuservice _menuservice;
        private readonly hazalusermenuservice _usermenuservice;
        private readonly int _userId;
        public string connectionString = @"Data source=HAZAL;Initial Catalog=hazal;Integrated Security=True";
        public Yetkilendirme(int userId)
        {
            _userId = userId;
            InitializeComponent();
            _menuservice = new hazalmenuservice(connectionString);
            _usermenuservice = new hazalusermenuservice(connectionString);
            gridView1.OptionsSelection.MultiSelect = true;
            gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            // check();
            yetkigoster();
        }
        //private void check()
        //{
        //    List<hazalmenu> menuList = _menuservice.GetDataToClass(null, null, null).ToList();
        //    dataGridView1.DataSource = menuList;
        //    DataGridViewCheckBoxColumn checkboxColumn = new DataGridViewCheckBoxColumn();
        //    checkboxColumn.HeaderText = "Yetki";
        //    checkboxColumn.Name = "Select";
        //    dataGridView1.Columns.Insert(0, checkboxColumn);
        //}

        private void yetkigoster()
        {
            List<hazalusermenu> yetkiliMenus = _usermenuservice.GetYetki(_userId);
            List<int> yetkiliMenuIds = yetkiliMenus.Select(menu => menu.Menuref).ToList();

            List<hazalmenu> menuList = _menuservice.GetDataToClass(null, null, null).ToList();
            gridControl1.DataSource = menuList;
            for (int rowIndex = 0; rowIndex < gridView1.RowCount; rowIndex++)
            {
                int menuId = Convert.ToInt32(gridView1.GetRowCellValue(rowIndex, "Id"));
                if (yetkiliMenuIds.Contains(menuId))
                {
                    gridView1.SelectRow(rowIndex);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int[] selectedRows = gridView1.GetSelectedRows();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM menu_user WHERE userref = @UserId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", _userId);
                var menuReader = command.ExecuteReader();

                foreach (int rowIndex in selectedRows)
                {
                    int menuId = Convert.ToInt32(gridView1.GetRowCellValue(rowIndex, "Id"));

                   
                    if (menuReader.Read())
                    {
                       int Id = (int)menuReader["id"];
                        hazalusermenu userMenu = new hazalusermenu
                        {
                            Id = Id,
                            Userref = _userId,
                            Menuref = menuId
                        };
                        _usermenuservice.Guncelle(userMenu);
                        MessageBox.Show("Güncelleme Başarılı.");
                    }
                    else
                    {
                        hazalusermenu userMenu = new hazalusermenu
                        {
                           
                            Userref = _userId,
                            Menuref = menuId
                        };
                        _usermenuservice.Ekle(userMenu);
                       
                    }
                    MessageBox.Show("Menüler kullanıcıya eklenmiştir.");
                }
            }

         
        }
            private void button2_Click(object sender, EventArgs e)
        {
            int[] selectedRows = gridView1.GetSelectedRows();
            foreach (int rowIndex in selectedRows)
            {
                DataTable dataSet = _usermenuservice.GetAll(null, null).Tables[0];
                int menuId = 0;
                if (dataSet.Rows.Count > 0)
                {
                    DataRow row = dataSet.Rows[0];
                    menuId = Convert.ToInt32(row["id"]);
                }

                _usermenuservice.Sil(menuId);
            }

            MessageBox.Show("Seçilen menüler kullanıcıdan kaldırılmıştır.");
            yetkigoster();
        }
    }
}

