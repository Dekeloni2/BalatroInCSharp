/*This class is for both planets, tarots and spectral cards.
 This also contains the functions for the consumable slots
 that the player can access.*/
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

using FinalProject;

public interface IConsumable
{
    string Name { get; }
    void Use(GameController game);
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
        
        game.NotifyPlanetUsed();

        
        _slots[index] = null;
        return true;
    }
}