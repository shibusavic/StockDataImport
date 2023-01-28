using System.Text.RegularExpressions;

namespace Import.Infrastructure.Domain
{
    public class Split
    {
        /// <summary>
        /// Creates a new instance of <see cref="Split"/> from the
        /// <see cref="EodHistoricalData.Sdk.Models.Split"/> value.
        /// </summary>
        /// <param name="symbol">The symbol to which the split is applied.</param>
        /// <param name="exchange">The exchange for the symbol.</param>
        /// <param name="split">The eodhistoricaldata.com model for splits.</param>
        /// <exception cref="ArgumentNullException">Thrown if symbol is null or empty.</exception>
        public Split(string symbol, string exchange, EodHistoricalData.Sdk.Models.Split split)
        {
            Symbol = string.IsNullOrWhiteSpace(symbol) ? throw new ArgumentNullException(nameof(symbol)) : symbol;
            Exchange = string.IsNullOrWhiteSpace(exchange) ? throw new ArgumentNullException(nameof(exchange)) : exchange.Trim();

            DateOfSplit = new DateOnly(split.Date.GetValueOrDefault().Year,
                split.Date.GetValueOrDefault().Month,
                split.Date.GetValueOrDefault().Day);

            (double BeforeSplit, double AfterSplit) = ParseSplit(split.SplitText);
            SharesBeforeSplit = BeforeSplit;
            SharesAfterSplit = AfterSplit;
            Ratio = $"{SharesBeforeSplit:#,##0.000}:{SharesAfterSplit:#,##0.000}";
        }

        public Split(string symbol, string exchange, DateOnly dateOfSplit, string ratio)
        {
            Symbol = string.IsNullOrWhiteSpace(symbol) ? throw new ArgumentNullException(nameof(symbol)) : symbol.Trim();
            Exchange = string.IsNullOrWhiteSpace(exchange) ? throw new ArgumentNullException(nameof(exchange)) : exchange.Trim();
            DateOfSplit = dateOfSplit;
            (double BeforeSplit, double AfterSplit) = ParseSplit(ratio);
            SharesBeforeSplit = BeforeSplit;
            SharesAfterSplit = AfterSplit;
            Ratio = $"{SharesBeforeSplit:#,##0.000}:{SharesAfterSplit:#,##0.000}";
        }

        public Split(string symbol,string exchange, DateOnly dateOfSplit, double beforeSplit, double afterSplit)
        {
            Symbol = string.IsNullOrWhiteSpace(symbol) ? throw new ArgumentNullException(nameof(symbol)) : symbol.Trim();
            Exchange = string.IsNullOrWhiteSpace(exchange) ? throw new ArgumentNullException(nameof(exchange)) : exchange.Trim();

            DateOfSplit = dateOfSplit;
            if (beforeSplit <= 0) { throw new ArgumentException($"{nameof(beforeSplit)} must be a positive number."); }
            if (afterSplit <= 0) { throw new ArgumentException($"{nameof(afterSplit)} must be a positive number."); }
            
            SharesBeforeSplit = beforeSplit;
            SharesAfterSplit = afterSplit;
            
            Ratio = $"{SharesBeforeSplit:#,##0.000}:{SharesAfterSplit:#,##0.000}";
        }

        /// <summary>
        /// Gets the ticker symbol of the security.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Gets the exchange for this symbol.
        /// </summary>
        public string Exchange { get; }

        /// <summary>
        /// Gets the date of the split.
        /// </summary>
        public DateOnly DateOfSplit { get; }

        /// <summary>
        /// Gets the ratio of the split, represented as 'n:d' or 'n/d', where n (the numerator) is the pre-split
        /// number of shares and d (the denominator) is the post-split number of shares.
        /// </summary>
        public string Ratio { get; }

        /// <summary>
        /// Gets the numer of shares before the split.
        /// </summary>
        public double SharesBeforeSplit { get; }

        /// <summary>
        /// Gets the number of shares after the split.
        /// </summary>
        public double SharesAfterSplit { get; }

        /// <summary>
        /// Gets the factor to be applied to price because of this split.
        /// </summary>
        public double PriceAdjustmentFactor => SharesBeforeSplit == 0 ? 1
            : SharesAfterSplit / SharesBeforeSplit;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => $"{Symbol} {Ratio} on {DateOfSplit:yyyy-MM-dd}";

        private static (double BeforeSplit, double AfterSplit) ParseSplit(string? ratio)
        {
            if (ratio == null) return (0D, 0D);

            string delimiter = ratio.Contains(':') ? ":"
                : ratio.Contains('/') ? "/"
                : throw new ArgumentException($"Invalid split ratio: {ratio}");

            string pattern = $@"([\d\.]+){delimiter}([\d\.]+)";

            Regex regex = new(pattern, RegexOptions.Singleline);

            var matches = regex.Matches(ratio);

            if (matches.Any())
            {
                var before = Convert.ToDouble(matches[0].Groups[1].Value);
                var after = Convert.ToDouble(matches[0].Groups[2].Value);
                return (before, after);
            }

            throw new ArgumentException($"Invalid split ratio: {ratio}");
        }
    }
}
