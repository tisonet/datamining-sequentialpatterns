using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZTDIP.Algorithms.Sequences.Spam.Bitmaps
{
  public struct UInt128
  {

    public const int BITS_PER_ONE_TRANSACTION_PART = 64;
    public const int BITS_PER_TRANSACTION = 128;

    internal ulong _first;
    internal ulong _second;


    internal UInt128(ulong first, ulong second)
    {
      _first = first;
      _second = second;
    }

    internal static UInt128 CreateNew(int firstTrue)
    {
      var newInstance = new UInt128();
      for (int i = firstTrue; i < BITS_PER_TRANSACTION; i++)
      {
        newInstance.SetBit(i);
      }
      return newInstance;
    }


    internal void SetBit(int transactionId)
    {
      if (transactionId < BITS_PER_ONE_TRANSACTION_PART)
      {
        _first |= (ulong) 1 << transactionId;
      }
      else
      {
        _second |= (ulong) 1 << (transactionId % BITS_PER_ONE_TRANSACTION_PART);
      }
    }


    public static bool operator ==(UInt128 a, ulong b)
    {
      return a._first == b && a._second == b;
    }

    public static bool operator !=(UInt128 a, ulong b)
    {
      return !(a == b);
    }

    public static UInt128 operator &(UInt128 a, UInt128 b)
    {
      return  new UInt128(a._first & b._first, a._second & b._second);
    }

    public bool Equals(UInt128 other)
    {
      return other._first == _first && other._second == _second;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (obj.GetType() != typeof (UInt128)) return false;
      return Equals((UInt128) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (_first.GetHashCode()*397) ^ _second.GetHashCode();
      }
    }

   
  }
}
