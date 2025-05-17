using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewFinal.Models.Atributes;

namespace NewFinal.Models.Characters.Enemy
{
    public interface IMonster
    {
        int Id { get; set; }
        string Name { get; set; }

        void Attack(ITargetable target);
    }
}