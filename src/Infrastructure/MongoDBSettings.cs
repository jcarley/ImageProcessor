namespace Infrastructure;

public class MongoDBSettings
{
    public string? MongoDbHost { get; set; }
    public string? MongoDbPort { get; set; }
    public string? MongoDbUser { get; set; }
    public string? MongoDbPassword { get; set; }
    public string? MongoDbAuthSource { get; set; }
    public string? MongoDbDatabaseName { get; set; }

    public string ConnectionString()
    {
        string connectionString =
            $"mongodb://{MongoDbUser}:{MongoDbPassword}@{MongoDbHost}:{MongoDbPort}/?authSource={MongoDbAuthSource}&readPreference=primary&ssl=false";
        return connectionString;
    }
}