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

namespace QLKS.Forms
{
    public partial class FormChangeRoom : Form
    {
        private List<int> MaPhieuDatPhong;
        static DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection");
        private void TimeChange(object sender, EventArgs e)
        {
            string startDate = dtpStart.Value.ToString("yyyy-MM-dd");
            string endDate = dtpEnd.Value.ToString("yyyy-MM-dd");

            if (DateTime.TryParse(startDate, out DateTime depTime) && DateTime.TryParse(endDate, out DateTime arrTime))
            {
                if (arrTime < depTime)
                {
                    MessageBox.Show("Ngày bắt đầu phải lớn hơn ngày kết thúc!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtpStart.Value = DateTime.Now;
                    dtpEnd.Value = DateTime.Now;
                    return;
                }
                LoadListRoom();
            }
        }
        void LoadListRoom()
        {
            string startDate = dtpStart.Value.ToString("yyyy-MM-dd");
            string endDate = dtpEnd.Value.ToString("yyyy-MM-dd");
            string sqlLoadPhong = $"exec SP_DSPHONG_TRONG_DOIPHONG '{startDate}', '{endDate}'";
            dgr_PhongTrong.DataSource = db.GetTable(sqlLoadPhong);
        }

        public void LoadFormChangeRoom()
        {
            cbc_MaPhieuDatPhong.DataSource = MaPhieuDatPhong;

            //Load số phòng cũ
            string sqlSoPhongCu = $"SELECT CTPD.MaPhong, SoPhong FROM [CHITIETPHIEUDAT] CTPD " +
                $"INNER JOIN PHONG P " +
                $"ON CTPD.MaPhong = P.MaPhong " +
                $"WHERE MaPhieuDatPhong = {cbc_MaPhieuDatPhong.SelectedValue}";
            cbc_SoPhongCu.DataSource = db.GetTable(sqlSoPhongCu);
            cbc_SoPhongCu.DisplayMember = "SoPhong";
            cbc_SoPhongCu.ValueMember = "MaPhong";

            //Load danh sách phòng trống
            LoadListRoom();
        }
        public FormChangeRoom(List<int> MaPhongs)
        {
            InitializeComponent();
            this.MaPhieuDatPhong = MaPhongs;    
            LoadFormChangeRoom();
            dtpStart.ValueChanged +=TimeChange;
            dtpEnd.ValueChanged +=TimeChange;
        }

        private void dgr_PhongTrong_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txt_SoPhongMoi.Text = dgr_PhongTrong.CurrentRow.Cells["SoPhong"].Value.ToString();
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }
    }
}
