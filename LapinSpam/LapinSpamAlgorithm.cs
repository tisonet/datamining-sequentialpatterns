using System.Collections.Generic;
using System.Linq;
using ZTDIP.Algorithms.Sequences.Logs;
using ZTDIP.Algorithms.Sequences.Shared;
using ZTDIP.Algorithms.Sequences.Spam;
using ZTDIP.Algorithms.Sequences.Spam.Bitmaps;

namespace ZTDIP.Algorithms.Sequences.LapinSpam
{
  public class LapinSpamAlgorithm : SequenceMiningAlgorithm
  {
    public static readonly int MAX_SEQUENCE_SIZE = 128;
    public static readonly string ALGORITHM_NAME = "LAPIN-SPAM";


    private readonly VerticalSequenceDatabase _sequenceSequenceDatabase;

    /// <summary>
    /// Remapped vertical sequence database. 
    /// Index (order) in the list represent item id.
    /// </summary>
    private List<SeqBitmap> _remappedSequenceDatabase;

    /// <summary>
    /// ITEM_IS_EXIST_TABLE for effective counting support.
    /// </summary>
    private ItemIsExistTable _itemIsExistTable;

    /// <summary>
    /// Maps item index (order) to original item id.
    /// </summary>
    private uint[] _itemOrderToIdMapping;

    public LapinSpamAlgorithm(VerticalSequenceDatabase verticalSequenceDatabase, ILogger logger, IItemsManager itemsManager, IStop stopper)
      : base(logger, itemsManager, stopper)
    {
      _sequenceSequenceDatabase = verticalSequenceDatabase;
    }


    #region SequenceMiningAlgorithm members

    public override string Name
    {
      get { return ALGORITHM_NAME; }
    }

    protected override void Mine()
    {
      RemapItemIdToOrder();

      CreateAndFillInItemIsExistTable();

      LapinSpam();
    }
   
    #endregion

    /// <summary>
    /// LAPIN-SPAM algorithm the first step of the mining. 
    /// </summary>
    private void LapinSpam()
    {
      _allSequentialPatterns.Clear();

      var frequentItems = GetListOfFrequentItemOrder();

      for (int i = 0, j = _remappedSequenceDatabase.Count ; i < j; i++)
      {
        var frequentItemBitmap = _remappedSequenceDatabase[i];

        var sequenceFromFrequentItem = new Sequence(_itemOrderToIdMapping[i], frequentItemBitmap.Support);

        LapinSpam(sequenceFromFrequentItem, frequentItemBitmap, frequentItems, frequentItems.GetRange(i + 1, j - i - 1));
       }
    }

    /// <summary>
    /// LAPIN-SPAM algorithm depth first traversal of the lexicografic tree.
    /// </summary>
    private void LapinSpam(Sequence prefix, SeqBitmap prefixBitmap, IList<uint> Sn, IList<uint> In)
    {
      if (Stopped) return;  

      _allSequentialPatterns.Add(prefix);
      
      LogPattern(prefix);

      // S-STEP
      List<uint> sTemp;

      if (Sn.Count > 0)
      {
        sTemp = _itemIsExistTable.GetFrequentItems(prefixBitmap, Sn);

        for (int i = 0, j = sTemp.Count; i < j; i++)
        {
          SeqBitmap newBitmap = prefixBitmap.CreateNewBySStep(_remappedSequenceDatabase[(int)sTemp[i]]);

          Sequence newPrefix = Sequence.CreateNewBySStep(prefix, _itemOrderToIdMapping[sTemp[i]], newBitmap.Support);
          
          LapinSpam(newPrefix, newBitmap, sTemp, sTemp.GetRange(i + 1, j - i - 1));
        }
      }
      else
      {
        sTemp = new List<uint>();
      }

      // I-STEP
      int inCandidatesCount = In.Count;

      if (inCandidatesCount <= 0) return;
      
      var iTemp = new List<uint>(inCandidatesCount);
      var iTempBitmaps = new List<SeqBitmap>(inCandidatesCount);

      for (int i = 0; i < inCandidatesCount; i++)
      {
        SeqBitmap newSeqBitmap = prefixBitmap.CreateNewByIStep(_remappedSequenceDatabase[(int)In[i]]);
        if (newSeqBitmap.Support < _minSupport) continue;

        iTemp.Add(In[i]);
        iTempBitmaps.Add(newSeqBitmap);
      }

      for (int i = 0, j = iTemp.Count; i < j; i++)
      {
        SeqBitmap newSeqBitmap = iTempBitmaps[i];

        Sequence newPrefix = Sequence.CreateNewByIStep(prefix, _itemOrderToIdMapping[iTemp[i]], newSeqBitmap.Support);

        LapinSpam(newPrefix, newSeqBitmap, sTemp, iTemp.GetRange(i + 1, j - i - 1));
      }
    }

    /// <summary>
    /// Creates and fill in ITEM_IS_EXIST_TABLE for counting support of S-Step candidates. 
    /// </summary>
    private void CreateAndFillInItemIsExistTable()
    {
      _itemIsExistTable = new ItemIsExistTable(_remappedSequenceDatabase, _minSupport);
      _itemIsExistTable.FillTable();
    }

    /// <summary>
    /// Remaps item id to item order. 
    /// For the fastest access, counting support and reduce memory usage.
    /// </summary>
    private void RemapItemIdToOrder()
    {
      _remappedSequenceDatabase = new List<SeqBitmap>(_sequenceSequenceDatabase.Count);
      _itemOrderToIdMapping = new uint[_sequenceSequenceDatabase.Count];

      int itemOrder = 0;
      foreach (var itemInfo in _sequenceSequenceDatabase.OrderBy(item => item.Key))
      {
        _remappedSequenceDatabase.Add(itemInfo.Value);
        _itemOrderToIdMapping[itemOrder] = itemInfo.Key;

        itemOrder++;
      }
    }

    /// <summary>
    /// Returns the list of frequent items remapped indices (orders). 
    /// </summary>
    /// <returns></returns>
    private List<uint> GetListOfFrequentItemOrder()
    {
      var frequentItemsList = new List<uint>(_remappedSequenceDatabase.Count);

      for (uint i = 0; i < _remappedSequenceDatabase.Count; i++)
      {
        frequentItemsList.Add(i);
      }

      return frequentItemsList;
    }



  }
}