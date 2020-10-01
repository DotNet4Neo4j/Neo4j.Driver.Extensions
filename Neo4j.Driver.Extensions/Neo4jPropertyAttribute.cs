namespace Neo4j.Driver.Extensions
{
    using System;

    /// <summary>
    ///     Allows the property in Neo4j to be mapped to a different name in a class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Neo4jPropertyAttribute : Attribute
    {
        /// <summary>
        ///     Gets or sets the name this property will be read from or written to Neo4j as.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets whether to ignore this property entirely.
        /// </summary>
        public bool Ignore { get; set; }
    }
}