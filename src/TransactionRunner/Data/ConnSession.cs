using Npgsql;

namespace KozLibraries.TransactionRunner.Data;

public readonly record struct ConnSession(NpgsqlConnection Connection);
