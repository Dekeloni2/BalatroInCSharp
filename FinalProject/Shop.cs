using System;
using System.Collections.Generic;
using System.Linq;
/*After beating a blind, this shop appears.
 This shop is fully functional with no duplicates,
 rerolls, selling and buying cards, card packs and
 joker appearing by rarity.*/
namespace BalatroGame
{
    public class ShopItem
    {
        public string Name { get; }
        public int Price { get; }
        public string Tier { get; set; }

        public ShopItem(string name, int price)
        {
            Name = name;
            Price = price;
        }
    }

    public class Shop
    {
        public List<ShopItem> Items { get; private set; }
        public int RerollCost { get; private set; } = 2;

        private List<Joker> _ownedJokers;
        private Random _rng = new Random();

        public Shop(List<Joker> ownedJokers)
        {
            _ownedJokers = ownedJokers;
            GenerateNewItems();
        }

        public void Reroll()
        {
            GenerateNewItems();
        }

        public void GenerateNewItems()
        {
            var allJokers = new List<ShopItem>
            {
                new ShopItem("Gros Michel", 5),
                new ShopItem("Misprint", 3),
                new ShopItem("Photograph", 4),
                new ShopItem("Hanging Chad", 4),
                new ShopItem("Mask", 3),
                new ShopItem("Zany Joker", 4),
                new ShopItem("Mad Joker", 4),
                new ShopItem("Crazy Joker", 4),
                new ShopItem("Jolly Joker", 3),
            };
            
            var filtered = allJokers
                .Where(j => !_ownedJokers.Any(o => o.Name == j.Name))
                .ToList();

            Items = new List<ShopItem>();
            
            if (filtered.Count == 0)
                return;
            
            for (int i = 0; i < 3 && filtered.Count > 0; i++)
            {
                int index = _rng.Next(filtered.Count);
                Items.Add(filtered[index]);
                filtered.RemoveAt(index);
            }
        }
        private JokerTier RollTier()
        {
            int roll = _rng.Next(100); // 0–99

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
                JokerTier.Common => new List<Joker> { new Joker.GrosMichel(), new Joker.Misprint(), new Joker.JollyJoker()},
                JokerTier.Uncommon => new List<Joker> {new Joker.Mask()},
                _ => new List<Joker>()
            };

            return pool[_rng.Next(pool.Count)];
        }

        private ConsoleColor TierColor(JokerTier tier)
        {
            return tier switch
            {
                JokerTier.Common => ConsoleColor.White,
                JokerTier.Uncommon => ConsoleColor.Cyan,
                JokerTier.Rare => ConsoleColor.Magenta,
                _ => ConsoleColor.Gray
            };
        }
    }
}