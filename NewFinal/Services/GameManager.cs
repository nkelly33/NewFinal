using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NewFinal.Data;
using NewFinal.Models.Characters;
using NewFinal.Models.Abilities;
using NewFinal.Models;
using System.Numerics;
using NewFinal.Models.Equipments;

public class GameManager
{
    private readonly GameContext _context;

    public GameManager(GameContext context)
    {
        _context = context;
    }

    // Add a new Character
    public void AddCharacter()
    {
        Console.Write("Enter character name: ");
        var name = Console.ReadLine();

        Console.Write("Enter health: ");
        var health = int.TryParse(Console.ReadLine(), out var h) ? h : 100;

        Console.Write("Enter level: ");
        var level = int.TryParse(Console.ReadLine(), out var l) ? l : 1;

        Console.Write("Enter gold: ");
        var gold = int.TryParse(Console.ReadLine(), out var g) ? g : 0;

        // Assign starting equipment
        Equipment? equipment = null;
        Console.Write("Assign starting equipment? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            equipment = new Equipment();

            // Weapon selection
            var weapons = _context.Items.Where(i => i.Type.ToLower() == "weapon").ToList();
            if (weapons.Any())
            {
                Console.WriteLine("Available weapons:");
                for (int i = 0; i < weapons.Count; i++)
                    Console.WriteLine($"{i + 1}. {weapons[i].Name} (Atk: {weapons[i].Attack})");
                Console.Write("Select weapon by number (or 0 for none): ");
                if (int.TryParse(Console.ReadLine(), out int wIdx) && wIdx > 0 && wIdx <= weapons.Count)
                    equipment.Weapon = weapons[wIdx - 1];
            }

            // Armor selection
            var armors = _context.Items.Where(i => i.Type.ToLower() == "armor").ToList();
            if (armors.Any())
            {
                Console.WriteLine("Available armors:");
                for (int i = 0; i < armors.Count; i++)
                    Console.WriteLine($"{i + 1}. {armors[i].Name}");
                Console.Write("Select armor by number (or 0 for none): ");
                if (int.TryParse(Console.ReadLine(), out int aIdx) && aIdx > 0 && aIdx <= armors.Count)
                    equipment.Armor = armors[aIdx - 1];
            }

            _context.Equipment.Add(equipment);
        }

        // Assign starting room
        Room? room = null;
        var rooms = _context.Rooms.ToList();
        if (rooms.Any())
        {
            Console.WriteLine("Available starting rooms:");
            for (int i = 0; i < rooms.Count; i++)
                Console.WriteLine($"{i + 1}. {rooms[i].Name}");
            Console.Write("Select starting room by number (or 0 for none): ");
            if (int.TryParse(Console.ReadLine(), out int rIdx) && rIdx > 0 && rIdx <= rooms.Count)
                room = rooms[rIdx - 1];
        }

        var player = new Player
        {
            Name = name,
            Health = health,
            Level = level,
            Gold = gold,
            Equipment = equipment,
            Inventory = new List<NewFinal.Models.Equipments.Item>(),
            Abilities = new List<Ability>(),
            Room = room,
            RoomId = room?.Id
        };

        _context.Players.Add(player);
        _context.SaveChanges();
        Console.WriteLine($"Character '{name}' added.");
    }

    // Edit an existing Character
    public void EditCharacter()
    {
        Console.Write("Enter character name to edit: ");
        var name = Console.ReadLine();
        var player = _context.Players.Include(p => p.Abilities).FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
        if (player == null)
        {
            Console.WriteLine("Character not found.");
            return;
        }

        Console.Write($"Enter new health (current: {player.Health}): ");
        player.Health = int.Parse(Console.ReadLine() ?? player.Health.ToString());
        Console.Write($"Enter new level (current: {player.Level}): ");
        player.Level = int.Parse(Console.ReadLine() ?? player.Level.ToString());
        Console.Write($"Enter new gold (current: {player.Gold}): ");
        player.Gold = int.Parse(Console.ReadLine() ?? player.Gold.ToString());

        _context.SaveChanges();
        Console.WriteLine("Character updated.");
    }

