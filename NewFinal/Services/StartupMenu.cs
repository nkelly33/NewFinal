using System;


namespace NewFinal.Services
{
    public class Menu
    {
        private readonly GameEngine _gameEngine;    
        private readonly GameManagerMenu _gameManagerMenu;

        public Menu(GameEngine gameEngine, GameManagerMenu gameManagerMenu)
        {
            _gameEngine = gameEngine;
            _gameManagerMenu = gameManagerMenu;
        }

        public void Show()
        {
            while (true)
            {
                Console.WriteLine("\n--- Main Menu ---");
                Console.WriteLine("1. Start Game");
                Console.WriteLine("2. Open Game Manager");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        _gameEngine.Run();
                        break;
                    case "2":
                        _gameManagerMenu.Show();
                        break;
                        
                    case "0":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }
    }
}