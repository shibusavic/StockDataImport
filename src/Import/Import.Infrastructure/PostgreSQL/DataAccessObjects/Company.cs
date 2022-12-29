using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "companies", Schema = "public")]
internal class Company
{
    public Company(EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.FundamentalsCollection company,
        Guid? companyId = null)
    {
        GlobalId = companyId ?? Guid.NewGuid();
        DateCaptured = DateTime.UtcNow;
        Symbol = company.General.Code;
        Exchange = company.General.Exchange;
        Name = company.General.Name;
        Type = company.General.Type;
        CurrencyCode = company.General.CurrencyCode;
        CurrencyName = company.General.CurrencyName;
        CurrencySymbol = company.General.CurrencySymbol;
        CountryName = company.General.CountryName;
        CountryIso = company.General.CountryIso;
        Isin = company.General.Isin;
        Lei = company.General.Lei;
        Cusip = company.General.Cusip;
        Cik = company.General.Cik;
        EmployerIdNumber = company.General.EmployerIdNumber;
        FiscalYearEnd = company.General.FiscalYearEnd;
        IpoDate = company.General.IPODate.ToDateTime(TimeOnly.MinValue);
        InternationalDomestic = company.General.InternationalDomestic;
        Sector = company.General.Sector;
        Industry = company.General.Industry;
        GicSector = company.General.GicSector;
        GicGroup = company.General.GicGroup;
        GicIndustry = company.General.GicIndustry;
        HomeCategory = company.General.HomeCategory;
        IsDelisted = company.General.IsDelisted.GetValueOrDefault();
        Description = company.General.Description;
        Phone = company.General.Phone;
        WebUrl = company.General.WebURL;
        LogoUrl = company.General.LogoURL;
        FullTimeEmployees = company.General.FullTimeEmployees ?? -1;
        UpdateAt = company.General.UpdatedAt.GetValueOrDefault().ToDateTime(TimeOnly.MinValue);
        UtcTimestamp = DateTime.UtcNow;
    }

