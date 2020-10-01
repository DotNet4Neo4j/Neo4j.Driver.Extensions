namespace Neo4j.Drivers.Extensions.Tests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Neo4j.Driver.Extensions;
    using Xunit;

    public class DictionaryExtensionsTests
    {
        public class ToObjectTMethod
        {
            [Fact]
            public void ThrowsArgumentNullException_WhenDictionaryIsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => DictionaryExtensions.ToObject<Foo>(null));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void IgnoresExtraValuesInTheDictionary()
            {
                var dictionary = new Dictionary<string, object>
                {
                    { "StringProperty", "a string" },
                    {"stringPropertyWithAttribute", "stringWithAttribute"},
                    {nameof(Foo.StringPropertyIgnored), "ignored"},
                    {"Extra Value", "FooBar"}
                };

                var foo = dictionary.ToObject<Foo>();

                foo.StringProperty.Should().Be("a string");
                foo.StringPropertyWithAttribute.Should().Be("stringWithAttribute");
                foo.StringPropertyIgnored.Should().BeNull();
            }

            [Fact]
            public void UsesNeo4jPropertyAttribute_WhenSettingProperties()
            {
                var dictionary = new Dictionary<string, object>
                {
                    { "StringProperty", "a string" },
                    {"stringPropertyWithAttribute", "stringWithAttribute"},
                    {nameof(Foo.StringPropertyIgnored), "ignored"}
                };

                var foo = dictionary.ToObject<Foo>();

                foo.StringProperty.Should().Be("a string");
                foo.StringPropertyWithAttribute.Should().Be("stringWithAttribute");
                foo.StringPropertyIgnored.Should().BeNull();
            }

            [Fact]
            public void LeavesValuesAsDefaultWhenNotInDictionary()
            {
                var dictionary = new Dictionary<string, object>
                {
                    { "StringProperty", "a string" },
                };

                var foo = dictionary.ToObject<Foo>();
                foo.StringProperty.Should().Be("a string");
                foo.StringPropertyWithAttribute.Should().BeNull();
                foo.StringPropertyIgnored.Should().BeNull();
            }
        }
    }
}