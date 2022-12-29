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
        DateCaptured = DateTime.UtcNow;
        Date = insiderTransaction.Date.ToDateTime(TimeOnly.MinValue);
        OwnerCik = insiderTransaction.OwnerCik;
        OwnerName = insiderTransaction.OwnerName;
        TransactionDate = insiderTransaction.TransactionDate.ToDateTime(TimeOnly.MinValue);
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
        DateTime dateCaptured,
        DateOnly date,
        string? ownerCik,
        string ownerName,
        DateOnly transactionDate,
        string transactionCode,
        int transactionAmount,
        decimal transactionPrice,
        string transactionAcquiredDisposed,
        int postTransactionAmount,
        string secLink,
        DateTime utcTimestamp)
    {
        CompanyId = companyId;
        DateCaptured = dateCaptured;
        Date = date.ToDateTime(TimeOnly.MinValue);
        OwnerCik = ownerCik;
        OwnerName = ownerName;
        TransactionDate = transactionDate.ToDateTime(TimeOnly.MinValue);
        TransactionCode = transactionCode;
        TransactionAmount = transactionAmount;
        TransactionPrice = transactionPrice;
        TransactionAcquiredDisposed = transactionAcquiredDisposed;
        PostTransactionAmount = postTransactionAmount;
        SecLink = secLink;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get;  }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = false)]
    public DateTime DateCaptured { get;  }

    [ColumnWithKey("date", Order = 3, TypeName = "date", IsPartOfKey = true)]
    public DateTime Date { get;  }

    [ColumnWithKey("owner_cik", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string? OwnerCik { get;  }

    [ColumnWithKey("owner_name", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string OwnerName { get;  }

    [ColumnWithKey("transaction_date", Order = 6, TypeName = "date", IsPartOfKey = false)]
    public DateTime TransactionDate { get;  }

    [ColumnWithKey("transaction_code", Order = 7, TypeName = "text", IsPartOfKey = false)]
    public string TransactionCode { get;  }

    [ColumnWithKey("transaction_amount", Order = 8, TypeName = "integer", IsPartOfKey = false)]
    public int TransactionAmount { get;  }

    [ColumnWithKey("transaction_price", Order = 9, TypeName = "numeric", IsPartOfKey = false)]
    public decimal TransactionPrice { get;  }

    [ColumnWithKey("transaction_acquired_disposed", Order = 10, TypeName = "text", IsPartOfKey = false)]
    public string TransactionAcquiredDisposed { get;  }

    [ColumnWithKey("post_transaction_amount", Order = 11, TypeName = "integer", IsPartOfKey = true)]
    public int PostTransactionAmount { get;  }

    [ColumnWithKey("sec_link", Order = 12, TypeName = "text", IsPartOfKey = false)]
    public string SecLink { get;  }

    [ColumnWithKey("utc_timestamp", Order = 13, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get;  }
}
