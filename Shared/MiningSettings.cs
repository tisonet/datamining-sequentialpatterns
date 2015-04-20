using System.Collections.Generic;

namespace ZTDIP.Algorithms.Sequences.Shared
{
  public class MiningSettings
  {
    /// <summary>
    /// Min. support threshold, absolute value.
    /// </summary>
    public double MinSupport { get; set; }

    /// <summary>
    /// If algorithm should log found pattern during mining.
    /// </summary>
    public bool LogPatterns { get; set; }

    /// <summary>
    /// 1-length sequential patterns for initially dividing the search space.
    /// </summary>
    public IList<Sequence> OneLengthSequentialPatterns { get; set; }
  }
}