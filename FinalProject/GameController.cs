using System;
using System.Collections.Generic;
using System.Linq;

namespace BalatroGame
{
    public class Blind
    {
        public string Name { get; }
        public int TargetScore { get; }
        public int Reward { get; }

        public Blind(string name, int targetScore, int reward)
        {
            Name = name;
            TargetScore = targetScore;
            Reward = reward;
        }
    }

    public class GameController
    {
        private List<Blind> _blindSequence;
        private int _blindIndex;
        
        private readonly Deck _deck;
        private readonly Hand _hand;

        private const int MaxDiscards = 3;
        private const int MaxHands = 4;

        private int _remainingDiscards;
        private int _remainingHands;

        private int _totalScore = 0;
        private int _money = 4;

        private Blind _currentBlind;
        private bool _blindDefeated = false;
        
        private List<Card> _discardPile = new List<Card>();


        // Jokers
        private List<Joker> _jokers = new List<Joker>();
        private const int MaxJokers = 5;

        public GameController(int? seed = null)
        {
            _deck = new Deck(seed);
            _hand = new Hand();

            _remainingDiscards = MaxDiscards;
            _remainingHands = MaxHands;

            //Blind system
            _blindSequence = BuildBlindSequence();
            _blindIndex = 0;
            _currentBlind = _blindSequence[_blindIndex];
        }



