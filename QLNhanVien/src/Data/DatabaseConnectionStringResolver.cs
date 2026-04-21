using Npgsql;

namespace QLNhanVien.src.Data;

public static class DatabaseConnectionStringResolver
{
    public static string Resolve()
    {
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL")?.Trim().Trim('"');
        if (!string.IsNullOrWhiteSpace(databaseUrl))
        {
            return Normalize(databaseUrl);
        }

        var dbHost = Environment.GetEnvironmentVariable("DB_HOST")?.Trim();
        var dbPort = int.TryParse(Environment.GetEnvironmentVariable("DB_PORT"), out var port) ? port : 5432;
        var dbUser = Environment.GetEnvironmentVariable("DB_USER")?.Trim();
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD")?.Trim();
        var dbName = Environment.GetEnvironmentVariable("DB_NAME")?.Trim();

        if (string.IsNullOrWhiteSpace(dbHost) || string.IsNullOrWhiteSpace(dbUser) || string.IsNullOrWhiteSpace(dbPassword))
        {
            throw new InvalidOperationException("Thiếu cấu hình CSDL. Cần DATABASE_URL hoặc DB_HOST/DB_USER/DB_PASSWORD.");
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = dbHost,
            Port = dbPort,
            Username = dbUser,
            Password = dbPassword,
            Database = string.IsNullOrWhiteSpace(dbName) ? "postgres" : dbName,
            SslMode = SslMode.Require
        };

        return builder.ConnectionString;
    }

    private static string Normalize(string connectionValue)
    {
        if (!connectionValue.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)
            && !connectionValue.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
        {
            return connectionValue;
        }

        var uri = new Uri(connectionValue);
        var userInfo = uri.UserInfo.Split(':', 2);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port,
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
            Database = string.IsNullOrWhiteSpace(uri.AbsolutePath.Trim('/')) ? "postgres" : uri.AbsolutePath.Trim('/'),
            SslMode = SslMode.Require
        };

        return builder.ConnectionString;
    }
}