    // Display all Characters
    public void DisplayAllCharacters()
    {
        var players = _context.Players.Include(p => p.Abilities).ToList();
        foreach (var player in players)
        {
            Console.WriteLine($"Name: {player.Name}, Health: {player.Health}, Level: {player.Level}, Gold: {player.Gold}");
            if (player.Abilities.Any())
            {
                Console.WriteLine("  Abilities:");
                foreach (var ab in player.Abilities)
                    Console.WriteLine($"    {ab.Name} (Atk: {ab.AttackBonus}, Def: {ab.DefenseBonus})");
            }
        }
    }

    // Search for a specific Character by name
    public void SearchCharacterByName()
    {
        Console.Write("Enter character name to search: ");
        var name = Console.ReadLine();
        var player = _context.Players.Include(p => p.Abilities).FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
        if (player == null)
        {
            Console.WriteLine("Character not found.");
            return;
        }
        Console.WriteLine($"Name: {player.Name}, Health: {player.Health}, Level: {player.Level}, Gold: {player.Gold}");
        if (player.Abilities.Any())
        {
            Console.WriteLine("  Abilities:");
            foreach (var ab in player.Abilities)
                Console.WriteLine($"    {ab.Name} (Atk: {ab.AttackBonus}, Def: {ab.DefenseBonus})");
        }
    }

    // Add Abilities to a Character
    public void AddAbilityToCharacter()
    {
        Console.Write("Enter character name: ");
        var name = Console.ReadLine();
        var player = _context.Players.Include(p => p.Abilities).FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
        if (player == null)
        {
            Console.WriteLine("Character not found.");
            return;
        }

        Console.Write("Enter ability type (magic/physical): ");
        var type = Console.ReadLine()?.ToLower();
        Console.Write("Enter ability name: ");
        var abName = Console.ReadLine();
        Console.Write("Enter attack bonus: ");
        var atk = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Enter defense bonus: ");
        var def = int.Parse(Console.ReadLine() ?? "0");

        Ability ability;
        if (type == "magic")
        {
            ability = new MagicAbility { Name = abName, AttackBonus = atk, DefenseBonus = def };
            _context.MagicAbilities.Add((MagicAbility)ability);
        }
        else
        {
            ability = new PhysicalAbility { Name = abName, AttackBonus = atk, DefenseBonus = def };
            _context.PhysicalAbilities.Add((PhysicalAbility)ability);
        }
        player.Abilities.Add(ability);
        _context.SaveChanges();
        Console.WriteLine($"Ability '{abName}' added to {player.Name}.");
    }

    // Display Character Abilities
    public void DisplayCharacterAbilities()
    {
        Console.Write("Enter character name: ");
        var name = Console.ReadLine();
        var player = _context.Players.Include(p => p.Abilities).FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
        if (player == null)
        {
            Console.WriteLine("Character not found.");
            return;
        }
        if (!player.Abilities.Any())
        {
            Console.WriteLine("No abilities found.");
            return;
        }
        Console.WriteLine($"{player.Name}'s Abilities:");
        foreach (var ab in player.Abilities)
            Console.WriteLine($"  {ab.Name} (Atk: {ab.AttackBonus}, Def: {ab.DefenseBonus})");
    }

    // Execute an ability during an attack
    public void ExecuteAbilityAttack()
    {
        Console.Write("Enter attacker name: ");
        var attackerName = Console.ReadLine();
        var attacker = _context.Players.Include(p => p.Abilities).FirstOrDefault(p => p.Name.ToLower() == attackerName.ToLower());
        if (attacker == null)
        {
            Console.WriteLine("Attacker not found.");
            return;
        }
        Console.Write("Enter target name: ");
        var targetName = Console.ReadLine();
        var target = _context.Players.FirstOrDefault(p => p.Name.ToLower() == targetName.ToLower());
        if (target == null)
        {
            Console.WriteLine("Target not found.");
            return;
        }
        Console.Write("Enter ability name to use: ");
        var abName = Console.ReadLine();
        var ability = attacker.Abilities.FirstOrDefault(a => a.Name.ToLower() == abName.ToLower());
        if (ability == null)
        {
            Console.WriteLine("Ability not found.");
            return;
        }
        ability.Activate(attacker, target);
        _context.SaveChanges();
        Console.WriteLine($"{attacker.Name} used {ability.Name} on {target.Name}.");
    }

