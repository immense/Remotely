using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Globalization;
using System.Linq.Expressions;

namespace Remotely.Server.Converters;

/// <summary>
/// <para>
///   Postgres can only handle DateTimeOffset with an offset of 0.  This converter
///   will allow us to use DateTimeOffset in entities without any manual conversion.
/// </para>
/// <para>
///   Docs: https://www.npgsql.org/efcore/release-notes/6.0.html?tabs=annotations#detailed-notes
/// </para>
/// </summary>
public class PostgresDateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public PostgresDateTimeOffsetConverter()
        : base(ToUtc(), ToLocalTime(), null)
    {

    }

    public PostgresDateTimeOffsetConverter(
        Expression<Func<DateTimeOffset, DateTimeOffset>> convertToProviderExpression, 
        Expression<Func<DateTimeOffset, DateTimeOffset>> convertFromProviderExpression, 
        ConverterMappingHints? mappingHints = null) 
        : base(convertToProviderExpression, convertFromProviderExpression, mappingHints)
    {
    }
    protected static Expression<Func<DateTimeOffset, DateTimeOffset>> ToUtc() 
        => v => v.ToUniversalTime();

    protected static Expression<Func<DateTimeOffset, DateTimeOffset>> ToLocalTime()
        => v => v.ToLocalTime();
}
