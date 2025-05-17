using NewFinal;
using NewFinal.Data;


using NewFinal.Data;
using NewFinal.Services;

class Program
{
    static void Main(string[] args)
    {
        using var context = new GameContext();
        DatabaseSeeder.Seed(context);

        var outputManager = new OutputManager();
        var mapManager = new MapManager(outputManager);
        var gameEngine = new GameEngine(context, mapManager, outputManager);

        var gameManager = new GameManager(context);
        var gameManagerMenu = new GameManagerMenu(gameManager);
        var menu = new Menu(gameEngine, gameManagerMenu);
        menu.Show(); 
    }
}