using System;
using System.Collections.Generic;
using System.Text;

public class AlphanumComparator<T> : IComparer<T>
{
    public int Compare(T x, T y)
    {
        if (x == null && y == null)
        {
            return 0;
        }
        if (x == null)
        {
            return -1;
        }
        if (y == null)
        {
            return 1;
        }
        var s1 = x.ToString();
        var s2 = y.ToString();
        int thisMarker = 0, thisNumericChunk = 0;
        int thatMarker = 0, thatNumericChunk = 0;
        while ((thisMarker < s1.Length) || (thatMarker < s2.Length))
        {
            if (thisMarker >= s1.Length)
            {
                return -1;
            }
            if (thatMarker >= s2.Length)
            {
                return 1;
            }
            var thisCh = s1[thisMarker];
            var thatCh = s2[thatMarker];
            var thisChunk = new StringBuilder();
            var thatChunk = new StringBuilder();
            while ((thisMarker < s1.Length) &&
                   (thisChunk.Length == 0 || InChunk(thisCh, thisChunk[0])))
            {
                thisChunk.Append(thisCh);
                thisMarker++;
                if (thisMarker < s1.Length)
                {
                    thisCh = s1[thisMarker];
                }
            }
            while ((thatMarker < s2.Length) &&
                   (thatChunk.Length == 0 || InChunk(thatCh, thatChunk[0])))
            {
                thatChunk.Append(thatCh);
                thatMarker++;
                if (thatMarker < s2.Length)
                {
                    thatCh = s2[thatMarker];
                }
            }
            var result = 0;
            // If both chunks contain numeric characters, sort them numerically
            if (char.IsDigit(thisChunk[0]) && char.IsDigit(thatChunk[0]))
            {
                thisNumericChunk = Convert.ToInt32(thisChunk.ToString());
                thatNumericChunk = Convert.ToInt32(thatChunk.ToString());
                if (thisNumericChunk < thatNumericChunk)
                {
                    result = -1;
                }
                if (thisNumericChunk > thatNumericChunk)
                {
                    result = 1;
                }
            }
            else
            {
                result = thisChunk.ToString().CompareTo(thatChunk.ToString());
            }
            if (result != 0)
            {
                return result;
            }
        }
        return 0;
    }

    private bool InChunk(char ch, char otherCh)
    {
        var type = ChunkType.Alphanumeric;
        if (char.IsDigit(otherCh))
        {
            type = ChunkType.Numeric;
        }
        if ((type == ChunkType.Alphanumeric && char.IsDigit(ch)) ||
            (type == ChunkType.Numeric && !char.IsDigit(ch)))
        {
            return false;
        }
        return true;
    }

    private enum ChunkType
    {
        Alphanumeric,
        Numeric
    };
}