namespace Neo4j.Driver.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EnsureThat;

    public static class ResultCursorExtensions
    {
        public static T GetValue<T>(this IResultCursor cursor, string identifier)
        {
            Ensure.That(cursor).IsNotNull();
            Ensure.That(identifier).IsNotNullOrWhiteSpace();

            return cursor.Current.Keys.Contains(identifier)
                ? cursor.Current[identifier].As<T>()
                : default;
        }

        public static T GetValueStrict<T>(this IResultCursor cursor, string identifier)
        {
            Ensure.That(cursor).IsNotNull();
            Ensure.That(identifier).IsNotNullOrWhiteSpace();

            if(cursor.Current.Keys.Contains(identifier))
                return cursor.Current[identifier].As<T>();
            
            throw new KeyNotFoundException($"'{identifier}' not returned from the query.");
        }

        /// <summary>
        ///     Simplifies the <c>while</c>, <see cref="IResultCursor.FetchAsync" /> pairing, allowing a called to just use a
        ///     <see cref="foreach" />.
        /// </summary>
        /// <remarks>
        ///     You will want <typeparamref name="T" /> to be one of the ones that the <see cref="IDriver" /> can handle, i.e.
        ///     <see cref="INode" /> etc.<br />
        ///     NB. If you want to pull more than just one property from the <paramref name="cursor" />, don't use this method, use
        ///     <see cref="GetRecords" /> instead.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type" /> to try to get the <paramref name="identifier" /> as.</typeparam>
        /// <param name="cursor">The <see cref="IResultCursor" /> instance to read from.</param>
        /// <param name="identifier">The identifier to pull out from.</param>
        /// <returns><c>yield</c>s the <typeparamref name="T" /> retrieved from the <paramref name="cursor" />.</returns>
        public static async IAsyncEnumerable<T> GetContent<T>(this IResultCursor cursor, string identifier)
        {
            var fetched = await cursor.FetchAsync();
            while (fetched)
            {
                yield return cursor.Current[identifier].As<T>();
                fetched = await cursor.FetchAsync();
            }
        }

        /// <summary>
        ///     Simplifies the <c>while</c>, <see cref="IResultCursor.FetchAsync" /> pairing, allowing a called to just use a
        ///     <see cref="foreach" />.
        /// </summary>
        /// <param name="cursor">The <see cref="IResultCursor" /> instance to read from.</param>
        /// <returns><c>yield</c>s the <see cref="IRecord" /> retrieved from the <paramref name="cursor" />.</returns>
        public static async IAsyncEnumerable<IRecord> GetRecords(this IResultCursor cursor)
        {
            var fetched = await cursor.FetchAsync();
            while (fetched)
            {
                yield return cursor.Current;
                fetched = await cursor.FetchAsync();
            }
        }
    }
}