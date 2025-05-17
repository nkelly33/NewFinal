using NewFinal.Models.Equipments;
using NewFinal.Models.Characters;
using NewFinal.Models;
using System.Collections.Generic;
using System.Linq;
using NewFinal.Models.Abilities;
using NewFinal.Models.Atributes;
using NewFinal.Models.Characters.Enemy;




namespace NewFinal.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(GameContext context)
        {
            // 1) Prevent re-seeding
            if (context.Players.Any()) return;

            // 2) Seed Items

            var sword = new Item { Name = "Sword of Truth", Type = "Weapon" };
            var shield = new Item { Name = "Shield of Valor", Type = "Armor" };
            var potion = new Item { Name = "Healing Potion", Type = "Consumable" };
            var bow = new Item { Name = "Bow of Eternity", Type = "Weapon" };
            var helmet = new Item { Name = "Helmet of Wisdom", Type = "Armor" };
            context.Items.AddRange(sword, shield, potion, bow, helmet);
            context.SaveChanges();

            // 3) Rooms Phase 1: create without links
            var townSquare = new Room { Name = "Town Square", Description = "The central hub of the town." };
            var enchantedForest = new Room { Name = "Enchanted Forest", Description = "A lush forest filled with mystical creatures." };
            var market = new Room { Name = "Market", Description = "Bustling with vendors and shoppers." };
            var tavern = new Room { Name = "Tavern", Description = "A lively place to unwind." };
            var dungeon = new Room { Name = "Dungeon", Description = "A dark and foreboding underground maze." };

            context.Rooms.AddRange(townSquare, enchantedForest, market, tavern, dungeon);
            context.SaveChanges();  // now each Room has its Id

            // 4) Rooms Phase 2: wire up the cardinal links by Id
            townSquare.NorthRoomId = enchantedForest.Id;
            townSquare.EastRoomId = market.Id;
            townSquare.WestRoomId = tavern.Id;
            townSquare.SouthRoomId = dungeon.Id;

            enchantedForest.SouthRoomId = townSquare.Id;
            market.WestRoomId = townSquare.Id;
            tavern.EastRoomId = townSquare.Id;
            dungeon.NorthRoomId = townSquare.Id;

            context.SaveChanges();  // simple UPDATEs, no cycle

            // 5) Seed Players (with attached Equipment/navigation)
            var player1 = new Player
            {
                Name = "Elandor",
                Level = 10,
                Gold = 100,
                Room = townSquare,
                Inventory = new List<Item> { sword, shield, potion },
                Equipment = new Equipment { Weapon = sword, Armor = shield }
            };

            var player2 = new Player
            {
                Name = "Archeron",
                Level = 8,
                Gold = 75,
                Room = townSquare,
                Inventory = new List<Item> { bow, helmet, potion },
                Equipment = new Equipment { Weapon = bow, Armor = helmet }
            };

            var player3 = new Player
            {
                Name = "Myranda",
                Level = 12,
                Gold = 150,
                Room = townSquare,
                Inventory = new List<Item> { potion, shield },
                Equipment = new Equipment { Weapon = sword }  // no armor
            };

            context.Players.AddRange(player1, player2, player3);
            context.SaveChanges();  // inserts Players then their Equipment dependents

            // 6) Seed Abilities for demo (requires Ability entity be added to GameContext)
            var ability1 = new PhysicalAbility
            {
                Name = "Power Strike",
                AttackBonus = 5,
                DefenseBonus = 0,
                AbilityType = "Physical",
                Description = "A powerful physical attack."
            };
            var ability2 = new MagicAbility
            {
                Name = "Shield Wall",
                AttackBonus = 0,
                DefenseBonus = 5,
                AbilityType = "Magic",
                Description = "A magical barrier that increases defense."
            };
            context.PhysicalAbilities.AddRange(ability1);
            context.MagicAbilities.AddRange(ability2);
            context.SaveChanges();
            // Assign abilities to players (many-to-many)
            player1.Abilities.Add(ability1);
            player2.Abilities.Add(ability2);
            context.SaveChanges();
            // 7) Seed Monsters
            var bunny = new Monster
            {
                Name = "Bunny",
                Health = 20,
                AggressionLevel = 1,
                MonsterType = "Beast",
                AttackPower = 3,
                Description = "A surprisingly fierce bunny.",
                Room = enchantedForest // or any room you want
            };

            var badMonster = new Monster
            {
                Name = "Bad Monster",
                Health = 50,
                AggressionLevel = 5,
                MonsterType = "Demon",
                AttackPower = 10,
                Description = "A truly bad monster.",
                Room = dungeon // or any room you want
            };

            context.Monsters.AddRange(bunny, badMonster);
            context.SaveChanges();
        }
    }
}
