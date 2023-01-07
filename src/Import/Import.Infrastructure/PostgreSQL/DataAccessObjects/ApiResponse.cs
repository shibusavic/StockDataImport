using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;
namespace Import.Infrastructure.PostgreSQL.DataAccessObjects
{
    [Table(name: "api_responses", Schema = "public")]
    internal class ApiResponse
    {
        public ApiResponse(
            string request,
            string response,
            int statusCode,
            DateTime utcTimestamp)
        {
            Request = request;
            Response = response;
            StatusCode = statusCode;
            UtcTimestamp = utcTimestamp;
        }

        [ColumnWithKey("request", Order = 1, TypeName = "text", IsPartOfKey = true)]
        public string Request { get;  }

        [ColumnWithKey("response", Order = 2, TypeName = "text", IsPartOfKey = false)]
        public string Response { get;  }

        [ColumnWithKey("status_code", Order = 3, TypeName = "integer", IsPartOfKey = false)]
        public int StatusCode { get;  }

        [ColumnWithKey("utc_timestamp", Order = 4, TypeName = "timestamp with time zone", IsPartOfKey = true)]
        public DateTime UtcTimestamp { get;  }
    }
}
