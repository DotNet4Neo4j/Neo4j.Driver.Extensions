namespace Neo4j.Driver.Extensions
{
    using System;
    using System.Linq;
    using EnsureThat;

    public static class RecordExtensions
    {
        /// <summary>
        ///     Attempts to cast an <see cref="IRecord"/> instance into a <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>This should be used for queries where you have returned the properties directly.</remarks>
        /// <example>
        /// <code>
        ///     MATCH (m:Movie) RETURN m.title AS title, m.released AS released
        /// </code>
        /// </example>
        /// <typeparam name="T">The <see cref="Type"/> to cast to.</typeparam>
        /// <param name="record">The <see cref="IRecord"/> to cast from.</param>
        /// <returns>An object of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="record" /> is null.</exception>
        public static T ToObject<T>(this IRecord record) where T : new()
        {
            Ensure.That(record).IsNotNull();

            var obj = new T();
            foreach (var property in typeof(T).GetProperties().Where(x => x.CanWrite))
            {
                var propertyName = property.Name;

                var neo4jProperty = property.GetNeo4jPropertyAttribute();
                if (neo4jProperty != null)
                {
                    if (neo4jProperty.Ignore)
                        continue;

                    propertyName = neo4jProperty.Name ?? propertyName;
                }

                property.SetValue(obj, record.GetValue(propertyName, property.PropertyType));
            }

            return obj;
        }
        /// <summary>
        ///     Gets a value from an <see cref="IRecord" /> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> to attempt to get the property as.</typeparam>
        /// <param name="record">The <see cref="IRecord" /> instance to pull the property from.</param>
        /// <param name="identifier">The name of the identifier to get.</param>
        /// <returns>The converted <typeparamref name="T" /> or <c>default</c></returns>
        /// <exception cref="InvalidCastException">
        ///     If any of the properties on the <paramref name="record" /> can't be cast to their
        ///     <typeparamref name="T" /> equivalents.
        /// </exception>
        public static T GetValue<T>(this IRecord record, string identifier)
        {
            return record.Keys.Contains(identifier)
                ? record.Values[identifier].As<T>()
                : default;
        }

        /// <summary>
        ///     Gets a value from an <see cref="IRecord"/> instance, by executing
        ///     the <see cref="GetValue{T}" /> method via reflection.
        /// </summary>
        /// <remarks>This exists primarily to allow the <see cref="ValueExtensions.As{T}(object)" /> method to be used to cast.</remarks>
        /// <param name="record">The <see cref="IRecord" /> instance to pull the property from.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <param name="propertyType">The <see cref="Type" /> to convert the property to.</param>
        /// <returns>The converted value, as an <see cref="object" />.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="record" /> is null.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="propertyName" /> is null.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="propertyType" /> is null.</exception>
        /// <exception cref="InvalidCastException">
        ///     If any of the properties on the <paramref name="record" /> can't be cast to their
        ///     <typeparamref name="T" /> equivalents.
        /// </exception>
        private static object GetValue(this IRecord record, string propertyName, Type propertyType)
        {
            Ensure.That(record).IsNotNull();
            Ensure.That(propertyName).IsNotNull();
            Ensure.That(propertyType).IsNotNull();

            var method = typeof(RecordExtensions).GetMethod(nameof(GetValue));
            var generic = method?.MakeGenericMethod(propertyType);

            return generic?.Invoke(record, new object[] {record, propertyName});
        }
    }
}