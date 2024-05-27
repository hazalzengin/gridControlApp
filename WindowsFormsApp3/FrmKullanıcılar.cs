using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Sistem.DB.Model;
using Sistem.DB.Service;

using System.Linq;
using DevExpress.XtraGrid.Columns;

namespace WindowsFormsApp3
{
    public partial class FrmKullanıcılar : Form
    {
        private readonly hazaluserservice _userService;
        private readonly hazalusermenuservice _usermenuservice;
        public string connectionString = @"Data source=HAZAL;Initial Catalog=hazal;Integrated Security=True";
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem saveMenuItem;
        private List<string> _columnOrder;
        private readonly int _userId;
        public FrmKullanıcılar(int userId)
        {
            _userId = userId;
            InitializeComponent();
            InitializeContextMenuStrip(); 
            _userService = new hazaluserservice(connectionString);
            _usermenuservice = new hazalusermenuservice(connectionString);

            saveMenuItem.Click += SaveMenuItem_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FillGridWithUserColumns();
            gridView1.ColumnPositionChanged += GridView1_ColumnPositionChanged;

        }
        private void GridView1_ColumnPositionChanged(object sender, EventArgs e)
        {
            UpdateColumnOrder();
        }
        private void griddoldur()
        {
            List<hazaluser> userList = _userService.GetAllUsers();
            gridControl1.DataSource = userList;
           
            UpdateColumnOrder();


        }
        private void InitializeContextMenuStrip()
        {
            contextMenuStrip = new ContextMenuStrip();
            saveMenuItem = new ToolStripMenuItem("Kaydet");
          
            contextMenuStrip.Items.Add(saveMenuItem);
            gridControl1.ContextMenuStrip = contextMenuStrip;
        }
        private void gridControl1_ColumnMoved(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            UpdateColumnOrder();
        }

        private void UpdateColumnOrder()
        {
            _columnOrder = gridView1.Columns.Cast<DevExpress.XtraGrid.Columns.GridColumn>()
                                       .OrderBy(column => column.VisibleIndex)
                                       .Select(column => column.FieldName)
                                       .ToList();


        }
      


        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveColumnOrder();
        }

