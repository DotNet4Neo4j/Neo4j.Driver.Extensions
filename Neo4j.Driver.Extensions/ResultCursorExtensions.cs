namespace Neo4j.Driver.Extensions
{
    using System;
    using System.Collections.Generic;
    using EnsureThat;

    /// <summary>
    ///     Extension methods for the <see cref="IResultCursor" />
    /// </summary>
    public static class ResultCursorExtensions
    {
        /// <summary>
        ///     Gets a value from the <see cref="IResultCursor.Current" /> element.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> to attempt to get the property as.</typeparam>
        /// <param name="cursor">The <see cref="IResultCursor" /> instance to pull the property from.</param>
        /// <param name="identifier">The name of the identifier to get.</param>
        /// <returns>The converted <typeparamref name="T" /> or <c>default</c></returns>
        /// <exception cref="NullReferenceException">
        ///     Thrown if the <paramref name="cursor" /> hasn't had
        ///     <see cref="IResultCursor.FetchAsync" /> called on it.
        /// </exception>
        public static T GetValue<T>(this IResultCursor cursor, string identifier)
        {
            return cursor.GetValueInternal<T>(identifier, false);
        }

        /// <summary>
        ///     Gets a value from the <see cref="IResultCursor.Current" /> element. Throwing an exception if the
        ///     <paramref name="identifier" /> isn't there.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> to attempt to get the property as.</typeparam>
        /// <param name="cursor">The <see cref="IResultCursor" /> instance to pull the property from.</param>
        /// <param name="identifier">The name of the identifier to get.</param>
        /// <returns>The converted <typeparamref name="T" /> or <c>default</c></returns>
        /// <exception cref="NullReferenceException">
        ///     Thrown if the <paramref name="cursor" /> hasn't had
        ///     <see cref="IResultCursor.FetchAsync" /> called on it.
        /// </exception>
        public static T GetValueStrict<T>(this IResultCursor cursor, string identifier)
        {
            return cursor.GetValueInternal<T>(identifier, true);
        }

        /// <summary>
        ///     Gets a value from the <see cref="IResultCursor.Current" /> element. Throwing an exception if the
        ///     <paramref name="identifier" /> isn't there.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> to attempt to get the property as.</typeparam>
        /// <param name="cursor">The <see cref="IResultCursor" /> instance to pull the property from.</param>
        /// <param name="identifier">The name of the identifier to get.</param>
        /// <param name="strict">If <c>true</c> then a <see cref="KeyNotFoundException"/> will be thrown if the <paramref name="identifier"/>
        /// isn't there, otherwise the <c>default</c> value will be returned.</param>
        /// <returns>The converted <typeparamref name="T" /> or <c>default</c></returns>
        /// <exception cref="NullReferenceException">
        ///     Thrown if the <paramref name="cursor" /> hasn't had
        ///     <see cref="IResultCursor.FetchAsync" /> called on it.
        /// </exception>
        private static T GetValueInternal<T>(this IResultCursor cursor, string identifier, bool strict)
        {
            Ensure.That(cursor).IsNotNull();
            Ensure.That(identifier).IsNotNullOrWhiteSpace();

            if (cursor.Current == null)
                throw new NullReferenceException("The cursor doesn't appear to have had 'FetchAsync' called on it?");

            return strict
                ? cursor.Current.GetValueStrict<T>(identifier)
                : cursor.Current.GetValue<T>(identifier);
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
            Ensure.That(cursor).IsNotNull();
            Ensure.That(identifier).IsNotNullOrWhiteSpace();

            var fetched = await cursor.FetchAsync();
            while (fetched)
            {
                yield return cursor.GetValue<T>(identifier);
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
            Ensure.That(cursor).IsNotNull();

            var fetched = await cursor.FetchAsync();
            while (fetched)
            {
                yield return cursor.Current;
                fetched = await cursor.FetchAsync();
            }
        }
    }
}