namespace Neo4j.Driver.Extensions
{
    using System.Collections;
    using System.Threading.Tasks;

    public static class AsyncSessionExtensions
    {
        public static async Task<T> RunReadTransaction<T>(this IAsyncSession session, string query, object parameters, string identifier) where T:new()
        {
            // var type = typeof(T);
            // if (typeof(IEnumerable).IsAssignableFrom(type) && !type.IsPrimitive && !type is typeof(string))
            //     return await session.RunReadTransactionEnumerableOut<T>(query, parameters, identifier);

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

        //
        // private static async Task<T> RunReadTransactionEnumerableOut<T>(this IAsyncSession session, string query, object parameters, string identifier) where T : new()
        // {
        //     var results = await session.ReadTransactionAsync(async work =>
        //     {
        //         var cursor = await work.RunAsync(query, parameters);
        //         var fetched = await cursor.FetchAsync();
        //
        //         while (fetched)
        //         {
        //             var node = cursor.Current[identifier].As<INode>();
        //             return node.ToObject<T>();
        //         }
        //
        //         return default;
        //     });
        //
        //     return results;
        // }
    }
}