    // Add new Room
    public void AddRoom()
    {
        Console.Write("Enter room name: ");
        var name = Console.ReadLine();
        Console.Write("Enter description: ");
        var desc = Console.ReadLine();

        var room = new Room { Name = name, Description = desc };
        _context.Rooms.Add(room);
        _context.SaveChanges();
        Console.WriteLine($"Room '{name}' added.");
    }

    // Display details of a Room
    public void DisplayRoomDetails()
    {
        Console.Write("Enter room name: ");
        var name = Console.ReadLine();
        var room = _context.Rooms.Include(r => r.Players).FirstOrDefault(r => r.Name.ToLower() == name.ToLower());
        if (room == null)
        {
            Console.WriteLine("Room not found.");
            return;
        }
        Console.WriteLine($"Room: {room.Name}\nDescription: {room.Description}");
        if (room.Players.Any())
        {
            Console.WriteLine("Inhabitants:");
            foreach (var p in room.Players)
                Console.WriteLine($"  {p.Name} (Health: {p.Health}, Level: {p.Level})");
        }
        else
        {
            Console.WriteLine("No inhabitants in this room.");
        }
    }

    // Navigate Rooms
    public void NavigateRooms()
    {
        Console.Write("Enter character name: ");
        var name = Console.ReadLine();
        var player = _context.Players.Include(p => p.Room).FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
        if (player == null)
        {
            Console.WriteLine("Character not found.");
            return;
        }
        while (true)
        {
            Console.WriteLine($"Current Room: {player.Room?.Name ?? "None"}");
            Console.WriteLine("Enter direction (north, south, east, west) or 'exit': ");
            var dir = Console.ReadLine()?.ToLower();
            if (dir == "exit") break;
            Room? nextRoom = dir switch
            {
                "north" => _context.Rooms.FirstOrDefault(r => r.Id == player.Room.NorthRoomId),
                "south" => _context.Rooms.FirstOrDefault(r => r.Id == player.Room.SouthRoomId),
                "east" => _context.Rooms.FirstOrDefault(r => r.Id == player.Room.EastRoomId),
                "west" => _context.Rooms.FirstOrDefault(r => r.Id == player.Room.WestRoomId),
                _ => null
            };
            if (nextRoom == null)
            {
                Console.WriteLine("No room in that direction.");
                continue;
            }
            player.Room = nextRoom;
            player.RoomId = nextRoom.Id;
            _context.SaveChanges();
            Console.WriteLine($"Moved to {nextRoom.Name}: {nextRoom.Description}");
        }

    }
    public void ListAllRoomsWithCharacters()
    {
        var rooms = _context.Rooms.Include(r => r.Players).ToList();
        foreach (var room in rooms)
        {
            Console.WriteLine($"Room: {room.Name}");
            if (room.Players.Any())
            {
                foreach (var p in room.Players)
                    Console.WriteLine($"  - {p.Name} (Health: {p.Health}, Level: {p.Level})");
            }
            else
            {
                Console.WriteLine("  No characters in this room.");
            }
        }
    }
    public void ListCharactersInRoomByAttribute()
    {
        Console.Write("Enter room name: ");
        var roomName = Console.ReadLine();
        var room = _context.Rooms.Include(r => r.Players).FirstOrDefault(r => r.Name.ToLower() == roomName.ToLower());
        if (room == null)
        {
            Console.WriteLine("Room not found.");
            return;
        }
        Console.Write("Sort by (Name/Health/Level): ");
        var attr = Console.ReadLine()?.ToLower();
        IEnumerable<Player> sorted = room.Players;
        switch (attr)
        {
            case "name": sorted = sorted.OrderBy(p => p.Name); break;
            case "health": sorted = sorted.OrderByDescending(p => p.Health); break;
            case "level": sorted = sorted.OrderByDescending(p => p.Level); break;
            default: break;
        }
        foreach (var p in sorted)
            Console.WriteLine($"{p.Name} (Health: {p.Health}, Level: {p.Level})");
    }
    public void FindEquipmentHolder()
    {
        Console.Write("Enter item name: ");
        var itemName = Console.ReadLine();
        var player = _context.Players
            .Include(p => p.Inventory)
            .Include(p => p.Room)
            .FirstOrDefault(p => p.Inventory.Any(i => i.Name.ToLower() == itemName.ToLower()));
        if (player != null)
        {
            Console.WriteLine($"Item '{itemName}' is held by {player.Name} in room {player.Room?.Name ?? "Unknown"}.");
        }
        else
        {
            Console.WriteLine("Item not found.");
        }
    }
    public void DisplayAllMonsters()
    {
        var monsters = _context.Monsters.Include(m => m.Room).ToList();
        foreach (var m in monsters)
            Console.WriteLine($"{m.Name} (HP: {m.Health}, Atk: {m.AttackPower}) in Room: {m.Room?.Name ?? "None"}");
    }

