using BalatroGame;
// ReSharper disable All


//This is just to remove a lot of the warnings. Don't worry, a lot of the warnings are just saying that
//the options might be nullable. There are no functions that are wasteful.
#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS0169, CS0649
#pragma warning disable CS8604

namespace FinalProject


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
        public int Money
        {
            get => _money;
            set => _money = value;
        }

        public int GetHandLevel(HandRank rank)
        {
            return _handLevels[rank];
        }
        public static GameController Instance { get; private set; }
        public void UseConsumableSlot(int slotIndex)
        {
            _consumables.UseConsumable(slotIndex, this);
        }
        
        public void LevelUpHand(HandRank rank)
        {
            _handLevels[rank]++;
            Console.WriteLine($"{rank} leveled up! New level: {_handLevels[rank]}");
        }
        

        private List<Blind> _blindSequence;
        private int _blindNumber;
        
        private readonly Deck _deck;
        private readonly Hand _hand;

        private const int MaxDiscards = 3;
        private const int MaxHands = 4;

        public int _remainingDiscards;
        public int _remainingHands;

        private ConsumableSlots _consumables = new ConsumableSlots(2);
        private int _totalScore;
        private int _money = 9999;

        private Blind _currentBlind;
        private bool _blindDefeated;
        
        private List<Card> _discardPile = new List<Card>();


        // Jokers
        private readonly List<Joker> _jokers = new List<Joker>();
        public List<Joker> Jokers { get; private set; } = new();
        private const int MaxJokers = 5;


        public GameController(int? seed = null)
        {
            _deck = new Deck(seed);
            _hand = new Hand();

            _remainingDiscards = MaxDiscards;
            _remainingHands = MaxHands;

            //Blind system
            _blindSequence = BuildBlindSequence();
            _blindNumber = 0;
            _currentBlind = _blindSequence[_blindNumber];
        }
        

        public void Run()
        {
            Console.WriteLine("Game started");

            while (_remainingHands > 0)
            {
                Console.WriteLine();
                Console.WriteLine("--- (Dekelatro) ---");
                
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
                    Console.WriteLine("- Press 'X' to look at Consumables");
                    Console.WriteLine("- Press 'C' to view remaining cards in deck");
                    Console.WriteLine("- Press 'J' to view Jokers");

                    string? line = Console.ReadLine()?.Trim();

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
                        ShowDeck();
                        continue;
                    }
                    
                    //CONSUMABLES
                    if (line.Equals("X", StringComparison.OrdinalIgnoreCase))
                    {
                        OpenConsumableMenu();
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

                        var discardIndices = PraseToIndexes(discardInput);

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
                    var playIndices = PraseToIndexes(line);
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
        
        private void ShowDeck()
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
                bool grosMichelFound = false;
                bool grosMichelBroke = false;
                Console.WriteLine();
                Console.WriteLine($"{_currentBlind.Name} DEFEATED");

                _money += _currentBlind.Reward;
                Console.WriteLine($"{_currentBlind.Reward}$ OBTAINED");

                Console.WriteLine("Dealer reshuffeling cards...");

                _deck.BuildFreshDeck();
                _deck.Shuffle();
                _hand.Clear();

                _remainingDiscards = MaxDiscards;
                _remainingHands = MaxHands;
                
                
                //Specifically looks for Gros Michel and removes it if it does pass the break check
                foreach (var joker in Jokers.ToList())
                {
                    if (joker is Joker.GrosMichel gm)
                    {
                        grosMichelFound = true;

                        if (gm.ShouldBreak())
                        {
                            Console.WriteLine("Your Gros Michel broke!");
                            Jokers.Remove(gm);
                            grosMichelBroke = true;
                        }
                    }
                }
                if (grosMichelFound && !grosMichelBroke)
                {
                    Console.WriteLine("Your Gros Michel didn't go extinct.");
                }
                
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
                
                new Blind("Small Blind (Ante 3)",  2500, 3),
                new Blind("Big Blind  (Ante 3)", 5000, 5),
                new Blind("Boss Blind   (Ante 3)", 10000, 5),
                
                new Blind("Small Blind (Ante 4)", 10000, 3),
                new Blind("Big Blind (Ante 4)", 15000, 5),
                new Blind("Boss Blind (Ante 4)", 25000, 5),
                
                new Blind("Small Blind (Ante 5)", 25000, 3),
                new Blind("Big Blind (Ante 5)", 37500, 5),
                new Blind("Boss Blind (Ante 5)", 56000, 5),
                
                new Blind("Small Blind (Ante 6)", 50000, 3),
                new Blind("Big Blind (Ante 6)", 75000, 5),
                new Blind("Boss Blind (Ante 6)", 110000, 5),
                
                new Blind("Small Blind (Ante 7)", 100000, 3),
                new Blind("Big Blind (Ante 7)", 115000, 5),
                new Blind("Boss Blind (Ante 7)", 150000, 5)
                
            };
        }

        
        //Protects the code
        private Blind GetNextBlind()
        {
            if (_blindSequence == null || _blindSequence.Count == 0)
            {
                Console.WriteLine("Blind not initialized!");
                Environment.Exit(0);
            }

            _blindNumber++;

            //If all blinds have been defeated (Up to ante 8)
            if (_blindNumber >= _blindSequence.Count)
            {
                Console.WriteLine("🎉 You beat all Blinds! You win!");
                Environment.Exit(0);
            }
            
            return _blindSequence[_blindNumber];
        }


        //Shop system
        private void OpenShop()
        {

            Shop shop = new Shop(this);

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("=== SHOP ===");
                Console.WriteLine($"Money: {_money}");
                Console.WriteLine();


                for (int i = 0; i < shop.Items.Count; i++)
                {
                    Console.WriteLine($"{i}. {shop.Items[i].Name} [{shop.Items[i].Tier}] — ${shop.Items[i].Price}");
                }

                Console.WriteLine();
                Console.WriteLine($"R. Reroll (${shop.RerollCost})");
                Console.WriteLine("X. View consumables");
                Console.WriteLine("J. View jokers");
                Console.WriteLine("F. Enter the next blind");

                string input = Console.ReadLine()?.Trim();

                if (input.Equals("F", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Leaving shop...");
                    return;
                }
                if (input.Equals("J", StringComparison.OrdinalIgnoreCase))
                {
                    OpenJokerMenu();
                    continue;
                }
                if (input.Equals("X", StringComparison.OrdinalIgnoreCase))
                {
                    OpenConsumableMenu();
                    continue;
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
                    
                    if (item.Consumable != null)
                    {
                        if (_consumables.AddConsumable(item.Consumable))
                            Console.WriteLine($"Added {item.Name} to consumable slots!");
                        else
                            Console.WriteLine("No space in consumable slots!");
                    }
                    else
                    {
                        AddJokerFromShop(item.Name);
                    }

                    shop.Items.RemoveAt(index);
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
                Console.WriteLine("S - Sell Jokers");
                Console.WriteLine("F - Finish");

                string input = Console.ReadLine()?.Trim();

                if (input.Equals("F", StringComparison.OrdinalIgnoreCase))
                    return;
                
                if (input.Equals("S", StringComparison.OrdinalIgnoreCase))
                {
                    SellJokerMenu();
                    continue;
                }

                if (input.Equals("O", StringComparison.OrdinalIgnoreCase))
                {
                    ReorderJokers();
                    continue;
                }

                Console.WriteLine("Invalid option.");
            }
        }
        public bool SellJoker(int index)
        {
            if (index < 0 || index >= _jokers.Count)
            {
                Console.WriteLine("Invalid joker index.");
                return false;
            }

            Joker joker = _jokers[index];
            int value = joker.SellValue;

            _money += value;
            _jokers.RemoveAt(index);

            Console.WriteLine($"Sold {joker.Name} for ${value}.");
            return true;
        }
        public void SellJokerMenu()
        {
            if (_jokers.Count == 0)
            {
                Console.WriteLine("You have no jokers to sell.");
                return;
            }

            Console.WriteLine("Your Jokers:");
            for (int i = 0; i < _jokers.Count; i++)
            {
                Console.WriteLine($"{i}: {_jokers[i].Name} (Sell: ${_jokers[i].SellValue})");
            }

            Console.WriteLine("Enter the index of the joker you want to sell:");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int index))
            {
                SellJoker(index);
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }
        private void OpenConsumableMenu()
        {
            while (true)
            {
                Console.Clear();
                _consumables.PrintSlots();
                Console.WriteLine();
                Console.WriteLine("Press slot number to use it, or F to exit.");

                string input = Console.ReadLine()?.Trim().ToUpper();

                if (input == "F")
                    return;

                if (int.TryParse(input, out int slot))
                {
                    if (_consumables.UseConsumable(slot, this))
                    {
                        Console.WriteLine("Consumable used!");
                        Console.WriteLine("Press Enter to continue.");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Invalid slot or empty slot.");
                        Console.WriteLine("Press Enter to continue.");
                        Console.ReadLine();
                    }
                }
            }
        }
        private void ReorderJokers()
        {
            Console.WriteLine("Enter two numbers to swap (e.g. 0 4):");

            string line = Console.ReadLine()?.Trim();

            var indices = PraseToIndexes(line);


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
                public bool PlayerAlreadyOwns(string itemName)
        {
            if (_jokers.Any(j => j.Name == itemName))
                return true;
            
            if (_consumables.Slots.Any(c => c != null && c.Name == itemName))
                return true;

            return false;
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
                    _jokers.Add(new Joker.GrosMichel());
                    Console.WriteLine("Added Gros Michel!");
                    break;
                
                case "Odd Todd":
                    _jokers.Add(new Joker.OddTodd());
                    Console.WriteLine("Added Odd Todd!");
                    break;
                
                case "Even Steven":
                    _jokers.Add(new Joker.EvenSteven());
                    Console.WriteLine("Added Even Steven!");
                    break;
                

                case "Mad Joker":
                    _jokers.Add(new Joker.MadJoker());
                    Console.WriteLine("Added Mad Joker!");
                    break;
                
                case "Fibonacci":
                    _jokers.Add(new Joker.Fibonacci());
                    Console.WriteLine("Added Fibonacci!");
                    break;
                
                case "Pi Man":
                    _jokers.Add(new Joker.PiMan());
                    Console.WriteLine("Added Pi Man!");
                    break;
                
                case "Mystic Summit":
                    _jokers.Add(new Joker.MysticSummit());
                    Console.WriteLine("Added Mystic Summit!");
                    break;
                
                case "Crazy Joker":
                    _jokers.Add(new Joker.CrazyJoker());
                    Console.WriteLine("Added Crazy Joker!");
                    break;
                
                case "Misprint":
                    _jokers.Add(new Joker.Misprint());
                    Console.WriteLine("Added Misprint!");
                    break;
                

                case "Mask":
                    _jokers.Add(new Joker.Mask());
                    Console.WriteLine("Added Mask!");
                    break;
                
                case "Zany Joker":
                    _jokers.Add(new Joker.ZanyJoker());
                    Console.WriteLine("Added Zanny Joker!");
                    break;

                case "Jolly Joker":
                    _jokers.Add(new Joker.JollyJoker());
                    Console.WriteLine("Added Jolly Joker!");
                    break;

                default:
                    Console.WriteLine("Item purchased, but it is not a Joker.");
                    break;
            }
            Console.WriteLine($"You now have {_jokers.Count} jokers.");
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

    string handName = HandEvaluator.GetHandNameFlexible(chosenCards);
    var (handChips, handMult) = HandEvaluator.GetHandValueFlexible(chosenCards);

 
    HandRank rank = HandRank.HighCard;
    HandRank level = 0;
    int levelChips = 0;
    int levelMult = 0;

    if (chosenCards.Count == 5)
    {
        rank = HandEvaluator.EvaluateFive(chosenCards).Rank; ;

        handChips += levelChips;
        handMult += levelMult;
    }

    foreach (var joker in _jokers)
        joker.ApplyEffect(chosenCards, _hand.Cards);

    for (int i = 0; i < chosenCards.Count; i++)
        chosenCards[i].PrintColored(parsed[i]);

    var chipsEval = Scorer.ChipsForPlayedCardsWithBonus(chosenCards);

    int discardsUsed = MaxDiscards - _remainingDiscards;
    int jokerBonusChips = 0;
    int jokerBonusMult = 0;

    foreach (var joker in _jokers)
    {
        jokerBonusChips += joker.GetBonusChips(chosenCards, discardsUsed);
        jokerBonusMult += joker.GetBonusMult(chosenCards, discardsUsed);
    }

    double chipMultiplier = 1.0;
    foreach (var joker in _jokers)
        chipMultiplier *= joker.GetBonusChipMultiplier(chosenCards, discardsUsed);

    double finalChipsDouble =
        (chipsEval.TotalBeforeMultiplier + jokerBonusChips + handChips)
        * chipMultiplier;

    int finalChips = (int)finalChipsDouble;
    int finalMult = chipsEval.Multiplier + jokerBonusMult + handMult;

    int finalScore = finalChips * finalMult;

    int displayMult = chipsEval.Multiplier + handMult;

    Console.WriteLine();
    Console.WriteLine("=== Hand Breakdown ===");
    Console.WriteLine();
    Console.WriteLine($"Hand Value: {handChips} Chips × {displayMult} Mult");
    Console.WriteLine($"Hand: {handName}");
    Console.WriteLine($"Chips: {finalChips}");
    Console.WriteLine($"Mult: {finalMult}");
    Console.WriteLine($"Final Score: {finalScore}");
    Console.WriteLine("======================");

    Console.WriteLine();
    Console.WriteLine("Confirm play? Press 'Y' to confirm, or 'F' to cancel.");

    string confirm = Console.ReadLine()?.Trim();


    if (confirm.Equals("Y", StringComparison.OrdinalIgnoreCase))
    {
        _totalScore += finalScore;
        return true;
    }

    foreach (var card in chosenCards)
        card.Enchantments.Clear();

    return false;
}
private List<int>? PraseToIndexes(string input)
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
        private Dictionary<HandRank, int> _handLevels = new()
        {
            { HandRank.HighCard, 1 },
            { HandRank.Pair, 1 },
            { HandRank.TwoPair, 1 },
            { HandRank.ThreeOfKind, 1 },
            { HandRank.Straight, 1 },
            { HandRank.Flush, 1 },
            { HandRank.FullHouse, 1 },
            { HandRank.FourOfKind, 1 },
            { HandRank.StraightFlush, 1 }
        };
    }
}