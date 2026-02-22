using System;
using System.Collections.Generic;
using System.Linq;

//Class for evaluation of hands played. This takes into account Jokers, Enchantments, Hand levels, Editions...
namespace BalatroGame
{
    public class ChipsEvaluation
    {
        public int ChipsFromCards { get; set; }      // סכום הערכים של הקלפים שנחשבים
        public int BonusChips { get; set; }          // בונוס לפי סוג היד
        public int Multiplier { get; set; }          // מכפיל סופי
        public int TotalBeforeMultiplier => ChipsFromCards + BonusChips;
        public int TotalAfterMultiplier => TotalBeforeMultiplier * Multiplier;
        public string Description { get; set; } = "";
    }

    public static partial class Scorer
    {
        //Hand type bonus chips and mult. Upgradable using Planet cards.
        private static (int bonus, int mult) BonusAndMultiplierForRank(HandRank rank)
        {
            return rank switch
            {
                HandRank.StraightFlush => (bonus: 100, mult: 6),
                HandRank.FourOfKind    => (bonus: 80,  mult: 5),
                HandRank.FullHouse     => (bonus: 60,  mult: 4),
                HandRank.Flush         => (bonus: 50,  mult: 4),
                HandRank.Straight      => (bonus: 30,  mult: 4), 
                HandRank.ThreeOfKind   => (bonus: 25,  mult: 3),
                HandRank.TwoPair       => (bonus: 18,  mult: 2),
                HandRank.Pair          => (bonus: 10,  mult: 2),
                HandRank.HighCard      => (bonus: 5,   mult: 1),
                _ => (0, 1)
            };
        }

        //Main function that returns all necessary information.
        public static ChipsEvaluation ChipsForPlayedCardsWithBonus(List<Card> chosen)
        {
            if (chosen == null || chosen.Count == 0)
                return new ChipsEvaluation { ChipsFromCards = 0, BonusChips = 0, Multiplier = 1, Description = "No cards selected" };

            int CardValue(Card c) => (int)c.Rank;

            //In the case of 5 cards
            if (chosen.Count == 5)
            {
                var result = HandEvaluator.EvaluateFive(chosen);
                var (bonus, mult) = BonusAndMultiplierForRank(result.Rank);

                int chipsFromCards;
                string desc;

                if (result.Rank != HandRank.HighCard)
                {
                    if (result.Rank == HandRank.Straight || result.Rank == HandRank.Flush || result.Rank == HandRank.StraightFlush)
                    {
                        chipsFromCards = chosen.Sum(CardValue);
                        desc = $"{result.Rank}: sum of all 5 cards = {chipsFromCards}";
                    }
                    else
                    {
                        var groups = chosen.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ToList();
                        var involvedRanks = new List<Rank>();

                        if (result.Rank == HandRank.FourOfKind)
                            involvedRanks.Add(groups[0].Key);
                        else if (result.Rank == HandRank.FullHouse)
                        {
                            involvedRanks.Add(groups[0].Key); // trips
                            involvedRanks.Add(groups[1].Key); // pair
                        }
                        else if (result.Rank == HandRank.ThreeOfKind)
                            involvedRanks.Add(groups[0].Key);
                        else if (result.Rank == HandRank.TwoPair)
                        {
                            involvedRanks.Add(groups[0].Key);
                            involvedRanks.Add(groups[1].Key);
                        }
                        else if (result.Rank == HandRank.Pair)
                            involvedRanks.Add(groups[0].Key);

                        chipsFromCards = chosen.Where(c => involvedRanks.Contains(c.Rank)).Sum(CardValue);
                        desc = $"{result.Rank}: sum of involved cards = {chipsFromCards}";
                    }
                }
                else
                {
                    chipsFromCards = chosen.Max(CardValue);
                    desc = $"High card only: {chipsFromCards}";
                }

                return new ChipsEvaluation
                {
                    ChipsFromCards = chipsFromCards,
                    BonusChips = bonus,
                    Multiplier = mult,
                    Description = $"{desc}; Bonus={bonus}; Mult={mult}"
                };
            }
            
            var groupsPartial = chosen.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ToList();
            bool isFlushPartial = chosen.Select(c => c.Suit).Distinct().Count() == 1;
            bool isStraightPartial = chosen.Count >= 3 && HandEvaluatorIsStraightLike(chosen.Select(c => c.Rank).ToList());

            // אם יש קבוצה (pair/trips/quads) — נסכם רק את הקלפים המשתתפים בקבוצה; בונוס/מכפיל נמוכים יותר
            if (groupsPartial[0].Count() >= 2)
            {
                var rankOfGroup = groupsPartial[0].Key;
                int sumGroup = chosen.Where(c => c.Rank == rankOfGroup).Sum(CardValue);
                int bonus = groupsPartial[0].Count() == 2 ? 6 : groupsPartial[0].Count() == 3 ? 15 : 40;
                int mult = groupsPartial[0].Count() == 2 ? 2 : groupsPartial[0].Count() == 3 ? 3 : 5;
                string desc = groupsPartial[0].Count() == 2 ? "Pair (partial)" :
                              groupsPartial[0].Count() == 3 ? "Three of a kind (partial)" : "Four of a kind (partial)";

                return new ChipsEvaluation
                {
                    ChipsFromCards = sumGroup,
                    BonusChips = bonus,
                    Multiplier = mult,
                    Description = $"{desc}: sum of group cards = {sumGroup}; Bonus={bonus}; Mult={mult}"
                };
            }

            //Partial straight flush (If a joker like Shortcut is picked)
            if ((isStraightPartial && chosen.Count >= 3) || (isFlushPartial && chosen.Count >= 3))
            {
                int sum = chosen.Sum(CardValue);
                int bonus = 12;
                int mult = 2;
                string desc = isStraightPartial ? "Partial straight" : "Partial flush";
                return new ChipsEvaluation
                {
                    ChipsFromCards = sum,
                    BonusChips = bonus,
                    Multiplier = mult,
                    Description = $"{desc}: sum of selected cards = {sum}; Bonus={bonus}; Mult={mult}"
                };
            }
            
            int maxCard = chosen.Max(CardValue);
            return new ChipsEvaluation
            {
                ChipsFromCards = maxCard,
                BonusChips = 0,
                Multiplier = 1,
                Description = $"No poker combination: high card only = {maxCard}"
            };
        }
        
        private static bool HandEvaluatorIsStraightLike(List<Rank> ranks)
        {
            var vals = ranks.Select(r => (int)r).Distinct().OrderBy(x => x).ToList();
            if (vals.Count < 3) return false;
            var aceLow = new List<int> { 2, 3, 4, 5, 14 };
            if (vals.SequenceEqual(aceLow.Take(vals.Count))) return true;
            return vals.Max() - vals.Min() == vals.Count - 1;
        }
    }
}
