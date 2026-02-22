using System;
using System.Collections.Generic;
using System.Linq;
#pragma  warning disable CS0169


namespace BalatroGame
{
    public class Hand
    {
        public List<Card> Cards { get; private set; } = new List<Card>();

        public void DrawInitialHand(Deck deck)
        {
            Cards = deck.DrawMany(8);
        }
        //When discard or drawing cards
        public int ReplaceSelectedIndices(List<int> indices, Deck deck)
        {
            var distinct = indices.Distinct().ToList();
            distinct.Sort();
            int replaced = 0;
            foreach (int idx in distinct)
            {
                Cards[idx] = deck.Draw();
                replaced++;
            }

            return replaced;
        }

        public void ShowHand()
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].PrintColored(i);
            }
        }

        public void Clear()
        {
            Cards.Clear();
        }
        
        //Function to sort cards by Rank
        public void SortDescending()
        {
            Cards = Cards
                .OrderByDescending(c => (int)c.Rank)
                .ThenByDescending(c => c.Suit)
                .ToList();
        }
        //Function to sort cards by Suit
        public void SortBySuit()
        {
            Cards = Cards
                .OrderBy(c => c.Suit)
                .ThenByDescending(c => (int)c.Rank)
                .ToList();
        }
        public List<Card> GetCardsAtIndices(List<int> indices)
        {
            var result = new List<Card>();

            foreach (int i in indices)
            {
                if (i >= 0 && i < Cards.Count)
                    result.Add(Cards[i]);
            }

            return result;
        }
    }
}