    public void DisplayAllRooms()
    {
        var rooms = _context.Rooms.Include(r => r.Players).Include(r => r.Monsters).ToList();
        foreach (var r in rooms)
        {
            Console.WriteLine($"Room: {r.Name} - {r.Description}");
            Console.WriteLine("  Players: " + (r.Players.Any() ? string.Join(", ", r.Players.Select(p => p.Name)) : "None"));
            Console.WriteLine("  Monsters: " + (r.Monsters.Any() ? string.Join(", ", r.Monsters.Select(m => m.Name)) : "None"));
        }
    }
    public void DeleteAllPlayers()
    {
        Console.WriteLine("Are you sure you want to remove the player");
        Console.WriteLine("1. Yes");
        Console.WriteLine("2. No");
        var choice = Console.ReadLine();
        if (choice != "1")
        {
            Console.WriteLine("Player deletion cancelled.");
            return;
        }
        var players = _context.Players
            .Include(p => p.Equipment)
            .Include(p => p.Abilities)
            .ToList();

        foreach (var player in players)
        {
            // Remove abilities
            if (player.Abilities != null && player.Abilities.Any())
            {
                _context.RemoveRange(player.Abilities);
            }

            // Remove equipment (with FK nulling as before)
            if (player.Equipment != null)
            {
                player.Equipment.WeaponId = null;
                player.Equipment.ArmorId = null;
                _context.SaveChanges();
                _context.Equipment.Remove(player.Equipment);
            }
        }

        _context.Players.RemoveRange(players);
        _context.SaveChanges();
        Console.WriteLine("All players have been deleted.");
    }

    public void DeletePlayerByName()
    {
        Console.Write("Enter the name of the player to delete: ");
        var name = Console.ReadLine();
        var player = _context.Players
            .Include(p => p.Equipment)
            .Include(p => p.Abilities)
            .FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
        Console.WriteLine("Are you sure you want to remove the player");
        Console.WriteLine("1. Yes");
        Console.WriteLine("2. No");
        var choice = Console.ReadLine();
        if (choice != "1")
        {
            Console.WriteLine("Player deletion cancelled.");
            return;
        }
        // Remove the player
        if (player == null)
        {
            Console.WriteLine("Player not found.");
            return;
        }

        // Remove abilities
        if (player.Abilities != null && player.Abilities.Any())
        {
            _context.RemoveRange(player.Abilities);
        }

        // Remove equipment (with FK nulling as before)
        if (player.Equipment != null)
        {
            player.Equipment.WeaponId = null;
            player.Equipment.ArmorId = null;
            _context.SaveChanges();
            _context.Equipment.Remove(player.Equipment);
        }

        _context.Players.Remove(player);
        _context.SaveChanges();
        Console.WriteLine($"Player '{name}' has been deleted.");
    }

}
