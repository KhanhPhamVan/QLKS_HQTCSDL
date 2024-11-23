using QLKS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKS.ViewModels
{
    public class ReservationRoomTicket
    {
        //Chưa thực hiện được theo cách này, đang sử dụng ADO thuần
        //BookingRoomDetails
        //Room
        //Customer
        //Employee
        BookingRoomDetail roomDetail;
        BookingRoom bookingRoom;
        Room room;
        Customer customer;
        Employee employee;

        public BookingRoomDetail RoomDetail { get => roomDetail; set => roomDetail = value; }
        public Room Room { get => room; set => room = value; }
        public Customer Customer { get => customer; set => customer = value; }
        public Employee Employee { get => employee; set => employee = value; }
        public BookingRoom BookingRoom { get => bookingRoom; set => bookingRoom = value; }
        public int Id { get; set; }
        public string roomName { get; set; }
        public string customerName { get; set; }
        public DateTime BookingDate { get; set; }
        public string employeeName { get; set; }

        public ReservationRoomTicket()
        {
        }
        //public static DataTable GetReservationRoomTickets()
        //{
        //    string sqls = "SELECT * FROM DS_PHIEU_DAT_PHONG";
        //    DataTable dt = db.GetTable(sqls);
        //    return dt;
        //}
    }
}
