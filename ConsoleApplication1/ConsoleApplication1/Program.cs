using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {

        //Variables
        static int N;
        //Array to store sudoku puzzle in
        static int[,] sudoku;
        static bool[,] unchangable;
        static bool backwards = false;
        const string direction = "right-up"; //Method 1 : left to right and downwards
        //const string direction = "right-up"; //Method 2 : right to left and upwards
        static int row = 0;
        static int col = 0;
        static bool foundsolution = false;


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
                if (direction == "right-up") { row = N - 1; col = N - 1; }

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
                BackTrack();
            }
        }

        static bool FoundSolution()
        {
            if (direction == "left-down") {
                if (row == N - 1 && col == N - 1)
                {
                    Console.WriteLine("----------------------------");
                    print_sudoku();
                    foundsolution = true;
                    Console.WriteLine("foundsolution");
                    return true;
                }
            }
            else if (direction == "right-up") {
                if (row == 0 && col == 0)
                {
                    Console.WriteLine("----------------------------");
                    print_sudoku();
                    foundsolution = true;
                    Console.WriteLine("foundsolution");
                    return true;
                }
            }
            return false;
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
            }
        }
        static void BackTrack()
        {
            while (true)
            {
                if (foundsolution) break;
               // print_sudoku();
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
                        if (!Violation())
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
        static bool Violation()
        {
            if ( RowViolation() || ColViolation() || BoxViolation() ) return true;

            return false;
        }

        // Check for violations in a row
        static bool RowViolation()
        {
            for (int i = 0; i < N; i++)
            {
                if ( sudoku[row, i] == sudoku[row, col] && i != col ) return true;
            }
            return false;
        }

        // Check for violations in a column
        static bool ColViolation( )
        {
            for ( int i = 0; i < N; i++ )
                if ( sudoku[i, col] == sudoku[row, col] && i != row ) return true;

            return false;
        }

        // Check for violations in a sqrt(N) by sqrt(N) box
        static bool BoxViolation()
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