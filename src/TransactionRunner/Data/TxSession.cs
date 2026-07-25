using Npgsql;

namespace KozLibraries.TransactionRunner.Data;

public readonly record struct TxSession(NpgsqlConnection Connection, NpgsqlTransaction Transaction);
