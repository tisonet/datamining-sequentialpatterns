using System.Linq;
using ZTDIP.Algorithms.Sequences.Extensions;
using ZTDIP.Algorithms.Sequences.PrefixSpan;
using System.Collections.Generic;

namespace ZTDIP.Algorithms.Sequences.Bide
{
  /// <summary>
  /// Helper class for bi-directional extension closure checking.
  /// </summary>
  internal class ClosureChecker
  {

    /// <summary>
    /// Checks if a given database contains backward-S-extension event.
    /// </summary>
    private static bool BackwardSExtensionCheck(ProjectedDatabase projectedDatabase)
    {
      var lastInLastAppearances = new int[projectedDatabase.Count];
      var seqMaximumPeriod = new HashSet<uint>();

      for (int i = projectedDatabase.Prefix.Size - 1; i >= 0; i--)
      {
        var ei = projectedDatabase.Prefix[i];

        var ithMaximumPeriod = default(HashSet<uint>);

        var isIthMaximumPeriodEmpty = false;

        for (int sid = 0; sid < projectedDatabase.Count; sid++)
        {
          PseudoSequence sequence = projectedDatabase[sid];

          // The i-th maximum period of a prefix sequence is defined:
          // 1) 1 < i <= n: it is the piece of sequence between the end 
          //                of the first instance of prefix e1e2...ei-1 and LLi.

          // 2) i == 1: it is the piece of sequence located before the first last-in-last appearance (LL1).   

          int lastInLast = sequence.GetLastInLast(ei, lastInLastAppearances[sid]);
          lastInLastAppearances[sid] = lastInLast;
         
          // If i-th maximum period is empty just continue fill in lastInLastAppearances for the checking  (i-1)-th maximum period.
          if (isIthMaximumPeriodEmpty) continue;

          int firstInstance = 0;
          if (i != 0) firstInstance = sequence.GetFirstInstance(i) + 1;

          seqMaximumPeriod.Clear();
          
          for (; firstInstance < lastInLast; firstInstance++)
          {
            seqMaximumPeriod.UnionWith(sequence.RawItems[firstInstance]);
          }

          // ScanSkip:
          if (ithMaximumPeriod == null)
          {
            ithMaximumPeriod = new HashSet<uint>(seqMaximumPeriod);   
          }
          else
          {
            ithMaximumPeriod.IntersectWith(seqMaximumPeriod);
          }

          // If i-th maximum period is empty and i==0 there cant exist any backward-S-extensions,
          // otherwise if i>0, try find backward-S-extensions in (i-1)-th maximum period.
          if (ithMaximumPeriod.Count != 0) continue;

          if (i == 0) return false;

          isIthMaximumPeriodEmpty = true;
        }

        // If all i-th maximum periods (in every sequence) contains any items, we got a backward-S-extension.
        if (ithMaximumPeriod.Count != 0) return true;
      }

      return false;
    }

    /// <summary>
    /// Checks if a given database contains backward-I-extension event.
    /// </summary>
    private static bool BackwardIExtensionCheck(ProjectedDatabase projectedDatabase)
    {
      var lastInLastAppearances = new int[projectedDatabase.Count];
      var seqBackwardIExtensionItemset = new HashSet<uint>();
      
      for (int i = projectedDatabase.Prefix.Size - 1; i >= 0; i--)
      {
        var ei = projectedDatabase.Prefix[i];

        var ithBackwardIExtensionItemset = default(HashSet<uint>);
       
        var isIthBackwardIExtensionItemsetEmpty = false;

        for (int sid = 0; sid < projectedDatabase.Count; sid++)
        {
          PseudoSequence sequence = projectedDatabase[sid];

          // The i-th I-extensions period of a prefix sequence is defined:
          // 1) 1 < i <= n: it is the piece of sequence between the end 
          //               of the first instance of prefix e1e2..ei-1 
          //               and the beginning of the first event after the LLi.

          // 2) i == 1: it is the piece of sequence located before the first event after LL1.    

          int lastInLast = sequence.GetLastInLast(ei, lastInLastAppearances[sid]);
          lastInLastAppearances[sid] = lastInLast;

          // If ith maximum period is empty just continue fill in lastInLastAppearances for the checking  (i-1)-th maximum period.
          if (isIthBackwardIExtensionItemsetEmpty) continue;

          int firstInstance = 0;
          if (i != 0) firstInstance = sequence.GetFirstInstance(i) + 1;

          seqBackwardIExtensionItemset.Clear();
          for (; firstInstance <= lastInLast; firstInstance++)
          {
            if (ei.IsSubsetOf(sequence.RawItems[firstInstance]))
            {
              seqBackwardIExtensionItemset.UnionWith(sequence.RawItems[firstInstance]);
            }
          }
          seqBackwardIExtensionItemset.ExceptWith(ei);


          // ScanSkip:
          if (ithBackwardIExtensionItemset == null)
          {
            ithBackwardIExtensionItemset = new HashSet<uint>(seqBackwardIExtensionItemset);
          }
          else
          {
            ithBackwardIExtensionItemset.IntersectWith(seqBackwardIExtensionItemset);
          }

          // If ith maximum period is empty and i==0 there cant exist any backward-I-extension,
          // otherwise if i>0, try find backward-I-extension in (i-1)-th maximum period.
          if (ithBackwardIExtensionItemset.Count != 0) continue;

          if (i == 0) return false;

          isIthBackwardIExtensionItemsetEmpty = true;
        }

        // If all ith maximum periods (in every sequence) contains some items we got a backward-I-extension.
        if (ithBackwardIExtensionItemset.Count > 0) return true;
      }

      return false;
    }

