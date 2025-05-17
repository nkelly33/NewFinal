using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewFinal.Models.Characters;
using NewFinal.Models.Atributes;

namespace NewFinal.Models.Abilities
{
    public interface IAbility
    {
        int Id { get; set; }
        string Name { get; set; }
        ICollection<Player> Players { get; set; }

        void Activate(IPlayer user, ITargetable target);
    }
}
