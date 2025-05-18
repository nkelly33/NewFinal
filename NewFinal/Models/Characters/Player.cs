using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewFinal.Models.Abilities;
using NewFinal.Models.Atributes;
using NewFinal.Models.Equipments;



namespace NewFinal.Models.Characters;


public class Player : ITargetable, IPlayer
{

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Gold { get; set; }
    public int Health { get; set; }
    public virtual Equipment? Equipment { get; set; }
    public virtual ICollection<Item> Inventory { get; set; } = new List<Item>();
    public int? RoomId { get; set; }
    public virtual Room? Room { get; set; }
    public virtual ICollection<Ability> Abilities { get; set; } = new List<Ability>();


    // Implementing IPlayer.Attack

    public void Attack(ITargetable target)
    {
        if (Equipment == null || Equipment.Weapon == null)
        {
            Console.WriteLine($"{Name} has no weapon equipped and cannot attack!");
            return;
        }

        Console.WriteLine($"{Name} attacks {target.Name} with a {Equipment.Weapon.Name} dealing {Equipment.Weapon.Attack} damage!");
        target.Health -= Equipment.Weapon.Attack;
        Console.WriteLine($"{target.Name} has {target.Health} health remaining.");
        if (target.Health <= 0)
        {
            Console.WriteLine($"{target.Name} has been defeated!");
            
        }
    }



    public void UseAbility(IAbility ability, ITargetable target)
    {
        if (Abilities.Contains(ability))
        {
            ability.Activate(this, target);
        }
        else
        {
            Console.WriteLine($"{Name} does not have the ability {ability.Name}!");
        }
    }
}
