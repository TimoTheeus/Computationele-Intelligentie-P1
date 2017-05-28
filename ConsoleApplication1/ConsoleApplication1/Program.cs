using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static int N = 9;
        //Array to store sudoku puzzle in
        static int[,] sudoku;
        static bool[,] unchangable;
        static bool backwards = false;
        const string direction = "left-right";
        static void Main(string[] args)
        {
            while (true)
            {
                //Initialise sudoku puzzle
                //Get a line of numbers
                string[] firstLine = Console.ReadLine().Split(' ');
                //determine sudoku puzzle size
                N = firstLine.Length;
                //make the array to store the sudoku
                sudoku = new int[N, N];
                unchangable = new bool[N, N];
                //store the first line
                for(int k =0; k < N; k++)
                {
                    int number = int.Parse(firstLine[k]);
                    if (number != 0) unchangable[0, k] = true;
                    sudoku[0, k] = number;
                }
                //store remaining lines
                for (int i = 1; i < N; i++)
                {
                    //Get the numbers in a line
                    string[] numbers = Console.ReadLine().Split(' ');
                    //Store numbers in sudoku array
                    for (int j = 0; j < N; j++)
                    {
                        int number = int.Parse(numbers[j]);
                        if (number != 0) unchangable[i, j] = true;
                        sudoku[i, j] = int.Parse(numbers[j]);
                    }
                }
                BackTrack(0, 0);
            }
        }
        static void MoveNext(int row, int col)
        {
            backwards = false;
            //Solution found
            if (row == N - 1 && col == N - 1) {
                Console.WriteLine("foundsolution");
                return;
            }
            switch (direction)
            {
                case "left-right":
                    {
                        //Backtrack next variable
                        if (col < N - 1) { BackTrack(row, col + 1); }
                        else { BackTrack(row + 1, 0); }
                    }
                    break;
            }
        }
        static void MoveBack(int row, int col)
        {
            backwards = true;
            //Reset the current variable
            sudoku[row, col] = 0;
            switch (direction)
            {
                case "left-right":
                    {
                        //Backtrack previous variable
                        if (col ==0) { BackTrack(row-1, N-1); }
                        else { BackTrack(row, col-1); }
                    }
                    break;
            }
        }
        static void BackTrack(int row, int col)
        {
            print_sudoku();
            //If the variable was given, move to the next or previous
            if (unchangable[row, col]) {
                if (backwards) MoveBack(row, col);
                else MoveNext(row, col);
            }
            else
            {
                //If the number can still be incremented
                if (sudoku[row, col] < N){
                    //increment it by 1
                    sudoku[row, col]++;

                    //If this doesnt create a violation
                    if (!Violation(row, col)){
                        //Move on to the next variable
                        MoveNext(row, col);
                    }
                    //Otherwise backtrack the same variable
                    else BackTrack(row, col);
                }
                //If the number cant be incremented, moveback to the previous variable
                else { MoveBack(row, col); }
            }
        }
        static void print_sudoku()
        {
            for(int i =0; i < sudoku.GetLength(1);i++)
            {
                string line = "";
                for (int j = 0; j < sudoku.GetLength(0);j++)
                {
                    line += sudoku[i,j] + " ";
                }
                Console.WriteLine(line);
            }
            Console.WriteLine("----------------------------");
        }
        static bool Violation(int row, int col )
        {
            if ( RowViolation(row, col )|| ColViolation(row, col )|| BoxViolation(row, col ) ) return true;

            return false;
        }

        static bool RowViolation(int row, int col )
        {
            int number = sudoku[row, col];
            // Check for violations in a row
            for (int i = 0; i < N; i++)
            {
                if (sudoku[row, i] == number&&i!=col) return true;
            }
            return false;
        }

        static bool ColViolation(int row, int col )
        {
            int number = sudoku[row, col];
            // Check for violations in a column
            for (int i = 0; i < N; i++)
                if (sudoku[i, col] == number&&i!=row) return true;

            return false;
        }

        static bool BoxViolation(int row, int col )
        {
            int number = sudoku[row, col];
            int beginRow = 0;
            int beginCol = 0;
            int sN = (int)Math.Sqrt(N);

            int restRow = 0;
            while (restRow != sN)
            {
                if (row % sN == restRow)
                {
                    beginRow = row - restRow;
                    break;
                    //endRow = beginRow + sN - 1;
                }
                restRow++;
            }

            int restCol = 0;
            while (restCol != sN)
            {
                if (col % sN == restCol)
                {
                    beginCol = col - restCol;
                    break;
                    //endCol = beginCol + sN - 1;
                }
                restCol++;
            }

            // Check for box violation
            for (int i = 0; i < sN; i++)
                for (int j = 0; j < sN; j++)
                    if (sudoku[beginRow + i, beginCol + j] == number&&!(beginRow+i==row&&beginCol+j==col)) return true;

            return false;
        }
    }
}
