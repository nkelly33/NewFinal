using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewFinal.Models.Characters;
using NewFinal.Models.Atributes;

namespace NewFinal.Models.Abilities
{
    public abstract class Ability : IAbility
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AbilityType { get; set; }
        public int AttackBonus { get; set; }
        public int DefenseBonus { get; set; }



        public virtual ICollection<Player> Players { get; set; }


        public virtual ICollection<Ability> Abilities { get; set; } = new List<Ability>();


        public abstract void Activate(IPlayer user, ITargetable target);
    }
}
