using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ConsoleApplication1
{
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
            Printer.print_sudoku();
            return;
        }
        Program.recursivecalls++;
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
            if (i != col && !Program.unchangable[row, i])
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
                    if (sudoku[i, col].domainSize == 0)
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
                if ((beginRow + i != row) && (beginCol + j != col) && !Program.unchangable[beginRow + i, beginCol + j])
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
}}