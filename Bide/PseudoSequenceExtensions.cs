using System.Linq;
using ZTDIP.Algorithms.Sequences.Extensions;
using ZTDIP.Algorithms.Sequences.PrefixSpan;

namespace ZTDIP.Algorithms.Sequences.Bide
{
  /// <summary>
  /// Extensions of PseudoSequence for BIDE features.
  /// </summary>
  internal static class PseudoSequenceExtensions
  {

    /// <summary>
    /// Returns the last in last appearance of the prefix ei event in sequence.
    /// </summary>
    /// <param name="ei">I-th event of the prefix.</param>
    /// <param name="last">Last in last appearance of ei+1 event.</param>
    /// <returns></returns>
    public static int GetLastInLast(this PseudoSequence me, uint[] ei, int last)
    {
      var items = me.RawItems;

      int i = (last == 0 ? items.Length : last) - 1;

      for (; i >= 0; i--)
      {
        if (ei.IsSubsetOf(items[i])) return i;
      }
      return 0;
    }

    /// <summary>
    /// Returns the first instance of the prefix (i-1)-th event in sequence.  
    /// </summary>
    /// <param name="me">PseudoSequence.</param>
    /// <param name="i">Index of i-th event in the prefix.</param>
    public static int GetFirstInstance(this PseudoSequence me, int i)
    {
      return me._firstInstances[i];
    }

    /// <summary>
    /// Returns the last in first appearance of a given ei event.
    /// </summary>
    /// <param name="i">Index of ei event in prefix.</param>
    /// <param name="ei">I-th event of the prefix.</param>
    /// <param name="last">Last in first appearance of ei+1 event.</param>
    public static int GetLastInFirst(this PseudoSequence me, int i, uint[] ei, int last)
    {
      if(last == 0)
      {
        return me._firstInstances.Last();
      }

      var items = me.RawItems;

      for (int j = last - 1; j >= 0; j--)
      {
        if (ei.IsSubsetOf(items[j])) return j;
      }

      return 0;
    }

  }
}
