namespace Neo4j.Driver.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using EnsureThat;
    using Neo4j.Driver;

    /// <summary>
    /// A collection of extensions for the <see cref="INode"/> interface.
    /// These should allow a user to deserialize things in an easier way.
    /// </summary>
    public static class NodeExtensions
    {
        /// <summary>
        ///     Gets a value from an <see cref="INode" /> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> to attempt to get the property as.</typeparam>
        /// <param name="node">The <see cref="INode" /> instance to pull the property from.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The converted <typeparamref name="T" /> or <c>default</c></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="propertyName"/> is an empty string or whitespace.</exception>
        /// <exception cref="FormatException">
        ///     If any of the properties on the <paramref name="node" /> can't be cast to their
        ///     <typeparamref name="T" /> equivalents.
        /// </exception>
        public static T GetValue<T>(this INode node, string propertyName)
        {
            Ensure.That(node).IsNotNull();
            Ensure.That(propertyName).IsNotNullOrWhiteSpace();

            return node.Properties.ContainsKey(propertyName)
                ? node.Properties[propertyName].As<T>()
                : default;
        }

        /// <summary>
        ///     Gets a value from an <see cref="INode" /> instance. Will throw a <see cref="KeyNotFoundException"/> if the <paramref name="propertyName"/> isn't on the <paramref name="node"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> to attempt to get the property as.</typeparam>
        /// <param name="node">The <see cref="INode" /> instance to pull the property from.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The converted <typeparamref name="T" /> or <c>default</c></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="propertyName"/> is an empty string or whitespace.</exception>
        /// <exception cref="FormatException">
        ///     If any of the properties on the <paramref name="node" /> can't be cast to their
        ///     <typeparamref name="T" /> equivalents.
        /// </exception>
        /// <exception cref="KeyNotFoundException">Thrown if <paramref name="propertyName"/> is not in the <paramref name="node"/>.</exception>
        public static T GetValueStrict<T>(this INode node, string propertyName)
        {
            Ensure.That(node).IsNotNull();
            Ensure.That(propertyName).IsNotNullOrWhiteSpace();

            if(node.Properties.ContainsKey(propertyName))
                return node.Properties[propertyName].As<T>();
            
            throw new KeyNotFoundException($"'{propertyName}' is not in the Node.");
        }


        /// <summary>
        ///     Gets a value from an <see cref="INode" /> instance, by executing
        ///     the <see cref="GetValue{T}" /> method via reflection.
        /// </summary>
        /// <remarks>This exists primarily to allow the <see cref="ValueExtensions.As{T}(object)" /> method to be used to cast.</remarks>
        /// <param name="node">The <see cref="INode" /> instance to pull the property from.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <param name="propertyType">The <see cref="Type" /> to convert the property to.</param>
        /// <param name="strict">If <c>true</c> this will throw <see cref="KeyNotFoundException"/> if properties aren't found on the <see cref="node"/>.</param>
        /// <returns>The converted value, as an <see cref="object" />.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="node" /> is null.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="propertyName" /> is null.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="propertyType" /> is null.</exception>
        /// <exception cref="InvalidCastException">
        ///     If any of the properties on the <paramref name="node" /> can't be cast to their
        ///     <typeparamref name="T" /> equivalents.
        /// </exception>
        private static object GetValue(this INode node, string propertyName, Type propertyType, bool strict = false)
        {
            Ensure.That(node).IsNotNull();
            Ensure.That(propertyName).IsNotNull();
            Ensure.That(propertyType).IsNotNull();

            var method = strict
                ? typeof(NodeExtensions).GetMethod(nameof(GetValueStrict))
                : typeof(NodeExtensions).GetMethod(nameof(GetValue));

            var generic = method?.MakeGenericMethod(propertyType);

            return generic?.Invoke(node, new object[] {node, propertyName});
        }

        /// <summary>
        ///     Attempts to cast the given <see cref="INode" /> instance into a complex type.
        /// </summary>
        /// <typeparam name="T">The type to try to cast to.</typeparam>
        /// <param name="node">The <see cref="INode" /> instance to cast from.</param>
        /// <returns>An instance of <typeparamref name="T" /> with it's properties set.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="node" /> is null.</exception>
        /// <exception cref="InvalidCastException">
        ///     If any of the properties on the <paramref name="node" /> can't be cast to their
        ///     <typeparamref name="T" /> equivalents.
        /// </exception>
        public static T ToObject<T>(this INode node) where T : new()
        {
            Ensure.That(node).IsNotNull();

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

                property.SetValue(obj, node.GetValue(propertyName, property.PropertyType));
            }

            return obj;
        }

        /// <summary>
        ///     Gets a <see cref="Neo4jPropertyAttribute" /> instance from a property.
        /// </summary>
        /// <remarks>This is to make it easier in <see cref="ToObject{T}" /> to get the right properties.</remarks>
        /// <param name="property">
        ///     The <see cref="PropertyInfo" /> representing the property that the
        ///     <see cref="Neo4jPropertyAttribute" /> is applied to.
        /// </param>
        /// <returns>A filled <see cref="Neo4jPropertyAttribute" /> instance, or default values otherwise.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="property" /> is null.</exception>
        internal static Neo4jPropertyAttribute GetNeo4jPropertyAttribute(this PropertyInfo property)
        {
            Ensure.That(property).IsNotNull();

            string nameValue = null;
            var ignoreValue = false;

            var attribute = property.CustomAttributes.SingleOrDefault(x => x.AttributeType == typeof(Neo4jPropertyAttribute));
            if (attribute != null)
            {
                var nameProperty = attribute?.NamedArguments?.SingleOrDefault(x => x.MemberName == nameof(Neo4jPropertyAttribute.Name));
                nameValue = nameProperty?.TypedValue.Value?.ToString();

                var ignoreProperty = attribute?.NamedArguments?.SingleOrDefault(x => x.MemberName == nameof(Neo4jPropertyAttribute.Ignore));
                bool.TryParse(ignoreProperty?.TypedValue.Value?.ToString(), out ignoreValue);
            }

            return new Neo4jPropertyAttribute {Ignore = ignoreValue, Name = nameValue};
        }
    }
}