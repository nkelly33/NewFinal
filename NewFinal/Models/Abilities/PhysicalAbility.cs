using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewFinal.Models.Atributes;
using NewFinal.Models.Characters;

namespace NewFinal.Models.Abilities
{
    public class PhysicalAbility : Ability
    {
        public int Damage { get; set; }

        public override void  Activate(IPlayer user, ITargetable target)
        {
            Console.WriteLine($"{user.Name} uses a physical ability on {target.Name}, dealing {Damage} damage!");
        }

    }
}
