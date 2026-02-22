using System;
using System.Collections.Generic;
using System.Linq;
using FinalProject;

namespace BalatroGame
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
        public int RerollCost { get; private set; } = 2;

        private GameController _game; 
        private Random _rng = new Random();

        public Shop(GameController game)
        {
            _game = game;
            GenerateNewItems();
        }

        public void Reroll()
        {
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

        // ⭐ מונע כפילויות בחנות ובאינבנטורי של השחקן
        public void GenerateNewItems()
        {
            Items = new List<ShopItem>();

            while (Items.Count < 3)
            {
                ShopItem item;

                // 50% ג'וקר, 50% פלנטה
                if (_rng.Next(2) == 0)
                    item = GenerateJokerItem();
                else
                    item = GeneratePlanetItem();

                // 1. לא להציג פריט שכבר יש לשחקן
                if (_game.PlayerAlreadyOwns(item.Name))
                    continue;

                // 2. לא להציג פריט שכבר בחנות
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
                    new Joker.JollyJoker()
                },

                JokerTier.Uncommon => new List<Joker>
                {
                    new Joker.Mask(),
                    new Joker.MadJoker(),
                    new Joker.CrazyJoker(),
                    new Joker.ZanyJoker()
                },

                JokerTier.Rare => new List<Joker>
                {
                    new Joker.PiMan(),   // אם תרצה להוסיף Rare
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