using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZTDIP.Algorithms.Sequences.Extensions;

namespace ZTDIP.Algorithms.Sequences.Shared
{
  /// <summary>
  /// Represents a Sequences in a Sequences database.
  /// </summary>
  public class Sequence : IEquatable<Sequence>
  {

    /// <summary>
    /// Array of items.
    /// </summary>
    internal uint[][] _items;
    private long? _hashCode;

    public Sequence()
    {
    }

    public Sequence(uint item)
    {
      _items = new[] 
        {
        new[] { item }
       };

      Length = 1;
    }

    public Sequence(uint item, int support)
      :this(item)
    {
      Support = support;
    }


    /// <summary>
    /// Initializes a new instance of Sequence class and copies items from lists into inner structure.
    /// </summary>
    /// <param name="seqItems">Items of Sequences.</param>
    /// <param name="sortItem">If items are not ordered and should be sorted.</param>
    public Sequence(IList<List<uint>> seqItems, bool sortItem = true)
    {
      _items = new uint[seqItems.Count][];

      for (int i = 0; i < _items.Length; i++)
      {
        List<uint> element = seqItems[i];

        _items[i] = new uint[element.Count];
        element.CopyTo(_items[i]);

        if (sortItem)
        {
          Array.Sort(_items[i]);
        }

        Length += element.Count;
      }
    }


    public int Support { get; set; }

    /// <summary>
    /// Gets or private sets count of items in the Sequences.
    /// </summary>
    public int Length { get; protected set; }

    /// <summary>
    /// Gets count of itemset in the Sequences.
    /// </summary>
    public int Size
    {
      get { return _items.Length; }
    }

    /// <summary>
    /// Gets an itemset with a given index.
    /// </summary>
    /// <param name="itemsetIndex">An index of a requested itemset.</param>
    public uint[] this[int itemsetIndex]
    {
      get { return _items[itemsetIndex]; }
    }

    public uint FirstItem
    {
      get { return _items[0][0]; }
    }

    public uint LastItem
    {
      get { return _items[_items.Length - 1][_items[_items.Length - 1].Length - 1]; }
    }

    /// <summary>
    /// If the last item was added to the Sequences by I-Step.
    /// </summary>
    public bool IsLastItemIExtension 
    { 
      get; private set; 
    }


    public bool Equals(Sequence other)
    {
      if (other == null) return false;

      if (Size != other.Size || Length != other.Length) return false;

      for (int sid = 0; sid < _items.Length; sid++)
      {
        if (_items[sid].Length != other._items[sid].Length) return false;

        for (int tid = 0; tid < _items[sid].Length; tid++)
        {
          if (_items[sid][tid] != other._items[sid][tid]) return false;
        }
      }

      return true;
    }

    public override String ToString()
    {
      var sb = new StringBuilder();
      ToString(sb, null);
      return sb.ToString();
    }

    public override int GetHashCode()
    {
      if(!_hashCode.HasValue)
      {
        _hashCode = _items.ComputeHash();
      }

      return (int) _hashCode.Value;
    }

    public override bool Equals(object right)
    {
      // check null:
      if (ReferenceEquals(right, null))
        return false;
      
      if (ReferenceEquals(this, right))
        return true;

      return Equals(right as Sequence);
    }




    /// <summary>
    /// Creates a new Sequences by adding a given item behind the end of a Sequences. 
    /// </summary>
    public static Sequence CreateNewBySStep(Sequence sourceSequence, uint item, int support)
    {
      //return CreateNew(sourceSequence, item, support);
      int sourceSequencesSize = sourceSequence.Size;

      var newSequence = new Sequence
      {
        Support = support,
        Length = sourceSequence.Length + 1,
        _items = new uint[sourceSequencesSize + 1][]
      };

      Array.Copy(sourceSequence._items, newSequence._items, sourceSequencesSize);

      newSequence._items[sourceSequencesSize] = new uint[] { item };

      return newSequence;
    }

    /// <summary>
    /// Creates a new Sequence by adding a given item to the last itemset. 
    /// </summary>
    public static Sequence CreateNewByIStep(Sequence sourceSequence, uint item, int support)
    {
      int lastItemsetIndex = sourceSequence.Size - 1;

      var newSequence = new Sequence
      {
        Support = support,
        IsLastItemIExtension = true,
        Length = sourceSequence.Length + 1,
        _items = new uint[sourceSequence.Size][]
      };

      Array.Copy(sourceSequence._items, newSequence._items, lastItemsetIndex);

      var lastItemset = sourceSequence._items[lastItemsetIndex].ToList();
      lastItemset.Add(item);
      newSequence._items[lastItemsetIndex] = lastItemset.ToArray();

      return newSequence;
    }

    /// <summary>
    /// Creates 1-length sequences from a given item list.
    /// </summary>
    public static IList<Sequence> CreateFromItems(IDictionary<uint,int> items)
    {
      IList<Sequence> sequences = new List<Sequence>();

      foreach (var item in items)
      {
        sequences.Add(new Sequence(item.Key) { Support =  item.Value} );
      }

      return sequences;
    }

    /// <summary>
    /// Appends string representation of Sequences to a given StringBuilder.
    /// </summary>
    public string ToString(IItemsManager itemsManager)
    {
      var sb = new StringBuilder();
      ToString(sb, itemsManager);
      return sb.ToString();
    }

    /// <summary>
    /// Appends string representation of Sequences to a given StringBuilder.
    /// </summary>
    public void ToString(StringBuilder builder, IItemsManager itemManager)
    {
      builder.AppendFormat("{0:D5} - <", Support);

      for (int i = 0, j = Size; i < j; i++)
      {
        uint[] itemset = _items[i];

        if (i > 0)
          builder.Append(" ");

        if (itemset.Length > 1)
          builder.Append("(");


        for (int k = 0; k < itemset.Length; k++)
        {
          if (k > 0)
            builder.Append(" ");

          if (itemManager != null)
          {
            builder.AppendFormat("'{0}'", itemManager.GetTextForItem(itemset[k]));
          }
          else
          {
            builder.Append(itemset[k]);
          }
        }


        if (itemset.Length > 1)
          builder.Append(")");
      }

      builder.Append(">");
    }

  }
}