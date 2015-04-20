namespace ZTDIP.Algorithms.Sequences.Shared
{
  public interface IItemsManager
  {

    /// <summary>
    /// Returns text value of a given item id.
    /// </summary>
    /// <param name="itemId">An id of an item.</param>
    string GetTextForItem(uint itemId);

  }
}