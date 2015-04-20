using System.Collections.Generic;
using ZTDIP.Algorithms.Sequences.Bits;
using ZTDIP.Algorithms.Sequences.Spam.Bitmaps;

namespace ZTDIP.Algorithms.Sequences.LapinSpam
{
  internal class ItemIsExistTable
  {
    /// <summary>
    /// Item is exist table bitVectors for a different size of sequences. 
    /// </summary>
    private byte[,][] _bitVectors8;
    private byte[,][] _bitVectors16;
    private byte[,][] _bitVectors32;
    private byte[,][] _bitVectors64;
    private byte[,][] _bitVectors128;

    private readonly double _minSupport;
    private readonly IList<SeqBitmap> _sequenceDatabase;

    public int FrequentItemsCount
    {
      get { return _sequenceDatabase.Count; }
    }

    public ItemIsExistTable(IList<SeqBitmap> sequenceDatabase, double minSupport)
    {
      _sequenceDatabase = sequenceDatabase;
      _minSupport = minSupport;
    }


    /// <summary>
    /// For a given item sets to true all transaction before a given last transaction in a given sequence.
    /// </summary>
    private static void SetBitVectorForItem(byte[,][] bitVectors, uint item, int sid, int lastTransaction)
    {
      for (int tid = 0; tid < lastTransaction; tid++)
      {
        bitVectors[sid, tid][item] = 1;
      }
    }

    /// <summary>
    /// Fill in bit vectors in the ITEM_IS_EXIST_TABLE according to a last positions in sequences.
    /// </summary>
    private void FillBitVectors()
    {
      uint itemOrder = 0;
      foreach (var item in _sequenceDatabase)
      {
        var itemBitmaps8 = item.Bitmaps8;
        for (int sid = 0; sid < item.Sequences8Count; sid++)
        {
          SetBitVectorForItem(_bitVectors8, itemOrder, sid, BitsHelper.LastSetBit(itemBitmaps8[sid]));
        }

        var itemBitmaps16 = item.Bitmaps16;
        for (int sid = 0; sid < item.Sequences16Count; sid++)
        {
          SetBitVectorForItem(_bitVectors16, itemOrder, sid, BitsHelper.LastSetBit(itemBitmaps16[sid]));
        }

        var itemBitmaps32 = item.Bitmaps32;
        for (int sid = 0; sid < item.Sequences32Count; sid++)
        {
          SetBitVectorForItem(_bitVectors32, itemOrder, sid, BitsHelper.LastSetBit(itemBitmaps32[sid]));
        }

        var itemBitmaps64 = item.Bitmaps64;
        for (int sid = 0; sid < item.Sequences64Count; sid++)
        {
          SetBitVectorForItem(_bitVectors64, itemOrder, sid, BitsHelper.LastSetBit(itemBitmaps64[sid]));
        }

        var itemBitmaps128 = item.Bitmaps128;
        for (int sid = 0; sid < item.Sequences128Count; sid++)
        {
          SetBitVectorForItem(_bitVectors128, itemOrder, sid, BitsHelper.LastSetBit(itemBitmaps128[sid]));
        }

        itemOrder++;
      }
    }

    /// <summary>
    /// Creates empty bit vectors for a sequence database.
    /// </summary>
    private void CreateBitVectors()
    {
      if (_sequenceDatabase.Count <= 0) return;
      
      var firstBitmap = _sequenceDatabase[0];

      _bitVectors8 = CreateBitVector(firstBitmap.Sequences8Count, 8, FrequentItemsCount);
      _bitVectors16 = CreateBitVector(firstBitmap.Sequences16Count, 16, FrequentItemsCount);
      _bitVectors32 = CreateBitVector(firstBitmap.Sequences32Count, 32, FrequentItemsCount);
      _bitVectors64 = CreateBitVector(firstBitmap.Sequences64Count, 64, FrequentItemsCount);
      _bitVectors128 = CreateBitVector(firstBitmap.Sequences128Count, 128, FrequentItemsCount);
    }

    private static byte[,][] CreateBitVector(int dim1, int dim2, int dim3)
    {
      if (dim1 > 0)
      {
        var array3D = new byte[dim1, dim2][];

        for (int sid = 0; sid < dim1; sid++)
        {
          for (int tid = 0; tid < dim2; tid++)
          {
            array3D[sid, tid] = new byte[dim3];
          }
        }
        return array3D;
      }

      return null;
    }

    public void FillTable()
    {
      CreateBitVectors();

      FillBitVectors();
    }

    /// <summary>
    /// For a given prefix returns the frequent candidates.
    /// </summary>
    public List<uint> GetFrequentItems(SeqBitmap prefixBitmap, IList<uint> candidates)
    {
      int transactions = 0;

      var transactionSBitVectors = new byte[
        prefixBitmap.Sequences8Count + prefixBitmap.Sequences16Count +
        prefixBitmap.Sequences32Count + prefixBitmap.Sequences64Count + prefixBitmap.Sequences128Count][];

      if (prefixBitmap.Sequences8Count > 0)
      {
        var prefixBitmaps8 = prefixBitmap.Bitmaps8;
        for (int sid = 0; sid < prefixBitmap.Sequences8Count; sid++)
        {
          if (prefixBitmaps8[sid] == 0) continue;
          transactionSBitVectors[transactions++] = _bitVectors8[sid, DeBruijn.FirstSetBit(prefixBitmaps8[sid])];
        }
      }

      if (prefixBitmap.Sequences16Count > 0)
      {
        var prefixBitmaps16 = prefixBitmap.Bitmaps16;
        for (int sid = 0; sid < prefixBitmap.Sequences16Count; sid++)
        {
          if (prefixBitmaps16[sid] == 0) continue;
          transactionSBitVectors[transactions++] = _bitVectors16[sid, DeBruijn.FirstSetBit(prefixBitmaps16[sid])];
        }
      }

      if (prefixBitmap.Sequences32Count > 0)
      {
        var prefixBitmaps32 = prefixBitmap.Bitmaps32;
        for (int sid = 0; sid < prefixBitmap.Sequences32Count; sid++)
        {
          if (prefixBitmaps32[sid] == 0) continue;
          transactionSBitVectors[transactions++] = _bitVectors32[sid, DeBruijn.FirstSetBit(prefixBitmaps32[sid])];
        }
      }

      if (prefixBitmap.Sequences64Count > 0)
      {
        var prefixBitmaps64 = prefixBitmap.Bitmaps64;
        for (int sid = 0; sid < prefixBitmap.Sequences64Count; sid++)
        {
          if (prefixBitmaps64[sid] == 0) continue;
          transactionSBitVectors[transactions++] = _bitVectors64[sid, DeBruijn.FirstSetBit(prefixBitmaps64[sid])];
        }
      }

      if (prefixBitmap.Sequences128Count > 0)
      {
        var prefixBitmaps128 = prefixBitmap.Bitmaps128;
        for (int sid = 0; sid < prefixBitmap.Sequences128Count; sid++)
        {
          if (prefixBitmaps128[sid] == 0) continue;
          transactionSBitVectors[transactions++] = _bitVectors128[sid, DeBruijn.FirstSetBit(prefixBitmaps128[sid])];
        }
      }


      var frequetItems = new List<uint>();
      for (int i = 0, j = candidates.Count; i < j; i++)
      {
        var candidate = candidates[i];
        int candidateSupport = 0;
        for (int tid = 0; tid < transactions; tid++)
        {
          candidateSupport += transactionSBitVectors[tid][candidate];

          if (candidateSupport < _minSupport) continue;

          frequetItems.Add(candidate);
          break;
        }
      }

      return frequetItems;
    }


  }
}
