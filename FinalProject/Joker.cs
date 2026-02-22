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
    public string Description { get; set; } = "";
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
    public virtual double GetBonusChipMultiplier(List<Card> playedCards, int discardsUsed)
    {
        return 1.0; 
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

        public Misprint() : base("Misprint", 4, JokerTier.Common)
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
    
    public class Fibonacci : Joker
    {
        public Fibonacci() : base("Fibonacci", 7, JokerTier.Uncommon)
        {
            Description = "+8 mult for each Ace, 2, 3, 5, or 8 played.";
        }

        public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
        {
            int fibbonaciBonus = 0;

            foreach (var card in playedCards)
            {
                if (card.Rank == Rank.Ace ||
                    card.Rank == Rank.Two ||
                    card.Rank == Rank.Three ||
                    card.Rank == Rank.Five ||
                    card.Rank == Rank.Eight)
                {
                    fibbonaciBonus += 8;
                }
            }

            return 0;
        }
    }
    public class PiMan : Joker
    {
        public PiMan() : base("Pi Man", 10, JokerTier.Rare)
        {
            Description = "Raises chips to the power of 3.14 if played hand contains Ace, 3 and 4.";
        }

        public override int GetBonusChips(List<Card> playedCards, int discardsUsed)
        {
            bool hasAce   = playedCards.Any(c => c.Rank == Rank.Ace);
            bool hasThree = playedCards.Any(c => c.Rank == Rank.Three);
            bool hasFour  = playedCards.Any(c => c.Rank == Rank.Four);

            if (!(hasAce && hasThree && hasFour))
                return 0;
            
            int baseChips = playedCards.Sum(c => c.ChipValue);
            
            double powered = Math.Pow(baseChips, 3.14);
            
            return (int)(powered - baseChips);
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

    public class ZanyJoker : Joker
    {
        public ZanyJoker() : base("Zanny Joker", 4, JokerTier.Common)
        {
            Description = "+12 Mult when hand contains Three of a Kind";
        }

        public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
        {
            if (playedCards.Count != 3)
                return 0;

            bool isThreeOfAKind = playedCards
                .All(c => c.Rank == playedCards[0].Rank);

            return isThreeOfAKind ? 12 : 0;
        }
    }

    public class MadJoker : Joker
    {
        public MadJoker() : base("Mad Joker", 4, JokerTier.Common)
        {
            Description = "+10 Mult when hand contains Two Pair";
        }

        public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
        {
            if (playedCards.Count != 4)
                return 0;

            var groups = playedCards
                .GroupBy(c => c.Rank)
                .Select(g => g.Count())
                .OrderByDescending(c => c)
                .ToList();

            bool isTwoPair = groups.SequenceEqual(new List<int> { 2, 2 });

            return isTwoPair ? 10 : 0;
        }
    }

    public class CrazyJoker : Joker
    {
        public CrazyJoker() : base("Crazy Joker", 4, JokerTier.Common)
        {
            Description = "+12 Mult when hand contains Straight";
        }

        public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
        {
            if (playedCards.Count != 5)
                return 0;

            var ranks = playedCards
                .Select(c => (int)c.Rank)
                .OrderBy(r => r)
                .ToList();

            bool isStraight = true;
            for (int i = 1; i < ranks.Count; i++)
            {
                if (ranks[i] != ranks[i - 1] + 1)
                {
                    isStraight = false;
                    break;
                }
            }

            bool isLowAceStraight =
                ranks.SequenceEqual(new List<int> { 2, 3, 4, 5, 14 }); // Ace = 14

            if (isStraight || isLowAceStraight)
                return 10;

            return 0;
        }
    }
}