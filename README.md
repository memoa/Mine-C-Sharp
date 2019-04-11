# Mine-C-Sharp
This program is my remake of game minesweeper written as console application.
User enters coordinates of field to open it. If field does not contain mine,
a number of mines surrounding a field will be shown. If there are no mines around,
all surrounded fields will be open. If user open field where mine is set, game is 
over and user did not solve game. If user open all fields which have no
mines inside, game is over and user successfully solved the game.

## User interface
Game starts with entering dimensions of board (number of rows and columns) and number of mines

    *** Mine ***
    Creating new game
    Enter number of rows [1-20]: 6
    Enter number of columns [1-35]: 6
    Enter number of mines [0-36]: 4
*Example: Creating a board of 6 rows, 6 columns and 4 mines inside*
    
Then, game is created and program will ask for coordinates of field to open each time while game is not solved

    Open field
    Enter row number [1-6]: 1
    Enter column number [1-6]: 1
*Example: Entering coordinates to open field in first row and first column*
    
After entering coordinates, field is open and complete board is showed

    0 0 0 1 ? ?
    0 0 1 3 ? ?
    0 0 1 ? ? ?
    0 1 2 ? ? ?
    0 1 ? ? ? ?
    0 1 ? ? ? ?
    Exit <Ctrl+C>
*Example of board after field is open*

`0`..`8` - number of mines in surrounding fields
`?` - field is not open yet
`*` - there is a mine set in a field

Program can be quit any time by pressing `Ctrl+C`

If all fields without mines are open, then mine board is solved and all mines will be discovered.

    0 0 0 1 1 1
    0 0 1 3 * 2
    0 0 1 * * 2
    0 1 2 3 2 1
    0 1 * 1 0 0
    0 1 1 1 0 0
    Exit <Ctrl+C>
    Congratulations, you win.
*Example of solved mine board*

or if open a field where mine is set then game is over and all mines will be discovered, too.

    1 1 0 0 0 0
    * 1 0 0 1 1
    ? 2 0 0 1 *
    * 1 0 0 1 1
    ? 2 1 0 0 0
    ? * 1 0 0 0
    Exit <Ctrl+C>
    Sorry, you lose.
*Example of unsolved mine board, field with mine has been open*

When game is over, program offers to play new game

    Do you want to continue [y/n]:
    
Press `n` or `N` to quit the program, otherwise new game will be started.
