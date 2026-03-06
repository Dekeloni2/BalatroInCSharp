namespace FinalProject;
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
    protected string _description;
    public virtual string Description => _description;
    public int Price { get; }

    public int SellValue { get; protected set; } = 1;
    public JokerTier Tier { get; }

    protected Joker(string name, int price, JokerTier tier = JokerTier.Common, int sellValue = 0)
    {
        Name = name;
        Price = price;
        Tier = tier;
        SellValue = sellValue;
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

    public class Constellation : Joker
    {
        private double _multBonus = 1.0;

        public Constellation() : base("Constellation", 8, JokerTier.Uncommon, 3)
        {
            _description = $"When using a Planet, this joker gains +0.1x ({_multBonus:0.0}x).";
        }

        public void OnPlanetUsed()
        {
            _multBonus += 0.1;
            Console.WriteLine($"Constellation glows brighter! New multiplier: {_multBonus:0.0}x");
        }

        public override double GetBonusChipMultiplier(List<Card> playedCards, int discardsUsed)
        {
            return _multBonus;
        }

        public override int GetBonusChips(List<Card> playedCards, int discardsUsed) => 0;
        public override int GetBonusMult(List<Card> playedCards, int discardsUsed) => 0;
    }
    
    public class GrosMichel : Joker
    {
        private readonly Random _rng = new Random();

        public GrosMichel() : base("Gros Michel", 5, JokerTier.Common, 2)
        {
            _description = "+15 mult. 1 out of 6 chance of breaking after hand.";
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

    public class TradingCard : Joker
    {
        public TradingCard() : base("Trading Card", 5, JokerTier.Uncommon, 2)
        {
            _description = "If your first discard is a single card, destroy it and gain $3.";
        }

        public void OnFirstDiscardTradingCard(GameController game, Card card)
        {
            game.DestroyCard(card);

            game.AddMoney(3);

            Console.WriteLine("Trading card activated! +3$");
        }
    }

    public class ChaosTheClown : Joker
    {
        public ChaosTheClown() : base("Chaos the Clown", 4, JokerTier.Common, 2)
        {
            _description = "Your first reroll costs $0.";
        }

        public bool FirstRerollIsFree => true;
    }

    public class EvenSteven : Joker
    {
        public EvenSteven() : base("Even Steven", 3, JokerTier.Common, 2)
        {
            _description = "+4 mult for each even-ranked card (2, 4, 6, 8, 10).";
        }

        public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
        {
            int bonus = 0;

            foreach (var card in playedCards)
            {
                if (IsEven(card))
                    bonus += 4;
            }

            return bonus;
        }

        private bool IsEven(Card card)
        {
            return card.Rank == Rank.Two ||
                   card.Rank == Rank.Four ||
                   card.Rank == Rank.Six ||
                   card.Rank == Rank.Eight ||
                   card.Rank == Rank.Ten;
        }
    }

    public class OddTodd : Joker
    {
        public OddTodd() : base("Odd Todd", 3, JokerTier.Common, 2)
        {
            _description = "+31 chips for each odd-ranked card (A, 3, 5, 7, 9).";
        }

        public override int GetBonusChips(List<Card> playedCards, int discardsUsed)
        {
            int bonus = 0;

            foreach (var card in playedCards)
            {
                if (IsOdd(card))
                    bonus += 31;
            }

            return bonus;
        }

        private bool IsOdd(Card card)
        {
            return card.Rank == Rank.Ace ||
                   card.Rank == Rank.Three ||
                   card.Rank == Rank.Five ||
                   card.Rank == Rank.Seven ||
                   card.Rank == Rank.Nine;
        }
    }

    public class MysticSummit : Joker
    {
        public MysticSummit() : base("Mystic Summit", 4, JokerTier.Common, 2)
        {
            _description = "+15 mult if you have no discards remaining.";
        }

        public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
        {

            int remaining = GameController.MaxDiscards - discardsUsed;

            if (remaining == 0)
                return 15;

            return 0;
        }
    }

    public class Misprint : Joker
    {
        private Random _rng = new Random();

        public Misprint() : base("Misprint", 4, JokerTier.Common, 2)
        {
            _description = "Randomly gain 1-20 mult";
        }

        public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
        {
            return _rng.Next(0, 21);
        }
    }

    public class Mask : Joker
    {
        public Mask() : base("The Mask", 7, JokerTier.Uncommon, 3)
        {
            _description = "+20 mult when hand DOES NOT contain face card.";
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
        public Fibonacci() : base("Fibonacci", 7, JokerTier.Uncommon, 3)
        {
            _description = "+8 mult for each Ace, 2, 3, 5, or 8 played.";
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

            return fibbonaciBonus;
        }
    }

    public class PiMan : Joker
    {
        public PiMan() : base("Pi Man", 10, JokerTier.Rare, 4)
        {
            _description = "Raises chips to the power of 3.14 if played hand contains Ace, 3 and 4.";
        }

        public override int GetBonusChips(List<Card> playedCards, int discardsUsed)
        {
            bool hasAce = playedCards.Any(c => c.Rank == Rank.Ace);
            bool hasThree = playedCards.Any(c => c.Rank == Rank.Three);
            bool hasFour = playedCards.Any(c => c.Rank == Rank.Four);

            if (!(hasAce && hasThree && hasFour))
                return 0;

            int baseChips = playedCards.Sum(c => c.ChipValue);

            double powered = Math.Pow(baseChips, 3.14);

            return (int)(powered - baseChips);
        }
    }

    public class JollyJoker : Joker
    {
        public JollyJoker() : base("Jolly Joker", 4, JokerTier.Common, 2)
        {
            _description = "+8 Mult when hand contains pair";
        }

        public override int GetBonusMult(List<Card> playedCards, int discardsUsed)
        {
            var groups = playedCards.GroupBy(c => c.Rank);
            bool hasPair = groups.Any(g => g.Count() >= 2);

            return hasPair ? 8 : 0;
        }
    }

    public class GoldenJoker : Joker
    {
        public GoldenJoker() : base("Golden Joker", 4, JokerTier.Common, 2)
        {
            _description = "Gain $4 after winning a Blind.";
        }

        public override int GetBonusChips(List<Card> playedCards, int discardsUsed) => 0;
        public override int GetBonusMult(List<Card> playedCards, int discardsUsed) => 0;
        public override double GetBonusChipMultiplier(List<Card> playedCards, int discardsUsed) => 1.0;
    }

    public class ZanyJoker : Joker
    {
        public ZanyJoker() : base("Zany Joker", 4, JokerTier.Common, 2)
        {
            _description = "+12 Mult when hand contains Three of a Kind";
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
        public MadJoker() : base("Mad Joker", 4, JokerTier.Common, 2)
        {
            _description = "+10 Mult when hand contains Two Pair";
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
        public CrazyJoker() : base("Crazy Joker", 4, JokerTier.Common, 2)
        {
            _description = "+12 Mult when hand contains Straight";
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

    public class VoidPortal : Joker
    {
        public VoidPortal() : base("Void Portal", 7, JokerTier.Rare, 3)
        {
            _description = "Every 3rd discard transforms the discarded cards into random cards of the same suit.";
        }

        public void OnDiscard(GameController game, List<Card> discarded)
        {
            if (discarded == null || discarded.Count == 0)
                return;

            game.IncrementDiscardCounter();

            if (game.DiscardCounter == 3)
            {
                game.TransformDiscardedCards(discarded);
                game.ResetDiscardCounter();
                Console.WriteLine("Void Portal warps your discarded cards!");
            }
        }
    }
}