    public Company(
        Guid globalId,
        DateTime dateCaptured,
        string symbol,
        string exchange,
        string name,
        string type,
        string currencyCode,
        string currencyName,
        string currencySymbol,
        string countryName,
        string countryIso,
        string isin,
        string lei,
        string cusip,
        string cik,
        string employerIdNumber,
        string fiscalYearEnd,
        DateTime ipoDate,
        string internationalDomestic,
        string sector,
        string industry,
        string gicSector,
        string gicGroup,
        string gicIndustry,
        string homeCategory,
        bool isDelisted,
        string description,
        string phone,
        string webUrl,
        string logoUrl,
        int fullTimeEmployees,
        DateTime updateAt,
        DateTime utcTimestamp)
    {
        GlobalId = globalId;
        DateCaptured = dateCaptured;
        Symbol = symbol;
        Exchange = exchange;
        Name = name;
        Type = type;
        CurrencyCode = currencyCode;
        CurrencyName = currencyName;
        CurrencySymbol = currencySymbol;
        CountryName = countryName;
        CountryIso = countryIso;
        Isin = isin;
        Lei = lei;
        Cusip = cusip;
        Cik = cik;
        EmployerIdNumber = employerIdNumber;
        FiscalYearEnd = fiscalYearEnd;
        IpoDate = ipoDate;
        InternationalDomestic = internationalDomestic;
        Sector = sector;
        Industry = industry;
        GicSector = gicSector;
        GicGroup = gicGroup;
        GicIndustry = gicIndustry;
        HomeCategory = homeCategory;
        IsDelisted = isDelisted;
        Description = description;
        Phone = phone;
        WebUrl = webUrl;
        LogoUrl = logoUrl;
        FullTimeEmployees = fullTimeEmployees;
        UpdateAt = updateAt;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("global_id", Order = 1, TypeName = "uuid", IsPartOfKey = false)]
    public Guid GlobalId { get; }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = false)]
    public DateTime DateCaptured { get; }

    [ColumnWithKey("symbol", Order = 3, TypeName = "text", IsPartOfKey = true)]
    public string Symbol { get; }

    [ColumnWithKey("exchange", Order = 4, TypeName = "text", IsPartOfKey = true)]
    public string Exchange { get; }

    [ColumnWithKey("name", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string Name { get; }

    [ColumnWithKey("type", Order = 6, TypeName = "text", IsPartOfKey = true)]
    public string Type { get; }

    [ColumnWithKey("currency_code", Order = 7, TypeName = "text", IsPartOfKey = false)]
    public string CurrencyCode { get; }

    [ColumnWithKey("currency_name", Order = 8, TypeName = "text", IsPartOfKey = false)]
    public string CurrencyName { get; }

    [ColumnWithKey("currency_symbol", Order = 9, TypeName = "text", IsPartOfKey = false)]
    public string CurrencySymbol { get; }

    [ColumnWithKey("country_name", Order = 10, TypeName = "text", IsPartOfKey = false)]
    public string CountryName { get; }

    [ColumnWithKey("country_iso", Order = 11, TypeName = "text", IsPartOfKey = false)]
    public string CountryIso { get; }

    [ColumnWithKey("isin", Order = 12, TypeName = "text", IsPartOfKey = false)]
    public string Isin { get; }

    [ColumnWithKey("lei", Order = 13, TypeName = "text", IsPartOfKey = false)]
    public string Lei { get; }

    [ColumnWithKey("cusip", Order = 14, TypeName = "text", IsPartOfKey = false)]
    public string Cusip { get; }

    [ColumnWithKey("cik", Order = 15, TypeName = "text", IsPartOfKey = false)]
    public string Cik { get; }

    [ColumnWithKey("employer_id_number", Order = 16, TypeName = "text", IsPartOfKey = false)]
    public string EmployerIdNumber { get; }

    [ColumnWithKey("fiscal_year_end", Order = 17, TypeName = "text", IsPartOfKey = false)]
    public string FiscalYearEnd { get; }

    [ColumnWithKey("ipo_date", Order = 18, TypeName = "date", IsPartOfKey = false)]
    public DateTime IpoDate { get; }

    [ColumnWithKey("international_domestic", Order = 19, TypeName = "text", IsPartOfKey = false)]
    public string InternationalDomestic { get; }

    [ColumnWithKey("sector", Order = 20, TypeName = "text", IsPartOfKey = false)]
    public string Sector { get; }

    [ColumnWithKey("industry", Order = 21, TypeName = "text", IsPartOfKey = false)]
    public string Industry { get; }

    [ColumnWithKey("gic_sector", Order = 22, TypeName = "text", IsPartOfKey = false)]
    public string GicSector { get; }

    [ColumnWithKey("gic_group", Order = 23, TypeName = "text", IsPartOfKey = false)]
    public string GicGroup { get; }

    [ColumnWithKey("gic_industry", Order = 24, TypeName = "text", IsPartOfKey = false)]
    public string GicIndustry { get; }

    [ColumnWithKey("home_category", Order = 25, TypeName = "text", IsPartOfKey = false)]
    public string HomeCategory { get; }

    [ColumnWithKey("is_delisted", Order = 26, TypeName = "boolean", IsPartOfKey = false)]
    public bool IsDelisted { get; }

    [ColumnWithKey("description", Order = 27, TypeName = "text", IsPartOfKey = false)]
    public string Description { get; }

    [ColumnWithKey("phone", Order = 28, TypeName = "text", IsPartOfKey = false)]
    public string Phone { get; }

    [ColumnWithKey("web_url", Order = 29, TypeName = "text", IsPartOfKey = false)]
    public string WebUrl { get; }

    [ColumnWithKey("logo_url", Order = 30, TypeName = "text", IsPartOfKey = false)]
    public string LogoUrl { get; }

    [ColumnWithKey("full_time_employees", Order = 31, TypeName = "integer", IsPartOfKey = false)]
    public int FullTimeEmployees { get; }

    [ColumnWithKey("update_at", Order = 32, TypeName = "date", IsPartOfKey = false)]
    public DateTime UpdateAt { get; }

    [ColumnWithKey("utc_timestamp", Order = 33, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
