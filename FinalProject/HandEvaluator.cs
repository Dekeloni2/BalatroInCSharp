using System;
using System.Collections.Generic;
using System.Linq;

namespace BalatroGame
{
    public enum HandRank
    {
        HighCard, Pair, TwoPair, ThreeOfKind, Straight, Flush, FullHouse, FourOfKind, StraightFlush
    }

    public class BestHandResult
    {
        public HandRank Rank { get; set; }
        public List<Card> Cards { get; set; }
        public int[] Kickers { get; set; } // לשבירת שוויון
    }

    public static class HandEvaluator
    {
        // מקבל בדיוק 5 קלפים ומחזיר את הדירוג
        public static BestHandResult EvaluateFive(List<Card> five)
        {
            if (five == null || five.Count != 5) throw new ArgumentException("Need exactly 5 cards");
            var ranks = five.Select(c => (int)c.Rank).OrderByDescending(x => x).ToList();
            bool isFlush = five.Select(c => c.Suit).Distinct().Count() == 1;
            var groups = five.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ThenByDescending(g => (int)g.Key).ToList();
            bool isStraight = IsStraight(five.Select(c => c.Rank).ToList());

            if (isStraight && isFlush) return new BestHandResult { Rank = HandRank.StraightFlush, Cards = five, Kickers = new[] { ranks.First() } };
            if (groups[0].Count() == 4) return new BestHandResult { Rank = HandRank.FourOfKind, Cards = five, Kickers = new[] { (int)groups[0].Key, (int)groups[1].Key } };
            if (groups[0].Count() == 3 && groups[1].Count() == 2) return new BestHandResult { Rank = HandRank.FullHouse, Cards = five, Kickers = new[] { (int)groups[0].Key, (int)groups[1].Key } };
            if (isFlush) return new BestHandResult { Rank = HandRank.Flush, Cards = five, Kickers = ranks.ToArray() };
            if (isStraight) return new BestHandResult { Rank = HandRank.Straight, Cards = five, Kickers = new[] { ranks.First() } };
            if (groups[0].Count() == 3) return new BestHandResult { Rank = HandRank.ThreeOfKind, Cards = five, Kickers = new[] { (int)groups[0].Key }.Concat(ranks.Where(r => r != (int)groups[0].Key)).ToArray() };
            if (groups[0].Count() == 2 && groups[1].Count() == 2) return new BestHandResult { Rank = HandRank.TwoPair, Cards = five, Kickers = new[] { (int)groups[0].Key, (int)groups[1].Key, (int)groups[2].Key } };
            if (groups[0].Count() == 2) return new BestHandResult { Rank = HandRank.Pair, Cards = five, Kickers = new[] { (int)groups[0].Key }.Concat(ranks.Where(r => r != (int)groups[0].Key)).ToArray() };
            return new BestHandResult { Rank = HandRank.HighCard, Cards = five, Kickers = ranks.ToArray() };
        }

        private static bool IsStraight(List<Rank> ranks)
        {
            var vals = ranks.Select(r => (int)r).Distinct().OrderBy(x => x).ToList();
            if (vals.Count != 5) return false;
            // Ace-low straight
            if (vals.SequenceEqual(new List<int> {2,3,4,5,14})) return true;
            return vals.Max() - vals.Min() == 4;
        }
    }
}
