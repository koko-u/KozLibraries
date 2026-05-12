using System;
using System.Data;
using Dapper;
using JetBrains.Annotations;

namespace KozLibraries.DapperDateOnlySupport;

/// <summary>
/// DateOnly type handler for Dapper (PostgreSQL)
/// </summary>
[PublicAPI]
public sealed class DateOnlyHandler : SqlMapper.TypeHandler<DateOnly>
{
    /// <summary>
    /// set DateOnly value to parameter as DateTime
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="value"></param>
    /// <exception cref="NotImplementedException"></exception>
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value;
    }

    /// <summary>
    /// parse query result to DateOnly
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override DateOnly Parse(object value)
    {
        return value switch
        {
            DateOnly dateOnly => dateOnly,
            DateTime date => DateOnly.FromDateTime(date),
            string s => DateOnly.Parse(s),
            _ => throw new DataException(
                $"Cannot parse {value}({value.GetType().FullName}) to DateOnly"
            ),
        };
    }
}
