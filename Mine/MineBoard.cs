/*
  Class MineBoard, core of the game

  Description:
    matrixMines:
      A matrix which contains numbers of mines surrounding that field, only for
      those fields which are open
    openedField:
      A matrix which contains true values for fields which are open. For other
      fields value false is assigned.
    Constructor:
      Constructor creates virtual mineboard with dimensions and number of
      mines assigned to parameters. It randomly assings mines to fields.
      If default constructor is called or wrong values are in constructor's
      parameters, then mineboard (10 x 10) with 8 mines will be created.
    ShowField:
      Used to give information of field in mineboard, specified by coordinates
      in parameters, for graphical interpretation. Returns data in enum Field type.
    OpenField:
      Opens field specified by coordinates in parameters, counting mines around
      that field and opens all fields around if there are no mines surrounding 
      them. If there is no mine in a field, count of mines will be added in 
      corresponding element of 'matrixMines' matrix, and value true will be added
      to corresponding element of 'openedField' matrix, to give a sign that field
      should not be opened anymore. If there is a mine then corresponding element
      in 'matrixField' will be assigned as '9' to mark a mine where stepped on.
      Method returns values if field is open or not depending of game status and 
      state of field. Return value is type of enum StatusOpenField.
    
  Author: Dejan Cvijetinovic
  Date: 11.04.2019
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mine {
  // Status game of instance of MineBoard
  public enum StatusGame {
    NotSolved = 0,
    Win = 1,
    Lose = 2
  }

  // Return values of method MineBoard.OpenField()
  public enum StatusOpenField {
    ErrorWrongEntry = -3,
    ErrorGameSolved = -2,
    ErrorFieldOpen = -1,
    Ok = 0
  }

  // Return values of method MineBoard.ShowField()
  public enum Field {
    Error = -1,
    // 0..8 = open field
    Mine = 9,
    NotOpen = 10
  };

  public class MineBoard {
    // Board maximum dimensions
    public const int
      MAX_ROWS = 20, // maximum value for number of rows
      MAX_COLUMNS = 35; // maximum value for number of columns

    // All class members are private
    private int 
      numRows, // Number of rows, user defined in constructor
      numColumns, // Number of columns, user defined in constructor
      numMines; // Number of mines, user defined in constructor
    private int numOpenedFields; // Number of open fields
    private StatusGame status; // Game status, NotSolved, Win or Lose
    private int[,] matrixMines; // Matrix of fields of the board
    private bool[,] openedField; // Matrix of values if fields are open (true) or not (false)

    // Getters for some of class members
    public int NumRows { get { return numRows; } } // Get number of rows
    public int NumColumns { get { return numColumns; } } // Get number of columns
    public StatusGame Status { get { return status; } } // Get game status

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
}
