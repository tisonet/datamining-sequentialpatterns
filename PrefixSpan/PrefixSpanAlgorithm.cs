using System.Collections.Generic;
using System.Linq;
using ZTDIP.Algorithms.Sequences.Logs;
using ZTDIP.Algorithms.Sequences.Shared;

namespace ZTDIP.Algorithms.Sequences.PrefixSpan
{
  /// <summary> 
  /// Implementation of the PrefixSpan (Prefix-Projected Pattern Growth) algorithm for mining sequential patterns.
  /// </summary>
  public class PrefixSpanAlgorithm : SequenceMiningAlgorithm
  {

    public static  readonly string ALGORITHM_NAME = "PrefixSpan";

    private readonly SequenceDatabase _sequenceDatabase;

    /// <summary>
    /// A dictionary <item, support> of found itemset-extension items, with their support.
    /// </summary>
    private readonly Dictionary<uint, int> _iExtensions = new Dictionary<uint, int>();

    /// <summary>
    /// A dictionary <item,support> of found sequence-extension items, with their support.
    /// </summary>
    private readonly Dictionary<uint, int> _sExtensions = new Dictionary<uint, int>();

    /// <summary>
    /// All 1-length sequential patterns.
    /// </summary>
    private IList<Sequence> _1LengthSequentialPatterns;

    public PrefixSpanAlgorithm(SequenceDatabase database, ILogger logger, IItemsManager itemsManager, IStop stopper)
      :base(logger, itemsManager, stopper)
    {
      _sequenceDatabase = database;
    }

    public override string Name
    {
      get { return ALGORITHM_NAME; }
    }

    private void PrefixSpan()
    {
      _allSequentialPatterns.Clear();

      ProjectedDatabase projectedDatabase = _sequenceDatabase.ConvertToProjectedDatabase();

      // Divides the search space according to frequent items and mine each recursively.
      for (int i = 0; i < _1LengthSequentialPatterns.Count; ++i)
      {
        Sequence seqPattern = _1LengthSequentialPatterns[i];

        _allSequentialPatterns.Add(seqPattern);

        LogPattern(seqPattern);

        ProjectedDatabase freqItemProjectedDb = ConstructProjectedDatabase(seqPattern, projectedDatabase);

        if (freqItemProjectedDb == null || !freqItemProjectedDb.IsNotEmpty) continue;
        
        PrefixSpan(freqItemProjectedDb);
      }
    }

    private void PrefixSpan(ProjectedDatabase projectedDatabase)
    {
      if (Stopped) return;

      _sExtensions.Clear();
      _iExtensions.Clear();

      // Scans projected database once and finds extension items.
      projectedDatabase.FindLocalFrequentItems(_minSupport, _sExtensions, _iExtensions);

      var newSequentialPatterns = new List<Sequence>();

      // Sequence-extension items.
      foreach (var pair in _sExtensions.OrderBy(e => e.Key))
      {
        newSequentialPatterns.Add(
          Sequence.CreateNewBySStep(projectedDatabase.Prefix, pair.Key, pair.Value));
      }

      // Itemset-extension items.
      foreach (var pair in _iExtensions.OrderBy(e => e.Key))
      {
        newSequentialPatterns.Add(
          Sequence.CreateNewByIStep(projectedDatabase.Prefix, pair.Key, pair.Value));
      }

      // Divides the search space and recursively mine the subset of all sequential patterns.
      for (int i = 0; i < newSequentialPatterns.Count; ++i)
      {
        Sequence seqPattern = newSequentialPatterns[i];

        _allSequentialPatterns.Add(seqPattern);
        LogPattern(seqPattern);

        ProjectedDatabase seqPatternProjectedDatabase = ConstructProjectedDatabase(seqPattern, projectedDatabase);

        if (seqPatternProjectedDatabase == null || !seqPatternProjectedDatabase.IsNotEmpty) continue;
        
        PrefixSpan(seqPatternProjectedDatabase);
      }
    }

    private ProjectedDatabase ConstructProjectedDatabase(Sequence prefix, ProjectedDatabase sourceProjectedDatabase)
    {
      var prefixLastItem = prefix.LastItem;
      var projectedDatabase = new ProjectedDatabase(prefix);

      // The count of sequences in a database which dont have to contains prefix
      // and projected database can still contains some sequential pattern.
      int minSequencesInProjectedDb = sourceProjectedDatabase.Size - (int) _minSupport;

      for (int sid = 0, lastSid = sourceProjectedDatabase.Size; sid < lastSid; sid++)
      {
        // If sequence contains the last item of a prefix, project sequence and add it to db.       

        int itemsetIndex;

        if ((itemsetIndex = sourceProjectedDatabase[sid].Contains(prefixLastItem)) >= 0)
        {
          var pseudoSequence = PseudoSequence.MakeProjection(sourceProjectedDatabase[sid], prefix, itemsetIndex);

          if (pseudoSequence != null && pseudoSequence.IsNotEmpty)
          {
            projectedDatabase.Add(pseudoSequence);
            continue;
          }
        }

        // If projected database can not contains new sequential pattern stop projection sooner.
        if (--minSequencesInProjectedDb < 0) return null;
      }

      return projectedDatabase;
    }

    private IList<Sequence> Get1LengthSequentialPatterns()
    {
      // If client provides patterns used it, otherwise find it.
      if (_miningSettings.OneLengthSequentialPatterns != null && _miningSettings.OneLengthSequentialPatterns.Count > 0)
      {
        return _miningSettings.OneLengthSequentialPatterns;
      }
      else
      {
        return _sequenceDatabase.FindOneLengthSequentialPattern(_minSupport);
      }
    }

    protected override void Mine()
    {
      _1LengthSequentialPatterns = Get1LengthSequentialPatterns();

      PrefixSpan();
    }

  }
}