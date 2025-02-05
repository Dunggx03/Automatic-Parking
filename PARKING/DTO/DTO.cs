using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PARKING.DTO
{
    public class CardUser
    {
        public int ID { get; set; }
        public string UName { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public string Access { get; set; }
    }

    public class Card
    {
        public string ID { get; set; }
        public string Vehicle { get; set; }
        public double Money { get; set; }
        public int UID { get; set; }
    }

    public class CardLog
    {
        public int LogID { get; set; }
        public string CardID { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public DateTime LogDate { get; set; }
    }

    internal class DTO
    {
    }
}
