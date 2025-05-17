using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NewFinal.Data;
using NewFinal.Models.Characters;
using NewFinal.Models.Abilities;
using NewFinal.Models;

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
        var health = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Enter level: ");
        var level = int.Parse(Console.ReadLine() ?? "1");
        Console.Write("Enter gold: ");
        var gold = int.Parse(Console.ReadLine() ?? "0");



        var player = new Player
        {
            Name = name,
            Health = health,
            Level = level,
            Gold = gold,
            Inventory = new List<NewFinal.Models.Equipments.Item>(),
            Abilities = new List<Ability>(),
            Room = _context.Rooms.FirstOrDefault()
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
}
