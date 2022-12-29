using EodHistoricalData.Sdk.Models;
using EodHistoricalData.Sdk.Models.Fundamentals.CommonStock;

namespace Import.Infrastructure.Abstractions;

internal partial interface IImportsDbContext 
{
    Task SaveCompanyAsync(FundamentalsCollection company, CancellationToken cancellationToken = default);
}
