/*
  Mine

  Description:
    This program is my remake of game minesweeper written as console application.
    User enters coordinates of field to open it. If field does not contain mine,
    a number of mines surrounding a field will be shown. If there are no mines around,
    all surrounded fields will be open. If user open field where mine is set, game is 
    over and user did not solve game. If user open all fields which have no
    mines inside, game is over and user successfully solved the game.

    Command Line Interface has been implemented in this file which uses instance of
    MineBoard class, and represents its behavior graphically.

  Author: Dejan Cvijetinovic
  Date: 11.04.2019 (modified)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mine {
  class Program {
    // Program execution
    static void Main(string[] args) {

      // User interface
      int numRows, numColumns, numMines, row, column;
      Console.WriteLine("*** Mine ***");

      // Repeating new games until user wants it
      while (true) {

        // Data entry for game creation and wrong entry protection
        while (true) {
          try {
            Console.WriteLine("Creating new game");
            do {
              Console.Write("Enter number of rows [1-{0}]: ", MineBoard.MAX_ROWS);
              numRows = Convert.ToInt16(Console.ReadLine());
            } while (numRows < 1 || numRows > MineBoard.MAX_ROWS);
            do {
              Console.Write("Enter number of columns [1-{0}]: ", MineBoard.MAX_COLUMNS);
              numColumns = Convert.ToInt16(Console.ReadLine());
            } while (numColumns < 1 || numColumns > MineBoard.MAX_COLUMNS);
            do {
              Console.Write("Enter number of mines [0-{0}]: ", numRows * numColumns);
              numMines = Convert.ToInt16(Console.ReadLine());
            } while (numMines < 0 || numMines > numRows * numColumns);
            break;
          }
          catch (Exception e) {
            Console.WriteLine("Error, wrong entry! Please try again.");
            continue;
          }
        }

        /* Debug
          Console.WriteLine("Number of rows: " + numRows);
          Console.WriteLine("Number of columns: " + numColumns);
          Console.WriteLine("Nummber of mines: " + numMines);
        */

        // Create game
        MineBoard matrixMines = new MineBoard(numRows, numColumns, numMines);

        // Game solving loop
        while (true) {

          // If game is not over, show fields
          if (matrixMines.Status == StatusGame.NotSolved) {

            // Enter coordinates to open field and wrong entry protection
            while (true) {
              try {
                Console.WriteLine("Open field");
                do {
                  Console.Write("Enter row number [1-{0}]: ", matrixMines.NumRows);
                  row = Convert.ToInt16(Console.ReadLine()) - 1;
                } while (row < 0 || row >= matrixMines.NumRows);
                do {
                  Console.Write("Enter column number [1-{0}]: ", matrixMines.NumColumns);
                  column = Convert.ToInt16(Console.ReadLine()) - 1;
                } while (column < 0 || column >= matrixMines.NumColumns);
                break;
              }
              catch (Exception e) {
                Console.WriteLine("Error, wrong entry! Please try again.");
                continue;
              }
            }

            // Open field
            StatusOpenField statusOpenField = matrixMines.OpenField(row, column);

            // Show all fields
            for (int i = 0; i < matrixMines.NumRows; ++i) {
              for (int j = 0; j < matrixMines.NumColumns; ++j) {
                Field field = matrixMines.ShowField(i, j);

                // Show field which is opened and there is no mine
                if ((int)field >= 0 && (int)field <= 8) {
                  Console.ForegroundColor = (int)field == 8 ? ConsoleColor.White : (ConsoleColor)((int)field + 8);
                  Console.Write((int)field + " ");
                  Console.ForegroundColor = ConsoleColor.Gray;
                }
                // Show field which is opened and has mine inside
                else if (field == Field.Mine) {
                  if (matrixMines.Status == StatusGame.Win)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                  else if (matrixMines.Status == StatusGame.Lose)
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                  Console.Write("* ");
                  Console.ForegroundColor = ConsoleColor.Gray;
                }
                // Show field which is not open yet
                else if (field == Field.NotOpen) {
                  Console.Write("? ");
                }
              }

              Console.WriteLine();
            }

            // If tried to open already opened field, show error and sent to input 
            // coordinates for opening other field
            if (statusOpenField == StatusOpenField.ErrorFieldOpen) {
              Console.WriteLine("Error, field with coordinates ({0}, {1}) has been already open!", row + 1, column + 1);
              continue;
            }

            Console.WriteLine("Exit <Ctrl+C>");

          }
          else { // If game over, write it on screen
            if (matrixMines.Status == StatusGame.Win)
              Console.WriteLine("Congratulations, you win.");
            else
              Console.WriteLine("Sorry, you lose.");

            // New game or quit the game
            Console.Write("Do you want to continue [y/n]: ");
            ConsoleKeyInfo answer = Console.ReadKey();
            Console.WriteLine();
            if (answer.KeyChar == 'n' || answer.KeyChar == 'N')
              return; // Program exit
            break; // New game
          }
        }
      }
    }
  }
}
