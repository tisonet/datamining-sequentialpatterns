using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZTDIP.Algorithms.Sequences.Shared.Extensions
{
  internal static  class UIntExtensions
  {
    public static  uint FingerPrint(this uint me)
    {
      return (1u << ((int)me%64));
    }
  }
}
