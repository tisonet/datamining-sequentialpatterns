namespace ZTDIP.Algorithms.Sequences.Logs
{
  /// <summary>
  /// Defines members for logging.
  /// </summary>
  public interface ILogger
  {


  
    /// <summary>
    /// Writes a line to the log file.
    /// </summary>
    /// <param name="text">An input text.</param>
    void WriteLine(string text);

    /// <summary>
    /// Close the log file.
    /// </summary>
    void Close();




  }
}