        private void SaveColumnOrder()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string query = "SELECT Column1 FROM grid_user WHERE UserId = @UserId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", _userId);
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    string updateQuery = "UPDATE grid_user SET Column1 = @Column1 WHERE UserId = @UserId";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@UserId", _userId);
                    updateCommand.Parameters.AddWithValue("@Column1", string.Join(",", _columnOrder.Skip(3))); 
                   //updateCommand.Parameters.AddWithValue("@Column1", string.Join(",", _columnOrder));
                    updateCommand.ExecuteNonQuery();


                }
                else
                {
                    string query2 = "INSERT INTO grid_user (UserId, Column1) VALUES (@UserId, @Column1)";
                    SqlCommand command2 = new SqlCommand(query2, connection);
                    command2.Parameters.AddWithValue("@UserId", _userId);
                    command2.Parameters.AddWithValue("@Column1", string.Join(",", _columnOrder.Skip(3))); 
                    command2.ExecuteNonQuery();
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show("Hataa: " + ex.Message);
            }
        }


        private void FillGridWithUserColumns()
        {
            try
            {
                List<string> columnOrder;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                   
                    var result = resultgetir(_userId, connection);
                    

                    if (result != null && result != DBNull.Value)
                    {
                        string columnOrderString = (string)result;
                        columnOrder = columnOrderString.Split(',')
                            .Select((columnName, index) => new { ColumnName = columnName, Index = index })
                            .Select(columnInfo => columnInfo.ColumnName)
                            .ToList();

                        for (int i = 0; i < columnOrder.Count; i++)
                        {
                            string columnName = columnOrder[i];
                            GridColumn column = gridView1.Columns.AddVisible(columnName);
                            column.VisibleIndex = i;
                            //column.Visible = true;

                        }

                        gridView1.Columns[0].VisibleIndex = 0;
                        gridView1.Columns[2].VisibleIndex = 1;
                        gridView1.Columns[4].VisibleIndex = 2;
                        List<hazaluser> userList = _userService.GetAllUsers();
                        gridControl1.DataSource = userList;
                        UpdateColumnOrder();
                        //gridView1.Columns.Clear();
                    }
                    else 
                    {
                        gridView1.Columns[0].VisibleIndex = 0;
                        gridView1.Columns[2].VisibleIndex = 1;
                        gridView1.Columns[4].VisibleIndex = 2;
                        List<hazaluser> userList = _userService.GetAllUsers();
                        UpdateColumnOrder();
                        gridControl1.DataSource = userList;

                    }
                }

             
            }
            catch (Exception ex)
            {
                MessageBox.Show("Grid doldurulurken bir hata oluştu: " + ex.Message);
            }
        }

        private object resultgetir(int userId, SqlConnection connection)
        {
            connection.Open();
            string query = "SELECT Column1 FROM grid_user WHERE UserId = " + _userId + "";
            SqlCommand command = new SqlCommand(query, connection);
            object result = command.ExecuteScalar();
            if (result == null || result == DBNull.Value)
            {
                query = "SELECT Column1 FROM grid_user WHERE UserId = 0";
                command = new SqlCommand(query, connection);
                result = command.ExecuteScalar();
            }
            connection.Close();

            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            NewUser form2 = new NewUser();
            form2.ShowDialog();
            griddoldur();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int[] selectedRows = gridView1.GetSelectedRows();
            if (selectedRows.Length > 0)
            {
                int selectedRowIndex = selectedRows[0];
                int userId = Convert.ToInt32(gridView1.GetRowCellValue(selectedRowIndex, "Id"));
                string username = Convert.ToString(gridView1.GetRowCellValue(selectedRowIndex, "username"));
                string email = Convert.ToString(gridView1.GetRowCellValue(selectedRowIndex, "email"));
                string password = _userService.GetUserPassword(userId);
                KullaniciGuncelle form3 = new KullaniciGuncelle(userId, username, email, password);
                form3.ShowDialog();
                griddoldur();
            }
            else
            {
                MessageBox.Show("güncellemede hata");
            }
        }




        private void button3_Click_1(object sender, EventArgs e)
        {
           
        }

        private void button4_Click(object sender, EventArgs e)
        {

            DataTable dataSet = _usermenuservice.GetAll(null, null).Tables[0];
            gridControl2.DataSource = dataSet;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            int[] selectedRows = gridView1.GetSelectedRows();
            if (selectedRows.Length > 0)
            {
                int userId = Convert.ToInt32(gridView1.GetRowCellValue(selectedRows[0], "Id"));
                Yetkilendirme form4 = new Yetkilendirme(userId);
                form4.ShowDialog();
                griddoldur();
            }
        }

     
        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Hide();
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            griddoldur();
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            griddoldur();
        }

        private void btnDelete_ButtonPressed(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            int[] selectedRows = gridView1.GetSelectedRows();
            int selectedRowIndex = selectedRows[0];
            int userId = Convert.ToInt32(gridView1.GetRowCellValue(selectedRowIndex, "Id"));
            DialogResult result = MessageBox.Show("Kullanıcıyı silmek ister misiniz?", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                _userService.Sil(userId);
                griddoldur();
            }
        }

        private void newuserbutton_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            NewUser form2 = new NewUser();
            form2.ShowDialog();
            griddoldur();
        }

        private void newuserbutton_ButtonPressed(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            NewUser form2 = new NewUser();
            form2.ShowDialog();
            griddoldur();
        }

        private void guncellebtn_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            int[] selectedRows = gridView1.GetSelectedRows();
            if (selectedRows.Length > 0)
            {
                int selectedRowIndex = selectedRows[0];
                int userId = Convert.ToInt32(gridView1.GetRowCellValue(selectedRowIndex, "Id"));
                string username = Convert.ToString(gridView1.GetRowCellValue(selectedRowIndex, "username"));
                string email = Convert.ToString(gridView1.GetRowCellValue(selectedRowIndex, "email"));
                string password = _userService.GetUserPassword(userId);
                KullaniciGuncelle form3 = new KullaniciGuncelle(userId, username, email, password);
                form3.ShowDialog();
                griddoldur();
            }
            else
            {
                MessageBox.Show("güncellemede hata");
            }
        }
    }


}


