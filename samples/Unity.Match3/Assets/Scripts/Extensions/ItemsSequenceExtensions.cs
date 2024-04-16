using System.Collections.Generic;

public static class ItemsSequenceExtensions
{
    public static IEnumerable<UnityGridSlot> GetUniqueSolvedGridSlots(this SolvedData solvedData, bool onlyMovable = false)
    {
        var solvedGridSlots = new HashSet<UnityGridSlot>();

        foreach (var solvedGridSlot in solvedData.GetSolvedGridSlots(onlyMovable))
        {
            if (!solvedGridSlots.Add(solvedGridSlot)) continue;

            yield return solvedGridSlot;
        }

        solvedGridSlots.Clear();
    }
}