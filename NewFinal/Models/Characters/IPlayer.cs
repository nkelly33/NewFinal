using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewFinal.Models.Abilities;
using NewFinal.Models.Atributes;

namespace NewFinal.Models.Characters
{
    public interface IPlayer
    {
        int Id { get; set; }
        string Name { get; set; }

        ICollection<Ability> Abilities { get; set; }

        void Attack(ITargetable target);
        void UseAbility(IAbility ability, ITargetable target);


    }
}
