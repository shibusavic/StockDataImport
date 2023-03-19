//namespace EodHistoricalData.Infrastructure.Attributes;

///// <summary>
///// Represents an attribute that can be applied to correlate a property on a database access object to a database column name.
///// </summary>
//[AttributeUsage(AttributeTargets.Property, Inherited = true)]
//public class ColumnNameAttribute : Attribute
//{
//    /// <summary>
//    /// Get the name of the database column.
//    /// </summary>
//    public string Name;

//    /// <summary>
//    /// Gets an indicator of whether the column is part of the table's primary key.
//    /// </summary>
//    public bool IsPartOfKey;

//    /// <summary>
//    /// Creates a new instance of the <see cref="ColumnNameAttribute"/> class.
//    /// </summary>
//    /// <param name="name">The name of the database column.</param>
//    /// <param name="isPartOfKey">An indicator of whether the column is part of the table's primary key.</param>
//    public ColumnNameAttribute(string name, bool isPartOfKey = false)
//    {
//        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name)) : name;
//        IsPartOfKey = isPartOfKey;
//    }
//}
