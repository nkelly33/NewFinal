using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using NewFinal.Models.Characters;
using NewFinal.Models.Characters.Enemy;

namespace NewFinal.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Cardinal direction links to other rooms

        public int? NorthRoomId { get; set; }
        [ForeignKey(nameof(NorthRoomId))]
        public virtual Room? NorthRoom { get; set; }

        public int? SouthRoomId { get; set; }
        [ForeignKey(nameof(SouthRoomId))]
        public virtual Room? SouthRoom { get; set; }

        public int? EastRoomId { get; set; }
        [ForeignKey(nameof(EastRoomId))]
        public virtual Room? EastRoom { get; set; }

        public int? WestRoomId { get; set; }
        [ForeignKey(nameof(WestRoomId))]
        public virtual Room? WestRoom { get; set; }

        // Navigation property for players in this room
        public virtual List<Player> Players { get; set; } = new();
        public virtual List<Monster> Monsters { get; set; } = new();
    }
}
