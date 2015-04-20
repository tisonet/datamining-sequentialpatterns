using System;
using ZTDIP.Algorithms.Sequences.Bits;
using ZTDIP.Algorithms.Sequences.Shared;

namespace ZTDIP.Algorithms.Sequences.Spam.Bitmaps
{
  internal class Bitmap32 : IBitmap
  {

    public const int BITS_PER_TRANSACTION = 32;

    private static readonly uint[] TransformedBitmap = new uint[BITS_PER_TRANSACTION]
    {
          0xfffffffe, 0xfffffffc, 0xfffffff8, 0xfffffff0, 0xffffffe0, 0xffffffc0, 0xffffff80, 0xffffff00,
          0xfffffe00, 0xfffffc00, 0xfffff800, 0xfffff000, 0xffffe000, 0xffffc000, 0xffff8000, 0xffff0000,
          0xfffe0000, 0xfffc0000, 0xfff80000, 0xfff00000, 0xffe00000, 0xffc00000, 0xff800000, 0xff000000,
          0xfe000000, 0xfc000000, 0xf8000000, 0xf0000000, 0xe0000000, 0xc0000000, 0x80000000, 0
    };


    /// <summary>
    /// Array for holding bits.
    /// </summary>
    private uint[] _bitmaps;

    /// <summary>
    /// The count of sequence in the bitmap.
    /// </summary>
    private int _sequencesCount = 0;

    /// <summary>
    /// The support of the bitmap.
    /// </summary>
    private int _support = BitmapHelper.NULL_VALUE;


    public Bitmap32()
    {
      _bitmaps = new uint[BitmapHelper.INITIAL_BITMAPS_SIZE];
    }

    public Bitmap32(uint[] bitmaps, int support)
    {
      _sequencesCount = bitmaps.Length;
      _bitmaps = bitmaps;
      _support = support;
    }

    /// <summary>
    /// Gets the bits of the bitmap.
    /// </summary>
    internal uint[] Bitmaps
    {
      get { return _bitmaps; }
    }

    /// <summary>
    /// Gets the count of sequences in the bitmap.
    /// </summary>
    internal int SequencesCount
    {
        get { return _sequencesCount; }
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
        BitmapHelper.ResizeArray<uint>(ref _bitmaps);
      }
    }

    public void SetTransaction(int transactionId)
    {
      _bitmaps[_sequencesCount - 1] |= (uint) (1 << transactionId);
    }

    #endregion


    /// <summary>
    /// Creates a new bitmap by I-Step with a given item-bitmap. 
    /// </summary>
    public Bitmap32 CreateNewByIStep(Bitmap32 itemBitmap)
    {
      // A new bitmap for I-Step will be create by ANDing sequence-bitmap with item-bitmap.
      // During creating a new bitmap also counts the support for a new bitmap.
      var support = 0;
      var newBitmaps = new uint[_sequencesCount];
      var itemBitmaps = itemBitmap._bitmaps;

      for (var sid = 0; sid < _sequencesCount; ++sid)
      {
        if ((newBitmaps[sid] = (_bitmaps[sid] & itemBitmaps[sid])) == 0) continue;
        
        ++support;
      }

     return support > 0 ? new Bitmap32(newBitmaps, support) : null;
    }

    /// <summary>
    /// Creates a new bitmap by S-Step with a given item-bitmap. 
    /// </summary>
    public Bitmap32 CreateNewBySStep(Bitmap32 itemBitmap)
    {
      // A new bitmap for S-Step will be create by ANDing transformed sequence-bitmap with item-bitmap.
      // During creating a new bitmap also counts the support for a new bitmap.
      var support = 0;
      var newBitmaps = new uint[_sequencesCount];
      var itemBitmaps = itemBitmap._bitmaps;

      for (var sid = 0; sid < _sequencesCount; ++sid)
      {
        if (_bitmaps[sid] == 0) continue;

        var tid = DeBruijn.FirstSetBit(_bitmaps[sid]);

        if ((newBitmaps[sid] = (TransformedBitmap[tid] & itemBitmaps[sid])) == 0) continue;
        
        ++support;
      }

      return support > 0 ? new Bitmap32(newBitmaps, support) : null;
    }

  }
}