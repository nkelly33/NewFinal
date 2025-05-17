using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewFinal.Models.Atributes;

namespace NewFinal.Models.Characters.Enemy
{
    public class Bunny : Monster
    {
        public override void Attack(ITargetable target)
        {
            Console.WriteLine($"{Name} throws a carrot at {target.Name}");
        }
    }
}
