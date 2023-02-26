using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_insider_transactions", Schema = "public")]
internal class CompanyInsiderTransaction
{
    public CompanyInsiderTransaction(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.InsiderTransaction insiderTransaction)
    {
        CompanyId = companyId;
        Date = insiderTransaction.Date?.ToDateTime(TimeOnly.MinValue) ??
            throw new ArgumentException($"{nameof(insiderTransaction)} has no {nameof(Date)}"); 
        OwnerCik = insiderTransaction.OwnerCik;
        OwnerName = insiderTransaction.OwnerName;
        TransactionDate = insiderTransaction.TransactionDate?.ToDateTime(TimeOnly.MinValue) ??
            throw new ArgumentException($"{nameof(insiderTransaction)} has no {nameof(TransactionDate)}");
        TransactionCode = insiderTransaction.TransactionCode;
        TransactionAmount = insiderTransaction.TransactionAmount;
        TransactionPrice = insiderTransaction.TransactionPrice;
        TransactionAcquiredDisposed = insiderTransaction.TransactionAcquiredDisposed;
        PostTransactionAmount = insiderTransaction.PostTransactionAmount.GetValueOrDefault();
        SecLink = insiderTransaction.SecLink;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyInsiderTransaction(
        Guid companyId,
        DateTime date,
        string? ownerCik,
        string? ownerName,
        DateTime? transactionDate,
        string? transactionCode,
        int? transactionAmount,
        decimal? transactionPrice,
        string? transactionAcquiredDisposed,
        int postTransactionAmount,
        string? secLink,
        DateTime? utcTimestamp = null)
    {
        CompanyId = companyId;
        Date = date;
        OwnerCik = ownerCik;
        OwnerName = ownerName;
        TransactionDate = transactionDate;
        TransactionCode = transactionCode;
        TransactionAmount = transactionAmount;
        TransactionPrice = transactionPrice;
        TransactionAcquiredDisposed = transactionAcquiredDisposed;
        PostTransactionAmount = postTransactionAmount;
        SecLink = secLink;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }

    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("date", Order = 2, TypeName = "date", IsPartOfKey = true)]
    public DateTime Date { get; }

    [ColumnWithKey("owner_cik", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string? OwnerCik { get; }

    [ColumnWithKey("owner_name", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string? OwnerName { get; }

    [ColumnWithKey("transaction_date", Order = 5, TypeName = "date", IsPartOfKey = false)]
    public DateTime? TransactionDate { get; }

    [ColumnWithKey("transaction_code", Order = 6, TypeName = "text", IsPartOfKey = false)]
    public string? TransactionCode { get; }

    [ColumnWithKey("transaction_amount", Order = 7, TypeName = "integer", IsPartOfKey = false)]
    public int? TransactionAmount { get; }

    [ColumnWithKey("transaction_price", Order = 8, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TransactionPrice { get; }

    [ColumnWithKey("transaction_acquired_disposed", Order = 9, TypeName = "text", IsPartOfKey = false)]
    public string? TransactionAcquiredDisposed { get; }

    [ColumnWithKey("post_transaction_amount", Order = 10, TypeName = "integer", IsPartOfKey = true)]
    public int PostTransactionAmount { get; }

    [ColumnWithKey("sec_link", Order = 11, TypeName = "text", IsPartOfKey = false)]
    public string? SecLink { get; }

    [ColumnWithKey("utc_timestamp", Order = 12, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
