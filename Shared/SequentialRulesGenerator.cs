using System.Collections.Generic;
using System.Linq;

namespace ZTDIP.Algorithms.Sequences.Shared
{
  /// <summary>
  /// Class for generating sequential rules from frequent sequences.
  /// </summary>
  public class SequentialRulesGenerator
  {

    private readonly IDictionary<Sequence, int> _patternsSupport = new Dictionary<Sequence, int>();
    private readonly IEnumerable<Sequence> _sequentialPatterns;

    public SequentialRulesGenerator(IEnumerable<Sequence> sequentialPatterns)
    {
      _sequentialPatterns = sequentialPatterns;


      foreach (var pattern in sequentialPatterns)
      {
        _patternsSupport.Add(pattern, pattern.Support);
      }
    }

    /// <summary>
    /// Generates sequential rules from a given patterns.
    /// </summary>
    /// <param name="minConfidence">An user given minimal confidence of a rule.</param>
    public IList<SequentialRule> Generate(double minConfidence)
    {
      var rules = new List<SequentialRule>();

      foreach (var pattern in _sequentialPatterns.Where(pat => pat.Size > 1))
      {
           foreach (var subSequence in GetSubSequences(pattern))
           {
             var conf = pattern.Support/(double) subSequence.Support;

             if (conf < minConfidence) continue;

             rules.Add(new SequentialRule(subSequence, pattern, conf));
           }
      }
      return rules;
    }

    private IEnumerable<Sequence> GetSubSequences(Sequence sequence)
    {
      var leftItemsets = new List<List<uint>>();
      for (int i = 0; i < sequence.Size - 1; i++)
      {
        leftItemsets.Clear();
        for (int startLeft = 0; startLeft <= i; startLeft++)
        {
          leftItemsets.Add(new List<uint>(sequence[startLeft]));
        }

        var subSequence = new Sequence(leftItemsets);
        
        if(!_patternsSupport.ContainsKey(subSequence)) continue;
        
        subSequence.Support = GetSupport(subSequence);
        
        yield return subSequence;
      }
  }

    private int GetSupport(Sequence pattern)
    {
     return _patternsSupport[pattern];
    }

  }
}
