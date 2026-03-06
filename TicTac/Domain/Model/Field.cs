using System;
using System.Reflection;

namespace Domain.Model;
public class Field
{
    #region Properties
    public const int MaxSize = 3;
    public int[,] Matrix { get; set; } =
    {
        { 0, 0, 0 },
        { 0, 0, 0 },
        { 0, 0, 0 }

    };
    public int EmptyCountCells
    {
        get
        {
            int count = 0;
            for (int i = 0; i < MaxSize; i++)
            {
                for (int j = 0; j < MaxSize; j++)
                {
                    if (Matrix[i, j] == 0)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
    public int this[int row,int column]{
        get
        {
            if (row < 0 || row >= MaxSize || column < 0 || column >= MaxSize)
                throw new IndexOutOfRangeException();
            return Matrix[row, column];
        }
        set
        {
            if (row < 0 || row >= MaxSize || column < 0 || column >= MaxSize)
                throw new IndexOutOfRangeException();
            Matrix[row, column] = value;
        }
    }
    #endregion Properties
    #region Constructors
    #endregion Constructors
    #region Methods
    /// <summary>
    /// Определяет кто победил в игре
    /// </summary>
    /// <returns>
    /// -1 - Игра продолжается
    ///  0 - Ничья
    ///  1 - Победил X
    ///  2 - Победил O
    /// </returns>
    public int CheckWin()
    {
        int horizontalWinCondition = CheckHorizontalWinCondition();
        int verticalWinCondition = CheckVerticalWinCondition();
        int diagonalWinCondition = CheckDiagonalWinCondition();
        int draw = horizontalWinCondition + verticalWinCondition+diagonalWinCondition;
        if (draw == 0 && CheckDraw()) return draw; // draw
        else if (draw == 0 && !CheckDraw()) return -1; // continue game 
        return draw;
    }

    #region PrivateMethods
    private int CheckHorizontalWinCondition()
    {
        bool res = false;
        for (int row = 0; row < MaxSize; row++)
        {
            res= Matrix[row, 0] == Matrix[row, 1] && Matrix[row, 1] == Matrix[row, 2] && Matrix[row, 0] != 0 && Matrix[row, 1] != 0 && Matrix[row, 2] != 0;
            if (res)
            {
                return Matrix[row, 0];
            }
        }
        return 0;
    }
    private int CheckVerticalWinCondition()
    {
        bool res = false;
        for (int col = 0; !res && col < MaxSize; col++)
        {
            res = Matrix[0, col] == Matrix[1, col] && Matrix[1, col] == Matrix[2, col] && Matrix[0, col] != 0 && Matrix[1, col] != 0 && Matrix[2, col] != 0;
            if (res)
            {
                return Matrix[0, col];
            }
        }
        return 0;
    }
    private int CheckDiagonalWinCondition()
    {
        bool res = (Matrix[0, 0] == Matrix[1, 1] && Matrix[1, 1] == Matrix[2, 2] && Matrix[0, 0] != 0 && Matrix[1, 1] != 0 && Matrix[2, 2] != 0) ||
        (Matrix[0, 2] == Matrix[1, 1] && Matrix[1, 1] == Matrix[2, 0] && Matrix[0, 2] != 0 && Matrix[1, 1] != 0 && Matrix[2, 0] != 0);
        if (res) return Matrix[1, 1];
        return 0;
    }
    private bool CheckDraw()
    {
        for(int i = 0; i < MaxSize; i++)
        {
            for (int j = 0; j < MaxSize; j++) { 
                if(Matrix[i, j] == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }
    #endregion PrivateMethods
    #region ICloneable
    public Field Clone()
        {
            Field newField = new Field();
            for (byte i = 0; i < 3; i++)
            {
                for (byte j = 0; j < 3; j++)
                {
                    newField.Matrix[i, j] = this.Matrix[i, j];
                }
            }
            return newField;
        }
#if DEBUG
    public void PrintField()
    {
        for (byte i = 0; i < 3; i++)
        {
            for (byte j = 0; j < 3; j++)
            {
                Console.Write(Matrix[i, j] + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine("==============");
    }

#endif
    #endregion ICloneable
    #endregion Methods
}
/*
    #region Properties
    #endregion Properties
    #region Constructors
    #endregion Constructors
    #region Methods
    #endregion Methods 
 */
