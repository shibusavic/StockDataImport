using Import.AppServices.Configuration;
using Import.Infrastructure.Configuration;
using YamlDotNet.Serialization;

namespace Import.AppServices.UnitTests;

public class ImportConfigurationTests
{
    [Fact]
    public void EmptyString_Null()
    {
        Assert.Throws<ArgumentNullException>(() => ImportConfiguration.Create(string.Empty));
    }

    [Fact]
    public void PropertiesNotPresent_NullValues()
    {
        var text = $@"
...
";

        var config = ImportConfiguration.Create(text);
        Assert.NotNull(config);
        Assert.NotNull(config.Exception);

        Assert.Null(config.MaxTokenUsage);
        Assert.Null(config.ApiKey);
        Assert.Null(config.Purges);
        Assert.Null(config.DataRetention);
        Assert.Null(config.Sunday);
        Assert.Null(config.Monday);
        Assert.Null(config.Tuesday);
        Assert.Null(config.Wednesday);
        Assert.Null(config.Thursday);
        Assert.Null(config.Friday);
        Assert.Null(config.Saturday);
        Assert.Null(config.OnEmptyDatabase);
        Assert.Null(config.AnyDay);
    }

    [Fact]
    public void MaxTokenUsageAsync_Invalid_Null()
    {
        var text = $@"
{Constants.ConfigurationKeys.MaxTokenUsage}: badint
...
";

        var config = ImportConfiguration.Create(text);
        Assert.Null(config.MaxTokenUsage);
    }

    [Fact]
    public void MaxTokenUsage_Valid_NotNull()
    {
        var text = $@"
{Constants.ConfigurationKeys.MaxTokenUsage}: 99000
...
";

        var config = ImportConfiguration.Create(text);
        Assert.Equal(99_000, config.MaxTokenUsage);
    }

    [Fact]
    public void ApiKey_Valid_NotNull()
    {
        string expected = "123.456";

        var text = $@"
{Constants.ConfigurationKeys.MaxTokenUsage}: 99000
{Constants.ConfigurationKeys.ApiKey}: {expected}
...
";

        var config = ImportConfiguration.Create(text);
        Assert.Equal(expected, config.ApiKey);
    }

    [Fact]
    public void Purges_Valid_NotNull()
    {
        var text = $@"
{Constants.ConfigurationKeys.DatabasePurge}: 
  - Logs
  - Imports
...
";

        var config = ImportConfiguration.Create(text);
        Assert.NotNull(config.Purges);
        Assert.NotEmpty(config.Purges);
    }

    [Fact]
    public void LogRetention_Valid_NotNull()
    {
        var text = $@"
{Constants.ConfigurationKeys.DataRetention}:
  Critical: 1 year
  Debug: 2 weeks
  Error: 1 year
  Information: 3 months
  Trace: 1 week
  Warning: 6 months
...
";

        var config = ImportConfiguration.Create(text);
        Assert.NotNull(config.DataRetention);
        Assert.Equal("1 year", config.DataRetention["Critical"]);
    }

    [Fact]
    public void OnEmptyDatabase_Valid_NotNull()
    {
        var text = $@"
Exchange Codes:
  US:
    Exchanges:
    - NYSE
    - NASDAQ
    - AMEX
    Symbol Type:
    - Common Stock
    - ETF
{Constants.ConfigurationKeys.OnEmptyDb}:
- Priority: 1
  Scope: Full
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Data Types:
  - Fundamentals

...
";

        var config = ImportConfiguration.Create(text);
        Assert.NotNull(config.OnEmptyDatabase);
    }

    [Fact]
    public void AnyDay_Valid_NotNull()
    {
        var text = $@"
Exchange Codes:
  US:
    Exchanges:
    - NYSE
    - NASDAQ
    - AMEX
    Symbol Type:
    - Common Stock
    - ETF
{Constants.ConfigurationKeys.AnyDay}:
- Priority: 1
  Scope: Full
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Data Types:
  - Fundamentals
...
";

        var config = ImportConfiguration.Create(text);
        Assert.NotNull(config.AnyDay);
    }

    [Fact]
    public void Sunday_Valid_NotNull()
    {
        var text = $@"
Exchange Codes:
  US:
    Exchanges:
    - NYSE
    - NASDAQ
    - AMEX
    Symbol Type:
    - Common Stock
    - ETF
{Constants.ConfigurationKeys.Sunday}:
- Priority: 1
  Scope: Full
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Data Types:
  - Fundamentals

...
";

        var config = ImportConfiguration.Create(text);
        Assert.NotNull(config.Sunday);
    }

    [Fact]
    public void Monday_Valid_NotNull()
    {
        var text = $@"
Exchange Codes:
  US:
    Exchanges:
    - NYSE
    - NASDAQ
    - AMEX
    Symbol Type:
    - Common Stock
    - ETF
{Constants.ConfigurationKeys.Monday}:
- Priority: 1
  Scope: Full
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Data Types:
  - Fundamentals

...
";

        var config = ImportConfiguration.Create(text);
        Assert.NotNull(config.Monday);
    }

    [Fact]
    public void Tuesday_Valid_NotNull()
    {
        var text = $@"
Exchange Codes:
  US:
    Exchanges:
    - NYSE
    - NASDAQ
    - AMEX
    Symbol Type:
    - Common Stock
    - ETF
{Constants.ConfigurationKeys.Tuesday}:
- Priority: 1
  Scope: Full
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Data Types:
  - Fundamentals

...
";

        var config = ImportConfiguration.Create(text);
        Assert.NotNull(config.Tuesday);
    }

