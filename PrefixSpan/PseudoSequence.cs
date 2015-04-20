using System.Collections.Generic;
using System.Text;
using ZTDIP.Algorithms.Sequences.Extensions;
using ZTDIP.Algorithms.Sequences.Shared;

namespace ZTDIP.Algorithms.Sequences.PrefixSpan
{
  /// <summary>
  /// Represents a PseudoSequence in a sequence database.
  /// Consists of a projected sequence 
  /// and the starting position of the projected postfix in a corresponding sequence.
  /// </summary>
  internal class PseudoSequence
  {

    private readonly uint[][] _items;

    /// <summary>
    /// First itemset of the projection. It can be different than in original sequence,
    /// because first item can start from middle, denoted as (_x...)... 
    /// </summary>
    private readonly uint[] _firstItemset;
    private readonly int _offset;
    private readonly Sequence _prefix;

    /// <summary>
    /// If the last item of the prefix was append by I-Step.
    /// </summary>
    private readonly bool _isLastItemIExtension;

    /// <summary>
    /// The list of prefix events first instances in a source sequence.
    /// </summary>
    internal IList<int> _firstInstances;


    /// <summary>
    /// Initializes a new instance of the PseudoSequence class.
    /// Pseudo projection starts at the first item.
    /// </summary>
    public PseudoSequence(Sequence sequence)
    {
      _items = sequence._items;
      _firstItemset = _items[0];
      _firstInstances = new List<int>() { -1 };
    }

    /// <summary>
    /// Initializes a new instance of the PseudoSequence class.
    /// </summary>
    /// <param name="isLastItemIExtension">If the last item of the prefix was append by I-Step. </param>
    public PseudoSequence(Sequence prefix, PseudoSequence sequence, int offset, bool isLastItemIExtension)
    {
      _offset = offset;
      _prefix = prefix;
      _items = sequence._items;
      _isLastItemIExtension = isLastItemIExtension;

      if (offset < _items.Length)
      {
        _firstItemset = _items[offset];
      }

      SetFirsInstanceOfPrefix(sequence);
    }

    /// <summary>
    /// Initializes a new instance of the PseudoSequence class.
    /// </summary>
    /// <param name="isLastItemIExtension">If the last item of the prefix was append by I-Step. </param>
    public PseudoSequence(Sequence prefix, PseudoSequence sequence, int offset, uint[] firstItemset, bool isLastItemIExtension)
      : this(prefix, sequence, offset, isLastItemIExtension)
    {
      _firstItemset = firstItemset;
    }

    /// <summary>
    /// Returns all items form source sequence.
    /// </summary>
    internal uint[][] RawItems
    {
      get { return _items; }
    }

    /// <summary>
    /// Gets the count of itemset in the sequence.
    /// </summary>
    public int Size
    {
      get { return _items.Length - _offset; }
    }

    public bool IsNotEmpty
    {
      get { return (Size > 0); }
    }

    /// <summary>
    /// Gets itemset at a given index.
    /// </summary>
    /// <param name="itemsetIndex">An index of requested itemset.</param>
    public uint[] this[int itemsetIndex]
    {
      get { return itemsetIndex == 0 ? _firstItemset : _items[_offset + itemsetIndex]; }
    }

    /// <summary>
    /// If the last item was added to the sequence by IStep.
    /// </summary>
    public bool IsLastItemIExtension
    {
      get { return _isLastItemIExtension; }
    }


    public override string ToString()
    {
      var builder = new StringBuilder();

      for (int i = 0, j = Size; i < j; i++)
      {
        uint[] itemset = this[i];

        if (i > 0)
          builder.Append(" ");

        if (itemset.Length > 1)
          builder.Append("(");


        for (int k = 0; k < itemset.Length; k++)
        {
          if (k > 0)
            builder.Append(" ");

          builder.Append(itemset[k]);
        }

        if (itemset.Length > 1)
          builder.Append(")");
      }

      builder.Append(">");

      return builder.ToString();
    }


