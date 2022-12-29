# Development Journal

## 2022-11-08

### Logging

I prefer to tackle low-level foundational stuff like _logging_ early in a project.
User management, security, and logging are my typical starting points.
Since user management and security are not concerns here, _logging_ is the first concern to address.

I think it makes sense to tap into the .NET _ILogger_ and write my own provider.
I'm certain I have this code already - just need to port it over with a few tweaks.

After working through the requirements document, I asked myself if I needed to capture (log) every request and response?
No, I don't think I do.
We may need this information when we have an error, so maybe we need to preserve these values in memory until we've successfully transformed them into data access objects?
It is still unproven, but it's unlikely that the database schema is going to tie one-for-one with the eodhistoricaldata.com API models.
This is especially true when it comes to types of values.
We want a database that we can query with maths, so numbers are going to be stored as numbers, not strings.
Consequently, we're likely to have some conversion issues along the way; these will need to be worked out as they arise and having the exact request and response for any error would certainly not hurt.
The request/response capture would have to tie to the exception, and therefore to the log.
It's potentially a sizable amount of data - would hate to clog up a database with it.
Well, maybe if it was its own table . . .

I think there should be some sort of _watchdog_ component for cleaning up "old" logs and keeping the database size under control. 
Maybe this should be part of the configuration.

### Testing

Definitely going to have to be able to mock the _DataClient_; otherwise testing will chew up API call credits.

### Configuration

**The more I consider how this works, the more I think that I only want to build the _automatic_ mode and control the scope of its effort by configuration only.**

How could it work?
[YAML](https://yaml.org/) files?
JSON files?
It's too much to pass on the command line.
Hard-coding would defeat our goals around flexibility for other devs.
Could store configuration in a database, but then I'd have to build some sort of interface into it.

I certainly could do it with [JSON](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/JSON).
This would lend itself to easy binding between the `appsettings.{ENVIRONMENT}.json` files and C# configuration classes.

It's tempting to try this with [YAML](https://yaml.org/).
For one, I don't think I've ever coded against YAML, and since I'm not getting paid, I might as well expand my horizons and learn a few things.
For another, the YAML structure might be a lot easier for people to manage and change.

#### Ignoring Data

There are going to be some situations in which people want to ignore certain data.
For example, you might not want any price data for companies that have fewer than 50 trading days since their IPO.
No matter the reason, having a _symbol ignore list_ probably makes sense.
That means a way to add or remove symbols from that list.
Unfortunately, the list could get large -- too large to live comfortably in a configuration file.

