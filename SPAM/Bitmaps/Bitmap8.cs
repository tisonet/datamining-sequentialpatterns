using System;
using ZTDIP.Algorithms.Sequences.Bits;
using ZTDIP.Algorithms.Sequences.Shared;

namespace ZTDIP.Algorithms.Sequences.Spam.Bitmaps
{
  internal class Bitmap8 : IBitmap
  {
    private const int BITS_PER_TRANSACTION = 8;

    private static readonly byte[] TransformedBitmap = new byte[BITS_PER_TRANSACTION]
    {
      0xfe, 0xfc, 0xf8, 0xf0, 0xe0, 0xc0, 0x80, 0             
    };


    /// <summary>
    /// Array for holding bits.
    /// </summary>
    private byte[] _bitmaps;

    /// <summary>
    /// The count of sequence in the bitmap.
    /// </summary>
    private int _sequencesCount = 0;

    /// <summary>
    /// The support of the bitmap.
    /// </summary>
    private int _support = BitmapHelper.NULL_VALUE;

    public Bitmap8()
    {
      _bitmaps = new byte[BitmapHelper.INITIAL_BITMAPS_SIZE];
    }

    public Bitmap8(byte[] bitmaps, int support)
    {
      _sequencesCount = bitmaps.Length;
      _bitmaps = bitmaps;
      _support = support;
    }

    /// <summary>
    /// Gets the bits of the bitmap.
    /// </summary>
    internal byte[] Bitmaps
    {
      get
      {
        return _bitmaps;
      }
    }

    /// <summary>
    /// Gets the count of sequences in the bitmap.
    /// </summary>
    internal int SequencesCount
    {
        get
        {
            return _sequencesCount;
        }
    }

    /// <summary>
    /// Gets the support of the bitmap.
    /// </summary>
    public int Support
    {
      get
      {
        if (_support != BitmapHelper.NULL_VALUE)
        {
          return _support;
        }
        else
        {
          _support = 0;
          for (int i = 0; i < _sequencesCount; i++)
          {
            if (_bitmaps[i] == 0) continue;
            _support++;
          }
          return _support;
        }
      }
    }




    #region IBitmap members

    public void AddSequence()
    {
      _sequencesCount++;

      if (_sequencesCount >= _bitmaps.Length)
      {
        BitmapHelper.ResizeArray<byte>(ref _bitmaps);
      }
    }

    public void SetTransaction(int transactionId)
    {
      _bitmaps[_sequencesCount - 1] |= (byte) (1 << transactionId);
    }

    #endregion


    /// <summary>
    /// Creates a new bitmap by I-Step with a given item-bitmap. 
    /// </summary>
    public Bitmap8 CreateNewByIStep(Bitmap8 itemBitmap)
    {
      // A new bitmap for I-Step will be create by ANDing sequence-bitmap with item-bitmap.
      // During creating a new bitmap also counts the support for a new bitmap.
      var support = 0;
      var newBitmaps = new byte[_sequencesCount];
      var itemBitmaps = itemBitmap._bitmaps;

      for (var sid = 0; sid < _sequencesCount; ++sid)
      {
        if ((newBitmaps[sid] = (byte) (_bitmaps[sid] & itemBitmaps[sid])) == 0) continue;
        
        ++support;
      }

     return support > 0 ? new Bitmap8(newBitmaps, support) : null;
    }

    /// <summary>
    /// Creates a new bitmap by S-Step with a given item-bitmap. 
    /// </summary>
    public Bitmap8 CreateNewBySStep(Bitmap8 itemBitmap)
    {
      // A new bitmap for S-Step will be create by ANDing transformed sequence-bitmap with item-bitmap.
      // During creating a new bitmap also counts the support for a new bitmap.
      var support = 0;
      var newBitmaps = new byte[_sequencesCount];
      var itemBitmaps = itemBitmap._bitmaps;

      for (var sid = 0; sid < _sequencesCount; ++sid)
      {
        if (_bitmaps[sid] == 0) continue;

        var tid = DeBruijn.FirstSetBit(_bitmaps[sid]);

        if ((newBitmaps[sid] = (byte) (TransformedBitmap[tid] & itemBitmaps[sid])) == 0) continue;
        
        ++support;
      }

      return support > 0 ? new Bitmap8(newBitmaps, support) : null;
    }
  }
}