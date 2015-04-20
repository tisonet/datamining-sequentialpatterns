using System;
using ZTDIP.Algorithms.Sequences.Bits;
using ZTDIP.Algorithms.Sequences.Shared;

namespace ZTDIP.Algorithms.Sequences.Spam.Bitmaps
{
  public class Bitmap64 : IBitmap
  {
    public const int BITS_PER_TRANSACTION = 64;

    public static readonly ulong[] TransformedBitmap = new ulong[BITS_PER_TRANSACTION]
    {
      0xfffffffffffffffe, 0xfffffffffffffffc, 0xfffffffffffffff8, 0xfffffffffffffff0, 0xffffffffffffffe0, 0xffffffffffffffc0, 0xffffffffffffff80, 0xffffffffffffff00, 
      0xfffffffffffffe00, 0xfffffffffffffc00, 0xfffffffffffff800, 0xfffffffffffff000, 0xffffffffffffe000, 0xffffffffffffc000, 0xffffffffffff8000, 0xffffffffffff0000, 
      0xfffffffffffe0000, 0xfffffffffffc0000, 0xfffffffffff80000, 0xfffffffffff00000, 0xffffffffffe00000, 0xffffffffffc00000, 0xffffffffff800000, 0xffffffffff000000, 
      0xfffffffffe000000, 0xfffffffffc000000, 0xfffffffff8000000, 0xfffffffff0000000, 0xffffffffe0000000, 0xffffffffc0000000, 0xffffffff80000000, 0xffffffff00000000, 
      0xfffffffe00000000, 0xfffffffc00000000, 0xfffffff800000000, 0xfffffff000000000, 0xffffffe000000000, 0xffffffc000000000, 0xffffff8000000000, 0xffffff0000000000, 
      0xfffffe0000000000, 0xfffffc0000000000, 0xfffff80000000000, 0xfffff00000000000, 0xffffe00000000000, 0xffffc00000000000, 0xffff800000000000, 0xffff000000000000, 
      0xfffe000000000000, 0xfffc000000000000, 0xfff8000000000000, 0xfff0000000000000, 0xffe0000000000000, 0xffc0000000000000, 0xff80000000000000, 0xff00000000000000, 
      0xfe00000000000000, 0xfc00000000000000, 0xf800000000000000, 0xf000000000000000, 0xe000000000000000, 0xc000000000000000, 0x8000000000000000, 0, 
    };

    /// <summary>
    /// Array for holding bits.
    /// </summary>
    private ulong[] _bitmaps;

    /// <summary>
    /// The count of sequence in the bitmap.
    /// </summary>
    private int _sequencesCount = 0;

    /// <summary>
    /// The support of the bitmap.
    /// </summary>
    private int _support = BitmapHelper.NULL_VALUE;



    public Bitmap64()
    {
      _bitmaps = new ulong[BitmapHelper.INITIAL_BITMAPS_SIZE];
    }

    public Bitmap64(ulong[] bitmaps, int support)
    {
      _sequencesCount = bitmaps.Length;
      _bitmaps = bitmaps;
      _support = support;
    }

    /// <summary>
    /// Gets the bits of the bitmap.
    /// </summary>
    internal ulong[] Bitmaps
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
        BitmapHelper.ResizeArray<ulong>(ref _bitmaps);
      }
    }

    public void SetTransaction(int transactionId)
    {
      _bitmaps[_sequencesCount - 1] |= (ulong) 1 << transactionId;
    }

    #endregion


    /// <summary>
    /// Creates a new bitmap by I-Step with a given item-bitmap. 
    /// </summary>
    public Bitmap64 CreateNewByIStep(Bitmap64 itemBitmap)
    {
      // A new bitmap for I-Step will be create by ANDing sequence-bitmap with item-bitmap.
      // During creating a new bitmap also counts the support for a new bitmap.
      var support = 0;
      var newBitmaps = new ulong[_sequencesCount];
      var itemBitmaps = itemBitmap._bitmaps;

      for (var sid = 0; sid < _sequencesCount; ++sid)
      {
        if ((newBitmaps[sid] = (_bitmaps[sid] & itemBitmaps[sid])) == 0) continue;
        
        ++support;
      }

     return support > 0 ? new Bitmap64(newBitmaps, support) : null;
    }

    /// <summary>
    /// Creates a new bitmap by S-Step with a given item-bitmap. 
    /// </summary>
    public Bitmap64 CreateNewBySStep(Bitmap64 itemBitmap)
    {
      // A new bitmap for S-Step will be create by ANDing transformed sequence-bitmap with item-bitmap.
      // During creating a new bitmap also counts the support for a new bitmap.
      var support = 0;
      var newBitmaps = new ulong[_sequencesCount];
      var itemBitmaps = itemBitmap._bitmaps;

      for (var sid = 0; sid < _sequencesCount; ++sid)
      {
        if (_bitmaps[sid] == 0) continue;

        var tid = DeBruijn.FirstSetBit(_bitmaps[sid]);

        if ((newBitmaps[sid] = (TransformedBitmap[tid] & itemBitmaps[sid])) == 0) continue;
        
        ++support;
      }

      return support > 0 ? new Bitmap64(newBitmaps, support) : null;
    }

  }
}