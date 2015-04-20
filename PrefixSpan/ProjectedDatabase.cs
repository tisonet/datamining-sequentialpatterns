using System.Collections.Generic;
using System.Linq;
using ZTDIP.Algorithms.Sequences.Extensions;
using ZTDIP.Algorithms.Sequences.Shared;

namespace ZTDIP.Algorithms.Sequences.PrefixSpan
{

    /// <summary>
    /// Represent a projected database. 
    /// Consists from a prefix sequence and collection of projected sequences, 
    /// with regard to the prefix sequence.
    /// </summary>
    internal class ProjectedDatabase : List<PseudoSequence>
    {

      private readonly Sequence _prefix;

      /// <summary>
      /// Dictionary of all found itemset-extension items in this projected database,
      /// and their corresponding support.  
      /// </summary>
      private IDictionary<uint, int> _iExtensions;

      /// <summary>
      /// Dictionary of all found sequence-extension items in this projected database,
      /// and their corresponding support.  
      /// </summary>
      private IDictionary<uint, int> _sExtensions;

      public ProjectedDatabase()
      {
        _prefix = new Sequence();
      }

      public ProjectedDatabase(Sequence prefix)
      {
        _prefix = prefix;
      }

      /// <summary>
      /// Gets the count of sequences in the projected database.
      /// </summary>
      public int Size
      {
        get { return Count; }
      }

      public Sequence Prefix
      {
        get { return _prefix; }
      }

      public bool IsNotEmpty
      {
        get { return (Size > 0); }
      }

      public override string ToString()
      {
        return Prefix.ToString();
      }

      /// <summary>
      /// Finds all items in the projected database and counts their support.
      /// </summary>
      private void FindLocalItems()
      {
        // Two types of last items, y and (_y).
        // Rules for next item:
        // 1) y:    xyz => xi,y,zi
        // 2) y:    (xyz) => xi, y, zi, (_zi)
        // 3) y:    (_xyz) => (_xi), (_y), (_zi)
        // 4) (_y): xyz => xi,y,zi    
        // 5) (_y): (xyz) => xi,y,zi
        // 6) (_y): (_z) => (_zi)
        // 7) (v):  (wz) => wi, zi, (_zi), w is superset of v.
        // Where x,z can be zero, one or more items and v,w one or more items   

        _sExtensions = new Dictionary<uint, int>();
        _iExtensions = new Dictionary<uint, int>();

        var prefixLastItemset = Prefix[Prefix.Size - 1];

        var alreadyAddedSExt = new HashSet<uint>();
        var alreadyAddedIExt = new HashSet<uint>();
 
        for (int sid = 0, lastSid = Size; sid < lastSid; sid++)
        {
          // Already counted items in a one sequence.
          alreadyAddedSExt.Clear();
          alreadyAddedIExt.Clear();

          for (int tid = 0, lastTid = this[sid].Size; tid < lastTid; tid++)
          {
            var actualItemset = this[sid][tid];

            var isLastItemsetOfPrefixSubsetOfActualItemset = false;
            var isActualItemsetPostfix = (tid == 0 && this[sid].IsLastItemIExtension);

            for (int itemId = 0, lastItemId = actualItemset.Length; itemId < lastItemId; itemId++)
            {
              uint actualItem = actualItemset[itemId];
              
              // An actual itemset is postfix:
              // => 3) and 6).
              if (isActualItemsetPostfix)
              {
                if (!alreadyAddedIExt.Add(actualItem)) continue;

                IncrementSupport(_iExtensions, actualItem);
              }
              else
              {
                // An actual itemset is not postfix:
                // => 1), 2), 4), 5), 7).

                // Always => xi,y,zi.
                if (alreadyAddedSExt.Add(actualItem))
                {
                  IncrementSupport(_sExtensions, actualItem);
                }

                // If an actual itemset contains the last itemset of prefix: 
                // => 2), 7) => (_zi).
                if (!isLastItemsetOfPrefixSubsetOfActualItemset)
                {
                  isLastItemsetOfPrefixSubsetOfActualItemset = prefixLastItemset.IsSubsetOf(actualItemset, itemId - 1);
                }

                if (isLastItemsetOfPrefixSubsetOfActualItemset && alreadyAddedIExt.Add(actualItem))
                {
                  IncrementSupport(_iExtensions, actualItem);
                }
              }
            }
          }
        }
      }

      /// <summary>
      /// Increment the support value of a given extension.
      /// </summary>
      private static void IncrementSupport(IDictionary<uint, int> extensionContainer, uint extension)
      {
        int support;
        extensionContainer.TryGetValue(extension, out support);
        extensionContainer[extension] = ++support;
      }

      /// <summary>
      /// Gets all local frequent items found in the projected database.
      /// </summary>

      public void FindLocalFrequentItems(
        double minSupport, 
        IDictionary<uint, int> sequenceExtensionsItems,
        IDictionary<uint, int> itemsetExtensionsItems)
      {
        FindLocalItems();

        foreach (var pair in _sExtensions.Where(pair => pair.Value >= minSupport))
        {
          sequenceExtensionsItems.Add(pair.Key, pair.Value);
        }

        foreach (var pair in _iExtensions.Where(pair => pair.Value >= minSupport))
        {
          itemsetExtensionsItems.Add(pair.Key, pair.Value);
        }
      }
    }

}