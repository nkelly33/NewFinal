using System;
using NewFinal.Data;


namespace NewFinal
{
    public class GameManagerMenu
    {
        private readonly GameManager _manager;

        public GameManagerMenu(GameManager manager)
        {
            _manager = manager;
        }

        public void Show()
        {
            while (true)
            {
                Console.WriteLine("\n--- God Manager Menu ---");
                Console.WriteLine("1. Add Character");
                Console.WriteLine("2. Edit Character");
                Console.WriteLine("3. Display All Characters");
                Console.WriteLine("4. Search Character by Name");
                Console.WriteLine("5. Add Ability to Character");
                Console.WriteLine("6. Display Character Abilities");
                Console.WriteLine("7. Execute Ability Attack");
                Console.WriteLine("8. Add Room");
                Console.WriteLine("9. Display Room Details");
                Console.WriteLine("10. Navigate Rooms");
                Console.WriteLine("11. List All Rooms with Characters");
                Console.WriteLine("12. List Characters in Room by Attribute");
                Console.WriteLine("13. Find Equipment Holder");
                Console.WriteLine("14. Delete All Players");
                Console.WriteLine("15. Delete Player By Name");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1": _manager.AddCharacter(); break;
                    case "2": _manager.EditCharacter(); break;
                    case "3": _manager.DisplayAllCharacters(); break;
                    case "4": _manager.SearchCharacterByName(); break;
                    case "5": _manager.AddAbilityToCharacter(); break;
                    case "6": _manager.DisplayCharacterAbilities(); break;
                    case "7": _manager.ExecuteAbilityAttack(); break;
                    case "8": _manager.AddRoom(); break;
                    case "9": _manager.DisplayRoomDetails(); break;
                    case "10": _manager.NavigateRooms(); break;
                    case "11": _manager.ListAllRoomsWithCharacters(); break;
                    case "12": _manager.ListCharactersInRoomByAttribute(); break;
                    case "13": _manager.FindEquipmentHolder(); break;
                    case "14": _manager.DeleteAllPlayers(); break;
                    case "15": _manager.DeletePlayerByName(); break;
                    case "0": Console.WriteLine("Exiting..."); return;
                    default: Console.WriteLine("Invalid option. Try again."); break;
                }
            }
        }

    }
}
