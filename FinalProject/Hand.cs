namespace BalatroGame
{
    public class Hand
    {
        public List<Card> Cards { get; private set; } = new List<Card>();

        //Draws up to 8 cards
        public void DrawInitialHand(Deck deck)
        {
            Cards = deck.DrawMany(8);
        }
        //When discard or drawing cards
        public int ReplaceSelectedIndexes(List<int> indexes, Deck deck)
        {
            var distinct = indexes.Distinct().ToList();
            distinct.Sort();
            int replaced = 0;
            foreach (int idx in distinct)
            {
                Cards[idx] = deck.Draw();
                replaced++;
            }

            return replaced;
        }

        //Prints out the drawn hand
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
        public List<Card> GetCardsAtIndexes(List<int> indexes)
        {
            var result = new List<Card>();

            foreach (int i in indexes)
            {
                if (i >= 0 && i < Cards.Count)
                    result.Add(Cards[i]);
            }

            return result;
        }
    }
}