    private static bool BackScanS(ProjectedDatabase projectedDatabase)
    {
      var lastInFirstAppearances = new int[projectedDatabase.Count];
      var seqSemiMaximumPeriod = new HashSet<uint>();

      for (int i = projectedDatabase.Prefix.Size - 1; i >= 0; i--)
      {
        var ei = projectedDatabase.Prefix[i];

        var ithSemiMaximumPeriod = default(HashSet<uint>);

        var isIthSemiMaximumPeriodEmpty = false;

        for (int sid = 0; sid < projectedDatabase.Count; sid++)
        {
          PseudoSequence sequence = projectedDatabase[sid];

          int lastInFirst = sequence.GetLastInFirst(i, ei, lastInFirstAppearances[sid]);
          lastInFirstAppearances[sid] = lastInFirst;

          // If i-th semimaximum period is empty just continue fill in lastInFirstAppearances for the checking  (i-1)-th semimaximum period.
          if (isIthSemiMaximumPeriodEmpty) continue;

          int firstInstance = 0;
          if (i != 0) firstInstance = sequence.GetFirstInstance(i) + 1;

          seqSemiMaximumPeriod.Clear();
          for (; firstInstance < lastInFirst; firstInstance++)
          {
            seqSemiMaximumPeriod.UnionWith(sequence.RawItems[firstInstance]);
          }

          // ScanSkip:
          if (ithSemiMaximumPeriod == null)
          {
            ithSemiMaximumPeriod = new HashSet<uint>(seqSemiMaximumPeriod);
          }
          else
          {
            ithSemiMaximumPeriod.IntersectWith(seqSemiMaximumPeriod);
          }

          // If i-th maximum period is empty and i==0 there cant exist any backward-S-extensions,
          // otherwise if i>0, try find backward-S-extensions in (i-1)-th maximum period.
          if (ithSemiMaximumPeriod.Count != 0) continue;

          if (i == 0) return false;

          isIthSemiMaximumPeriodEmpty = true;
        }

        // If all i-th maximum periods (in every sequence) contains any items, we got a backward-S-extension.
        if (ithSemiMaximumPeriod.Count != 0) return true;
      }

      return false;
    }

    private static bool BackScanI(ProjectedDatabase projectedDatabase)
    {
      var lastInFirstAppearances = new int[projectedDatabase.Count];
      var seqSemiMaximumPeriod = new HashSet<uint>();
      var n = projectedDatabase.Prefix.Size - 1;

      for (int i = n; i >= 0; i--)
      {
        var ei = projectedDatabase.Prefix[i];
        var eiLastItemId = ei.Last();
        var ithSemiMaximumPeriod = default(HashSet<uint>);

        var isIthSemiMaximumPeriodEmpty = false;

        for (int sid = 0; sid < projectedDatabase.Count; sid++)
        {
          PseudoSequence sequence = projectedDatabase[sid];

          int lastInFirst = sequence.GetLastInFirst(i, ei, lastInFirstAppearances[sid]);
          lastInFirstAppearances[sid] = lastInFirst;

          // If i-th semimaximum period is empty just continue fill in lastInFirstAppearances for the checking  (i-1)-th semimaximum period.
          if (isIthSemiMaximumPeriodEmpty) continue;

          int firstInstance = 0;
          if (i != 0) firstInstance = sequence.GetFirstInstance(i) + 1;

          seqSemiMaximumPeriod.Clear();
          for (; firstInstance <= lastInFirst; firstInstance++)
          {
            if (!ei.IsSubsetOf(sequence.RawItems[firstInstance])) continue;

            seqSemiMaximumPeriod.UnionWith(
              i == n 
              ? sequence.RawItems[firstInstance].Where(a => a < eiLastItemId) 
              :  sequence.RawItems[firstInstance]);
          }
          seqSemiMaximumPeriod.ExceptWith(ei);

          // ScanSkip:
          if (ithSemiMaximumPeriod == null)
          {
            ithSemiMaximumPeriod = new HashSet<uint>(seqSemiMaximumPeriod);
          }
          else
          {
            ithSemiMaximumPeriod.IntersectWith(seqSemiMaximumPeriod);
          }

          // If i-th maximum period is empty and i==0 there cant exist any backward-S-extensions,
          // otherwise if i>0, try find backward-S-extensions in (i-1)-th maximum period.
          if (ithSemiMaximumPeriod.Count != 0) continue;

          if (i == 0) return false;

          isIthSemiMaximumPeriodEmpty = true;
        }

        // If all i-th maximum periods (in every sequence) contains any items, we got a backward-S-extension.
        if (ithSemiMaximumPeriod.Count != 0) return true;
      }

      return false;
    }

    /// <summary>
    /// Checks if a given database contains some backward-extension event.
    /// </summary>
    public static bool BackwardExtensionCheck(ProjectedDatabase projectedDatabase)
    {
      return BackwardSExtensionCheck(projectedDatabase) || BackwardIExtensionCheck(projectedDatabase);
    }

    /// <summary>
    /// Returns TRUE if a prefix of projection can be safely pruned.
    /// </summary>
    public static bool BackScan(ProjectedDatabase projectedDatabase)
    {
      return BackScanS(projectedDatabase) || BackScanI(projectedDatabase);
    }

  }
}