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
        static int[,] sudoku = new int[N, N];
        
        static void Main(string[] args)
        {
            while (true)
            {
                //Initialise sudoku puzzle
                //Get a line
                for (int i = 0; i < N; i++)
                {
                    //Get the numbers in a line
                    string[] numbers = Console.ReadLine().Split(' ');
                    //Store numbers in sudoku array
                    for (int j = 0; j < N; j++)
                    {
                        sudoku[i, j] = int.Parse(numbers[j]);
                    }
                }
            }
        }

        static bool Violation( int number, int row, int col )
        {
            if ( RowViolation( number, row, col )|| ColViolation( number, row, col )|| BoxViolation( number, row, col ) ) return true;

            return false;
        }

        static bool RowViolation( int number, int row, int col )
        {
            // Check for violations in a row
            for (int i = 0; i < N; i++)
                if (sudoku[row, i] == number) return true;

            return false;
        }

        static bool ColViolation( int number, int row, int col )
        {
            // Check for violations in a column
            for (int i = 0; i < N; i++)
                if (sudoku[i, col] == number) return true;

            return false;
        }

        static bool BoxViolation( int number, int row, int col )
        {
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
                    if (sudoku[beginRow + i, beginCol + j] == number) return true;

            return false;
        }
    }
}
