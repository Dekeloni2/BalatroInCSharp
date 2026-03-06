using BalatroGame;

#pragma warning disable CS0169, CS0649
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).


namespace FinalProject
{
    public class BestHandResult
    {
        public HandRank Rank { get; set; }
        public List<Card> Cards { get; set; } = new();
        public int[] RankValues { get; set; } = null!;
        
    }
    public static class HandEvaluator
    {
        //Gets exactly 5 cards and looks for the best possible combination 
        public static BestHandResult EvaluateFive(List<Card> five)
        {
            if (five == null || five.Count != 5) throw new ArgumentException("Need exactly 5 cards");
            var ranks = five.Select(c => (int)c.Rank).OrderByDescending(x => x).ToList();
            bool isFlush = five.Select(c => c.Suit).Distinct().Count() == 1;
            var groups = five.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ThenByDescending(g => (int)g.Key).ToList();
            bool isStraight = IsStraight(five.Select(c => c.Rank).ToList());

            if (isStraight && isFlush) 
                return new BestHandResult 
                    { Rank = HandRank.StraightFlush, Cards = five, RankValues = new[] { ranks.First() } };
            if (groups[0].Count() == 4) 
                return new BestHandResult 
                    { Rank = HandRank.FourOfKind, Cards = five, RankValues = new[] { (int)groups[0].Key, (int)groups[1].Key } };
            if (groups[0].Count() == 3 && groups[1].Count() == 2) 
                return new BestHandResult 
                    { Rank = HandRank.FullHouse, Cards = five, RankValues = new[] { (int)groups[0].Key, (int)groups[1].Key } };
            if (isFlush) 
                return new BestHandResult 
                    { Rank = HandRank.Flush, Cards = five, RankValues = ranks.ToArray() };
            if (isStraight) 
                return new BestHandResult 
                    { Rank = HandRank.Straight, Cards = five, RankValues = new[] { ranks.First() } };
            if (groups[0].Count() == 3) 
                return new BestHandResult 
                    { Rank = HandRank.ThreeOfKind, Cards = five, RankValues = new[] { (int)groups[0].Key }.Concat(ranks.Where(r => r != (int)groups[0].Key)).ToArray() };
            if (groups[0].Count() == 2 && groups[1].Count() == 2) 
                return new BestHandResult 
                    { Rank = HandRank.TwoPair, Cards = five, RankValues = new[] { (int)groups[0].Key, (int)groups[1].Key, (int)groups[2].Key } };
            if (groups[0].Count() == 2) 
                return new BestHandResult 
                    { Rank = HandRank.Pair, Cards = five, RankValues = new[] { (int)groups[0].Key }.Concat(ranks.Where(r => r != (int)groups[0].Key)).ToArray() }; 
            return new BestHandResult 
                { Rank = HandRank.HighCard, Cards = five, RankValues = ranks.ToArray() };
        }
        public static (int handChips, int handMult) GetHandValueFlexible(List<Card> cards)
        {
            if (cards.Count == 5)
            {
                var result = EvaluateFive(cards);

                return result.Rank switch
                {
                    HandRank.HighCard      => (5, 0),
                    HandRank.Pair          => (10, 1),
                    HandRank.TwoPair       => (20, 1),
                    HandRank.ThreeOfKind   => (30, 2),
                    HandRank.Straight      => (30, 3),
                    HandRank.Flush         => (35, 3),
                    HandRank.FullHouse     => (40, 3),
                    HandRank.FourOfKind    => (60, 6),
                    HandRank.StraightFlush => (100, 7),
                    _ => (0, 0)
                };
            }
            
            var groups = cards.GroupBy(c => c.Rank).Select(g => g.Count()).OrderByDescending(x => x).ToList();

            if (groups[0] == 4) return (60, 6);   // Four of a Kind
            if (groups[0] == 3) return (30, 2);   // Three of a Kind
            if (groups[0] == 2 && groups.Count == 2) return (20, 1); // Two Pair
            if (groups[0] == 2) return (10, 1);   // Pair

            return (5, 0); // High Card
        }
        //Considers if the hand is a straight and takes an Ace into consideration
        private static bool IsStraight(List<Rank> ranks)
        {
            var vals = ranks.Select(r => (int)r).Distinct().OrderBy(x => x).ToList();
            if (vals.Count != 5) return false;
            // Ace-low straight
            if (vals.SequenceEqual(new List<int> {2,3,4,5,14})) return true;
            return vals.Max() - vals.Min() == 4;
        }
        public static string GetHandNameFlexible(List<Card> cards)
        {
            if (cards.Count == 5)
                return EvaluateFive(cards).Rank.ToString();

            var groups = cards.GroupBy(c => c.Rank).Select(g => g.Count()).OrderByDescending(x => x).ToList();

            if (groups[0] == 4) return "Four of a Kind";
            if (groups[0] == 3) return "Three of a Kind";
            if (groups[0] == 2 && groups.Count == 2) return "Two Pair";
            if (groups[0] == 2) return "Pair";

            return "High Card";
        }
    }
}

