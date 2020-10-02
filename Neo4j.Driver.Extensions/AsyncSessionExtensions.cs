namespace Neo4j.Driver.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EnsureThat;

    /// <summary>
    ///     Extension methods for the <see cref="IAsyncSession" />
    /// </summary>
    public static class AsyncSessionExtensions
    {
        /// <summary>
        ///     Executes a <see cref="IAsyncSession.ReadTransactionAsync{T}(System.Func{IAsyncTransaction,Task{T}})" /> transaction
        ///     returning the <typeparamref name="T" /> specified.
        /// </summary>
        /// <remarks>
        ///     This should be used with queries returning <see cref="INode" /> values, for example: <c>MATCH (n) RETURN n</c>
        /// </remarks>
        /// <typeparam name="T">The type to attempt to cast to. This should be a class.</typeparam>
        /// <param name="session">The <see cref="IAsyncSession" /> to run the transaction on.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="parameters">The parameters to the query.</param>
        /// <param name="identifier">
        ///     The identifier to cast into <typeparamref name="T" />. e.g. if the query is
        ///     <c>MATCH (n) RETURN n</c> the identifier is <c>n</c>.
        /// </param>
        /// <returns>The results of the query.</returns>
        public static async Task<IEnumerable<T>> RunReadTransactionForObjects<T>(this IAsyncSession session, string query, object parameters, string identifier)
            where T : new()
        {
            Ensure.That(session).IsNotNull();
            Ensure.That(query).IsNotNullOrWhiteSpace();
            Ensure.That(identifier).IsNotNullOrWhiteSpace();

            return await session.ReadTransactionAsync(tx => ReadTransactionAsList(tx, query, parameters, cursor => cursor.Current.ToObject<T>(identifier)));
        }

        /// <summary>
        ///     Executes a <see cref="IAsyncSession.ReadTransactionAsync{T}(System.Func{IAsyncTransaction,Task{T}})" /> transaction
        ///     returning the <typeparamref name="T" /> specified.
        /// </summary>
        /// <remarks>
        ///     This should be used with queries not returning <see cref="INode" /> values, for example:
        ///     <c>MATCH (n) RETURN n.title AS title</c>
        /// </remarks>
        /// <typeparam name="T">The type to attempt to cast to. This should be a class.</typeparam>
        /// <param name="session">The <see cref="IAsyncSession" /> to run the transaction on.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="parameters">The parameters to the query.</param>
        /// <param name="identifier">
        ///     The identifier to cast into <typeparamref name="T" />. e.g. if the query is
        ///     <c>MATCH (n) RETURN n.title AS title</c> the identifier is <c>title</c>.
        /// </param>
        /// <returns>The results of the query.</returns>
        public static async Task<IEnumerable<T>> RunReadTransaction<T>(this IAsyncSession session, string query, object parameters, string identifier)
        {
            Ensure.That(session).IsNotNull();
            Ensure.That(query).IsNotNullOrWhiteSpace();
            Ensure.That(identifier).IsNotNullOrWhiteSpace();

            return await session.ReadTransactionAsync(tx => ReadTransactionAsList(tx, query, parameters, cursor => cursor.GetValue<T>(identifier)));
        }

        internal static async Task<List<T>> ReadTransactionAsList<T>(this IAsyncTransaction tx, string query, object parameters, Func<IResultCursor, T> conversionFunction)
        {
            var cursor = await tx.RunAsync(query, parameters);
            var fetched = await cursor.FetchAsync();
            var output = new List<T>();
            while (fetched)
            {
                output.Add(conversionFunction(cursor));
                fetched = await cursor.FetchAsync();
            }

            return output;
        }
    }
}