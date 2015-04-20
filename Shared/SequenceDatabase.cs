using System.Collections.Generic;
using System.Linq;
using ZTDIP.Algorithms.Sequences.PrefixSpan;
using ZTDIP.Algorithms.Sequences.Spam;
using ZTDIP.Algorithms.Sequences.Spam.Bitmaps;

namespace ZTDIP.Algorithms.Sequences.Shared
{
  /// <summary>
  /// Represent a sequence database, it consists of a list of sequences. 
  /// </summary>
  public class SequenceDatabase : List<Sequence>
  {
    public SequenceDatabase()
    {

    }

    public SequenceDatabase(IEnumerable<Sequence> sequences)
    {
      AddRange(sequences);
    }

    /// <summary>
    /// Scans sequence database and finds frequent items.
    /// </summary>
    /// <returns></returns>
    public IList<Sequence> FindOneLengthSequentialPattern(double minSupport)
    {
      Sequence itemSequence;
      
      var oneLengthSequentialPatterns = new List<Sequence>();
      var onelengthSequences = new Dictionary<uint, Sequence>();
      var alreadyAddedItems = new HashSet<uint>();

      foreach (Sequence sequence in this)
      {
        alreadyAddedItems.Clear();

        for (int tid = 0; tid < sequence.Size; tid++)
        {
          var itemset = sequence[tid];

          for (int iid = 0; iid < itemset.Length; iid++)
          {
            if (!alreadyAddedItems.Add(itemset[iid])) continue;
            
            if (onelengthSequences.TryGetValue(itemset[iid], out itemSequence))
            {
              itemSequence.Support++;
            }
            else
            {
              itemSequence = new Sequence(itemset[iid]) { Support = 1 };
              onelengthSequences.Add(itemset[iid], itemSequence);
            }
          }
        }
      }


      foreach (Sequence sequence in onelengthSequences.Values)
      {
        if (sequence.Support < minSupport) continue;

        oneLengthSequentialPatterns.Add(sequence);
      }


      return oneLengthSequentialPatterns.OrderBy(a => a.FirstItem).ToList();
    }

    /// <summary>
    /// Converts sequence database to the projected sequence database.
    /// </summary>
    internal ProjectedDatabase ConvertToProjectedDatabase()
    {
      var projectedDatabase = new ProjectedDatabase();

      for (int sid = 0, j = this.Count; sid < j; ++sid)
      {
        projectedDatabase.Add(new PseudoSequence(this[sid]));
      }

      return projectedDatabase;
    }

    /// <summary>
    /// Converts sequence database to the vertical format.
    /// </summary>
    public VerticalSequenceDatabase ConvertToVerticalFormat(IDictionary<uint, int> frequentItems)
    {
      
      var verticalDb = new VerticalSequenceDatabase();
      SeqBitmap seqBitmap;

      foreach (Sequence seq in this)
      {
        foreach (var item in frequentItems)
        {
          if (!verticalDb.TryGetValue(item.Key, out seqBitmap))
          {
            seqBitmap = new SeqBitmap {Support = item.Value};
           
            verticalDb.Add(item.Key, seqBitmap);
          }

          seqBitmap.AddSequence(seq.Size);
        }

        for (int tid = 0; tid < seq.Size; tid++)
        {
          foreach (uint item in seq[tid])
          {
            verticalDb[item].SetTransaction(tid);
          }
        }
      }

      return verticalDb;
    }
  }
}