using System;
using System.Collections.Generic;
using NewFinal.Models;
using NewFinal.Services;

public class MapManager
{
    private const int RoomNameLength = 5;
    private const int gridRows = 5;
    private const int gridCols = 5;
    private readonly OutputManager _outputManager;
    private readonly string[,] mapGrid;
    private Room _currentRoom;

    public MapManager(OutputManager outputManager)
    {
        _currentRoom = null;
        _outputManager = outputManager;
        mapGrid = new string[gridRows, gridCols];
    }

    public void DisplayMap()
    {
        _outputManager.WriteLine("Map:");

        // Clear the grid
        for (var i = 0; i < gridRows; i++)
            for (var j = 0; j < gridCols; j++)
                mapGrid[i, j] = "       ";

        if (_currentRoom != null)
        {
            var startRow = gridRows / 2;
            var startCol = gridCols / 2;
            PlaceRoom(_currentRoom, startRow, startCol, new HashSet<int>());
        }

        for (var i = 0; i < gridRows; i++)
        {
            for (var j = 0; j < gridCols; j++)
            {
                _outputManager.Write($"{mapGrid[i, j],-7}");
            }
            _outputManager.WriteLine("");
        }

        _outputManager.Display();
    }

    public void UpdateCurrentRoom(Room currentRoom)
    {
        _currentRoom = currentRoom;
    }

    private void PlaceRoom(Room room, int row, int col, HashSet<int> visited)
    {
        if (room == null || row < 0 || row >= gridRows || col < 0 || col >= gridCols)
            return;

        if (visited.Contains(room.Id))
            return; // Prevent infinite loops

        visited.Add(room.Id);

        var roomName = room.Name.Length > RoomNameLength
            ? room.Name.Substring(0, RoomNameLength)
            : room.Name.PadRight(RoomNameLength);

        mapGrid[row, col] = $"[{roomName}]";

        // North
        if (room.NorthRoom != null && row > 1)
        {
            mapGrid[row - 1, col] = "   |   ";
            PlaceRoom(room.NorthRoom, row - 2, col, visited);
        }
        // South
        if (room.SouthRoom != null && row < gridRows - 2)
        {
            mapGrid[row + 1, col] = "   |   ";
            PlaceRoom(room.SouthRoom, row + 2, col, visited);
        }
        // East
        if (room.EastRoom != null && col < gridCols - 2)
        {
            mapGrid[row, col + 1] = "  ---  ";
            PlaceRoom(room.EastRoom, row, col + 2, visited);
        }
        // West
        if (room.WestRoom != null && col > 1)
        {
            mapGrid[row, col - 1] = "  ---  ";
            PlaceRoom(room.WestRoom, row, col - 2, visited);
        }
    }
}