/*
  Mine

  Description:
    This program is my remake of game minesweeper written as console application.
    User enters coordinates of field to open it. If field does not contain mine,
    a number of mines surrounding a field will be shown. If there are no mines around,
    all surrounded fields will be open. If user open field where mine is set, game is 
    over and user did not solve game. If user open all fields which have no
    mines inside, game is over and user successfully solved the game.

  Author: Dejan Cvijetinovic
  Date: 11.04.2019 (modified)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mine {
  class Program {

    // Constants used in class MineBoard
    const int
      MAX_ROWS = 20,
      MAX_COLUMNS = 35;

    // Status game of instance of MineBoard
    enum StatusGame {
      NotSolved = 0,
      Win = 1,
      Lose = 2
    }

    // Return values of method MineBoard.OtvorenoPolje()
    enum StatusOpenField {
      ErrorWrongEntry = -3,
      ErrorGameSolved = -2,
      ErrorFieldOpen = -1,
      Ok = 0
    }

    // Return values of method MineBoard.ShowField()
    enum Field {
      Error = -1,
      // 0..8 = open field
      Mine = 9,
      NotOpen = 10
    };

    class MineBoard {

      // All class members are private
      int numRows, numColumns, numMines;
      int numOpenedFields;
      StatusGame status;
      int[,] matrixMines;
      bool[,] openedField;

      // Getters for some of class members
      public int NumRows { get { return numRows; } }
      public int NumColumns { get { return numColumns; } }
      public StatusGame Status { get { return status; } }

      // Constructor
      public MineBoard(int iNumRows = 10, int iNumColumns = 10, int iNumMines = 8) {
        if (
          // Wrong entry protection, default values
          iNumRows < 1 || iNumRows > MAX_ROWS ||
          iNumColumns < 1 || iNumColumns > MAX_COLUMNS ||
          iNumMines < 0 || iNumMines > iNumRows * iNumColumns) {
          numRows = 10;
          numColumns = 10;
          numMines = 8;
        }
        else {

          // If all values are correct, add those to class members
          numRows = iNumRows;
          numColumns = iNumColumns;
          numMines = iNumMines;
        }

        /* Debug
          Console.WriteLine("Number of rows: " + numRows);
          Console.WriteLine("Number of columns: " + numColumns);
          Console.WriteLine("Number of mines: " + numMines);
        */

        // Initialization of rest of class members
        numOpenedFields = 0;
        status = StatusGame.NotSolved;
        matrixMines = new int[numRows, numColumns];
        openedField = new bool[numRows, numColumns];

        // Generate random numbers, set mines
        Random random = new Random();
        for (int i = 0; i < numMines; ++i) {
          int mina = random.Next(0, numRows * numColumns);

          /* Debug
            Console.WriteLine("Random: " + mina);
            Console.WriteLine("row: " + mina / numColumns);
            Console.WriteLine("column: " + mina % numColumns);
          */

          // In case of mine repeated on place where mine already exist, set mine on next free field
          // With this, time to set all mines is shorten
          while (matrixMines[mina / numColumns, mina % numColumns] == 9) {
            ++mina;
            if (mina == numRows * numColumns)
              mina = 0;
          }
          matrixMines[mina / numColumns, mina % numColumns] = 9;
        }
      }

      // Method for field access in purpose of show it on screen
      public Field ShowField(int row, int column) {
        // Wrong entry protection
        if (row < 0 || row >= numRows || column < 0 || column >= numColumns)
          return Field.Error;
        // Show value for requested field
        else if (openedField[row, column])
          return (Field)matrixMines[row, column];
        else
          return Field.NotOpen;
      }

      // Open field, count mines around requested field and open all fields
      // surrounding requested field if there are no mines around
      public StatusOpenField OpenField(int row, int column) {

        // Wrong entry protection
        if (row < 0 || row >= numRows || column < 0 || column >= numColumns)
          return StatusOpenField.ErrorWrongEntry;

        // Open already opened field protection and open field when game over protection
        // Exit condition for recursive calls
        if (status == StatusGame.NotSolved && !openedField[row, column]) {
          int foundMines = 0;
          openedField[row, column] = true;

          // Mine opened, Show rest of mines and finish the game
          if (matrixMines[row, column] == 9) {
            for (int i = 0; i < numRows; ++i)
              for (int j = 0; j < numColumns; ++j)
                if (matrixMines[i, j] == 9)
                  openedField[i, j] = true;
            status = StatusGame.Lose;
            return StatusOpenField.Ok;
          }

          // Counting mines around the field

          // Up left
          if (row - 1 >= 0 && column - 1 >= 0)
            if (matrixMines[row - 1, column - 1] == 9)
              ++foundMines;

          // Up
          if (row - 1 >= 0)
            if (matrixMines[row - 1, column] == 9)
              ++foundMines;

          // Up right
          if (row - 1 >= 0 && column + 1 < numColumns)
            if (matrixMines[row - 1, column + 1] == 9)
              ++foundMines;

          // Right
          if (column + 1 < numColumns)
            if (matrixMines[row, column + 1] == 9)
              ++foundMines;

          // Down right
          if (row + 1 < numRows && column + 1 < numColumns)
            if (matrixMines[row + 1, column + 1] == 9)
              ++foundMines;

          // Down
          if (row + 1 < numRows)
            if (matrixMines[row + 1, column] == 9)
              ++foundMines;

          // Down left
          if (row + 1 < numRows && column - 1 >= 0)
            if (matrixMines[row + 1, column - 1] == 9)
              ++foundMines;

          // Left
          if (column - 1 >= 0)
            if (matrixMines[row, column - 1] == 9)
              ++foundMines;

          // Mines counted, write mines count in field matrix
          matrixMines[row, column] = foundMines;
          ++numOpenedFields;

          // If all fields are opened, game over
          if (numOpenedFields == numRows * numColumns - numMines) {
            for (int i = 0; i < numRows; ++i)
              for (int j = 0; j < numColumns; ++j)
                if (!openedField[i, j])
                  openedField[i, j] = true;
            status = StatusGame.Win;
            return StatusOpenField.Ok;
          }

          /* debug
            Console.WriteLine("Open field: [{0}, {1}]", row, column);
            Console.WriteLine("Number of open fields: " + numOpenedFields);
          */

          // No mines around, open all surrounding fields (recursive calls)
          if (foundMines == 0) {

            // Up left
            if (row - 1 >= 0 && column - 1 >= 0)
              OpenField(row - 1, column - 1);

            // Up
            if (row - 1 >= 0)
              OpenField(row - 1, column);

            // Up right
            if (row - 1 >= 0 && column + 1 < numColumns)
              OpenField(row - 1, column + 1);

            // Right
            if (column + 1 < numColumns)
              OpenField(row, column + 1);

            // Down right
            if (row + 1 < numRows && column + 1 < numColumns)
              OpenField(row + 1, column + 1);

            // Down
            if (row + 1 < numRows)
              OpenField(row + 1, column);

            // Down left
            if (row + 1 < numRows && column - 1 >= 0)
              OpenField(row + 1, column - 1);

            // Left
            if (column - 1 >= 0)
              OpenField(row, column - 1);
          }
          return StatusOpenField.Ok; // No error, everything's fine
        }
        else { // Field has been already opened or game over
          if (openedField[row, column])
            return StatusOpenField.ErrorFieldOpen;
          else
            return StatusOpenField.ErrorGameSolved;
        }
      }
    }

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
              Console.Write("Enter number of rows [1-{0}]: ", MAX_ROWS);
              numRows = Convert.ToInt16(Console.ReadLine());
            } while (numRows < 1 || numRows > MAX_ROWS);
            do {
              Console.Write("Enter number of columns [1-{0}]: ", MAX_COLUMNS);
              numColumns = Convert.ToInt16(Console.ReadLine());
            } while (numColumns < 1 || numColumns > MAX_COLUMNS);
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