using QLKS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace QLKS.Forms
{
    public partial class FormService : Form
    {
        public FormService()
        {
            InitializeComponent();
        }
        static DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection");
        void LoadDataSource()
        {
            dtgvService.DataSource = db.GetTable<Service>().ToList();
        }
        void LoadRoomId()
        {
            cboId.DataSource = null;
            foreach (Service service in db.GetTable<Service>())
            {
                cboId.Items.Add(service.Id);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                MessageBox.Show("Vui lòng nhập vào thông tin để tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Service service = db.GetTable<Service>(t => t.Name == txtSearch.Text).FirstOrDefault();
            if (service == null)
            {
                MessageBox.Show("Không tìm thấy dịch vụ này", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            txtName.Text = service.Name;
            txtPrice.Text = service.Price.ToString();
            cboId.Text = service.Id.ToString();
        }

        private void FormService_Load(object sender, EventArgs e)
        {
            LoadDataSource();
            LoadRoomId();
        }

        private void dtgvService_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dtgvService.Rows[e.RowIndex];
                txtName.Text = row.Cells["ServiceName"].Value?.ToString();
                txtPrice.Text = row.Cells["Price"].Value?.ToString();
                cboId.Text = row.Cells["Id"].Value?.ToString();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (db.GetTable<Service>(t => t.Name == txtName.Text).FirstOrDefault() != null)
            {
                MessageBox.Show("Dịch vụ đã tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Service service= new Service();
            service.Name = txtName.Text;
            service.Price = decimal.Parse(txtPrice.Text);
            if (db.AddRow(service) == null)
            {
                MessageBox.Show("Thêm dịch vụ không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            };
            LoadDataSource();
            LoadRoomId();
            MessageBox.Show("Thêm dịch vụ thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        string ErrorMessage()
        {
            if (string.IsNullOrEmpty(txtName.Text))
                return "Vui lòng nhập vào tên dịch vụ";
            if (string.IsNullOrEmpty(txtPrice.Text))
                return "Vui lòng nhập vào giá dịch vụ";
            return null;
        }
        private void cboId_SelectedIndexChanged(object sender, EventArgs e)
        {
            Service service = db.GetTable<Service>(t => t.Id == int.Parse(cboId.Text)).First();
            txtName.Text = service.Name;
            txtPrice.Text = service.Price.ToString();
            cboId.Text = service.Id.ToString();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboId.Text))
            {
                MessageBox.Show("Vui lòng nhập vào mã dịch vụ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Service service = new Service();
            service.Name = txtName.Text;
            service.Price = decimal.Parse(txtPrice.Text);
            service.Id = int.Parse(cboId.Text);
            if (!db.UpdateRow(service))
            {
                MessageBox.Show("Cập nhật dịch vụ không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LoadDataSource();
            MessageBox.Show("Cập nhật dịch vụ thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboId.Text))
            {
                MessageBox.Show("Vui lòng nhập vào mã dịch vụ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Bạn có chắc muốn xóa dịch vụ này?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            Func<Service, bool> predicate = p => p.Id == int.Parse(cboId.Text);
            if (db.DeleteRows<Service>($"MADV={cboId.Text}") == 0)
            {
                MessageBox.Show("Xóa dịch vụ không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LoadDataSource();
            LoadRoomId();
            MessageBox.Show("Xóa dịch vụ thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
