using System;

namespace BalatroGame
{
    //Generates all cards
    public enum Suit
    {
        Spades,
        Hearts,
        Diamonds,
        Clubs
    }

    public enum Rank
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14
    }

    public class Card
    {
        public Suit Suit { get; }
        public Rank Rank { get; }
        public int BaseMultiplier { get; set; } = 1;

        
        public List<string> VisualTags { get; } = new List<string>();

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }
        public int ChipValue
        {
            get
            {
                return Rank switch
                {
                    Rank.Ace   => 14,
                    Rank.Jack  => 11,
                    Rank.Queen => 12,
                    Rank.King  => 13,
                    _          => (int)Rank
                };
            }
        }
        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
        //Converts cards to symbols for better UX
        public string ToUnicode()
        {
            string suitSymbol = Suit switch
            {
                Suit.Spades => "♠",
                Suit.Hearts => "♥",
                Suit.Diamonds => "♦",
                Suit.Clubs => "♣",
                _ => "?"
            };

            string rankSymbol = Rank switch
            {
                Rank.Ace => "A",
                Rank.King => "K",
                Rank.Queen => "Q",
                Rank.Jack => "J",
                Rank.Ten => "10",
                Rank.Nine => "9",
                Rank.Eight => "8",
                Rank.Seven => "7",
                Rank.Six => "6",
                Rank.Five => "5",
                Rank.Four => "4",
                Rank.Three => "3",
                Rank.Two => "2",
                _ => "?"
            };

            return rankSymbol + suitSymbol;
        }

        public void PrintColored(int index)
        {
            string suitSymbol = Suit switch
            {
                Suit.Spades => "♠",
                Suit.Hearts => "♥",
                Suit.Diamonds => "♦",
                Suit.Clubs => "♣",
                _ => "?"
            };

            string rankSymbol = Rank switch
            {
                Rank.Ace => "A",
                Rank.King => "K",
                Rank.Queen => "Q",
                Rank.Jack => "J",
                Rank.Ten => "10",
                Rank.Nine => "9",
                Rank.Eight => "8",
                Rank.Seven => "7",
                Rank.Six => "6",
                Rank.Five => "5",
                Rank.Four => "4",
                Rank.Three => "3",
                Rank.Two => "2",
                _ => "?"
            };
            //Differentiates cards by color
            ConsoleColor color = Suit switch
            {
                Suit.Hearts => ConsoleColor.Red,
                Suit.Diamonds => ConsoleColor.Yellow,
                Suit.Spades => ConsoleColor. Gray,
                Suit.Clubs => ConsoleColor.Blue,
                _ => ConsoleColor.White
            };

            Console.Write($"{index}: {rankSymbol}");

            Console.ForegroundColor = color;
            Console.Write(suitSymbol);
            Console.ResetColor();

            Console.WriteLine();
            //If the card has any special enchantment
            if (VisualTags.Count > 0)
            {
                Console.Write("  [");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(string.Join(", ", VisualTags));
                Console.ResetColor();
                Console.Write("]");
            }
        }
    }
}
