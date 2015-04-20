using System;
using System.Timers;

namespace ZTDIP.Algorithms.Sequences.Utility
{
  public class MemoryCrawler
  {
    private static long _maxMemoryInBytes = 0;
    private static long _memoryBeforeRunInBytes = 0;
    private static readonly object _mutex = new object();
    private readonly Timer _memoryCrawler = new Timer(100);

    public MemoryCrawler()
    {
      _memoryCrawler.AutoReset = true;
      _memoryCrawler.Elapsed += MemoryCrawlerOnElapsed;
    }

    private static void MemoryCrawlerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
    {
      lock (_mutex)
      {
        long actualMemory = GC.GetTotalMemory(false);
        _maxMemoryInBytes = actualMemory > _maxMemoryInBytes ? actualMemory : _maxMemoryInBytes;
      }
    }

    public void Start()
    {
      _maxMemoryInBytes = 0;
      _memoryBeforeRunInBytes = GC.GetTotalMemory(true);

      _memoryCrawler.Start();
    }

    public void Stop()
    {
      _memoryCrawler.Stop();
    }

    public long GetMaxMemoryInMBytes()
    {
      return (_maxMemoryInBytes - _memoryBeforeRunInBytes) / 1048576;
    }

  }
}
