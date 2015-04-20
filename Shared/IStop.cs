using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZTDIP.Algorithms.Sequences.Shared
{
  /// <summary>
  /// Interface to allow early completion of mining.
  /// </summary>
  public interface IStop
  {
    /// <summary>
    /// When true, indicates to the parameter learning algorithm to complete early.
    /// </summary>
    bool Stop { get; }
    
  }
}
