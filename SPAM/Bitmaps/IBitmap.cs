namespace ZTDIP.Algorithms.Sequences.Spam.Bitmaps
{
  /// <summary>
  /// Defines members fo bitmap.
  /// </summary>
  internal interface IBitmap
  {
    /// <summary>
    /// Add a new sequence to bitmap.
    /// </summary>
    void AddSequence();

    /// <summary>
    /// Set a transaction with a given id to TRUE in actual sequence.
    /// </summary>
    /// <param name="transactionId">A transaction id.</param>
    void SetTransaction(int transactionId);
  }
}