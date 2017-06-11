using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    static class Printer
    {
        
        //Print the sudoku
        static public void print_sudoku()
        {
            int N = Program.N;
            string direction = Program.direction;
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
                    if (direction == "fcmcv") number = Program.sudokufc[i, j].number;
                    else number = Program.sudoku[i, j];

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
    }

}
