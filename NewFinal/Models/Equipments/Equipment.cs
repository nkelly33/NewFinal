using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewFinal.Models.Equipments;
public class Equipment
{
    public int Id { get; set; }


    public int? WeaponId { get; set; }  // Nullable to avoid cascade issues
    public int? ArmorId { get; set; }   // Nullable to avoid cascade issues

    // Navigation properties
    [ForeignKey("WeaponId")]
    public virtual Item Weapon { get; set; }

    [ForeignKey("ArmorId")]
    public virtual Item Armor { get; set; }
}

