using Import.AppServices.Configuration;
using Import.Infrastructure.Configuration;
using Xunit.Abstractions;
using YamlDotNet.Serialization;

namespace Import.AppServices.UnitTests;

public class ImportConfigurationTests
{
    private readonly ITestOutputHelper testOutputHelper;

    public ImportConfigurationTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

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
        Assert.NotNull(config.Options);

        Assert.Null(config.Options.ApiKey);
        Assert.Null(config.Purges);
        Assert.Null(config.Options.DataRetention);
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
    public void ApiKey_Valid_NotNull()
    {
        string expected = "123.456";

        var text = $@"
{Constants.ConfigurationKeys.Options}:
  {Constants.ConfigurationKeys.ApiKey}: {expected}
...
";

        var config = ImportConfiguration.Create(text);
        Assert.Equal(expected, config.Options.ApiKey);
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
{Constants.ConfigurationKeys.Options}:
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
        Assert.NotNull(config.Options.DataRetention);
        Assert.Equal("1 year", config.Options.DataRetention["Critical"]);
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
            Options = new ConfigurationOptions()
            {
                ApiKey = "TEST",
                DataRetention = new Dictionary<string, string>()
                {
                    { "Critical", "1 year" },
                    { "Debug", "2 weeks" },
                    { "Error", "1 year" },
                    { "Information", "3 months" },
                    { "Trace", "1 week" },
                    { "Warning", "6 months" }
                },
                ReasonsToIgnore = new string[] {
                   "No Fundamentals"
                },
            },

            Purges = new string[] {
                Constants.PurgeName.Logs,
                Constants.PurgeName.Imports
            },
            Exchanges = new Dictionary<string, IDictionary<string, string[]>>()
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
                    DataTypes = new[] { "Symbols", "Prices", "Options", "Dividends", "Splits" },
                    Mode = Constants.Modes.Discovery
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

        testOutputHelper.WriteLine(yaml);

        var deserializer = new DeserializerBuilder().Build();

        var otherConfig = deserializer.Deserialize<ImportConfiguration>(yaml);

        Assert.NotNull(otherConfig);
    }
}
