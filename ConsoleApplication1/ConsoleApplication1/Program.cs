using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    struct Square
    {
       public int row;
       public int column;
       public int number;
       public int domainSize;
       public List<int> variables;
    }

    struct Location
    {
        public int row;
        public int column;
        public int size;
    }
    class Sudoku_Grid
    {
        public Sudoku_Grid parent;
        public Location[] sorted_on_domainsize;
        public Square[,] sudoku;
        public int currentSquareIndex;
        public int currentVariableIndex;
        int N;
        public Sudoku_Grid()
        {
            currentSquareIndex = 0;
            currentVariableIndex = 0;
            N = Program.N;
        }
        public void ForwardCheck()
        {
            //Copy this grid to make changes
            Sudoku_Grid child = this;
            //get location of most constraining square
            int row = sorted_on_domainsize[currentSquareIndex].row;
            int col = sorted_on_domainsize[currentSquareIndex].column;
            //get a variable of the most constraining square
            int variable = sudoku[row, col].variables[currentVariableIndex];
            //Set the square to this variable
            child.sudoku[row, col].number = variable;
            MakeConsistent(child, row, col);
        }
        void MakeConsistent(Sudoku_Grid grid,int row, int col)
        {
            int number = grid.sudoku[row, col].number;
            for(int i = 0; i < N; i++)
            {
                //remove variable from domains in same row
                if (i != col)
                {
                    grid.sudoku[row, i].variables.Remove(number);
                    //if empty
                    if (!grid.sudoku[row, i].variables.Any())
                    {
                        //move to next index
                        if (currentVariableIndex < grid.sudoku[row, col].domainSize - 1)
                            currentVariableIndex++;
                        else
                        {
                            currentVariableIndex = 0;
                            currentSquareIndex++;
                        }
                    }
                }
                //remove variable from domains in same column
                grid.sudoku[i, col].variables.Remove(number);
            }
        }
    }
    class Program
    {
        //Variables
        static public int N;
        //Array to store sudoku puzzle in
        static public int[,] sudoku;
        static bool[,] unchangable;
        static Sudoku_Grid currentGrid;
        static bool backwards = false;
        //const string direction = "left-down"; //Method 1 : left to right and downwards
        //const string direction = "right-up"; //Method 2 : right to left and upwards
        //const string direction = "domain-oriented"; //Method 3 : based on amount of numbers that can be chosen from (domainsize)
        const string direction = "fcmcv";
        static int row = 0;
        static int col = 0;
        static bool foundsolution = false;
        static Square[] squaresArray;
        static int current_arrayindex = 0;
        static int endRow;
        static int endCol;

        static void Main(string[] args)
        {
            while (true)
            {
                //Initialise sudoku puzzle
                //Get a line of numbers
                string[] line = Console.ReadLine().Split(' ');
                //Determine sudoku puzzle size
                N = line.Length;
                //Make the array to store the sudoku
                sudoku = new int[N, N];
                unchangable = new bool[N, N];
                
                

                //Store lines
                for (int i = 0; i < N; i++)
                {
                    //Get the numbers in a line
                    if (i != 0)
                    {
                        line = Console.ReadLine().Split(' ');
                    }
                    //Store numbers in sudoku array
                    for (int j = 0; j < N; j++)
                    {
                        int number = int.Parse(line[j]);
                        if (number != 0) unchangable[i, j] = true;
                        sudoku[i, j] = number;
                    }
                }

                if ( direction == "domain-oriented") initialise_domainlist(); 
                else if (direction == "right-up") { row = N - 1; col = N - 1; endRow = 0; endCol = 0; }
                else if (direction == "left-down"){ endRow = N - 1; endCol = N - 1; }

                BackTrack();
            }
        }
        static void fcMCV()
        {
        }
        static void initialise_forwardchecking()
        {
            currentGrid = new Sudoku_Grid();
            List<Location> sortedLocations = new List<Location>();
            //Add squares with their domain size to the 
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    currentGrid.sudoku[i, j].row = i;
                    currentGrid.sudoku[i, j].column = j;
                    currentGrid.sudoku[i, j].number = sudoku[i, j];
                    if (!unchangable[i, j])
                    {
                        int size = 0;
                        for (int k = 1; k <= N; k++)
                        {
                            sudoku[row, col] = k;
                            if (!Violation(row, col))
                            {
                                currentGrid.sudoku[i, j].variables.Add(k);
                                size++;
                            }
                        }
                        sudoku[row, col] = 0;
                        currentGrid.sudoku[i, j].domainSize = size;
                        Location l = new Location();
                        l.row = i;
                        l.column = j;
                        l.size = size;
                        sortedLocations.Add(l);
                    }
                }
            }
            currentGrid.sorted_on_domainsize = sortedLocations.ToArray();
            Array.Sort(currentGrid.sorted_on_domainsize, (x, y) => x.size.CompareTo(y.size));
        }
        static void initialise_domainlist()
        {
            List<Square> sortedSquares = new List<Square>();
            //Add squares with their domain size to the 
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (!unchangable[i, j])
                    {
                        Square sqr = new Square();
                        sqr.row = i;
                        sqr.column = j;
                        sqr.domainSize = domainSize(i,j);
                        sortedSquares.Add(sqr);
                    }
                }
            }
            sortedSquares.Sort((d1, d2) => d1.domainSize.CompareTo(d2.domainSize));
            squaresArray = sortedSquares.ToArray();
            row = squaresArray[0].row;
            col = squaresArray[0].column;
            int lastIndex = squaresArray.Length - 1;
            endRow = squaresArray[lastIndex].row;
            endCol = squaresArray[lastIndex].column;
        }
        static int domainSize(int row, int col)
        {
            int size = 0;
            for(int i = 1; i <= N; i++)
            {
                sudoku[row, col] = i;
                if (!Violation(row,col)){
                    size++;
                }
            }
            sudoku[row, col] = 0;
            return size;
        }
        static bool FoundSolution()
        {
            if(row == endRow && col == endCol)
            {
                setandprint_found_solution();
                return true;
            }
            return false;
        }
        static void setandprint_found_solution()
        {
            Console.WriteLine("----------------------------");
            print_sudoku();
            foundsolution = true;
            Console.WriteLine("foundsolution");
        }
        static void MoveNext()
        {
            backwards = false;
            //Solution found
            if (FoundSolution()) return;

            switch (direction)
            {
                case "left-down":
                        //Backtrack next variable
                        if (col < N - 1)
                        {
                            col++;
                            //BackTrack();
                        }
                        else {
                            row++;
                            col = 0;
                            //BackTrack();
                        }
                        break;
                case "right-up":
                        //Backtrack next variable
                        if (col != 0)
                        {
                            col--;
                        }
                        else
                        {
                            row--;
                            col = N-1;
                        }
                    break;
                case "domain-oriented":
                    current_arrayindex++;
                    row = squaresArray[current_arrayindex].row;
                    col = squaresArray[current_arrayindex].column;
                    break;
            }
        }
        static void MoveBack()
        {
            backwards = true;
            //Reset the current variable
            //Reset the current variable
            if (!unchangable[row, col])
                sudoku[row, col] = 0;
            switch (direction)
            {
                case "left-down":
                        //Backtrack previous variable
                        if (col == 0)
                        {
                            row--;
                            col = N - 1;
                        }
                        else
                        {
                            col--;
                        }
                    break;
                case "right-up":
                    //Backtrack previous variable
                    if (col == N - 1)
                    {
                        row++;
                        col = 0;
                    }
                    else
                    {
                        col++;
                    }
                    break;
                case "domain-oriented":
                    current_arrayindex--;
                    row = squaresArray[current_arrayindex].row;
                    col = squaresArray[current_arrayindex].column;
                    break;
            }
        }
        static void BackTrack()
        {
            while (true)
            {
                if (foundsolution) break;
                //print_sudoku();
                //If the variable was given, move to the next or previous
                if (unchangable[row, col])
                {
                    if (backwards) MoveBack();
                    else MoveNext();
                }
                else
                {
                    //If the number can still be incremented
                    if (sudoku[row, col] < N)
                    {
                        //increment it by 1
                        sudoku[row, col]++;

                        //If this doesnt create a violation
                        if (!Violation(row,col))
                        {
                            //Move on to the next variable
                            MoveNext();
                        }
                    }
                    //If the number cant be incremented, moveback to the previous variable
                    else { MoveBack(); }
                }
            }
        }

        //Print the sudoku
        static void print_sudoku()
        {
            for(int i =0; i < sudoku.GetLength(1);i++)
            {
                string line = "";
                for ( int j = 0; j < sudoku.GetLength(0); j++ )
                {
                    line += sudoku[i,j] + " ";
                }
                Console.WriteLine(line);
            }
            Console.WriteLine("----------------------------");
        }

        //Check for row, column and box violations
        static bool Violation(int row, int col)
        {
            if ( RowViolation(row,col) || ColViolation(row, col) || BoxViolation(row, col) ) return true;

            return false;
        }

        // Check for violations in a row
        static bool RowViolation(int row, int col)
        {
            for (int i = 0; i < N; i++)
            {
                if ( sudoku[row, i] == sudoku[row, col] && i != col ) return true;
            }
            return false;
        }

        // Check for violations in a column
        static bool ColViolation(int row, int col)
        {
            for ( int i = 0; i < N; i++ )
                if ( sudoku[i, col] == sudoku[row, col] && i != row ) return true;

            return false;
        }

        // Check for violations in a sqrt(N) by sqrt(N) box
        static bool BoxViolation(int row, int col)
        {
            //Variables to determine begin rows and columns
            int beginRow = 0;
            int beginCol = 0;
            int sN = (int)Math.Sqrt(N);

            //Determine which row we are to get the begin row
            int restRow = 0;
            while ( restRow != sN )
            {
                if ( row % sN == restRow )
                {
                    beginRow = row - restRow;
                    break;
                }
                restRow++;
            }

            //Determine which column we are to get the begin column
            int restCol = 0;
            while ( restCol != sN )
            {
                if ( col % sN == restCol )
                {
                    beginCol = col - restCol;
                    break;
                }
                restCol++;
            }

            // Check for box violation
            for ( int i = 0; i < sN; i++ )
                for ( int j = 0; j < sN; j++ )
                    if ( sudoku[beginRow + i, beginCol + j] == sudoku[row, col] && !( beginRow + i == row && beginCol + j == col )) return true;

            return false;
        }
    }
}