using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewFinal.Models.Atributes;

namespace NewFinal.Models.Characters.Enemy
{
    public class Monster : IMonster, ITargetable
    {
        public Monster() 
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }
        public int AggressionLevel { get; set; }
        public string MonsterType { get; set; }
        public int AttackPower { get; set; }
        public string Description { get; set; }
        public int? RoomId { get; set; }
        public virtual Room? Room { get; set; }
        public virtual void Attack(ITargetable target)
        {
            // Implementation of the attack logic  
        }
    }
}