    /// <summary>
    /// Creates pseudo-projected sequence from a given sequence, 
    /// the projection will be start at the first occurence of the last prefix item.  
    /// </summary>
    /// <param name="firstItemsetIndex">An index of the first itemset, where should be started finding occurence of the last prefix item.</param>
    public static PseudoSequence MakeProjection(PseudoSequence sequence, Sequence prefix, int firstItemsetIndex = 0, bool removeEmpty = true)
    {
      // Two types of prefix last item: y and (_y).
      // Rules for projection:
      // 1) y:    xyz... => z...
      // 2) y:    (xyz)... => (_z)...
      // 3) (_y): (vyz)... => (_z)...
      // 4) (_y): (_xyz)... => (_z)...
      // Where x,z can be zero, one or more items and v one or more items, 
      // and v union y contains all items from the last itemset of prefix.

      var prefixLastItem = prefix.LastItem;
      var lastItemsetIndex = sequence.Size - 1;
      var prefixLastItemset = prefix[prefix.Size - 1];
      var isPrefixLastItemsetIExtensions = prefix.IsLastItemIExtension;

      // Finds last item of prefix and creates projection.
      for (int tid = firstItemsetIndex, lastTid = sequence.Size; tid < lastTid; tid++)
      {
        var isActualItemsetPostfix = (tid == 0 && sequence.IsLastItemIExtension);

        // Projection is not allow in situation: (x): (_x).... => {}
        if (!isPrefixLastItemsetIExtensions && isActualItemsetPostfix) continue;

        var actualItemset = sequence[tid];
        var lastItemIndexInActualItemset = actualItemset.Length - 1;

        // Every item in an actual itemset compare with prefix last item.
        for (var itemId = 0; itemId < actualItemset.Length; itemId++)
        {
          if (actualItemset[itemId] != prefixLastItem) continue;

          // If is not the end of a sequence makes projection, otherwise stop.
          if (removeEmpty && tid == lastItemsetIndex && itemId == lastItemIndexInActualItemset) return null;

          // 1) y: xyz... => z...,  
          // 2) y: (xyz)... => (_z)...
          if (!isPrefixLastItemsetIExtensions ||
              // OR 4) (_y): (_xyz)... => (_z)...
              isActualItemsetPostfix ||
              // OR 3) (_y): (vyz)... => (_z)...
              prefixLastItemset.IsSubsetOf(actualItemset, itemId))
          {
              return itemId == lastItemIndexInActualItemset 
              ? 
              new PseudoSequence(prefix, sequence, sequence._offset + tid + 1, false) 
              : 
              new PseudoSequence(prefix, sequence, sequence._offset + tid, sequence[tid].Slice(itemId + 1),  true);
          }
        }
      }
      return null;
    }

    /// <summary>
    /// Checks if the sequence contains a given item.
    /// </summary>
    /// <returns>Returns -1 when sequence does not contain a given item, 
    /// otherwise return index of itemset where was an item found.</returns>
    public int Contains(uint item)
    {
      if (_firstItemset.BinarySearch(item) != -1)
        return 0;

      for (var sid = _offset + 1; sid < _items.Length; )
      {
        if (_items[sid++].BinarySearch(item) == -1) continue;

        return sid - _offset - 1;
      }

      return -1;
    }


    /// <summary>
    /// Sets the first instance of prefix in the pseudo sequence.
    /// </summary>
    /// <param name="sequence"></param>
    private void SetFirsInstanceOfPrefix(PseudoSequence sequence)
    {
      _firstInstances = new List<int>(sequence._firstInstances);

      if (_prefix.IsLastItemIExtension)
      {
        _firstInstances[_firstInstances.Count - 1] = _isLastItemIExtension ? _offset : _offset - 1;
      }
      else
      {
        _firstInstances.Add(_isLastItemIExtension ? _offset : _offset - 1);
      }
    }

  }
}