        public void Run()
        {
            Console.WriteLine("Game started");

            while (_remainingHands > 0)
            {
                Console.WriteLine();
                Console.WriteLine($"--- (Dekelatro) ---");
                
                Console.WriteLine($"Current Blind: {_currentBlind.Name} — Target: {_currentBlind.TargetScore} points");


                if (_deck.Count < 0)
                {
                    Console.WriteLine("Your deck is empty — you have been defeated!");
                    return;
                }

                _hand.DrawInitialHand(_deck);

                Console.WriteLine("Your hand:");
                _hand.ShowHand();

                bool handPlayed = false;

                while (!handPlayed)
                {
                    Console.WriteLine();
                    Console.WriteLine(
                        $"Discards left: {_remainingDiscards} | Hands left: {_remainingHands} | Score: {_totalScore} | Money: {_money}$");
                    Console.WriteLine($"Cards left in deck: {_deck.Count}");
                    Console.WriteLine("Controls:");
                    Console.WriteLine("- Type numbers (e.g. 0 2 4) to PLAY cards");
                    Console.WriteLine("- Press 'D' to discard cards");
                    Console.WriteLine("- Press 'S' to sort by rank");
                    Console.WriteLine("- Press 'A' to sort by suit");
                    Console.WriteLine("- Press 'C' to view remaining cards in deck");
                    Console.WriteLine("- Press 'J' to view Jokers");

                    string line = Console.ReadLine()?.Trim();

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        Console.WriteLine("No action taken.");
                        continue;
                    }

                    // SORT BY RANK
                    if (line.Equals("S", StringComparison.OrdinalIgnoreCase))
                    {
                        _hand.SortDescending();
                        Console.WriteLine("Hand sorted by rank:");
                        _hand.ShowHand();
                        continue;
                    }

                    // SORT BY SUIT
                    if (line.Equals("A", StringComparison.OrdinalIgnoreCase))
                    {
                        _hand.SortBySuit();
                        Console.WriteLine("Hand sorted by suit:");
                        _hand.ShowHand();
                        continue;
                    }

                    // OPEN JOKER MENU
                    if (line.Equals("J", StringComparison.OrdinalIgnoreCase))
                    {
                        OpenJokerMenu();
                        continue;
                    }

                    if (line.Equals("C", StringComparison.OrdinalIgnoreCase))
                    {
                        ShowDeckComposition();
                        continue;
                    }

                    // DISCARD
                    if (line.Equals("D", StringComparison.OrdinalIgnoreCase))
                    {
                        if (_remainingDiscards == 0)
                        {
                            Console.WriteLine("No discards left.");
                            continue;
                        }

                        Console.WriteLine("Enter cards to discard, or press 'F' to cancel:");

                        string discardInput = Console.ReadLine()?.Trim();

                        // Cancel discard
                        if (discardInput.Equals("F", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Discard cancelled.");
                            continue;
                        }

                        var discardIndices = ParseIndices(discardInput);

                        if (discardIndices == null)
                        {
                            Console.WriteLine("Invalid input.");
                            continue;
                        }

                        if (_deck.Count < discardIndices.Count)
                        {
                            Console.WriteLine("Not enough cards in deck.");
                            continue;
                        }

                        var discarded = _hand.GetCardsAtIndices(discardIndices);
                        _discardPile.AddRange(discarded);

                        _hand.ReplaceSelectedIndices(discardIndices, _deck);
                        Console.WriteLine("Cards discarded.");
                        _hand.ShowHand();

                        _remainingDiscards--;
                        continue;
                    }

                    // PLAY CARDS
                    var playIndices = ParseIndices(line);
                    if (playIndices != null)
                    {
                        bool confirmed = HandlePlayCardsFlow(playIndices);

                        if (confirmed)
                        {
                            handPlayed = true;
                            _remainingHands--;

                            CheckBlindStatus();
                        }

                        continue;
                    }

                    Console.WriteLine("Invalid command.");
                }

                if (_blindDefeated)
                {
                    Console.WriteLine("Blind defeated — stopping hand loop.");
                    break;
                }

                if (_remainingHands == 0)
                {
                    Console.WriteLine("No hands left — game over!");
                    break;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Final score: " + _totalScore);
            Console.WriteLine("Final hand:");
            _hand.ShowHand();
            Console.WriteLine($"Cards left in deck: {_deck.Count}");
        }
        
        private void ShowDeckComposition()
        {
            Console.WriteLine();
            Console.WriteLine("=== DECK COMPOSITION ===");
            Console.WriteLine();

           //Remaining cards
            var remaining = _deck.Cards;

            //Order by suit
            var groups = remaining
                .GroupBy(c => c.Suit)
                .OrderBy(g => g.Key);

            foreach (var group in groups)
            {
                Console.WriteLine($"{GetSuitSymbol(group.Key)} {group.Key}:");

                var rankGroups = group
                    .GroupBy(c => c.Rank)
                    .OrderByDescending(g => g.Key);

                foreach (var rank in rankGroups)
                {
                    Console.Write($"{rank.Key}: {rank.Count()}   ");
                }

                Console.WriteLine("\n");
            }

            Console.WriteLine($"Total cards remaining: {remaining.Count}");
        }


        private string GetSuitSymbol(Suit suit)
        {
            return suit switch
            {
                Suit.Spades => "♠",
                Suit.Hearts => "♥",
                Suit.Diamonds => "♦",
                Suit.Clubs => "♣",
                _ => "?"
            };
        }


        
        
        private void CheckBlindStatus()
        {
            if (_totalScore >= _currentBlind.TargetScore)
            {
                Console.WriteLine();
                Console.WriteLine($"{_currentBlind.Name} DEFEATED");

                _money += _currentBlind.Reward;
                Console.WriteLine($"{_currentBlind.Reward} OBTAINED");

                Console.WriteLine("Dealer reshuffeling cards...");

                _deck.BuildFreshDeck();
                _deck.Shuffle();
                _hand.Clear();

                _remainingDiscards = MaxDiscards;
                _remainingHands = MaxHands;

                //Opens the shop
                OpenShop();

                _blindDefeated = false;  
                _currentBlind = GetNextBlind();
                _totalScore = 0;
                return;
                
            }

            //Losing
            if (_remainingHands == 0)
            {
                Console.WriteLine();
                Console.WriteLine($"You failed to reach {_currentBlind.TargetScore} points.");
                Console.WriteLine("Game Over!");
                Environment.Exit(0);
            }
        }

        //List of blinds
        private List<Blind> BuildBlindSequence()
        {
            return new List<Blind>
            {
                new Blind("Small Blind (Ante 1)", 300, 3),
                new Blind("Big Blind (Ante 1)", 450, 4),
                new Blind("Boss Blind (Ante 1)", 600, 6),

                new Blind("Small Blind (Ante 2)", 900, 6),
                new Blind("Big Blind (Ante 2)", 1200, 7),
                new Blind("Boss Blind (Ante 2)", 1500, 8),
                
            };
        }

        
        //Protects the code
        private Blind GetNextBlind()
        {
            if (_blindSequence == null || _blindSequence.Count == 0)
            {
                Console.WriteLine("Blind sequence not initialized!");
                Environment.Exit(0);
            }

            _blindIndex++;

            //If all blinds have been defeated (Up to ante 8)
            if (_blindIndex >= _blindSequence.Count)
            {
                Console.WriteLine("🎉 You beat all Blinds! You win!");
                Environment.Exit(0);
            }

            return _blindSequence[_blindIndex];
        }


        //Shop system
        private void OpenShop()
        {
            Shop shop = new Shop(_jokers);

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("=== SHOP ===");
                Console.WriteLine($"Money: {_money}");
                Console.WriteLine();


                for (int i = 0; i < shop.Items.Count; i++)
                {
                    Console.WriteLine($"{i}. {shop.Items[i].Name} — ${shop.Items[i].Price}");
                }

                Console.WriteLine();
                Console.WriteLine($"R. Reroll (${shop.RerollCost})");
                Console.WriteLine($"C. View consumables");
                Console.WriteLine($"J. View jokers");
                Console.WriteLine("F. Enter the next blind");

                string input = Console.ReadLine()?.Trim();

                if (input.Equals("F", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Leaving shop...");
                    return;
                }

                if (input.Equals("R", StringComparison.OrdinalIgnoreCase))
                {
                    if (_money < shop.RerollCost)
                    {
                        Console.WriteLine("Not enough money for reroll.");
                        continue;
                    }

                    _money -= shop.RerollCost;
                    shop.Reroll();
                    continue;
                }

                if (int.TryParse(input, out int index))
                {
                    if (index < 0 || index >= shop.Items.Count)
                    {
                        Console.WriteLine("Invalid item index.");
                        continue;
                    }

                    var item = shop.Items[index];

                    if (_money < item.Price)
                    {
                        Console.WriteLine("Not enough money.");
                        continue;
                    }

                    _money -= item.Price;
                    Console.WriteLine($"Purchased: {item.Name}");

                    _money -= item.Price;
                    Console.WriteLine($"Purchased: {item.Name}");
                    
                    AddJokerFromShop(item.Name);
                    
                    shop.Items.RemoveAt(index);
                    
                    continue;
                }
            }
        }

        private void OpenJokerMenu()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("=== JOKERS ===");
                Console.WriteLine($"You have {_jokers.Count}/{MaxJokers} Jokers");
                Console.WriteLine();

                for (int i = 0; i < _jokers.Count; i++)
                {
                    Console.WriteLine($"{i}. {_jokers[i].Name}");
                    Console.WriteLine($"   {_jokers[i].Description}");
                }

                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine("O - Reorder Jokers");
                Console.WriteLine("F - Finish");

                string input = Console.ReadLine()?.Trim();

                if (input.Equals("F", StringComparison.OrdinalIgnoreCase))
                    return;

                if (input.Equals("O", StringComparison.OrdinalIgnoreCase))
                {
                    ReorderJokers();
                    continue;
                }

                Console.WriteLine("Invalid option.");
            }
        }

