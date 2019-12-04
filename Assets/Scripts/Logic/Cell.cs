using System;
using System.Collections;

public class Cell : ICloneable
{
    private int horizontal = 0;

    private int vertical = 0;
    
    public int Horizontal
    {
        get
        {
            return horizontal;
        }
        set
        {
            if ((value >= 0) && (value < 8))
            {
                horizontal = value;
            }
        }
    }

    public int Vertical
    {
        get
        {
            return vertical;
        }
        set
        {
            if ((value >= 0) && (value < 8))
            {
                vertical = value;
            }
        }
    }

    public Cell(int vertical, int horizontal)
    {
        Vertical = vertical;
        Horizontal = horizontal;
    }

    public Cell(String cellString)
    {
        if (cellString.Length != 2)
        {
            return;
        }
        Vertical = cellString[0] - 'a';
        Horizontal = cellString[1] - '1';
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Cell p = (Cell)obj;
            return (horizontal == p.horizontal) && (vertical == p.vertical);
        }
    }

    public override int GetHashCode()
    {
        return (vertical * 10) + horizontal;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    public override string ToString()
    {
        char verticalChar = Convert.ToChar(vertical + Convert.ToInt32('a'));
        char horizontalChar = Convert.ToChar(vertical + Convert.ToInt32('1'));
        return "" + verticalChar + horizontalChar;
    }
}
