using System.Collections.Generic;
using System.Linq;
using ZTDIP.Algorithms.Sequences.Logs;

namespace ZTDIP.Algorithms.Sequences.Shared
{
  public abstract class SequenceMiningAlgorithm
  {

    protected double _minSupport;
    protected IList<Sequence> _allSequentialPatterns = new List<Sequence>();
    protected  MiningSettings _miningSettings;
    protected readonly IItemsManager _itemsManager;
    protected readonly ILogger _logger;
    protected readonly IStop _stopper;

    /// <summary>
    /// Return true if the mining was stopped.
    /// </summary>
    protected bool Stopped
    {
      get { return _stopper != null && _stopper.Stop; }
    }

    public abstract string Name { get; }

    public IList<Sequence> FoundPatterns 
    { 
      get { return _allSequentialPatterns; } 
    }

    protected SequenceMiningAlgorithm(ILogger logger, IItemsManager itemsManager, IStop stopper)
    {
      _logger = logger;
      _itemsManager = itemsManager;
      _stopper = stopper;
    }

    public void Mine(MiningSettings settings)
    {
      _miningSettings = settings;
      _minSupport = settings.MinSupport;

      Mine();
    }

    protected abstract void Mine();

    protected void Log(string text)
    {
      if (_logger != null)
      {
        _logger.WriteLine(text);
      }
    }

    protected void LogPattern(Sequence sequentialPattern)
    {
      if (!_miningSettings.LogPatterns) return;
      
      Log(sequentialPattern.ToString(_itemsManager));
    }

  }
}