        private void ReorderJokers()
        {
            Console.WriteLine("Enter two numbers to swap (e.g. 0 4):");

            string line = Console.ReadLine()?.Trim();
            var indices = ParseIndices(line);

            if (indices == null || indices.Count != 2)
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            int a = indices[0];
            int b = indices[1];

            if (a < 0 || a >= _jokers.Count || b < 0 || b >= _jokers.Count)
            {
                Console.WriteLine("Invalid indices.");
                return;
            }

            var temp = _jokers[a];
            _jokers[a] = _jokers[b];
            _jokers[b] = temp;

            Console.WriteLine("Jokers reordered!");
        }

        private bool HandlePlayCardsFlow(List<int> parsed)
        {
            parsed = parsed.Distinct().ToList();

            if (parsed.Count < 1 || parsed.Count > 5)
            {
                Console.WriteLine("You must choose between 1 and 5 cards.");
                return false;
            }

            if (parsed.Any(i => i < 0 || i > 7))
            {
                Console.WriteLine("Numbers must be between 0 and 7.");
                return false;
            }

            //Chosen cards
            List<Card> chosenCards = _hand.GetCardsAtIndices(parsed);

            Console.WriteLine();
            
            foreach (var joker in _jokers)
                joker.ApplyEffect(chosenCards);
            
            Console.WriteLine("Selected cards (after Joker effects):");
            for (int i = 0; i < chosenCards.Count; i++)
                chosenCards[i].PrintColored(parsed[i]);


            // חישוב צ'יפים ומכפלה
            var chipsEval = Scorer.ChipsForPlayedCardsWithBonus(chosenCards);

            int discardsUsed = MaxDiscards - _remainingDiscards;
            int jokerBonusChips = 0;
            int jokerBonusMult = 0;

            foreach (var joker in _jokers)
            {
                jokerBonusChips += joker.GetBonusChips(chosenCards, discardsUsed);
                jokerBonusMult += joker.GetBonusMult(chosenCards, discardsUsed);
            }

            int finalChips = chipsEval.TotalBeforeMultiplier + jokerBonusChips;
            int finalMult = chipsEval.Multiplier + jokerBonusMult;
            int finalScore = finalChips * finalMult;

            Console.WriteLine();
            Console.WriteLine($"Joker bonus chips: {jokerBonusChips}");
            Console.WriteLine($"Joker bonus mult: {jokerBonusMult}");
            Console.WriteLine($"Final score for this hand: {finalScore}");

            Console.WriteLine();
            Console.WriteLine("Confirm play? Press 'Y' to confirm, or 'F' to cancel.");

            string confirm = Console.ReadLine()?.Trim();


            if (confirm.Equals("Y", StringComparison.OrdinalIgnoreCase))
            {
                _totalScore += finalScore;
                return true;
            }
            foreach (var card in chosenCards)
                card.VisualTags.Clear();

            return false;
        }


