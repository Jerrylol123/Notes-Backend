using Dapper;
using Microsoft.Data.SqlClient;

namespace NotesApi.Data;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitializeAsync()
    {
        // Connect to master to create the database if it doesn't exist
        var masterConnectionString = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = "master"
        }.ConnectionString;

        using (var masterConnection = new SqlConnection(masterConnectionString))
        {
            await masterConnection.ExecuteAsync(@"
                IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'NotesDb')
                BEGIN
                    CREATE DATABASE NotesDb;
                END
            ");
        }

        // Connect to NotesDb and create tables if they don't exist
        using var connection = new SqlConnection(_connectionString);

        await connection.ExecuteAsync(@"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
            CREATE TABLE Users (
                Id           INT IDENTITY(1,1) PRIMARY KEY,
                Username     NVARCHAR(100)  NOT NULL,
                Email        NVARCHAR(255)  NOT NULL UNIQUE,
                PasswordHash NVARCHAR(500)  NOT NULL,
                CreatedAt    DATETIME2      NOT NULL DEFAULT GETUTCDATE()
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Notes' AND xtype='U')
            CREATE TABLE Notes (
                Id        INT IDENTITY(1,1) PRIMARY KEY,
                UserId    INT            NOT NULL,
                Title     NVARCHAR(500)  NOT NULL,
                Content   NVARCHAR(MAX)  NULL,
                CreatedAt DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
                UpdatedAt DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
                CONSTRAINT FK_Notes_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            );
        ");
    }
}
