using System;

namespace ZTDIP.Algorithms.Sequences.Spam.Bitmaps
{
  /// <summary>
  /// Helper class for work with bitmaps.
  /// </summary>
  internal static class BitmapHelper
  {

    public const int INITIAL_BITMAPS_SIZE = 200;
    public const int NULL_VALUE = -1;

    /// <summary>
    /// Resize a given bitmap array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static void ResizeArray<T>(ref T[] array)
    {
      var resizeArray = new T[array.Length * 2];
      Array.Copy(array, resizeArray, array.Length);
      array = resizeArray;
    }

  }
}
