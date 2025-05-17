using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewFinal.Models.Atributes
{
    public interface ITargetable
    {
        string Name { get; set; }
        int Health { get; set; }
    }
}
