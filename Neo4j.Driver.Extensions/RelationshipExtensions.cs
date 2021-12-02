namespace Neo4j.Driver.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EnsureThat;
    using Neo4j.Driver;

    /// <summary>
    /// A collection of extensions for the <see cref="IRelationship"/> interface.
    /// These should allow a user to deserialize things in an easier way.
    /// </summary>
    public static class RelationshipExtensions
    {
        /// <summary>
        ///     Gets a value from an <see cref="IRelationship" /> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> to attempt to get the property as.</typeparam>
        /// <param name="relationship">The <see cref="IRelationship" /> instance to pull the property from.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The converted <typeparamref name="T" /> or <c>default</c></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="relationship"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="propertyName"/> is an empty string or whitespace.</exception>
        /// <exception cref="FormatException">
        ///     If any of the properties on the <paramref name="relationship" /> can't be cast to their
        ///     <typeparamref name="T" /> equivalents.
        /// </exception>
        public static T GetValue<T>(this IRelationship relationship, string propertyName)
        {
            Ensure.That(relationship).IsNotNull();
            Ensure.That(propertyName).IsNotNullOrWhiteSpace();

            return relationship.Properties.ContainsKey(propertyName)
                ? relationship.Properties[propertyName].As<T>()
                : default;
        }

        /// <summary>
        ///     Gets a value from an <see cref="IRelationship" /> instance. Will throw a <see cref="KeyNotFoundException"/> if the <paramref name="propertyName"/> isn't on the <paramref name="relationship"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> to attempt to get the property as.</typeparam>
        /// <param name="relationship">The <see cref="IRelationship" /> instance to pull the property from.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The converted <typeparamref name="T" /> or <c>default</c></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="relationship"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="propertyName"/> is an empty string or whitespace.</exception>
        /// <exception cref="FormatException">
        ///     If any of the properties on the <paramref name="relationship" /> can't be cast to their
        ///     <typeparamref name="T" /> equivalents.
        /// </exception>
        /// <exception cref="KeyNotFoundException">Thrown if <paramref name="propertyName"/> is not in the <paramref name="relationship"/>.</exception>
        public static T GetValueStrict<T>(this IRelationship relationship, string propertyName)
        {
            Ensure.That(relationship).IsNotNull();
            Ensure.That(propertyName).IsNotNullOrWhiteSpace();

            if(relationship.Properties.ContainsKey(propertyName))
                return relationship.Properties[propertyName].As<T>();
            
            throw new KeyNotFoundException($"'{propertyName}' is not in the Relationship.");
        }


        /// <summary>
        ///     Gets a value from an <see cref="IRelationship" /> instance, by executing
        ///     the <see cref="GetValue{T}" /> method via reflection.
        /// </summary>
        /// <remarks>This exists primarily to allow the <see cref="ValueExtensions.As{T}(object)" /> method to be used to cast.</remarks>
        /// <param name="relationship">The <see cref="IRelationship" /> instance to pull the property from.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <param name="propertyType">The <see cref="Type" /> to convert the property to.</param>
        /// <param name="strict">If <c>true</c> this will throw <see cref="KeyNotFoundException"/> if properties aren't found on the <see cref="relationship"/>.</param>
        /// <returns>The converted value, as an <see cref="object" />.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="relationship" /> is null.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="propertyName" /> is null.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="propertyType" /> is null.</exception>
        /// <exception cref="InvalidCastException">
        ///     If any of the properties on the <paramref name="relationship" /> can't be cast to their
        ///     <typeparamref name="T" /> equivalents.
        /// </exception>
        private static object GetValue(this IRelationship relationship, string propertyName, Type propertyType, bool strict = false)
        {
            Ensure.That(relationship).IsNotNull();
            Ensure.That(propertyName).IsNotNull();
            Ensure.That(propertyType).IsNotNull();

            var method = strict
                ? typeof(RelationshipExtensions).GetMethod(nameof(GetValueStrict))
                : typeof(RelationshipExtensions).GetMethod(nameof(GetValue));

            var generic = method?.MakeGenericMethod(propertyType);

            return generic?.Invoke(relationship, new object[] {relationship, propertyName});
        }

        /// <summary>
        ///     Attempts to cast the given <see cref="IRelationship" /> instance into a complex type.
        /// </summary>
        /// <typeparam name="T">The type to try to cast to.</typeparam>
        /// <param name="relationship">The <see cref="IRelationship" /> instance to cast from.</param>
        /// <returns>An instance of <typeparamref name="T" /> with it's properties set.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="relationship" /> is null.</exception>
        /// <exception cref="InvalidCastException">
        ///     If any of the properties on the <paramref name="relationship" /> can't be cast to their
        ///     <typeparamref name="T" /> equivalents.
        /// </exception>
        public static T ToObject<T>(this IRelationship relationship) where T : new()
        {
            Ensure.That(relationship).IsNotNull();

            var properties = typeof(T).GetProperties().Where(p => p.CanWrite);
            var obj = new T();
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var neo4jProperty = property.GetNeo4jPropertyAttribute();
                if (neo4jProperty != null)
                {
                    if (neo4jProperty.Ignore)
                        continue;

                    propertyName = neo4jProperty.Name ?? propertyName;
                }

                property.SetValue(obj, relationship.GetValue(propertyName, property.PropertyType));
            }

            return obj;
        } 
    }
}