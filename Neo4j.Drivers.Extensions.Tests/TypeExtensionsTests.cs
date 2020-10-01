namespace Neo4j.Drivers.Extensions.Tests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Neo4j.Driver.Extensions;
    using Xunit;

    public class TypeExtensionsTests
    {
        public class GetValidPropertiesMethod
        {
            [Fact]
            public void ThrowsArgumentNullException_WhenTypeGivenIsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => Driver.Extensions.TypeExtensions.GetValidProperties(null));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void GetsTheCorrectProperties()
            {
                var properties = typeof(Foo).GetValidProperties().ToList();

                properties.Count.Should().Be(2);
                var stringProperty = properties.Single(x => x.SerializedName == "StringProperty");
                stringProperty.Should().NotBeNull();

                var stringPropertyWithAttribute = properties.Single(x => x.SerializedName == "stringPropertyWithAttribute");
                stringPropertyWithAttribute.Should().NotBeNull();

                var stringPropertyIgnored = properties.SingleOrDefault(x => x.SerializedName == "StringPropertyIgnored");
                stringPropertyIgnored.Should().BeNull();
            }
        }

    }
}