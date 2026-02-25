namespace FinalProject
{
    public class ShopItem(string name, int price)
    {
        public string Name { get; } = name;
        public int Price { get; } = price;
        public string? Tier { get; set; }

        public IConsumable? Consumable { get; set; }
    }

    public class Shop
    {
        public List<ShopItem> Items { get; private set; } = new();
        public int RerollCost { get; private set; } = 5;

        private GameController _game; 
        private Random _rng = new Random();

        public Shop(GameController game)
        {
            _game = game;
            GenerateNewItems();
        }

        public void Reroll()
        {
            Console.Clear();
            if (_game.Money < RerollCost)
            {
                Console.WriteLine("Not enough money to reroll!");
                Console.ReadLine();
                return;
            }

            _game.Money -= RerollCost;
            RerollCost++;

            GenerateNewItems();
        }
        
        private ShopItem GenerateJokerItem()
        {
            Joker j = GenerateRandomJoker();

            return new ShopItem(j.Name, j.Price)
            {
                Tier = j.Tier.ToString(),
                Consumable = null
            };
        }
        // ⭐ פונקציה שמייצרת Planet Card כ‑ShopItem
        private ShopItem GeneratePlanetItem()
        {
            var planets = new List<PlanetCard>
            {
                new PlanetCard(HandRank.Pair),
                new PlanetCard(HandRank.TwoPair),
                new PlanetCard(HandRank.ThreeOfKind),
                new PlanetCard(HandRank.Straight),
                new PlanetCard(HandRank.Flush),
                new PlanetCard(HandRank.FullHouse),
                new PlanetCard(HandRank.FourOfKind),
                new PlanetCard(HandRank.StraightFlush)
            };

            var chosen = planets[_rng.Next(planets.Count)];

            return new ShopItem(chosen.Name, 4)
            {
                Tier = "Planet",
                Consumable = chosen
            };
        }
        public void GenerateNewItems()
        {
            Items = new List<ShopItem>();

            while (Items.Count < 3)
            {
                ShopItem item = null!;

                int roll = _rng.Next(3);

                if (roll == 0)
                    item = GenerateJokerItem();
                else if (roll == 1)
                    item = GeneratePlanetItem();
                else
                continue;

                if (_game.PlayerAlreadyOwns(item.Name))
                    continue;

                if (Items.Any(i => i.Name == item.Name))
                    continue;

                Items.Add(item);
            }
        }

        private JokerTier RollTier()
        {
            int roll = _rng.Next(100);

            if (roll < 70)
                return JokerTier.Common;

            if (roll < 95)
                return JokerTier.Uncommon;

            return JokerTier.Rare;
        }

        private Joker GenerateRandomJoker()
        {
            JokerTier tier = RollTier();

            List<Joker> pool = tier switch
            {
                JokerTier.Common => new List<Joker>
                {
                    new Joker.GrosMichel(),
                    new Joker.Misprint(),
                    new Joker.JollyJoker(),
                    new Joker.MadJoker(),
                    new Joker.CrazyJoker(),
                    new Joker.ZanyJoker()
                },

                JokerTier.Uncommon => new List<Joker>
                {
                    new Joker.Mask(),
                    new Joker.Fibonacci()
                },

                JokerTier.Rare => new List<Joker>
                {
                    new Joker.PiMan(),
                },

                _ => new List<Joker>()
            };

            return pool[_rng.Next(pool.Count)];
        }

        private ConsoleColor TierColor(JokerTier tier)
        {
            return tier switch
            {
                JokerTier.Common => ConsoleColor.White,
                JokerTier.Uncommon => ConsoleColor.Green,
                JokerTier.Rare => ConsoleColor.Red,
                _ => ConsoleColor.Gray
            };
        }
    }
}