using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_esg_scores", Schema = "public")]
internal class CompanyEsgScore
{
    public CompanyEsgScore(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.EsgScores esgScores)
    {
        CompanyId = companyId;
        DateCaptured = DateTime.UtcNow;
        RatingDate = esgScores.RatingDate.GetValueOrDefault().ToDateTime(TimeOnly.MinValue);
        TotalEsg = esgScores.TotalEsg.GetValueOrDefault();
        TotalEsgPercentile = esgScores.TotalEsgPercentile.GetValueOrDefault();
        EnvironmentScore = esgScores.EnvironmentScore.GetValueOrDefault();
        EnvironmentScorePercentile = esgScores.EnvironmentScorePercentile.GetValueOrDefault();
        SocialScore = esgScores.SocialScore.GetValueOrDefault();
        SocialScorePercentile = esgScores.SocialScorePercentile.GetValueOrDefault();
        GovernanceScore = esgScores.GovernanceScore.GetValueOrDefault();
        GovernanceScorePercentile = esgScores.GovernanceScorePercentile.GetValueOrDefault();
        ControversyLevel = esgScores.ControversyLevel.GetValueOrDefault();
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyEsgScore(
        Guid companyId,
        DateTime dateCaptured,
        DateTime ratingDate,
        double totalEsg,
        double totalEsgPercentile,
        double environmentScore,
        double environmentScorePercentile,
        double socialScore,
        double socialScorePercentile,
        double governanceScore,
        double governanceScorePercentile,
        double controversyLevel,
        DateTime utcTimestamp)
    {
        CompanyId = companyId;
        DateCaptured = dateCaptured;
        RatingDate = ratingDate;
        TotalEsg = totalEsg;
        TotalEsgPercentile = totalEsgPercentile;
        EnvironmentScore = environmentScore;
        EnvironmentScorePercentile = environmentScorePercentile;
        SocialScore = socialScore;
        SocialScorePercentile = socialScorePercentile;
        GovernanceScore = governanceScore;
        GovernanceScorePercentile = governanceScorePercentile;
        ControversyLevel = controversyLevel;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = false)]
    public DateTime DateCaptured { get; }

    [ColumnWithKey("rating_date", Order = 3, TypeName = "date", IsPartOfKey = true)]
    public DateTime RatingDate { get; }

    [ColumnWithKey("total_esg", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
    public double TotalEsg { get; }

    [ColumnWithKey("total_esg_percentile", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
    public double TotalEsgPercentile { get; }

    [ColumnWithKey("environment_score", Order = 6, TypeName = "double precision", IsPartOfKey = false)]
    public double EnvironmentScore { get; }

    [ColumnWithKey("environment_score_percentile", Order = 7, TypeName = "double precision", IsPartOfKey = false)]
    public double EnvironmentScorePercentile { get; }

    [ColumnWithKey("social_score", Order = 8, TypeName = "double precision", IsPartOfKey = false)]
    public double SocialScore { get; }

    [ColumnWithKey("social_score_percentile", Order = 9, TypeName = "double precision", IsPartOfKey = false)]
    public double SocialScorePercentile { get; }

    [ColumnWithKey("governance_score", Order = 10, TypeName = "double precision", IsPartOfKey = false)]
    public double GovernanceScore { get; }

    [ColumnWithKey("governance_score_percentile", Order = 11, TypeName = "double precision", IsPartOfKey = false)]
    public double GovernanceScorePercentile { get; }

    [ColumnWithKey("controversy_level", Order = 12, TypeName = "double precision", IsPartOfKey = false)]
    public double ControversyLevel { get; }

    [ColumnWithKey("utc_timestamp", Order = 13, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
