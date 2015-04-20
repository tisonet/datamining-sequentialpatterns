using System.Collections.Generic;
using System.Linq;
using ZTDIP.Algorithms.Sequences.Spam.Bitmaps;
using SPAM = ZTDIP.Algorithms.Sequences.Spam.Bitmaps;

namespace ZTDIP.Algorithms.Sequences.LapinSpam
{
  public class Bitmap
  {
    private readonly int _itemsCount;

    private IBitmap _actualBitmap;
    private Bitsets.Bitmap32 _bitmap32;
    private int? _support;

    public Bitmap(int itemsCount)
    {
      _itemsCount = itemsCount;
      _bitmap32 = new Bitsets.Bitmap32(itemsCount);
    }

    public int Support
    {
      get
      {
        if (!_support.HasValue)
        {
          CountSupport();
        }

        return _support.Value;
      }
      set { _support = value; }
    }

    public void Add(int sequenceSize)
    {
      _actualBitmap = _bitmap32;

      _actualBitmap.AddSequence();
    }

    public void SetTrue(int tid)
    {
      _actualBitmap.SetTransaction(tid);
    }

    internal Bitmap CreateNewBySStep(Bitmap itemBitmap)
    {
      var newBitmap = new Bitmap(_itemsCount);

      newBitmap._bitmap32 = _bitmap32.CreateNewBySStep(itemBitmap._bitmap32);

      return newBitmap;
    }

    internal Bitmap CreateNewByIStep(Bitmap itemBitmap)
    {
      var newBitmap = new Bitmap(_itemsCount);

      newBitmap._bitmap32 = _bitmap32.CreateNewByIStep(itemBitmap._bitmap32);

      return newBitmap;
    }

    internal List<uint> FindFrequentSExtensions(uint[] sequenceExtensionsCandidates, double minSupport)
    {
      var candidatesSupport = new uint[sequenceExtensionsCandidates.Max(can => can) + 1];

      _bitmap32.CountCandidatesSupport(candidatesSupport, sequenceExtensionsCandidates);

      var frequentExtensions = new List<uint>();

      for (int i = 0; i < sequenceExtensionsCandidates.Length; i++)
      {
        if (candidatesSupport[sequenceExtensionsCandidates[i]] >= minSupport)
        {
          frequentExtensions.Add(sequenceExtensionsCandidates[i]);
        }
      }

      return frequentExtensions;
    }

    public void AddItemIsExistTableRow(Dictionary<uint, int> lastPositions)
    {
      Bitsets.Bitmap32.AddItemIsExistTableRow(lastPositions, _itemsCount);
    }

    private void CountSupport()
    {
      _support = 0;
      _support += _bitmap32.CountSupport();
    }
  }
}