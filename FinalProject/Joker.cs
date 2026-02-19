using BalatroGame;
//Each joker is a separate class. This comes with their name, ability description, price and tier.
public enum JokerTier
{
    Common,
    Uncommon,
    Rare
}

public class Joker
{
    public string Name { get; }
    public string Description { get; protected set; }
    public int Price { get; }
    public JokerTier Tier { get; }

    public Joker(string name, int price, JokerTier tier = JokerTier.Common)
    {
        Name = name;
        Price = price;
        Tier = tier;
    }

    public virtual int GetBonusChips(List<Card> playedCards, int discardsUsed)
    {
        return 0;
    }

    public virtual void ApplyEffect(List<Card> chosenCards, List<Card> playedCards)
    {

    }


    public virtual int GetBonusMult(List<Card> playedCards, int discardsUsed)
    {
        return 0;
    }


    public class GrosMichel : Joker
    {
        private Random _rng = new Random();

        public GrosMichel() : base("Gros Michel", 3, JokerTier.Common)
        {
            Description = "+15 mult. 1 out of 6 chance of breaking after hand.";
        }


        public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
        {
            return 15;
        }

        public bool ShouldBreak()
        {
            return _rng.Next(6) == 0;
        }
    }

    public class Misprint : Joker
    {
        private Random _rng = new Random();

        public Misprint() : base("Misprint", 4,  JokerTier.Common)
        {
            Description = "Randomly gain 1-20 mult";
        }

        public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
        {
            return _rng.Next(0, 21);
        }
    }

    public class Mask : Joker
    {
        public Mask() : base("The Mask", 7, JokerTier.Uncommon)
        {
            Description = "+20 mult when hand DOES NOT contain face card.";
        }

        public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
        {
            bool hasFace = playedCards.Any(c =>
                c.Rank == Rank.Jack ||
                c.Rank == Rank.Queen ||
                c.Rank == Rank.King);

            if (hasFace)
                return 0;

            return 20;
        }
    }

    public class JollyJoker : Joker
    {
        public JollyJoker() : base("Jolly Joker", 4, JokerTier.Common)
        {
            Description = "+8 Mult when hand contains pair";
        }

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
            : base("Abstract Joker", 4, JokerTier.Common)
        {
            Description = "Gains +3 mult per joker";
            _getJokerCount = getJokerCount;
        }

        public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
        {
            return _getJokerCount() * 3;
        }
    }
    public class Baron : Joker
    {
        public Baron() : base("Baron", 8, JokerTier.Rare)
        {
            Description = "Kings remaining in hand give ×1.5 multiplier.";
        }

        public double BonusMultiplier { get; private set; } = 1.0;

        public override void ApplyEffect(List<Card> playedCards, List<Card> fullHand)
        {
            var unplayed = fullHand.Except(playedCards).ToList();
            var kings = unplayed.Where(c => c.Rank == Rank.King).ToList();

            if (kings.Count == 0)
                return;

            foreach (var king in kings)
                king.VisualTags.Add("×1.5 (Baron)");

            BonusMultiplier = Math.Pow(1.5, kings.Count);
        }

        public void Reset()
        {
            BonusMultiplier = 1.0;
        }
    }
}
