using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ConsoleApplication1
{
    // A square variable in a sudoku grid
    class Square
    {
       public int row;
       public int column;
       public int number;
       public int domainSize;
       public List<int> variables;

        public Square()
        {
        }
    }

    // Location and size of a domain for a variable
    class Location
    {
        public int row;
        public int column;
        public int size;

        public Location()
        {
        }
    }

    // Sudoku grid
    class Sudoku_Grid
    {
        public Square[,] sudoku;
        Sudoku_Grid child;
        Sudoku_Grid parent;
        public int currentVariableIndex;
        Location MCV;
        int N;

        public Sudoku_Grid()
        {
            currentVariableIndex = 0;
            N = Program.N;
            sudoku = new Square[N, N];
        }

        // Method for cloning a sudoku grid
        public Sudoku_Grid Clone(Sudoku_Grid other)
        {
            // Make a new grid
            Sudoku_Grid returnClone = new Sudoku_Grid();
            returnClone.currentVariableIndex = 0;
            
            // Go through the sudoku grid that needs to be copied
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    // Make a new square, because we don't want to point to the old square
                    Square temp = new Square();
                    temp.variables = new List<int>();
                    // Add all the variables of the domain in the variables list of the new square
                    if (other.sudoku[i, j].variables != null)
                    {
                        foreach (int x in other.sudoku[i, j].variables) temp.variables.Add(x);
                    }
                    // Copy all the other membervariables
                    temp.row = other.sudoku[i, j].row;
                    temp.column = other.sudoku[i, j].column;
                    temp.number = other.sudoku[i, j].number;
                    temp.domainSize = other.sudoku[i, j].domainSize;
                    // Assign the square to the location in the grid
                    returnClone.sudoku[i, j] = temp;
                }
            }

            return returnClone;
        }

        // Recursive backtracking method that uses forward checking with the most constraining variable
        public void ForwardCheck()
        {
            // Check if the most constraining variable is empty
            if (MCV == null)
            {
                // Create a new location with a high domain size
                Location l = new Location();
                l.size = 900;

                //Check for each square in the sudoku which square has the lowest domainsize
                foreach (Square s in sudoku)
                {
                    if (!Program.unchangable[s.row, s.column])
                    {
                        if (s.domainSize <= l.size)
                        {
                            l.row = s.row;
                            l.column = s.column;
                            l.size = s.domainSize;
                            if (l.size == 1)
                                break;
                        }
                    }
                }
                // Assign the location to the most contraining variable
                MCV = l;
            }

            // If the most constraining variable has a size of 900, the sudoku is solved and print the solution
            if (MCV.size > 800)
            {
                //PrintSolution();
                Program.sudokufc = sudoku;
                Program.print_sudoku();
                return;
            }

            // Copy this grid to make changes in the child
            child = new Sudoku_Grid();
            child = Clone(this);
            child.parent = this;

            // Get location of most constraining square
            int row = MCV.row;
            int col = MCV.column;

            // Check if index has exceeded the amount of variables there are
            if (currentVariableIndex < sudoku[row, col].variables.Count)
            {
                
                int variable = sudoku[row, col].variables[currentVariableIndex];
                // Set the square to this variable
                child.sudoku[row, col].number = variable;

                // If there are no empty domains move next with the child
                if (child.MakeConsistent(row, col))
                {
                    
                    child.ForwardCheck();
                }
                // Else increase the variable index to try the next possible variable
                else
                {
                    currentVariableIndex++;
                    ForwardCheck();
                }
            }
            // If it has exceeded the amount of variables go back to the parent
            else
            {
                parent.currentVariableIndex++;
                parent.ForwardCheck();
            }
        }

        // Try to make the grid consistent and return a bool
        bool MakeConsistent(int row, int col)
        {
            int number = sudoku[row, col].number;
            //Remove from rows and columns
            for (int i = 0; i < N; i++)
            {
                //remove variable from domains in same row
                if (i != col&&!Program.unchangable[row,i])
                {
                    //If the number was succesfully removed, decrease domainsize by 1
                    if (sudoku[row, i].variables.Remove(number))
                    {
                        sudoku[row, i].domainSize--;
                        //if empty
                        if (sudoku[row, i].domainSize == 0)
                        {
                           // Console.WriteLine("deleted {0} from [{1},{2}], new domainsize is: {3}", number, row, i, sudoku[row, i].domainSize);
                            return false;
                        }
                    }
                }
                if (i != row && !Program.unchangable[i, col])
                {
                    //If the number was succesfully removed, decrease domainsize by 1
                    if (sudoku[i, col].variables.Remove(number))
                    {
                        sudoku[i, col].domainSize--;
                        // If empty
                        if (sudoku[i,col].domainSize == 0)
                        {
                            return false;
                        }
                    }
                }
            }
            // Variables to determine begin rows and columns
            int beginRow = Program.GetBeginRowOrColumn(row);
            int beginCol = Program.GetBeginRowOrColumn(col);
            int sN = (int)Math.Sqrt(N);

            // Remove from domains in the same box
            for (int i = 0; i < sN; i++)
                for (int j = 0; j < sN; j++)
                {
                    if ((beginRow + i != row) && (beginCol + j != col)&&!Program.unchangable[beginRow + i,beginCol+j])
                    {
                        if (sudoku[beginRow + i, beginCol + j].variables.Remove(number))
                        {
                            sudoku[beginRow + i, beginCol + j].domainSize--;
                            if (sudoku[beginRow + i, beginCol + j].domainSize == 0)
                            {
                                return false;
                            }
                        }
                    }
                }
            // Set domain size to 900 so that it won't be the most constraining variable in the MCV method
            sudoku[row, col].domainSize = 900;
            return true;

        }
    }

    class Program
    {
        // Choose which algorithm
        //const string direction = "left-down"; //Method 1 : left to right and downwards
        const string direction = "right-up"; //Method 2 : right to left and upwards
        //const string direction = "domain-oriented"; //Method 3 : based on amount of numbers that can be chosen from (domainsize)
        //const string direction = "fcmcv"; // Method 4 : forward checking based on the most constraining variable
        
        // Variables
        static public int N;
        static public int[,] sudoku;
        static public Square[,] sudokufc;
        static public bool[,] unchangable;
        static Square[] squaresArray;
        static Sudoku_Grid currentGrid;
        static bool foundsolution;
        static bool backwards;
        static int current_arrayindex = 0;
        static int row = 0;
        static int col = 0;
        static int endRow;
        static int endCol;

        static void Main(string[] args)
        {
            while (true)
            {
                foundsolution = false;
                backwards = false;
                //Get a line of numbers
                string readline = Console.ReadLine();
                string split = string.Join(" ", readline.ToCharArray());
                string[] line = split.Split(' ');
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
                        string readline2 = Console.ReadLine();
                        string split2 = string.Join(" ", readline2.ToCharArray());
                        line = split2.Split(' ');
                    }

                    // Store numbers in sudoku array
                    for (int j = 0; j < N; j++)
                    {
                        int number = int.Parse(line[j]);
                        if (number != 0) unchangable[i, j] = true;
                        sudoku[i, j] = number;
                    }
                }

                // Check which algorithm to execute
                if (direction == "domain-oriented") initialise_domainlist();
                else if (direction == "right-up") { row = N - 1; col = N - 1; endRow = 0; endCol = 0; }
                else if (direction == "left-down") { endRow = N - 1; endCol = N - 1; }

                if (direction == "fcmcv")
                {
                    initialise_forwardchecking();
                }
                else
                {
                    BackTrack();
                }
            }
        }

        // Initialise the forward checking algorithm
        static void initialise_forwardchecking()
        {
            // Make a new grid
            currentGrid = new Sudoku_Grid();
            // Add squares to the grid with their number, row and column
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    currentGrid.sudoku[i, j] = new Square();
                    currentGrid.sudoku[i, j].row = i;
                    currentGrid.sudoku[i, j].column = j;
                    currentGrid.sudoku[i, j].number = sudoku[i, j];
                    // Make a domain list of all the possible numbers a square can have if it is changeable
                    if (!unchangable[i, j])
                    {
                        currentGrid.sudoku[i, j].variables = new List<int>();
                        int size = 0;
                        for (int k = 1; k <= N; k++)
                        {
                            sudoku[i, j] = k;
                            if (!Violation(i, j))
                            {
                                currentGrid.sudoku[i, j].variables.Add(k);
                                size++;
                            }
                        }
                        // Reset the value
                        sudoku[i, j] = 0;
                        currentGrid.sudoku[i, j].domainSize = size;
                    }
                }
            }
            currentGrid.ForwardCheck();
        }

        // Initialse the backtrack on domain size algorithm
        static void initialise_domainlist()
        {
            List<Square> sortedSquares = new List<Square>();
            //Add squares with their domain size to the list
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
            // Sort the squares on domain size and convert to an array
            sortedSquares.Sort((d1, d2) => d1.domainSize.CompareTo(d2.domainSize));
            squaresArray = sortedSquares.ToArray();
            // Set the row and column to square with the lowest domain size
            row = squaresArray[0].row;
            col = squaresArray[0].column;
            // Set the last row and column to the last index of the array
            int lastIndex = squaresArray.Length - 1;
            endRow = squaresArray[lastIndex].row;
            endCol = squaresArray[lastIndex].column;
        }

        // Method to determine domain size of a square
        static int domainSize(int row, int col)
        {
            // Set size to 0
            int size = 0;
            // Check for every possible number if there is a violation
            for(int i = 1; i <= N; i++)
            {
                sudoku[row, col] = i;
                // If the number can be set increase the size
                if (!Violation(row,col)){
                    size++;
                }
            }
            // Set the square back to it's original number
            sudoku[row, col] = 0;
            return size;
        }

        // Check to see if a solution is found
        static bool FoundSolution()
        {
            if(row == endRow && col == endCol)
            {
                setandprint_found_solution();
                return true;
            }
            return false;
        }

        // Print solution and set the found solution to true;
        static void setandprint_found_solution()
        {
            print_sudoku();
            foundsolution = true;
        }

        //Print the sudoku
        static public void print_sudoku()
        {
            Console.WriteLine(" "); Console.WriteLine(" ");
            Console.WriteLine("{0}x{1} sudoku", N, N);
            Console.WriteLine("found solution:");
            Console.WriteLine(" ");

            // Lines in the middle of the sudoku
            string longLine = "";
            int numberL = N + (int)Math.Sqrt(N);
            // For a 9x9 sudoku
            if (N < 10)
            {
                for (int i = 0; i < numberL - 1; i++)
                {
                    longLine += "- ";
                }
            }
            // For double digits sudoku
            else
            {
                int counter1 = 0;
                for (int i = 0; i < numberL - 1; i++)
                {
                    if (counter1 == 4)
                    {
                        longLine += "- ";
                        counter1 = 0;
                    }
                    else
                    {
                        longLine += "-- ";
                        counter1++;
                    }
                }
            }

            int number = 0;

            // Writing the solution
            for (int i = 0; i < N; i++)
            {
                if (i % Math.Sqrt(N) == 0 && i != 0)
                {
                    Console.WriteLine(longLine);
                }
                string line = "";
                int counter2 = 0;
                for (int j = 0; j < N; j++)
                {
                    if (direction == "fcmcv") number = sudokufc[i, j].number;
                    else number = sudoku[i, j];

                    if (counter2 == Math.Sqrt(N) - 1 && j != N - 1)
                    {
                        if (number < 10 && N > 10)
                        {
                            line += number + "  | ";
                        }
                        else
                        {
                            line += number + " | ";
                        }
                        counter2 = 0;
                    }
                    else
                    {
                        if (number < 10 && N > 10)
                        {
                            line += number + "  ";
                        }
                        else
                        {
                            line += number + " ";
                        }
                        counter2++;
                    }
                }
                Console.WriteLine(line);
            }

        }

        // Move to a next square
        static void MoveNext()
        {
            backwards = false;
            // if a solution has been found, stop
            if (FoundSolution()) return;

            switch (direction)
            {
                case "left-down":
                    // Backtrack next variable
                    if (col < N - 1)
                    {
                        col++;
                    }
                    else
                    {
                        row++;
                        col = 0;
                    }
                    break;
                case "right-up":
                    // Backtrack next variable
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
                    // Backtrack next variable 
                    current_arrayindex++;
                    row = squaresArray[current_arrayindex].row;
                    col = squaresArray[current_arrayindex].column;
                    break;
            }
        }

        // Move a sqaure back
        static void MoveBack()
        {
            backwards = true;
            //Reset the current variable
            if (!unchangable[row, col])
                sudoku[row, col] = 0;

            switch (direction)
            {
                case "left-down":
                    // Backtrack previous variable
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
                    // Backtrack previous variable
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
                    // Backtrack previous variable
                    current_arrayindex--;
                    row = squaresArray[current_arrayindex].row;
                    col = squaresArray[current_arrayindex].column;
                    break;
            }
        }

        // Backtrack method
        static void BackTrack()
        {
            ulong recursivecalls = 0;
            DateTime dt = DateTime.Now;
            while (true)
            {

                // If a solution was found break the loop
                if (foundsolution)
                {
                    TimeSpan runTime = DateTime.Now -dt;
                    Console.WriteLine("Recursive calls: {0}", recursivecalls);
                    Console.WriteLine("Runtime: {0} milliseconds", runTime.TotalMilliseconds);
                    break;
                }

                recursivecalls++;
                //If the variable was given, move to the next or previous square
                if (unchangable[row, col])
                {
                    if (backwards) MoveBack();
                    else MoveNext();
                }
                else
                {
                    // If the number can still be incremented
                    if (sudoku[row, col] < N)
                    {
                        // Increment it by 1
                        sudoku[row, col]++;

                        // If this doesnt create a violation
                        if (!Violation(row,col))
                        {
                            // Move on to the next variable
                            MoveNext();
                        }
                    }
                    // If the number cant be incremented, moveback to the previous variable
                    else { MoveBack(); }
                }
            }
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
            int beginRow = GetBeginRowOrColumn(row);
            int beginCol = GetBeginRowOrColumn(col);
            int sN = (int)Math.Sqrt(N);
            // Check for box violation
            for ( int i = 0; i < sN; i++ )
                for ( int j = 0; j < sN; j++ )
                    if ( sudoku[beginRow + i, beginCol + j] == sudoku[row, col] && !( beginRow + i == row && beginCol + j == col )) return true;

            return false;
        }
        static public int GetBeginRowOrColumn(int row_or_col)
        {
            //Variables to determine begin rows and columns
            int beginRowOrCol = 0;
            int sN = (int)Math.Sqrt(N);

            //Determine which row we are to get the begin row
            int restRowOrCol = 0;
            while (restRowOrCol != sN)
            {
                if (row_or_col % sN == restRowOrCol)
                {
                    beginRowOrCol = row_or_col - restRowOrCol;
                    break;
                }
                restRowOrCol++;
            }
            return beginRowOrCol;
        }
    }
}