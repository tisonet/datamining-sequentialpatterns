using System.Collections.Generic;
using System.Linq;
using ZTDIP.Algorithms.Sequences.Logs;
using ZTDIP.Algorithms.Sequences.PrefixSpan;
using ZTDIP.Algorithms.Sequences.Shared;

namespace ZTDIP.Algorithms.Sequences.Bide
{
  /// <summary> 
  /// Implementation of the BIDE (BI-Directional extension) algorithm for mining sequential patterns.
  /// </summary>
  public class BideAlgorithm : SequenceMiningAlgorithm
  {
    public static readonly string ALGORITHM_NAME = "BIDE";

    private readonly SequenceDatabase _sequenceDatabase;
    private readonly Dictionary<uint, int> _iExtensions = new Dictionary<uint, int>();
    private readonly Dictionary<uint, int> _sExtensions = new Dictionary<uint, int>();
    private IList<Sequence> _1LengthSequentialPatterns;

    public BideAlgorithm(SequenceDatabase database, ILogger logger, IItemsManager itemsManager, IStop stopper)
      : base(logger, itemsManager, stopper)
    {
      _sequenceDatabase = database;
    }

    public int NoClosedSequencesCount { get; private set; }
    public int PrunedSequencesCount { get; private set; }

    #region SequenceMiningAlgorithm members

    public override string Name
    {
      get { return ALGORITHM_NAME; }
    }

    protected override void Mine()
    {
      _1LengthSequentialPatterns = Get1LengthSequentialPatterns();

      Bide();
    }

    #endregion


    private void Bide()
    {
      _allSequentialPatterns.Clear();

      ProjectedDatabase projectedDatabase = _sequenceDatabase.ConvertToProjectedDatabase();

      // Divides the search space according to frequent items and mine each recursively.
      for (int i = 0; i < _1LengthSequentialPatterns.Count; ++i)
      {
        ProjectedDatabase freqItemProjectedDb = ConstructProjectedDatabase(_1LengthSequentialPatterns[i], projectedDatabase);

        if (freqItemProjectedDb == null || !freqItemProjectedDb.IsNotEmpty) continue;

        if (ClosureChecker.BackScan(freqItemProjectedDb))
        {
          PrunedSequencesCount++;
        }
        else
        {
          Bide(freqItemProjectedDb);
        }
      }
    }

    private void Bide(ProjectedDatabase projectedDatabase)
    {
      if(Stopped) return;

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

      // If projected database does not contain backward or forward extensions 
      // than a prefix of projection is closed sequential pattern.
      if(!HasProjectedDatabaseAnyExtensions(projectedDatabase, newSequentialPatterns))
      {
        _allSequentialPatterns.Add(projectedDatabase.Prefix);

        LogPattern(projectedDatabase.Prefix);
      }
      else
      {
        NoClosedSequencesCount++;
      }

      // Divides the search space and recursively mine the subset of all sequential patterns.
      for (int i = 0; i < newSequentialPatterns.Count; ++i)
      {
        ProjectedDatabase seqPatternProjectedDatabase = ConstructProjectedDatabase(newSequentialPatterns[i], projectedDatabase);

        if (seqPatternProjectedDatabase == null || !seqPatternProjectedDatabase.IsNotEmpty) continue;

        if (ClosureChecker.BackScan(seqPatternProjectedDatabase))
        {
          PrunedSequencesCount++;
        }
        else
        {
          Bide(seqPatternProjectedDatabase);
        }
      }
    }

    /// <summary>
    /// Checks if a given database contains some forward extensions items or backward extensions items.
    /// </summary>
    private static bool HasProjectedDatabaseAnyExtensions(ProjectedDatabase projectedDatabase, IEnumerable<Sequence> newSequentialPatterns)
    {
      // Checks if exist any forward extensions. If not tries find any backward extension
      return newSequentialPatterns.Any(sequence => sequence.Support == projectedDatabase.Prefix.Support) 
        || 
        ClosureChecker.BackwardExtensionCheck(projectedDatabase);
    }

    private ProjectedDatabase ConstructProjectedDatabase(Sequence prefix, ProjectedDatabase database)
    {
      var prefixLastItem = prefix.LastItem;
      var projectedDatabase = new ProjectedDatabase(prefix);

      // The count of sequences in a database which dont have to contains prefix
      // and projected database can still contains some sequential pattern.
      var minSequencesInProjectedDb = database.Size - (int) _minSupport;

      for (int sid = 0, lastSid = database.Size; sid < lastSid; sid++)
      {
        // If sequence contains the last item of a prefix, project sequence and add it to db.       

        int itemsetIndex;
        
        if ((itemsetIndex = database[sid].Contains(prefixLastItem)) >= 0)
        {
          var pseudoSequence = PseudoSequence.MakeProjection(database[sid], prefix, itemsetIndex, false);

          if (pseudoSequence != null)
          {
            projectedDatabase.Add(pseudoSequence);
            continue;
          }
        }

        // If projected database can not contains new sequential pattern stop projection sooner.
        if (--minSequencesInProjectedDb >= 0) continue;
        
        return null;
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

      return  _sequenceDatabase.FindOneLengthSequentialPattern(_minSupport);
    }

  }
}