    [Fact]
    public void Wednesday_Valid_NotNull()
    {
        var text = $@"
Exchange Codes:
  US:
    Exchanges:
    - NYSE
    - NASDAQ
    - AMEX
    Symbol Type:
    - Common Stock
    - ETF
{Constants.ConfigurationKeys.Wednesday}:
- Priority: 1
  Scope: Full
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Data Types:
  - Fundamentals

...
";

        var config = ImportConfiguration.Create(text);
        Assert.NotNull(config.Wednesday);
    }

    [Fact]
    public void Thursday_Valid_NotNull()
    {
        var text = $@"
Exchange Codes:
  US:
    Exchanges:
    - NYSE
    - NASDAQ
    - AMEX
    Symbol Type:
    - Common Stock
    - ETF
{Constants.ConfigurationKeys.Thursday}:
- Priority: 1
  Scope: Full
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Data Types:
  - Fundamentals

...
";

        var config = ImportConfiguration.Create(text);
        Assert.NotNull(config.Thursday);
    }

    [Fact]
    public void Friday_Valid_NotNull()
    {
        var text = $@"
Exchange Codes:
  US:
    Exchanges:
    - NYSE
    - NASDAQ
    - AMEX
    Symbol Type:
    - Common Stock
    - ETF
{Constants.ConfigurationKeys.Friday}:
- Priority: 1
  Scope: Full
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Data Types:
  - Fundamentals

...
";

        var config = ImportConfiguration.Create(text);
        Assert.NotNull(config.Friday);
    }

    [Fact]
    public void Saturday_Valid_NotNull()
    {
        var text = $@"
Exchange Codes:
  US:
    Exchanges:
    - NYSE
    - NASDAQ
    - AMEX
    Symbol Type:
    - Common Stock
    - ETF
{Constants.ConfigurationKeys.Saturday}:
- Priority: 1
  Scope: Full
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Data Types:
  - Fundamentals

...
";

        var config = ImportConfiguration.Create(text);
        Assert.NotNull(config.Saturday);
    }

    [Fact]
    public void Serialization_Deserialization()
    {

        var config = new ImportConfiguration()
        {
            MaxTokenUsage = 990,
            ApiKey = "TEST",
            Purges = new string[] {
                Constants.PurgeName.Logs,
                Constants.PurgeName.ActionItems,
                Constants.PurgeName.Imports
            },
            DataRetention = new Dictionary<string, string>()
            {
                { "Critical", "1 year" },
                { "Debug", "2 weeks" },
                { "Error", "1 year" },
                { "Information", "3 months" },
                { "Trace", "1 week" },
                { "Warning", "6 months" }
            },
            Exchanges = new Dictionary<string, Dictionary<string, string[]>>()
            {
                { "US", new Dictionary<string, string[]>()
                    {
                        { "Exchanges", new string[] { "NYSE", "NASDAQ", "AMEX" } },
                        { "Symbol Type", new string[] { "Common Stock", "ETF" } }
                    }
               }
            },
            OnEmptyDatabase = new ImportActions[] {
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 1,
                    DataTypes = new[] { "Symbols", "Prices", "Options", "Dividends", "Splits" }
                },
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 2,
                    DataTypes = new[] { "Fundamentals" }
                    },
            },
            AnyDay = new ImportActions[] {
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Bulk,
                    Priority = 1,
                    DataTypes = new[] { "Symbols", "Prices", "Options", "Dividends", "Splits" }
                },
                new ImportActions()
{
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 2,
                    DataTypes = new[] { "Fundamentals" }
                }
            },
            Sunday = new ImportActions[] {
                new ImportActions() { Skip = true}
            },
            Monday = new ImportActions[] {
                new ImportActions() { Skip = true}
            },
            Tuesday = new ImportActions[] {
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Bulk,
                    Priority = 1,
                    DataTypes = new [] {"Symbols","Prices","Options","Dividends","Splits"}
                },
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 2,
                    DataTypes = new [] {"Fundamentals"}
                }
            },
            Wednesday = new ImportActions[] {
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Bulk,
                    Priority = 1,
                    DataTypes = new [] {"Symbols","Prices","Options","Dividends","Splits"}
                },
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 2,
                    DataTypes = new [] {"Fundamentals"}
                }
            },
            Thursday = new ImportActions[] {
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Bulk,
                    Priority = 1,
                    DataTypes = new [] {"Symbols","Prices","Options","Dividends","Splits"}
                },
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 2,
                    DataTypes = new [] {"Fundamentals"}
                }
            },
            Friday = new ImportActions[] {
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Bulk,
                    Priority = 1,
                    Skip = false,
                    DataTypes = new [] {"Symbols","Prices","Options","Dividends","Splits"}
                },
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 2,
                    DataTypes = new [] {"Fundamentals"}
                }
            },
            Saturday = new ImportActions[] {
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 1,
                    DataTypes = new [] {"Symbols","Prices","Options","Dividends","Splits"}
                },
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 2,
                    DataTypes = new [] {"Fundamentals"}
                }
            }
        };

        var serializer = new SerializerBuilder().Build();

        var yaml = serializer.Serialize(config);

        Assert.NotNull(yaml);

        var deserializer = new DeserializerBuilder().Build();

        var otherConfig = deserializer.Deserialize<ImportConfiguration>(yaml);

        Assert.NotNull(otherConfig);
    }
}
