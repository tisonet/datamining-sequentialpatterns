using System;
using System.IO;

namespace ZTDIP.Algorithms.Sequences.Logs
{
  /// <summary>
  /// Provides a set of methods for writing text into log files.
  /// </summary>
  public class Log : ILogger
  {



    /// <summary>
    /// For critical section.
    /// </summary>
    private static readonly object _locker = new object();

    /// <summary>
    /// Date-time of last created file.
    /// </summary>
    private static DateTime _lastCreatedDT = DateTime.MinValue;

    /// <summary>
    /// A counter for files created at the same time.
    /// </summary>
    private static int _fileCounter;

    private readonly string _algorithmId;

    /// <summary>
    /// A prefix of the log file name.
    /// </summary>
    private readonly string _fileNamePrefix = String.Empty;

    /// <summary>
    /// A file stream for writing.
    /// </summary>
    private FileStream _fileStream;

    /// <summary>
    /// A text writer.
    /// </summary>
    private TextWriter _textWriter;




    /// <summary>
    /// Initializes a new instance of the Log class.
    /// </summary>
    /// <param name="fileNamePrefix">Prefix of the log file name.</param>
    public Log(string fileNamePrefix, string algorithmId)
    {
      _fileNamePrefix = fileNamePrefix;
      _algorithmId = algorithmId;
    }




    /// <summary>
    /// Gets or private sets a name of current log file.
    /// </summary>
    public string CurrentFileName { get; private set; }




    #region ILogger Members

    /// <summary>
    /// Writes text into log file with ID of current algorithm and current date and time.
    /// Log file is automatically created in the Common documents folder.
    /// </summary>
    /// <param name="text">Text to write into log file.</param>
    public void WriteLine(string text)
    {
      WriteLine(_algorithmId, text);
    }

    #endregion

    /// <summary>
    /// Writes an empty line into log file with ID of current algorithm and current date and time.
    /// Log file is automatically created in the Common documents folder.
    /// </summary>
    public void WriteEmptyLine()
    {
      WriteLine(null);
    }


    /// <summary>
    /// Writes text into log file with given ID and current date and time.
    /// Log file is automatically created in the Common documents folder.
    /// </summary>
    /// <param name="algorithmId">An ID of the algorithm logs.</param>
    /// <param name="text">Text to write into log file.</param>
    public void WriteLine(string algorithmId, string text)
    {
      var datetime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                                  DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

      if (_fileStream == null)
      {
        string path =
          Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) +
          @"\DMPluginsLogs\";

        // Ensure that only one thread will access this critical section. This ensures that
        // two threads won't create file with the same name.
        lock (_locker)
        {
          if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

          if (datetime == _lastCreatedDT)
            _fileCounter++;
          else
            _fileCounter = 0;

          _lastCreatedDT = datetime;

          CurrentFileName = _fileNamePrefix + algorithmId + "_" +
                            datetime.ToString("yyyyMMdd_HHmmss");

          string fileName = Path.Combine(
            path,
            CurrentFileName + ".txt");

          _fileStream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
          _textWriter = new StreamWriter(_fileStream);
        } // -- end of lock (CS)       
      }

      if (text == null)
        _textWriter.WriteLine();
      else
        _textWriter.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] " + text);

      _textWriter.Flush();
    }


    /// <summary>
    /// Closes log file.
    /// </summary>
    public void Close()
    {
      if (_textWriter != null)
      {
        _textWriter.Close();
        _textWriter = null;
      }

      if (_fileStream != null)
      {
        _fileStream.Close();
        _fileStream = null;
      }
    }




  }
}