using System;
using System.Collections.Generic;
using ZTDIP.Algorithms.Sequences.Spam.Bitmaps;

namespace ZTDIP.Algorithms.Sequences.Bits
{
  /// <summary>
  /// FFS (first sets) bit positions with De Bruijn sequences.
  /// </summary>
  public static class DeBruijn
  {

    private static readonly int[] DE_BRUIJN_BIT_POSITION_32 =
      {
        0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
        31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
      };

    private static readonly int[] DE_BRUIJN_BIT_POSITION_64 =
      {
        63, 0, 58, 1, 59, 47, 53, 2,
        60, 39, 48, 27, 54, 33, 42, 3,
        61, 51, 37, 40, 49, 18, 28, 20,
        55, 30, 34, 11, 43, 14, 22, 4,
        62, 57, 46, 52, 38, 26, 32, 41,
        50, 36, 17, 19, 29, 10, 13, 21,
        56, 45, 25, 31, 35, 16, 9, 12,
        44, 24, 15, 8, 23, 7, 6, 5
      };

    /// <summary>
    /// Returns the first set bit (FFS), or 0 if no bits are set.
    /// </summary>
    public static int FirstSetBit(UInt32 value)
    {
      return DE_BRUIJN_BIT_POSITION_32[unchecked((uint) (value & -value)*0x077CB531U) >> 27];
    }

    /// <summary>
    /// Returns the first set bit (FFS), or 0 if no bits are set.
    /// </summary>
    public static int FirstSetBit(UInt16 value)
    {
      return DE_BRUIJN_BIT_POSITION_32[unchecked((UInt16) (value & -value)*0x077CB531U) >> 27];
    }

    /// <summary>
    /// Returns the first set bit (FFS), or 0 if no bits are set.
    /// </summary>
    public static int FirstSetBit(Byte value)
    {
      return DE_BRUIJN_BIT_POSITION_32[unchecked((Byte) (value & -value)*0x077CB531U) >> 27];
    }

    /// <summary>
    /// Returns the first set bit (FFS), or 0 if no bits are set.
    /// </summary>
    public static int FirstSetBit(ulong value)
    {
      var signedValue = (long) value;
      return DE_BRUIJN_BIT_POSITION_64[unchecked((ulong) (signedValue & -signedValue)*0x07EDD5E59A4E28C2) >> 58];
    }

    /// <summary>
    /// Returns the first set bit (FFS), or 0 if no bits are set.
    /// </summary>
    public static int FirstSetBit(UInt128 value)
    {
      if (value._first != 0)
      {
        return FirstSetBit(value._first);
      }
      else
      {
        return FirstSetBit(value._second) + UInt128.BITS_PER_ONE_TRANSACTION_PART;
      }
    }

  }
}