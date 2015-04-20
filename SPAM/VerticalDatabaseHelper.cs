using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZTDIP.Algorithms.Sequences.Shared;
using ZTDIP.Algorithms.Sequences.Spam.Bitmaps;

namespace ZTDIP.Algorithms.Sequences.Spam
{
  public static class VerticalDatabaseHelper
  {
    public static IDictionary<uint, SeqBitmap> ConvertSequenceDatabaseToVerticalFormat(SequenceDatabase sequenceDatabase, IDictionary<uint, int> frequentItems)
    {
      var verticalDb = new Dictionary<uint, SeqBitmap>();
      SeqBitmap seqBitmap;

      foreach (Sequence seq in sequenceDatabase)
      {
        foreach (var item in frequentItems)
        {
          if (!verticalDb.TryGetValue((uint) item.Key, out seqBitmap))
          {
            seqBitmap = new SeqBitmap();
            seqBitmap.Support = item.Value;
            verticalDb.Add((uint) item.Key, seqBitmap);
          }

          seqBitmap.Addsequence(seq.Size);
        }

        for (int tid = 0; tid < seq.Size; tid++)
        {
          foreach (uint item in seq[tid])
          {
            verticalDb[item].SetTransaction(tid);
          }
        }
      }

      return verticalDb;
    }
  }
}
