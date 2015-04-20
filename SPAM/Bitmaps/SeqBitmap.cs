namespace ZTDIP.Algorithms.Sequences.Spam.Bitmaps
{
  /// <summary>
  /// A vertical bitmap for a sequence or an item in a sequence database.
  /// </summary>
  public class SeqBitmap
  {
    /// <summary>
    /// Support of the bitmap.
    /// </summary>
    private int _support = BitmapHelper.NULL_VALUE;

    /// <summary>
    /// Active bitmap partition.
    /// </summary>
    private IBitmap _actualBitmap;

    /// <summary>
    /// Bitmap partition for sequences with max. 8 transaction.
    /// </summary>
    private Bitmap8 _bitmap8 = new Bitmap8();

    /// <summary>
    /// Bitmap partition for sequences with max. 16 transaction.
    /// </summary>
    private Bitmap16 _bitmap16 = new Bitmap16();

    /// <summary>
    /// Bitmap partition for sequences with max. 32 transaction.
    /// </summary>
    private Bitmap32 _bitmap32 = new Bitmap32();

    /// <summary>
    /// Bitmap partition for sequences with max. 64 transaction.
    /// </summary>
    private Bitmap64 _bitmap64 = new Bitmap64();

    /// <summary>
    /// Bitmap partition for sequences with max. 128 transaction.
    /// </summary>
    private Bitmap128 _bitmap128 = new Bitmap128();

    /// <summary>
    /// Gets the support of the bitmap.
    /// </summary>
    public int Support
    {
      get
      {
        // If a support wasnt counted, first count it.
        if (_support == BitmapHelper.NULL_VALUE)
        {
          CountSupport();
        }
        return _support;
      }
      set
      {
        _support = value;
      }
    }

    internal byte[] Bitmaps8
    {
      get { return _bitmap8.Bitmaps; }
    }

    internal ushort[] Bitmaps16
    {
      get { return _bitmap16.Bitmaps; }
    }

    internal uint[] Bitmaps32
    {
      get { return _bitmap32.Bitmaps; }
    }

    internal ulong[] Bitmaps64
    {
      get { return _bitmap64.Bitmaps; }
    }

    internal UInt128[] Bitmaps128
    {
      get { return _bitmap128.Bitmaps; }
    }

    internal int Sequences8Count
    {
        get { return _bitmap8 == null ? 0 : _bitmap8.SequencesCount; }
    }

    internal int Sequences16Count
    {
        get { return _bitmap16 == null ? 0 : _bitmap16.SequencesCount; }
    }

    internal int Sequences32Count
    {
        get { return _bitmap32 == null ? 0 : _bitmap32.SequencesCount; }
    }

    internal int Sequences64Count
    {
        get { return _bitmap64 == null ? 0 : _bitmap64.SequencesCount; }
    }

    internal int Sequences128Count
    {
      get { return _bitmap128 == null ? 0 : _bitmap128.SequencesCount; }
    }

    /// <summary>
    /// Counts and returns the support of the bitmap. 
    /// </summary>
    private void CountSupport()
    {
      // Support is equals the sum of all partition. 
      
      _support = 0;

      if (_bitmap8 != null)
        _support += _bitmap8.Support;

      if (_bitmap16 != null)
        _support += _bitmap16.Support;

      if (_bitmap32 != null)
        _support += _bitmap32.Support;

      if (_bitmap64 != null)
        _support += _bitmap64.Support;

      if (_bitmap128 != null)
        _support += _bitmap128.Support;
    }
   
    /// <summary>
    /// Returns a bitmap partition for a given sequence size.
    /// </summary>
    private IBitmap GetRightSizeBitmap(int sequenceSize)
    {
      if(sequenceSize < 17)
      {
        return sequenceSize > 8 ? (IBitmap) _bitmap16 : _bitmap8;
      } 
      else if(sequenceSize < 65)
      {
        return sequenceSize > 32 ? (IBitmap) _bitmap64 : _bitmap32;
      }
      return _bitmap128;
    }

    /// <summary>
    /// Creates a new sequence bitmap by apply S-Step with a given item-bitmap. 
    /// </summary>
    internal SeqBitmap CreateNewBySStep(SeqBitmap itemBitmap)
    {
      var newBitmap = new SeqBitmap();

      if (_bitmap8 != null)
        newBitmap._bitmap8 = _bitmap8.CreateNewBySStep(itemBitmap._bitmap8);

      if (_bitmap16 != null)
        newBitmap._bitmap16 = _bitmap16.CreateNewBySStep(itemBitmap._bitmap16);

      if (_bitmap32 != null)
        newBitmap._bitmap32 = _bitmap32.CreateNewBySStep(itemBitmap._bitmap32);

      if (_bitmap64 != null)
        newBitmap._bitmap64 = _bitmap64.CreateNewBySStep(itemBitmap._bitmap64);

      if (_bitmap128 != null)
        newBitmap._bitmap128 = _bitmap128.CreateNewBySStep(itemBitmap._bitmap128);

      return newBitmap;
    }

    /// <summary>
    /// Creates a new sequence bitmap by apply I-Step with a given item-bitmap. 
    /// </summary>
    internal SeqBitmap CreateNewByIStep(SeqBitmap itemBitmap)
    {
      var newBitmap = new SeqBitmap();

      if (_bitmap8 != null)
        newBitmap._bitmap8 = _bitmap8.CreateNewByIStep(itemBitmap._bitmap8);

      if (_bitmap16 != null)
        newBitmap._bitmap16 = _bitmap16.CreateNewByIStep(itemBitmap._bitmap16);

      if (_bitmap32 != null)
        newBitmap._bitmap32 = _bitmap32.CreateNewByIStep(itemBitmap._bitmap32);

      if (_bitmap64 != null)
        newBitmap._bitmap64 = _bitmap64.CreateNewByIStep(itemBitmap._bitmap64);

      if (_bitmap128 != null)
        newBitmap._bitmap128 = _bitmap128.CreateNewByIStep(itemBitmap._bitmap128);

      return newBitmap;
    }

    /// <summary>
    /// Add a new sequence to bitmap with a given size.
    /// </summary>
    public void AddSequence(int sequenceSize)
    {
      _actualBitmap = GetRightSizeBitmap(sequenceSize);

      _actualBitmap.AddSequence();
    }

    /// <summary>
    /// Set a transaction with a given id to TRUE in actual sequence bitmap.
    /// </summary>
    /// <param name="transactionId">A transaction id.</param>
    public void SetTransaction(int transactionId)
    {
      _actualBitmap.SetTransaction(transactionId);
    }

  }
}