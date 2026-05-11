using System;
using System.Data;
using Dapper;

namespace KozLibraries.DapperDateOnlySupport;

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
        parameter.Value = value.ToDateTime(TimeOnly.MinValue);
    }

    /// <summary>
    /// parse query result to DateOnly
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override DateOnly Parse(object value)
    {
        var datetime = (DateTime)value;
        return DateOnly.FromDateTime(datetime);
    }
}
