using Npgsql;

namespace KozLibraries.TransactionRunner.Data;

public readonly record struct ConnSession(NpgsqlConnection Connection)
{
    public static implicit operator ConnSession(TxSession session) => new(session.Connection);
}
