# MondoCore.MongoDB
Wrapper of MongoDB API using interfaces from [MondoCore.Data](https://github.com/jim-lightfoot/MondoCore.Data).


    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection collection, string dbName, string connectionString)
        {
            collection.AddSingleton<IDatabase>((p) => new MongoDB(dbName, connectionString) ); 

            return collection;
        }
    
        public static IServiceCollection AddRepositoryReader<T>(this IServiceCollection collection, string repoName)
        {
            collection.AddSingleton<IReadRepository<T>>((p) => 
            {
                var db = p.GetRequiredService<IDatabase>();

                return db.GetRepositoryReader<Guid, T>(repoName); 
            }

            return collection;
        }
    
        public static void SetUpMyApp<T>(this IServiceCollection collection)
        {
            collection.AddDatabase("customer", customerConnectionString);      
                      .AddRepositoryReader<Customer>("customer")
        }
    }