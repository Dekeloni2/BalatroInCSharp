using System;
using System.Collections.Generic;
using System.Linq;
#pragma warning disable CS0169, CS0649


namespace BalatroGame
{
    public class Deck
    {
        private readonly List<Card> _cards = new List<Card>();
        private readonly Random _rng;
        public List<Card> Cards
        {
            get { return _cards; }
        }


        public int Count => _cards.Count;
        
        public Deck(int? seed = null)
        {
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
            BuildDeck();
            Shuffle();
        }
        //Constructs the entire deck
        private void BuildDeck()
        {
            _cards.Clear();
            foreach (Suit s in Enum.GetValues(typeof(Suit)))
            foreach (Rank r in Enum.GetValues(typeof(Rank)))
                _cards.Add(new Card(s, r));
        }
        //Shuffles the cards. Every run is different.
        public void Shuffle()
        {
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }
        public Card Draw()
        {
            if (_cards.Count == 0) throw new InvalidOperationException("Deck is empty!");
            var c = _cards[0];
            _cards.RemoveAt(0);
            return c;
        }

        public List<Card> DrawMany(int n)
        {
            if (n > _cards.Count) throw new InvalidOperationException("Not enough cards in deck.");
            var res = _cards.Take(n).ToList();
            _cards.RemoveRange(0, n);
            return res;
        }
        public void BuildFreshDeck()
        {
            _cards.Clear();

            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    _cards.Add(new Card(suit, rank));
                }
            }
        }
    }
}