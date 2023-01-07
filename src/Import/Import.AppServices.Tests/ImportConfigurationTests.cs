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
        Assert.Null(config.LogRetention);
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
{Constants.ConfigurationKeys.LogRetention}:
  Critical: 1 year
  Debug: 2 weeks
  Error: 1 year
  Information: 3 months
  Trace: 1 week
  Warning: 6 months
...
";

        var config = ImportConfiguration.Create(text);
        Assert.NotNull(config.LogRetention);
        Assert.Equal("1 year", config.LogRetention["Critical"]);
    }

    [Fact]
    public void OnEmptyDatabase_Valid_NotNull()
    {
        var text = $@"
{Constants.ConfigurationKeys.OnEmptyDb}:
- Priority: 1
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
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
{Constants.ConfigurationKeys.AnyDay}:
- Priority: 1
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
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
{Constants.ConfigurationKeys.Sunday}:
- Priority: 1
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
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
{Constants.ConfigurationKeys.Monday}:
- Priority: 1
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
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
{Constants.ConfigurationKeys.Tuesday}:
- Priority: 1
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
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
{Constants.ConfigurationKeys.Wednesday}:
- Priority: 1
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
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
{Constants.ConfigurationKeys.Thursday}:
- Priority: 1
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
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
{Constants.ConfigurationKeys.Friday}:
- Priority: 1
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
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
{Constants.ConfigurationKeys.Saturday}:
- Priority: 1
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
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
            LogRetention = new Dictionary<string, string>()
            {
                { "Critical", "1 year"},
                { "Debug", "2 weeks"},
                { "Error", "1 year"},
                { "Information", "3 months"},
                { "Trace", "1 week"},
                { "Warning", "6 months"}
            },
            OnEmptyDatabase = new ImportActions[] {
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 1,
                    Exchanges = new [] {"NYSE","NASDAQ","AMEX"},
                    DataTypes = new [] {"Symbols","Prices","Options","Dividends","Splits"}
                },
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 2,
                    Exchanges = new [] {"NYSE","NASDAQ","AMEX"},
                    DataTypes = new [] {"Fundamentals"}
                }},
            AnyDay = new ImportActions[] {
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Bulk,
                    Priority = 1,
                    Exchanges = new [] {"NYSE","NASDAQ","AMEX"},
                    DataTypes = new [] {"Symbols","Prices","Options","Dividends","Splits"}
                },
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 2,
                    Exchanges = new [] {"NYSE","NASDAQ","AMEX"},
                    DataTypes = new [] {"Fundamentals"}
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
                    Exchanges = new [] {"NYSE","NASDAQ","AMEX"},
                    DataTypes = new [] {"Symbols","Prices","Options","Dividends","Splits"}
                },
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 2,
                    Exchanges = new [] {"NYSE","NASDAQ","AMEX"},
                    DataTypes = new [] {"Fundamentals"}
                }
            },
            Wednesday = new ImportActions[] {
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Bulk,
                    Priority = 1,
                    Exchanges = new [] {"NYSE","NASDAQ","AMEX"},
                    DataTypes = new [] {"Symbols","Prices","Options","Dividends","Splits"}
                },
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 2,
                    Exchanges = new [] {"NYSE","NASDAQ","AMEX"},
                    DataTypes = new [] {"Fundamentals"}
                }
            },
            Thursday = new ImportActions[] {
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Bulk,
                    Priority = 1,
                    Exchanges = new [] {"NYSE","NASDAQ","AMEX"},
                    DataTypes = new [] {"Symbols","Prices","Options","Dividends","Splits"}
                },
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 2,
                    Exchanges = new [] {"NYSE","NASDAQ","AMEX"},
                    DataTypes = new [] {"Fundamentals"}
                }
            },
            Friday = new ImportActions[] {
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Bulk,
                    Priority = 1,
                    Exchanges = new [] {"NYSE","NASDAQ","AMEX"},
                    Skip = false,
                    DataTypes = new [] {"Symbols","Prices","Options","Dividends","Splits"}
                },
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 2,
                    Exchanges = new [] {"NYSE","NASDAQ","AMEX"},
                    DataTypes = new [] {"Fundamentals"}
                }
            },
            Saturday = new ImportActions[] {
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 1,
                    Exchanges = new [] {"NYSE","NASDAQ","AMEX"},
                    DataTypes = new [] {"Symbols","Prices","Options","Dividends","Splits"}
                },
                new ImportActions()
                {
                    Scope = Constants.DataTypeScopes.Full,
                    Priority = 2,
                    Exchanges = new [] {"NYSE","NASDAQ","AMEX"},
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
