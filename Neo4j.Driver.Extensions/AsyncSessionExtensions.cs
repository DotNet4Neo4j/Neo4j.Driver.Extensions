namespace Neo4j.Driver.Extensions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class AsyncSessionExtensions
    {
        public static async Task<T> RunReadTransactionForNodeResults<T>(this IAsyncSession session, string query, object parameters, string identifier) 
            where T : new()
        {
            var results = await session.ReadTransactionAsync(async work =>
            {
                var cursor = await work.RunAsync(query, parameters);
                var fetched = await cursor.FetchAsync();

                while (fetched)
                {
                    var node = cursor.Current[identifier].As<INode>();
                    return node.ToObject<T>();
                }

                return default;
            });

            return results;
        }

        public static async Task<T> RunReadTransaction<T>(this IAsyncSession session, string query, object parameters, string identifier) 
            where T : new()
        {
            var results = await session.ReadTransactionAsync(async work =>
            {
                var cursor = await work.RunAsync(query, parameters);
                var fetched = await cursor.FetchAsync();

                while (fetched)
                {
                    return cursor.Current[identifier].As<T>();
                }

                return default;
            });

            return results;
        }

        public static async Task<IEnumerable<T>> RunReadTransactionEnumerable<T>(this IAsyncSession session, string query, object parameters, string identifier)
            where T : new()
        {
            var results = await session.ReadTransactionAsync(async work =>
            {
                var cursor = await work.RunAsync(query, parameters);
                var fetched = await cursor.FetchAsync();
                var output = new List<T>();
                while (fetched)
                {
                    output.Add(cursor.Current[identifier].As<T>());
                    fetched = await cursor.FetchAsync();
                }

                return output;
            });

            return results;
        }
    }
}