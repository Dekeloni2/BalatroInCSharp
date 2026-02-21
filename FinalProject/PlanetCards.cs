public enum HandRank
{
    HighCard, Pair, TwoPair, ThreeOfKind, Straight, Flush, FullHouse, FourOfKind, StraightFlush
}
public class PlanetCard
{
    public HandRank TargetHand { get; }

    public PlanetCard(HandRank target)
    {
        TargetHand = target;
    }

    public void Use()
    {
        HandLevelSystem.LevelUp(TargetHand);
        Console.WriteLine($"{TargetHand} leveled up to {HandLevelSystem.GetLevel(TargetHand)}!");
    }
}
public static class HandLevelSystem
{
    private static Dictionary<HandRank, int> _levels = new Dictionary<HandRank, int>();

    static HandLevelSystem()
    {
        foreach (HandRank rank in Enum.GetValues(typeof(HandRank)))
            _levels[rank] = 0; // כל הידיים מתחילות ברמה 0
    }

    public static int GetLevel(HandRank rank) => _levels[rank];

    public static void LevelUp(HandRank rank)
    {
        _levels[rank]++;
    }
}
public static class HandLevelValues
{
    public static (int chips, int mult) GetBonusForLevel(HandRank rank, int level)
    {
        // דוגמה — אתה יכול לשנות את הערכים איך שבא לך
        return rank switch
        {
            HandRank.Pair          => (level * 10, level * 1),
            HandRank.TwoPair       => (level * 12, level * 1),
            HandRank.ThreeOfKind   => (level * 15, level * 1),
            HandRank.Straight      => (level * 20, level * 1),
            HandRank.Flush         => (level * 22, level * 1),
            HandRank.FullHouse     => (level * 25, level * 2),
            HandRank.FourOfKind    => (level * 30, level * 2),
            HandRank.StraightFlush => (level * 40, level * 3),
            _ => (0, 0)
        };
    }
}
