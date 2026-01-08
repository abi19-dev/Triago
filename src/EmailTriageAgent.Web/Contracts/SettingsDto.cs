namespace EmailTriageAgent.Web.Contracts;

public sealed class SettingsDto
{
    public double ReviewThreshold { get; set; }
    public double BlockThreshold { get; set; }
    public bool RetrainEnabled { get; set; }
    public int GoldThreshold { get; set; }
    public int NewGoldSinceLastTrain { get; set; }
    public string SpamKeywordsCsv { get; set; } = string.Empty;
}
