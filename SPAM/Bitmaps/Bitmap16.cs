using ZTDIP.Algorithms.Sequences.Bits;
using ZTDIP.Algorithms.Sequences.Shared;
using ZTDIP.Algorithms.Sequences.Spam.Bitmaps;

namespace ZTDIP.Algorithms.Sequences.Spam.Bitmaps
{
  internal class Bitmap16 : IBitmap
  {

    public const int BITS_PER_TRANSACTION = 16;

    private static readonly ushort[] TransformedBitmap = new ushort[BITS_PER_TRANSACTION]
    {
        0xfffe, 0xfffc, 0xfff8, 0xfff0, 0xffe0, 0xffc0, 0xff80, 0xff00,
        0xfe00, 0xfc00, 0xf800, 0xf000, 0xe000, 0xc000, 0x8000, 0                            
    };

    /// <summary>
    /// Array for holding bits.
    /// </summary>
    private ushort[] _bitmaps;

    /// <summary>
    /// The count of sequence in the bitmap.
    /// </summary>
    private int _sequencesCount = 0;

    /// <summary>
    /// The support of the bitmap.
    /// </summary>
    private int _support = BitmapHelper.NULL_VALUE;


    public Bitmap16()
    {
      _bitmaps = new ushort[BitmapHelper.INITIAL_BITMAPS_SIZE];
    }

    public Bitmap16(ushort[] bitmaps, int support)
    {
      _sequencesCount = bitmaps.Length;
      _bitmaps = bitmaps;
      _support = support;
    }


    /// <summary>
    /// Gets the bits of the bitmap.
    /// </summary>
    internal ushort[] Bitmaps
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
        BitmapHelper.ResizeArray<ushort>(ref _bitmaps);
      }
    }

    public void SetTransaction(int transactionId)
    {
      _bitmaps[_sequencesCount - 1] |= (ushort) (1 << transactionId);
    }

    #endregion


    /// <summary>
    /// Creates a new bitmap by I-Step with a given item-bitmap. 
    /// </summary>
    public Bitmap16 CreateNewByIStep(Bitmap16 itemBitmap)
    {
      // A new bitmap for I-Step will be create by ANDing sequence-bitmap with item-bitmap.
      // During creating a new bitmap also counts the support for a new bitmap.
      var support = 0;
      var newBitmaps = new ushort[_sequencesCount];
      var itemBitmaps = itemBitmap._bitmaps;

      for (var sid = 0; sid < _sequencesCount; ++sid)
      {
        if ((newBitmaps[sid] = (ushort) (_bitmaps[sid] & itemBitmaps[sid])) == 0) continue;
        
        ++support;
      }

     return support > 0 ? new Bitmap16(newBitmaps, support) : null;
    }

    /// <summary>
    /// Creates a new bitmap by S-Step with a given item-bitmap. 
    /// </summary>
    public Bitmap16 CreateNewBySStep(Bitmap16 itemBitmap)
    {
      // A new bitmap for S-Step will be create by ANDing transformed sequence-bitmap with item-bitmap.
      // During creating a new bitmap also counts the support for a new bitmap.
      var support = 0;
      var newBitmaps = new ushort[_sequencesCount];
      var itemBitmaps = itemBitmap._bitmaps;

      for (var sid = 0; sid < _sequencesCount; ++sid)
      {
        if (_bitmaps[sid] == 0) continue;

        var tid = DeBruijn.FirstSetBit(_bitmaps[sid]);
       
        if ((newBitmaps[sid] = (ushort) (TransformedBitmap[tid] & itemBitmaps[sid])) == 0) continue;
        
        ++support;
      }

      return support > 0 ? new Bitmap16(newBitmaps, support) : null;
    }

  }
}