namespace EodHistoricalData.Sdk.Models.Fundamentals.CommonStock;

/// <summary>
/// <seealso href="https://eodhistoricaldata.com/financial-apis/stock-etfs-fundamental-data-feeds/"/>
/// </summary>
public struct EsgScores
{
    public string? Disclaimer;
    public DateOnly? RatingDate;
    public double? TotalEsg;
    public double? TotalEsgPercentile;
    public double? EnvironmentScore;
    public int? EnvironmentScorePercentile;
    public double? SocialScore;
    public int? SocialScorePercentile;
    public double? GovernanceScore;
    public int? GovernanceScorePercentile;
    public int? ControversyLevel;
    public IDictionary<string, ActivityInvolvement>? ActivitiesInvolvement;
}
