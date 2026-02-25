/*This class is for both planets, tarots and spectral cards.
 This also contains the functions for the consumable slots
 that the player can access.*/
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

using FinalProject;

public interface IConsumable
{
    string Name { get; }
    void Use(GameController game);

    public class TarotCard
    {
        
    }
}

public enum HandRank
{
    HighCard, Pair, TwoPair, ThreeOfKind, Straight, Flush, FullHouse, FourOfKind, StraightFlush
}
public class PlanetCard : IConsumable
{
    public string Name { get; }
    public HandRank TargetHand { get; }

    public PlanetCard(HandRank target)
    {
        TargetHand = target;
        Name = $"{target} Planet";
    }

    public void Use(GameController game)
    {
        game.LevelUpHand(TargetHand);
        Console.WriteLine($"{Name} increases the level of {TargetHand}!");
    }
}

public class ConsumableSlots
{
    private readonly IConsumable[] _slots;
    public IConsumable[] Slots => _slots;

    public ConsumableSlots(int size = 3)
    {
        _slots = new IConsumable[size];
    }

    public bool AddConsumable(IConsumable item)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null)
            {
                _slots[i] = item;
                return true;
            }
        }

        return false; //If there is no space in the inventory
    }

    public void PrintSlots()
    {
        Console.WriteLine("=== Consumable Slots ===");
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null)
                Console.WriteLine($"{i}: [Empty]");
            else
                Console.WriteLine($"{i}: {_slots[i].Name}");
        }

        Console.WriteLine("========================");
    }

    public bool UseConsumable(int index, GameController game)
    {
        if (index < 0 || index >= _slots.Length)
            return false;

        if (_slots[index] == null)
            return false;

        _slots[index]!.Use(game);
        _slots[index] = null;
        return true;
    }
    public void LevelUpHand(HandRank rank)
    {
    }

    
    
    public abstract class TarotCard : IConsumable
    {
        public string Name { get; }
        public string Description { get; }

        protected TarotCard(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public abstract void Use(GameController game);
    }

    public static class HandLevelSystem
    {
        private static Dictionary<HandRank, int> _levels = new Dictionary<HandRank, int>();

        static HandLevelSystem()
        {
            foreach (HandRank rank in Enum.GetValues(typeof(HandRank)))
                _levels[rank] = 0;
        }

        public static int GetLevel(HandRank rank) => _levels[rank];

        public static void LevelUp(HandRank rank)
        {
            _levels[rank]++;
        }
    }

    public static class HandLevelValues
    {
        public static (int chips, int mult) GetBonusForLevel(HandRank rank, int level)
        {
            return rank switch
            {
                HandRank.Pair => (level * 10, level * 1),
                HandRank.TwoPair => (level * 12, level * 1),
                HandRank.ThreeOfKind => (level * 15, level * 1),
                HandRank.Straight => (level * 20, level * 1),
                HandRank.Flush => (level * 22, level * 1),
                HandRank.FullHouse => (level * 25, level * 2),
                HandRank.FourOfKind => (level * 30, level * 2),
                HandRank.StraightFlush => (level * 40, level * 3),
                _ => (0, 0)
            };
        }
    }
}