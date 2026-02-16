using System;
using System.Collections.Generic;
using System.Linq;

namespace BalatroGame
{
    public class ShopItem
    {
        public string Name { get; }
        public int Price { get; }

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
            // כל הג'וקרים האפשריים
            var allJokers = new List<ShopItem>
            {
                new ShopItem("Gros Michel", 5),
                new ShopItem("Misprint", 3),
                new ShopItem("Photograph", 4),
                new ShopItem("Hanging Chad", 4),
                new ShopItem("Ride the Bus", 3),
                new ShopItem("Abstract Joker", 4),
                new ShopItem("Jolly Joker", 3)
            };

            // סינון: רק ג'וקרים שאין לשחקן
            var filtered = allJokers
                .Where(j => !_ownedJokers.Any(o => o.Name == j.Name))
                .ToList();

            Items = new List<ShopItem>();

            // אם נגמרו ג'וקרים — החנות תהיה ריקה
            if (filtered.Count == 0)
                return;

            // בוחרים עד 3 מוצרים רנדומליים
            for (int i = 0; i < 3 && filtered.Count > 0; i++)
            {
                int index = _rng.Next(filtered.Count);
                Items.Add(filtered[index]);
                filtered.RemoveAt(index);
            }
        }
    }
}