        private List<int> ParseIndices(string input)
        {
            var parts = input.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            var parsed = new List<int>();

            foreach (var p in parts)
            {
                if (int.TryParse(p, out int v))
                    parsed.Add(v);
                else
                    return null;
            }

            return parsed.Distinct().ToList();
        }

        private void AddJokerFromShop(string itemName)
        {
            if (_jokers.Count >= MaxJokers)
            {
                Console.WriteLine("You already have the maximum number of Jokers.");
                return;
            }

            switch (itemName)
            {
                case "Gros Michel":
                    _jokers.Add(new GrosMichel());
                    Console.WriteLine("Added Gros Michel!");
                    break;

                case "Misprint":
                    _jokers.Add(new Misprint());
                    Console.WriteLine("Added Misprint!");
                    break;
                

                case "Ride the Bus":
                    _jokers.Add(new Mask());
                    Console.WriteLine("Added Ride the Bus!");
                    break;

                case "Abstract Joker":
                    _jokers.Add(new AbstractJoker(() => _jokers.Count));
                    Console.WriteLine("Added Abstract Joker!");
                    break;

                case "Jolly Joker":
                    _jokers.Add(new JollyJoker());
                    Console.WriteLine("Added Jolly Joker!");
                    break;

                default:
                    Console.WriteLine("Item purchased, but it is not a Joker.");
                    break;
            }
        }
    }
}