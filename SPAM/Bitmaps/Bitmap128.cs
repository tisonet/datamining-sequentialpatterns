using System;
using ZTDIP.Algorithms.Sequences.Bits;
using ZTDIP.Algorithms.Sequences.Shared;

namespace ZTDIP.Algorithms.Sequences.Spam.Bitmaps
{
  internal class Bitmap128 : IBitmap
  {

    public const int BITS_PER_TRANSACTION = 128;

    private static readonly UInt128[] TransformedBitmap; 
    
    //= new UInt128[]
    //{
    //  UInt128.CreateNew(1), UInt128.CreateNew(2), UInt128.CreateNew(3), UInt128.CreateNew(4), UInt128.CreateNew(5),UInt128.CreateNew(6),UInt128.CreateNew(7), UInt128.CreateNew(8), UInt128.CreateNew(9), 
    //  UInt128.CreateNew(10), UInt128.CreateNew(11), UInt128.CreateNew(12),UInt128.CreateNew(13),UInt128.CreateNew(14), UInt128.CreateNew(15), UInt128.CreateNew(16), UInt128.CreateNew(17), UInt128.CreateNew(18), UInt128.CreateNew(19), 
    //  UInt128.CreateNew(20),UInt128.CreateNew(21), UInt128.CreateNew(22), UInt128.CreateNew(23), UInt128.CreateNew(24), UInt128.CreateNew(25), UInt128.CreateNew(26),UInt128.CreateNew(27),UInt128.CreateNew(28), UInt128.CreateNew(29), 
    //  UInt128.CreateNew(30), UInt128.CreateNew(31), UInt128.CreateNew(32), UInt128.CreateNew(33),UInt128.CreateNew(34),UInt128.CreateNew(35), UInt128.CreateNew(36), UInt128.CreateNew(37), UInt128.CreateNew(38), UInt128.CreateNew(39), 
    //  UInt128.CreateNew(40), UInt128.CreateNew(41), UInt128.CreateNew(42), UInt128.CreateNew(43),UInt128.CreateNew(44),UInt128.CreateNew(45), UInt128.CreateNew(46), UInt128.CreateNew(47), UInt128.CreateNew(48), UInt128.CreateNew(49), 
    //  UInt128.CreateNew(50), UInt128.CreateNew(51), UInt128.CreateNew(52), UInt128.CreateNew(53),UInt128.CreateNew(54),UInt128.CreateNew(55), UInt128.CreateNew(56), UInt128.CreateNew(57), UInt128.CreateNew(58), UInt128.CreateNew(59), 
    //  UInt128.CreateNew(60), UInt128.CreateNew(61), UInt128.CreateNew(62), UInt128.CreateNew(63),UInt128.CreateNew(64),UInt128.CreateNew(65), UInt128.CreateNew(66), UInt128.CreateNew(67), UInt128.CreateNew(68), UInt128.CreateNew(69), 
    //  UInt128.CreateNew(70), UInt128.CreateNew(71), UInt128.CreateNew(72), UInt128.CreateNew(73),UInt128.CreateNew(74),UInt128.CreateNew(75), UInt128.CreateNew(76), UInt128.CreateNew(77), UInt128.CreateNew(78), UInt128.CreateNew(79),
    //  UInt128.CreateNew(80), UInt128.CreateNew(81), UInt128.CreateNew(82), UInt128.CreateNew(83),UInt128.CreateNew(84),UInt128.CreateNew(85), UInt128.CreateNew(86), UInt128.CreateNew(87), UInt128.CreateNew(88), UInt128.CreateNew(89),
    //  UInt128.CreateNew(90), UInt128.CreateNew(91), UInt128.CreateNew(92), UInt128.CreateNew(93),UInt128.CreateNew(94),UInt128.CreateNew(95), UInt128.CreateNew(96), UInt128.CreateNew(97), UInt128.CreateNew(98), UInt128.CreateNew(99),
    //  UInt128.CreateNew(100), UInt128.CreateNew(101), UInt128.CreateNew(102), UInt128.CreateNew(103),UInt128.CreateNew(104),UInt128.CreateNew(105), UInt128.CreateNew(106), UInt128.CreateNew(107), UInt128.CreateNew(108), UInt128.CreateNew(109),
    //  UInt128.CreateNew(110), UInt128.CreateNew(111), UInt128.CreateNew(112), UInt128.CreateNew(113),UInt128.CreateNew(114),UInt128.CreateNew(115), UInt128.CreateNew(116), UInt128.CreateNew(117), UInt128.CreateNew(118), UInt128.CreateNew(119),
    //  UInt128.CreateNew(120), UInt128.CreateNew(121), UInt128.CreateNew(122), UInt128.CreateNew(123),UInt128.CreateNew(124),UInt128.CreateNew(125), UInt128.CreateNew(126), UInt128.CreateNew(127), UInt128.CreateNew(128)

