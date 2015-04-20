using System.Collections.Generic;
using System.Text;

namespace ZTDIP.Algorithms.Sequences.Shared
{
    /// <summary>
    /// Calculates a variety of statistics for a set of sequences
    /// </summary>
    public class SequenceDatabaseStatistics
    {
      /// <summary>
      /// The number of sequences
      /// </summary>
      public long SequenceCount { get; protected set; }

      /// <summary>
      /// The total number of elements
      /// </summary>
      public long ElementCount { get; protected set; }

      /// <summary>
      /// The total number of items
      /// </summary>
      public long ItemCount { get; protected set; }

      /// <summary>
      /// The average number of elements in a <see cref="Sequence"/>
      /// </summary>
      public double AverageElements { get; protected set; }

      /// <summary>
      /// The average number of items in a <see cref="Sequence"/>
      /// </summary>
      public double AverageItems { get; protected set; }

      /// <summary>
      /// The average number of items in an element
      /// </summary>
      public double AverageItemsPerElement { get; protected set; }

      /// <summary>
      /// The number of distinct items
      /// </summary>
      public long DistinctItemCount { get; protected set; }

      /// <summary>
      /// The longest sequence (number of elements)
      /// </summary>
      public long LongestSequenceElementCount { get; protected set; }

      /// <summary>
      /// The longest sequence (number of items)
      /// </summary>
      public long LongestSequenceItemCount { get; protected set; }

      /// <summary>
      /// The longest element (number of items)
      /// </summary>
      public long LongestElement { get; protected set; }


      public SequenceDatabaseStatistics(IEnumerable<Sequence> sequences)
      {
        var uniqueItems = new HashSet<uint>();
        foreach (var sequence in sequences)
        {
          int itemsInSequence = 0;

          for (int sid = 0; sid < sequence.Size; sid++)
          {
            var element = sequence[sid];

            int itemsInElement = 0;
            
            foreach (var item in element)
            {
              uniqueItems.Add(item);
              ItemCount++;
              itemsInElement++;
              itemsInSequence++;
            }
            if (itemsInElement > LongestElement)
              LongestElement = itemsInElement;
            ElementCount++;
          }

          if (sequence.Size > LongestSequenceElementCount)
            LongestSequenceElementCount = sequence.Size;

          if (itemsInSequence > LongestSequenceItemCount)
            LongestSequenceItemCount = itemsInSequence;

          SequenceCount++;
        }

        DistinctItemCount = uniqueItems.Count;
        AverageElements = (double) (ElementCount) / SequenceCount;
        AverageItems = (double) (ItemCount) / SequenceCount;
        AverageItemsPerElement = (double) (ItemCount) / ElementCount;
      }

      public override string ToString()
      {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat("Sequence Statistics:\n");
        stringBuilder.AppendFormat("{0} sequences, containing a total of {1} elements and {2} items\n",
            SequenceCount,
            ElementCount,
            ItemCount);
        stringBuilder.AppendFormat("{0} unique items\n",
            DistinctItemCount);
        stringBuilder.AppendFormat("On average, a sequence contains {0:N2} elements and a total of {1:N2} items\n",
            AverageElements,
            AverageItems);
        stringBuilder.AppendFormat("On average, an element contained {0:N2} items\n",
            AverageItemsPerElement);
        stringBuilder.AppendFormat("The maximum sequence length was either {0} elements or {1} items\n",
            LongestSequenceElementCount,
            LongestSequenceItemCount);
        stringBuilder.AppendFormat("The longest element contained {0} items\n",
            LongestElement);
        return stringBuilder.ToString();
      }

    }
  }
