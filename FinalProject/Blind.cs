public partial class Blind
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