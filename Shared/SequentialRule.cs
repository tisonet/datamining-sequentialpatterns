namespace ZTDIP.Algorithms.Sequences.Shared
{
  /// <summary>
  /// Represent a sequential rule.
  /// </summary>
  public class SequentialRule
  {
    public Sequence LeftSide { get; private set; }
    public Sequence RightSide { get; private set; }

    public double Confidence { get; private set; }

    public SequentialRule()
    {
    }

    public SequentialRule(Sequence leftSide, Sequence rightSide, double confidence)
    {
      LeftSide = leftSide;
      RightSide = rightSide;
      Confidence = confidence;
    }

    public string ToString(IItemsManager itemsManager)
    {
      return string.Format("{0} => {1}", LeftSide.ToString(itemsManager), RightSide.ToString(itemsManager));
    }
  }
}
