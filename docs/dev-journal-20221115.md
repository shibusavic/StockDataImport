# Development Journal

## 2022-11-15

### Progress

Taking inventory.

Logging is solid; it's not fully tested yet, but all the pieces are there, including preservation of "import action" logs.

The structure of CLI is kind of flushed out, except for the part where it actually imports something. :)
The `DataImportService` is in place, but doesn't do anything yet.
Been preoccupied with the mechanics of the `ImportConfiguration` class.

Parsing the YAML file is a little heavy:

```
    private static int GetMaxTokenUsage(dynamic config)
    {
        int result = 100_000;

        if (HasProperty(config, Constants.ConfigurationKeys.MaxTokenUsage))
        {
            var val = ((IDictionary<string, object>)config)[Constants.ConfigurationKeys.MaxTokenUsage].ToString();
            if (!int.TryParse(val, out result))
            {
                throw new ArgumentException($"{Constants.ConfigurationKeys.MaxTokenUsage} has an invalid value: '{val}'");
            }
        }

        return result;
    }
```

Built an `ApiService` to house the URLs for the API and also to keep track of their costs.
The service calls the `user` endpoint, which provides some information about the account, including current calls and call limit for the current day.
It's worthy of note that the API cycle begins at 1 AM UTC (0:00:00 UTC + 1).
The ApiService has static properties that house this information and the `DataClient` has a function to do the work.

```
    public async Task<(int Requests, int Limit)> ResetUsageAsync(int limit = 99_000)
    {
        var results = await GetUsageAsync();
        ApiService.Usage = results.Requests;
        ApiService.DailyLimit = Math.Min(results.Limit, limit);
        return (ApiService.Usage, ApiService.DailyLimit);
    }

    public async Task<(int Requests, int Limit)> GetUsageAsync()
    {
        string json = await GetStringResponseAsync(BuildUserUri(), nameof(GetUsageAsync));

        if (string.IsNullOrWhiteSpace(json)) { return (0, 0); }

        var user = JsonSerializer.Deserialize<User>(json, SerializerOptions);

        return (user.ApiRequests, user.DailyRateLimit);
    }
```

Calling the `user` endpoint on the API has zero cost, but we don't want to abuse that, so there are some functions on `ApiService` to facilitate real-time tracking and also planning (thinking ahead about "Filler" actions).

```
    public static int Usage { get; internal set; }

    public static int DailyLimit { get; internal set; }

    public static void AddCall(string uri)
    {
        Usage += FindEndPointForUri(uri).Cost.GetValueOrDefault();
    }

    public static int GetCost(string uri, int factor = 1) => FindEndPointForUri(uri).Cost.GetValueOrDefault() * factor;

    private static EndPoint FindEndPointForUri(string uri)
    {
        foreach (var endpoint in EndPoints)
        {
            if (!string.IsNullOrWhiteSpace(endpoint.Uri) &&
                uri.StartsWith(endpoint.Uri, StringComparison.InvariantCultureIgnoreCase))
            {
                return endpoint;
            }
        }
        return EndPoint.Empty;
    }
```

### Next Step(s)

1. Need to finish the configuration, or at least get to the collection of action items.
    1. Build the collection.
    1. Remove "dupe effort" (Full vs Bulk)
    1. Sort by priority (Full, Filler)
1. Grab the unfinished actions items from the database and insert them into the collection.
    1. Replace existing items as needed.
1. Execute the action items until usage is hit or work is done.
    