using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using NewFinal.Models;
using NewFinal.Models.Characters;
using NewFinal.Models.Characters.Enemy;
using NewFinal.Services;
using NewFinal.Data;
using NewFinal.Models.Abilities;
using NewFinal.Models.Equipments;

public class GameEngine
{
    private readonly GameContext _context;
    private readonly MapManager _mapManager;
    private readonly OutputManager _outputManager;
    private List<Room> _rooms;
    private Player _player;
    private List<Monster> _monsters;
    private List<Item> _items;
    private List<Ability> _abilities;
    private List<Equipment> _equipments;



    public GameEngine(GameContext context, MapManager mapManager, OutputManager outputManager)
    {
        _context = context;
        _mapManager = mapManager;
        _outputManager = outputManager;
    }

    public void Run()
    {
        SetupGame();
    }

    private void SetupGame()
    {
        // Load all rooms with their monsters and players
        _rooms = _context.Rooms
            .Include(r => r.Monsters)
            .Include(r => r.Players)
            .Include(r => r.NorthRoom)
            .Include(r => r.SouthRoom)
            .Include(r => r.EastRoom)
            .Include(r => r.WestRoom)
            .ToList();

        // Load all monsters with their rooms
        _monsters = _context.Monsters
            .Include(m => m.Room)
            .ToList();

        // Load the first player (or prompt for selection)
        _player = _context.Players
            .Include(p => p.Room)
            .Include(p => p.Equipment)
            .Include(p => p.Inventory)
            .Include(p => p.Abilities)
            .FirstOrDefault();

        if (_player == null)
        {
            _player = PromptCreatePlayer();
        }

        if (_player == null)
        {
            _outputManager.WriteLine("No player found in the database.");
            _outputManager.Display();
            return;
        }

        if (_player.Room == null)
        {
            // Place player in the first room if not already assigned
            var startingRoom = _rooms.FirstOrDefault();
            if (startingRoom == null)
            {
                _outputManager.WriteLine("No rooms found in the database.");
                _outputManager.Display();
                return;
            }
            _player.Room = startingRoom;
            _context.SaveChanges();
        }

        _mapManager.UpdateCurrentRoom(_player.Room);

        _outputManager.WriteLine($"{_player.Name} has entered the game.");
        Thread.Sleep(1000);
        GameLoop();
    }

    private void GameLoop()
    {
        while (true)
        {
            Thread.Sleep(1000);
            _outputManager.Clear();
            _mapManager.UpdateCurrentRoom(_player.Room);
            _mapManager.DisplayMap();

            _outputManager.WriteLine("Choose an action:");
            _outputManager.WriteLine("1. Move North");
            _outputManager.WriteLine("2. Move South");
            _outputManager.WriteLine("3. Move East");
            _outputManager.WriteLine("4. Move West");
            _outputManager.WriteLine("5. Attack Monster (if present)");
            _outputManager.WriteLine("6. Exit Game");
            _outputManager.Display();

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    MovePlayer(_player.Room.NorthRoom, "north");
                    break;
                case "2":
                    MovePlayer(_player.Room.SouthRoom, "south");
                    break;
                case "3":
                    MovePlayer(_player.Room.EastRoom, "east");
                    break;
                case "4":
                    MovePlayer(_player.Room.WestRoom, "west");
                    break;
                case "5":
                    AttackMonster();
                    break;
                case "6":
                    _outputManager.WriteLine("Exiting game...");
                    _outputManager.Display();
                    return;
                default:
                    _outputManager.WriteLine("Invalid selection. Please choose a valid option.");
                    break;
            }
        }
    }

    private void MovePlayer(Room nextRoom, string direction)
    {
        if (nextRoom != null)
        {
            _player.Room = nextRoom;
            _context.SaveChanges();
            _outputManager.WriteLine($"You move {direction} to {nextRoom.Name}.");
            _outputManager.WriteLine($"{nextRoom.Description}");

            // Show monsters in the room
            var monsters = _context.Monsters.Where(m => m.RoomId == nextRoom.Id).ToList();
            if (monsters.Any())
            {
                _outputManager.WriteLine("You see:");
                foreach (var m in monsters)
                    _outputManager.WriteLine($"- {m.Name} (HP: {m.Health})");
            }
            else
            {
                _outputManager.WriteLine("The room is quiet. No monsters here.");
            }
        }
        else
        {
            _outputManager.WriteLine("You can't move in that direction.");
        }
    }

    private void AttackMonster()
    {
        // Find monsters in the current room
        var monstersInRoom = _context.Monsters
            .Where(m => m.RoomId == _player.Room.Id)
            .ToList();

        if (monstersInRoom.Count == 0)
        {
            _outputManager.WriteLine("No monsters to attack in this room.");
            return;
        }

        Monster monster = null;

        if (monstersInRoom.Count == 1)
        {
            monster = monstersInRoom[0];
        }
        else
        {
            _outputManager.WriteLine("Which monster would you like to attack?");
            for (int i = 0; i < monstersInRoom.Count; i++)
            {
                _outputManager.WriteLine($"{i + 1}. {monstersInRoom[i].Name} (HP: {monstersInRoom[i].Health})");
            }
            _outputManager.Display();

            int choice;
            Console.Write("Enter the number of the monster: ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out choice) && choice >= 1 && choice <= monstersInRoom.Count)
            {
                monster = monstersInRoom[choice - 1];
            }
            else
            {
                _outputManager.WriteLine("Invalid selection.");
                return;
            }
        }

        _outputManager.WriteLine($"You attack {monster.Name}!");
        _player.Attack(monster);
        _outputManager.WriteLine($"{monster.Name} has {monster.Health} health remaining.");

        // Remove monster if defeated
        if (monster.Health <= 0)
        {
            _outputManager.WriteLine($"{monster.Name} has been defeated and removed from the room!");

            // Remove from room's monster list (if loaded)
            var room = _rooms.FirstOrDefault(r => r.Id == monster.RoomId);
            if (room != null)
            {
                room.Monsters.Remove(monster);
            }

            // Remove from database
            _context.Monsters.Remove(monster);
        }

        _context.SaveChanges();
    }
    private Player PromptCreatePlayer()
    {
        _outputManager.WriteLine("No player found. Let's create your Fighter!");
        _outputManager.Display();

        _outputManager.Write("Enter player name: ");
        _outputManager.Display();
        var name = Console.ReadLine();

        _outputManager.Write("Enter starting health: ");
        _outputManager.Display();
        int.TryParse(Console.ReadLine(), out int health);

        _outputManager.Write("Enter starting level: ");
        _outputManager.Display();
        int.TryParse(Console.ReadLine(), out int level);

        _outputManager.Write("Enter starting gold: ");
        _outputManager.Display();
        int.TryParse(Console.ReadLine(), out int gold);

        var player = new Player
        {
            Name = name,
            Health = health > 0 ? health : 100,
            Level = level > 0 ? level : 1,
            Gold = gold,
            Inventory = new List<Item>(),
            Abilities = new List<Ability>()
        };

        _context.Players.Add(player);
        _context.SaveChanges();
        _outputManager.WriteLine($"Player '{name}' created!");
        _outputManager.Display();
        return player;
    }

}

