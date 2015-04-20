using System.Collections.Generic;
using System.Linq;
using ZTDIP.Algorithms.Sequences.Logs;
using ZTDIP.Algorithms.Sequences.Shared;
using ZTDIP.Algorithms.Sequences.Spam.Bitmaps;

namespace ZTDIP.Algorithms.Sequences.Spam
{
  public class SpamAlgorithm : SequenceMiningAlgorithm
  {

    public static readonly int MAX_SEQUENCE_SIZE = 128;
    public static readonly string ALGORITHM_NAME = "SPAM";

    /// <summary>
    /// Input sequence database at vertical format.
    /// </summary>
    private readonly VerticalSequenceDatabase _sequenceDatabase;

    public SpamAlgorithm(VerticalSequenceDatabase sequenceDatabase, ILogger logger, IItemsManager itemsManager, IStop stopper)
      : base(logger, itemsManager, stopper)
    {
      _sequenceDatabase = sequenceDatabase;
    }

    public override string Name
    {
      get { return ALGORITHM_NAME; }
    }

    protected override  void Mine()
    {
      Spam();
    }



    /// <summary>
    /// SPAM algorithm first step of the DFS pruning process. 
    /// </summary>
    private void Spam()
    {
      _allSequentialPatterns.Clear();

      // Traverse lexicografic tree according to a frequent items. 
      var frequentItems = new List<uint>(_sequenceDatabase.Keys.OrderBy(item => item));

      for (int i = 0, j = frequentItems.Count; i < j; i++)
      {
        SeqBitmap seqBitmap = _sequenceDatabase[frequentItems[i]];

        var prefix = new Sequence(frequentItems[i], seqBitmap.Support); 

        Spam(prefix, seqBitmap, frequentItems, frequentItems.GetRange(i + 1, j - i - 1));
      }
    }

    /// <summary>
    /// SPAM algorithm depth first traversal of the lexicografic tree.
    /// </summary>
    private void Spam(Sequence prefix, SeqBitmap prefixBitmap, IList<uint> Sn, IList<uint> In)
    {
      if(Stopped) return;

      _allSequentialPatterns.Add(prefix);

      LogPattern(prefix);

      // S-STEP
      int snCandidatesCount = Sn.Count;
      var sTemp = new List<uint>(snCandidatesCount);
      
      if (snCandidatesCount > 0)
      {
        var sTempBitmaps = new List<SeqBitmap>(snCandidatesCount);

        for (int i = 0; i < snCandidatesCount; i++)
        {
          SeqBitmap newSeqBitmap = prefixBitmap.CreateNewBySStep(_sequenceDatabase[Sn[i]]);

          if (newSeqBitmap.Support < _minSupport) continue;
            
          sTemp.Add(Sn[i]);
          sTempBitmaps.Add(newSeqBitmap);
        }

        for (int i = 0, j = sTemp.Count; i < j; i++)
        {
          SeqBitmap newSeqBitmap = sTempBitmaps[i];
          Sequence newPrefix = Sequence.CreateNewBySStep(prefix, sTemp[i], newSeqBitmap.Support); 

          Spam(newPrefix, newSeqBitmap, sTemp, sTemp.GetRange(i + 1, j - i - 1));
        }
      }

      // I-STEP
      int inCandidatesCount = In.Count;

      if (inCandidatesCount <= 0) return;
      
      var iTemp = new List<uint>(inCandidatesCount);
      var iTempBitmaps = new List<SeqBitmap>(inCandidatesCount);

      for (int i = 0; i < inCandidatesCount; i++)
      {
        SeqBitmap newSeqBitmap = prefixBitmap.CreateNewByIStep(_sequenceDatabase[In[i]]);

        if (newSeqBitmap.Support < _minSupport) continue;
          
        iTemp.Add(In[i]);
        iTempBitmaps.Add(newSeqBitmap);
          
      }

      for (int i = 0, j = iTemp.Count; i < j; i++)
      {
        SeqBitmap newSeqBitmap = iTempBitmaps[i];
        Sequence newPrefix = Sequence.CreateNewByIStep(prefix, iTemp[i], newSeqBitmap.Support);

        Spam(newPrefix, newSeqBitmap, sTemp, iTemp.GetRange(i + 1, j - i - 1));
      }
    }

  }
}