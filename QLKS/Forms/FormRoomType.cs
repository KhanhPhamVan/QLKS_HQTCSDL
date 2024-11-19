using QLKS.Models;
using QLKS.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace QLKS.Forms
{
    public partial class FormRoomType : Form
    {
        public FormRoomType()
        {
            InitializeComponent();
        }
        DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection");
        void LoadDataSource()
        {
            dtgvRoomType.DataSource = db.GetTable<RoomType>().ToList();
        }
        void LoadRoomId()
        {
            cboId.DataSource = null;
            foreach (RoomType roomType in db.GetTable<RoomType>())
            {
                cboId.Items.Add(roomType.Id);
            }
        }
        private void FormRoomType_Load(object sender, EventArgs e)
        {
            LoadDataSource();
            LoadRoomId();
        }

        private void dtgvRoomType_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dtgvRoomType.Rows[e.RowIndex];
                txtName.Text = row.Cells["RoomName"].Value?.ToString();
                txtMaxPeople.Text = row.Cells["MaxPeople"].Value?.ToString(); ;
                txtPrice.Text = row.Cells["Price"].Value?.ToString();
                cboId.Text = row.Cells["Id"].Value?.ToString();
            }
        }

        private void cboId_SelectedIndexChanged(object sender, EventArgs e)
        {
            RoomType roomType=db.GetTable<RoomType>(t=>t.Id==int.Parse(cboId.Text)).First();
            txtName.Text = roomType.Name;
            txtMaxPeople.Text = roomType.MaxPeople.ToString();
            txtPrice.Text = roomType.Price.ToString();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                MessageBox.Show("Vui lòng nhập vào thông tin để tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            RoomType roomType = db.GetTable<RoomType>(t=>t.Name==txtSearch.Text).FirstOrDefault();
            if (roomType == null)
            {
                MessageBox.Show("Không tìm thấy loại phòng này", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            txtName.Text = roomType.Name;
            txtMaxPeople.Text = roomType.MaxPeople.ToString();
            txtPrice.Text = roomType.Price.ToString();
            cboId.Text=roomType.Id.ToString();
        }
        string ErrorMessage()
        {
            if (string.IsNullOrEmpty(txtName.Text))
                return "Vui lòng nhập vào tên loại phòng";
            if (string.IsNullOrEmpty(txtPrice.Text))
                return "Vui lòng nhập vào giá phòng";
            if (string.IsNullOrEmpty(txtMaxPeople.Text))
                return "Vui lòng nhập vào số lượng người tối đa";
            return null;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (db.GetTable<RoomType>(t => t.Name == txtName.Text).FirstOrDefault() != null)
            {
                MessageBox.Show("Loại phòng đã tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            RoomType room = new RoomType();
            room.Name = txtName.Text;
            room.Price = decimal.Parse(txtPrice.Text);
            room.MaxPeople = int.Parse(txtMaxPeople.Text);
            if (db.AddRow(room) == null)
            {
                MessageBox.Show("Thêm loại phòng không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            };
            LoadDataSource();
            LoadRoomId();
            MessageBox.Show("Thêm loại phòng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboId.Text))
            {
                MessageBox.Show("Vui lòng nhập vào mã loại phòng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            RoomType room = new RoomType();
            room.Name = txtName.Text;
            room.Price = decimal.Parse(txtPrice.Text);
            room.MaxPeople = int.Parse(txtMaxPeople.Text);
            room.Id = int.Parse(cboId.Text);
            if (!db.UpdateRow(room))
            {
                MessageBox.Show("Cập nhật loại phòng không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LoadDataSource();
            MessageBox.Show("Cập nhật loại phòng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboId.Text))
            {
                MessageBox.Show("Vui lòng nhập vào mã loại phòng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Bạn có chắc muốn xóa loại phòng này?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            Func<RoomType, bool> predicate = p => p.Id == int.Parse(cboId.Text);
            if (db.DeleteRows<RoomType>($"MALOAIPHONG={cboId.Text}") == 0)
            {
                MessageBox.Show("Xóa loại phòng không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LoadDataSource();
            LoadRoomId();
            MessageBox.Show("Xóa loại phòng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        void ClearControl(Control control)
        {
            foreach (Control control1 in control.Controls)
            {
                if (control1 is TextBox)
                {
                    TextBox textBox = (TextBox)control1;
                    textBox.Clear();
                }
                else if (control1 is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)control1;
                    comboBox.SelectedIndex = 0;
                }
                else if (control1 is DateTimePicker)
                {
                    DateTimePicker dateTimePicker = (DateTimePicker)control1;
                    dateTimePicker.Value = DateTime.Now;
                }
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearControl(groupBox1);
            ClearControl(groupBox2);
        }
    }
}
