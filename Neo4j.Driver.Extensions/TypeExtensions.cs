namespace Neo4j.Driver.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using EnsureThat;

    internal static class TypeExtensions
    {
        public class PropertyInfoAndSerializedName
        {
            public PropertyInfo Property { get; set; }
            public string SerializedName { get; set; }
        }

        public static IEnumerable<PropertyInfoAndSerializedName> GetValidProperties(this Type type)
        {
            Ensure.That(type).IsNotNull();

            var output = new List<PropertyInfoAndSerializedName>();
            foreach(var property in type.GetTypeInfo().GetProperties().Where(x => x.CanWrite))
            {
                var propertyName = property.Name;

                var neo4jProperty = property.GetNeo4jPropertyAttribute();
                if (neo4jProperty != null)
                {
                    if (neo4jProperty.Ignore)
                        continue;

                    propertyName = neo4jProperty.Name ?? propertyName;
                }
                output.Add(new PropertyInfoAndSerializedName{Property = property, SerializedName = propertyName});
            }
            return output;
        }
    }
}