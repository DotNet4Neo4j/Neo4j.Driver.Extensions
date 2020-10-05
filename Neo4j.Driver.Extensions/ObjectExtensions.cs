namespace Neo4j.Driver.Extensions
{
    using System;

    /// <summary>
    /// Extension methods for the <see cref="object"/> class.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// A helper method to explicitly cast the value streamed back via Bolt to a local type. 
        /// </summary>
        /// <typeparam name="T">The type to attempt to convert to.</typeparam>
        /// <param name="obj">The value that streamed back via Bolt protocol, e.g.Properties.</param>
        /// <param name="value">The value converted to <typeparamref name="T"/>, default(T) otherwise.</param>
        /// <returns><c>true</c> if the <paramref name="obj"/> could be converted, <c>false</c> otherwise.</returns>
        public static bool TryAs<T>(this object obj, out T value)
        {
            try
            {
                value = obj.As<T>();
                return true;
            }
            catch(SystemException ex ) 
                when (ex is FormatException || ex is InvalidCastException)
            {
                value = default;
                return false;
            }
        }

    }
}