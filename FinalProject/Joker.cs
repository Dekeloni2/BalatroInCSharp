using BalatroGame;

public class Joker
{
    public string Name { get; }
    public string Description { get; }

    public Joker(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public virtual int GetBonusChips(List<Card> playedCards, int discardsUsed)
    {
        return 0;
    }
    
    public virtual void ApplyEffect(List<Card> playedCards)
    {
        
    }


    public virtual int GetBonusMult(List<Card> playedCards, int discardsUsed)
    {
        return 0;
    }
}
public class Photograph : Joker
{
    Photograph() : base("Photograph", "The first face card played this hand gives ×2 multiplier.")
    {
        throw new NotImplementedException();
    }

    public override void ApplyEffect(List<Card> playedCards)
    {
        if (playedCards == null || playedCards.Count == 0)
            return;

        // מוצא את ה-Face הראשון
        var face = playedCards.FirstOrDefault(c =>
            c.Rank == Rank.Jack ||
            c.Rank == Rank.Queen ||
            c.Rank == Rank.King);

        if (face == null)
            return;

        // מוסיף תגית ויזואלית
        face.VisualTags.Add("×2");

        // מכפיל את ה-MULTIPLIER של הקלף
        face.BaseMultiplier *= 2;
    }
}


public class GrosMichel : Joker
{
    private Random _rng = new Random();

    public GrosMichel() : base("Gros Michel", "+15 mult, 1 in 6 chance of breaking") {}

    public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
    {
        return 15;
    }

    public bool ShouldBreak()
    {
        return _rng.Next(6) == 0; // 1 מתוך 6
    }
}

public class Misprint : Joker
{
    private Random _rng = new Random();

    public Misprint() : base("Misprint", "Gain 0–20 Mult") {}

    public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
    {
        return _rng.Next(0, 21); // כולל 20
    }
}

public class Mask : Joker
{
    public Mask() : base("The Mask", "+10 mult if no J/Q/K in played hand") {}

    public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
    {
        bool hasFace = playedCards.Any(c =>
            c.Rank == Rank.Jack ||
            c.Rank == Rank.Queen ||
            c.Rank == Rank.King);

        if (hasFace)
            return 0; 

        return 10;
    }
}
public class JollyJoker : Joker
{
    public JollyJoker() : base("Jolly Joker", "+8 Mult when hand contains a pair") {}

    public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
    {
        var groups = playedCards.GroupBy(c => c.Rank);
        bool hasPair = groups.Any(g => g.Count() >= 2);

        return hasPair ? 8 : 0;
    }
}

public class AbstractJoker : Joker
{
    private Func<int> _getJokerCount;

    public AbstractJoker(Func<int> getJokerCount)
        : base("Abstract Joker", "+3 Mult for each Joker you own")
    {
        _getJokerCount = getJokerCount;
    }

    public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
    {
        return _getJokerCount() * 3;
    }
}