    //};



    /// <summary>
    /// Array for holding bits.
    /// </summary>
    private UInt128[] _bitmaps;

    /// <summary>
    /// The support of the bitmap.
    /// </summary>
    private int _support = BitmapHelper.NULL_VALUE;

    static Bitmap128()
    {
      TransformedBitmap = new UInt128[BITS_PER_TRANSACTION];

      for (int i = 0; i < BITS_PER_TRANSACTION; i++)
      {
        TransformedBitmap[i] = UInt128.CreateNew(i + 1);
      }
    }

    public Bitmap128()
    {
      _bitmaps = new UInt128[BitmapHelper.INITIAL_BITMAPS_SIZE];
    }

    public Bitmap128(UInt128[] bitmaps, int support)
    {
      SequencesCount = bitmaps.Length;
      _bitmaps = bitmaps;
      _support = support;
    }

    /// <summary>
    /// Gets the bits of the bitmap.
    /// </summary>
    internal UInt128[] Bitmaps
    {
      get { return _bitmaps; }
    }

    /// <summary>
    /// Gets the count of sequences in the bitmap.
    /// </summary>
    internal int SequencesCount { get; private set; }

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
        
        _support = 0;
        for (int i = 0; i < SequencesCount; i++)
        {
          if (_bitmaps[i] == 0) continue;
          _support++;
        }
        return _support;
      }
    }

    #region IBitmap members

    public void AddSequence()
    {
      SequencesCount++;

      if (SequencesCount >= _bitmaps.Length)
      {
        BitmapHelper.ResizeArray<UInt128>(ref _bitmaps);
      }
    }

    public void SetTransaction(int transactionId)
    {
      _bitmaps[SequencesCount - 1].SetBit(transactionId);
    }

    #endregion

    /// <summary>
    /// Creates a new bitmap by I-Step with a given item-bitmap. 
    /// </summary>
    public Bitmap128 CreateNewByIStep(Bitmap128 itemBitmap)
    {
      // A new bitmap for I-Step will be create by ANDing sequence-bitmap with item-bitmap.
      // During creating a new bitmap also counts the support for a new bitmap.
      var support = 0;
      var newBitmaps = new UInt128[SequencesCount];
      var itemBitmaps = itemBitmap._bitmaps;

      for (var sid = 0; sid < SequencesCount; ++sid)
      {
        if ((newBitmaps[sid] = (_bitmaps[sid] & itemBitmaps[sid])) == 0) continue;
        
        ++support;
      }

     return support > 0 ? new Bitmap128(newBitmaps, support) : null;
    }

    /// <summary>
    /// Creates a new bitmap by S-Step with a given item-bitmap. 
    /// </summary>
    public Bitmap128 CreateNewBySStep(Bitmap128 itemBitmap)
    {
      // A new bitmap for S-Step will be create by ANDing transformed sequence-bitmap with item-bitmap.
      // During creating a new bitmap also counts the support for a new bitmap.
      var support = 0;
      var newBitmaps = new UInt128[SequencesCount];
      var itemBitmaps = itemBitmap._bitmaps;

      for (var sid = 0; sid < SequencesCount; ++sid)
      {
        if (_bitmaps[sid] == 0) continue;

        var tid = DeBruijn.FirstSetBit(_bitmaps[sid]);

        if ((newBitmaps[sid] = (TransformedBitmap[tid] & itemBitmaps[sid])) == 0) continue;
        
        ++support;
      }

      return support > 0 ? new Bitmap128(newBitmaps, support) : null;
    }

  }
}