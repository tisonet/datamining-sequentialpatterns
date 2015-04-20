namespace ZTDIP.Algorithms.Sequences.Extensions
{
  public static class ArrayExtensions
  {
    private const int NOT_FOUND = -1;

    /// <summary>
    /// Returns true, if the me contains all items from a given me.
    /// </summary>
    /// <param name="lastIndex">Constraint, cuts a given me and checks only the first half.</param>
    public static bool IsSubsetOf(this uint[] me, uint[] array, int lastIndex)
    {
      if (lastIndex + 1 < me.Length) return false;

      int actualItemIndex = 0;
      for (int i = 0; i <= lastIndex; i++)
      {
        if (array[i] != me[actualItemIndex]) continue;

        if (++actualItemIndex == me.Length) return true;

      }
      return false;
    }

    /// <summary>
    /// Returns true, if a given me contains all items from this me.
    /// </summary>
    public static bool IsSubsetOf(this uint[] me, uint[] array)
    {
      if (array == null || array.Length < me.Length) return false;
     
      int actualItemIndex = 0;
      for (int i = 0; i < array.Length; i++)
      {
        if (array[i] != me[actualItemIndex]) continue;

        if (++actualItemIndex == me.Length) return true;
      }
      return false;
    }

    /// <summary>
    /// Gets the me slice between the two indexes.
    /// Inclusive for start index, exclusive for end index.
    /// </summary>
    public static uint[] Slice(this uint[] me, int start)
    {
      int len = me.Length - start;

      // Return new me.
      var res = new uint[len];
      for (int i = 0; i < len; i++)
      {
        res[i] = me[i + start];
      }
      return res;
    }

    /// <summary>
    /// Returns the last item of me.
    /// </summary>
    public static uint Last(this uint[] me)
    {
      return me[me.Length - 1];
    }

    /// <summary>
    /// Binary search or half-interval search algorithm finds the position of a specified value within a sorted me.
    /// </summary>
    /// <param name="value">A searching value.</param>
    /// <returns>If found returns index of me where item was found,
    /// otherwise return ArraySearch.NOT_FOUND.</returns>
    public static int BinarySearch(this uint[] me, uint value)
    {
      if (me != null)
      {
        int low = 0, high = me.Length - 1, midpoint = 0;

        while (low <= high)
        {
          midpoint = low + (high - low) / 2;

          // check to see if value is equal to item in me
          if (value == me[midpoint])
          {
            return midpoint;
          }
          else if (value < me[midpoint])
            high = midpoint - 1;
          else
            low = midpoint + 1;
        }
      }
      return -1;
    }

    /// <summary>
    /// Sequential search is a method for finding a particular value in a list, 
    /// that consists of checking every one of its elements, one at a time and in sequence, 
    /// until the desired one is found.
    /// </summary>
    /// <param name="value">A searching value.</param>
    /// <returns>If found returns index of me where item was found,
    /// otherwise return ArraySearch.NOT_FOUND.</returns>
    public static int SequentialSearch(this uint[] me, uint value)
    {
      int arrayLength = me.Length;
      for (int i = 0; i < arrayLength; i++)
      {
        if (me[i] == value) return i;
      }
      return NOT_FOUND;
    }

    /// <summary>
    /// Compute the hascode of a array.
    /// </summary>
    public static int ComputeHash(this uint[] me)
    {
      unchecked
      {
        const int prime = 16777619;
        var hash = (int) 2166136261;

        for (int i = 0; i < me.Length; i++)
          hash = (hash ^ (int)me[i]) * prime;

        hash += hash << 13;
        hash ^= hash >> 7;
        hash += hash << 3;
        hash ^= hash >> 17;
        hash += hash << 5;
        
        return hash;
      }
    }

    /// <summary>
    /// Compute the hascode of a 2D array.
    /// </summary>
    public static long ComputeHash(this uint[][] me) 
    {
      unchecked
      {
        const int prime = 16777619;
        long hash = 2166136261;

        for (int i = 0; i < me.Length; i++)
        {
          for (int j = 0; j < me[i].Length; j++)
          {
            hash = (hash ^ me[i][j] ^ (i*j))*prime;
          }
        }

        hash += hash << 13;
        hash ^= hash >> 7;
        hash += hash << 3;
        hash ^= hash >> 17;
        hash += hash << 5;

        return hash;
      }
    }
  }
}