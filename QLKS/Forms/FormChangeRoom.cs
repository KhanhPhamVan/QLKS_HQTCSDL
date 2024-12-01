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
using static QLKS.FormReservation;

namespace QLKS.Forms
{
    public partial class FormChangeRoom : Form
    {
        private List<int> MaPhieuDatPhong;
        private int MaNV;
        private int MaKH;
        bool isCompleted = false;
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
                LoadingListRoom();
            }
        }
        IEnumerable<ReservationRoomStatus> viewModel = ReservationRoomStatus.GetRooms(db);
        public void LoadingListRoom()
        {
            dgr_PhongTrong.Rows.Clear();
            int i = 0;
            List<string> roomEmpty = new List<string>();
            foreach (ReservationRoomStatus room in viewModel)
            {
                string status = CheckRoomStatus(room.Id, dtpStart.Value, dtpEnd.Value);
                if (status == "Phòng trống")
                {
                    dgr_PhongTrong.Rows.Add();
                    dgr_PhongTrong.Rows[i].Cells[0].Value = room.Id;
                    dgr_PhongTrong.Rows[i].Cells["SoPhong"].Value = room.Number;
                    dgr_PhongTrong.Rows[i].Cells["TENLOAIPHONG"].Value = room.NameRoomType;
                    dgr_PhongTrong.Rows[i].Cells["GIA"].Value = room.Price;
                    dgr_PhongTrong.Rows[i].Cells["SOLUONGNGUOITOIDA"].Value = room.MaxPeople;
                    i++;
                }
            }
            isCompleted = true;
        }
        string CheckRoomStatus(int roomId, DateTime dayStart, DateTime dayEnd)
        {
            List<BookingRoom> bookings = db.GetTable<BookingRoom>(p => !(p.ExpectedDate <= dayStart.Date || p.ArrivedDate >= dayEnd.Date)).ToList();
            foreach (BookingRoom booking in bookings)
            {
                Invoice invoice = db.GetTable<Invoice>(p => p.BookingRoom == booking.Id).FirstOrDefault();
                if (invoice != null)
                {
                    foreach (BookingRoomDetail room in db.GetTable<BookingRoomDetail>(p => p.BookingRoom == booking.Id))
                    {
                        if (roomId == room.Room)
                        {
                            return "Phòng trống";
                        }
                    }
                }
                else
                {
                    ReceivingRoom receiving = db.GetTable<ReceivingRoom>(p => p.BookingRoom == booking.Id).FirstOrDefault();
                    if (receiving != null)
                    {
                        foreach (BookingRoomDetail room in db.GetTable<BookingRoomDetail>(p => p.BookingRoom == booking.Id))
                        {
                            if (roomId == room.Room)
                            {
                                return "Đã nhận";
                            }
                        }
                    }
                    else
                    {
                        foreach (BookingRoomDetail room in db.GetTable<BookingRoomDetail>(p => p.BookingRoom == booking.Id))
                        {
                            if (roomId == room.Room)
                            {
                                return "Đã đặt";
                            }
                        }
                    }
                }
            }
            return "Phòng trống";
        }
        //void LoadListRoom()
        //{
        //    string startDate = dtpStart.Value.ToString("yyyy-MM-dd");
        //    string endDate = dtpEnd.Value.ToString("yyyy-MM-dd");
        //    string sqlLoadPhong = $"exec SP_DSPHONG_TRONG_DOIPHONG '{startDate}', '{endDate}'";
        //    dgr_PhongTrong.DataSource = db.GetTable(sqlLoadPhong);
        //}

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
            LoadingListRoom();
        }
        public FormChangeRoom(List<int> MaPhongs, int MaNV, int MaKH)
        {
            InitializeComponent();
            this.MaPhieuDatPhong = MaPhongs;
            this.MaNV = MaNV;
            this.MaKH = MaKH;
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
            try
            {
                int MaPhieuDatPhong = (int)cbc_MaPhieuDatPhong.SelectedValue;
                int MaPhongCu = (int)cbc_SoPhongCu.SelectedValue;
                int MaPhongMoi = (int)dgr_PhongTrong.CurrentRow.Cells["MaPhong"].Value;
                string NgayDoi = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                string LyDo = txt_LyDo.Text;

                string sqls = $"EXEC SP_CAPNHAT_DOITRAPHONG  {MaPhieuDatPhong} , {MaPhongCu} , {MaPhongMoi} , '{NgayDoi}' , '{LyDo}', {MaNV}, {MaKH} ";
                int rowAffected =  db.ExecuteNonQuery(sqls);

                if (rowAffected > 0)
                {
                    MessageBox.Show("Đổi phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                MessageBox.Show("Đổi phòng không thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
