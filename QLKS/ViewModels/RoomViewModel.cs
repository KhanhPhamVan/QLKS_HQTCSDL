using QLKS.Models;
using QLKS.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace QLKS.ViewModels
{
    public class RoomViewModel
    {
        Room room;
        RoomType type;

        //public Room Room => room;
        //public RoomType RoomType => type;

        public int Id { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public int TypeId {  get; set; }
        public decimal Price { get; set; }
        public int MaxPeople { get; set; }

        public RoomViewModel() { }

        public RoomViewModel(Room r, DbContext db)
        {
            room = r;
            type = r.GetRoomType(db);

            Id = r.Id;
            Number = r.Name;
            Status = r.Status;
            Type = type.Name;
            Price = type.Price;
            MaxPeople = type.MaxPeople;
            TypeId = type.Id;
        }

        public static IEnumerable<RoomViewModel> GetRooms(DbContext context)
        {
            foreach (Room room in context.GetTable<Room>())
            {
                yield return new RoomViewModel(room, context);
            }
        }
    }
}
