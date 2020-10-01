namespace Neo4j.Driver.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using EnsureThat;

    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Attempts to convert a <see cref="IDictionary{TKey,TValue}"/> to a type defined by <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="dictionary">The dictionary containing the properties to be converted</param>
        /// <returns>An object of type <typeparamref name="T"/>.</returns>
        public static T ToObject<T>(this IDictionary<string, object> dictionary) 
            where T : new()
        {
            Ensure.That(dictionary).IsNotNull();
            
            var obj = new T();
            foreach (var property in typeof(T).GetValidProperties())
            {
                if (!dictionary.ContainsKey(property.SerializedName))
                    continue;

                var value = dictionary[property.SerializedName];
                property.Property.SetValue(obj, value.AsInternal(property.Property.PropertyType));
            }

            return obj;
        }

        /// <summary>
        /// An internal way to call the <see cref="ValueExtensions.As{T}"/> method.
        /// </summary>
        /// <param name="o">The object instance to attempt to coerce.</param>
        /// <param name="propertyType">The type to try to coerce the <paramref name="o"/> to.</param>
        /// <returns>An object boxing the <paramref name="o"/> converted to <paramref name="propertyType"/>.</returns>
        private static object AsInternal(this object o, Type propertyType)
        {
            var method = typeof(ValueExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .SingleOrDefault(x => 
                    x.Name == nameof(ValueExtensions.As) 
                    && x.IsGenericMethod 
                    && x.GetParameters().Length == 1);
            var generic = method?.MakeGenericMethod(propertyType);
            return generic?.Invoke(o, new [] { o });
        }
    }
}