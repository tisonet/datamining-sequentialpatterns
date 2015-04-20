using ZTDIP.Algorithms.Sequences.Spam.Bitmaps;

namespace ZTDIP.Algorithms.Sequences.Bits
{
  public class BitsHelper
  {
    public static int FirstSetBit(uint value)
    {
      int pos = 0;
      while ( (value & 1) != 1)
      {
        value >>= 1;
        ++pos;
      }
      return pos;
    }


    public static int FirstSetBit(ulong value)
    {
      int pos = 0;
      while ((value & 1) != 1)
      {
        value >>= 1;
        ++pos;
      }
      return pos;
    }

    /// <summary>
    /// Returns the MSB most significant bit. 
    /// </summary>
    public static int LastSetBit(uint value)
    {
      int r = 0;

      while ((value >>= 1) > 0)
        r++;

      return r;
    }

    /// <summary>
    /// Returns the MSB most significant bit. 
    /// </summary>
    public static int LastSetBit(byte value)
    {
      int r = 0;

      while ((value >>= 1) > 0)
        r++;

      return r;
    }

    /// <summary>
    /// Returns the MSB most significant bit. 
    /// </summary>
    public static int LastSetBit(ulong value)
    {
      int r = 0;

      while ((value >>= 1) > 0)
        r++;

      return r;
    }

    /// <summary>
    /// Returns the MSB most significant bit. 
    /// </summary>
    public static int LastSetBit(UInt128 value)
    {
      if (value == 0) return 0;

      if(value._second != 0)
      {
        return LastSetBit(value._second) + UInt128.BITS_PER_ONE_TRANSACTION_PART;
      }
      return LastSetBit(value._first);
    }


    /// <summary>
    /// Sets to FALSE bit on a given index.
    /// </summary>
    public static uint SetFalse(uint value, int index)
    {
      return value & (uint) ~(1 << index );
    }

    /// <summary>
    /// Sets to TRUE bit on a given index.
    /// </summary>
    public static uint SetTrue(uint value, int index)
    {      
      return value | (uint) (1 << index);
    }
   
  }
}
