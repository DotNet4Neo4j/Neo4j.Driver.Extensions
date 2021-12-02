using System.ComponentModel;

namespace Neo4j.Drivers.Extensions.Tests
{
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Neo4j.Driver;
    using Neo4j.Driver.Extensions;

    using Xunit;

    public class RecordExtensionsTests
    {
        public class ToObjectT
        {
            [Fact]
            public void ThrowsArgumentNullException_WhenRecordIsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => RecordExtensions.ToObject<Foo>(null));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void TreatsRecordAsDictionary_WhenIdentifierSupplied()
            {
                const string identifier = "foo";
                const string stringPropertyValue = "baa";
                const string stringPropertyWithAttributeValue = "bar";

                var mock = new Mock<IRecord>();
                mock.Setup(x => x[identifier]).Returns(new Dictionary<string, object> {
                    {nameof(Foo.StringProperty), stringPropertyValue},
                    {"stringPropertyWithAttribute", stringPropertyWithAttributeValue}
                });

                var foo = mock.Object.ToObject<Foo>(identifier);
                foo.StringProperty.Should().Be(stringPropertyValue);
                foo.StringPropertyWithAttribute.Should().Be(stringPropertyWithAttributeValue);
            }

            [Fact]
            public void TreatsRecordAsRelationShip_WhenIdentifierSupplied()
            {
                const string identifier = "foo";
                const string stringPropertyValue = "baa";

                var data = new Dictionary<string, object> { { "StringProperty", stringPropertyValue } };
                var mock = new Mock<IRecord>();
                var mockRelation = new Mock<IRelationship>(MockBehavior.Loose);
                mockRelation.SetupGet(x => x.Properties).Returns(data);
                mock.Setup(x => x[identifier]).Returns(mockRelation.Object);

                var foo = mock.Object.ToObject<Foo>(identifier);
                foo.StringProperty.Should().Be(stringPropertyValue);
            }

            [Fact]
            public void AssumesAllTheResponseIsAnObject_WhenIdentifierNotSupplied()
            {
                const string stringPropertyValue = "baa";
                const string stringPropertyWithAttributeValue = "bar";

                var mock = new Mock<IRecord>();
                mock.Setup(x => x.Keys).Returns(new List<string> { "StringProperty", "stringPropertyWithAttribute" });
                mock.Setup(x => x.Values).Returns(new Dictionary<string, object> {
                    {"StringProperty", stringPropertyValue},
                    {"stringPropertyWithAttribute", stringPropertyWithAttributeValue},
                });

                var foo = mock.Object.ToObject<Foo>();
                foo.StringProperty.Should().Be(stringPropertyValue);
                foo.StringPropertyWithAttribute.Should().Be(stringPropertyWithAttributeValue);
            }
        }

        public class GetValueT
        {
            [Fact]
            public void ThrowsArgumentNullException_WhenRecordIsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => RecordExtensions.GetValue<string>(null, "identifier"));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ThrowsArgumentNullException_WhenIdentifierIsNull()
            {
                var mock = new Mock<IRecord>();

                var ex = Assert.Throws<ArgumentNullException>(() => mock.Object.GetValue<string>(null));
                ex.Should().NotBeNull();
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            public void ThrowsArgumentException_WhenIdentifierIsNull(string identifier)
            {
                var mock = new Mock<IRecord>();

                var ex = Assert.Throws<ArgumentException>(() => mock.Object.GetValue<string>(identifier));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ReturnsDefault_WhenIdentifierNotThere()
            {
                var mock = new Mock<IRecord>();
                mock.Setup(x => x.Keys).Returns(new List<string>());
                mock.Setup(x => x.Values).Returns(new Dictionary<string, object>());

                var intResult = mock.Object.GetValue<int>("not-there");
                intResult.Should().Be(default);

                var stringResult = mock.Object.GetValue<string>("not-there");
                stringResult.Should().Be(default);
            }

            [Fact]
            public void ReturnsCorrectValue()
            {
                const string stringIdentifier = "foo";
                const string intIdentifier = "bar";
                const string expectedStringValue = "string";
                const int expectedIntValue = 42;

                var mock = new Mock<IRecord>();


                mock.Setup(x => x.Keys).Returns(new List<string> { stringIdentifier, intIdentifier });
                mock.Setup(x => x.Values).Returns(new Dictionary<string, object>
                {
                    {stringIdentifier, expectedStringValue},
                    {intIdentifier, expectedIntValue}
                });

                mock.Object.GetValue<int>(intIdentifier).Should().Be(expectedIntValue);
                mock.Object.GetValue<string>(stringIdentifier).Should().Be(expectedStringValue);
            }
        }

        public class GetValueStrictT
        {
            [Fact]
            public void ThrowsArgumentNullException_WhenRecordIsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => RecordExtensions.GetValueStrict<string>(null, "identifier"));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ThrowsArgumentNullException_WhenIdentifierIsNull()
            {
                var mock = new Mock<IRecord>();

                var ex = Assert.Throws<ArgumentNullException>(() => mock.Object.GetValueStrict<string>(null));
                ex.Should().NotBeNull();
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            public void ThrowsArgumentException_WhenIdentifierIsNull(string identifier)
            {
                var mock = new Mock<IRecord>();

                var ex = Assert.Throws<ArgumentException>(() => mock.Object.GetValueStrict<string>(identifier));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ReturnsDefault_WhenIdentifierNotThere()
            {
                var mock = new Mock<IRecord>();
                mock.Setup(x => x.Keys).Returns(new List<string>());
                mock.Setup(x => x.Values).Returns(new Dictionary<string, object>());

                var ex = Assert.Throws<KeyNotFoundException>(() => mock.Object.GetValueStrict<int>("not-there"));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ReturnsCorrectValue()
            {
                const string stringIdentifier = "foo";
                const string intIdentifier = "bar";
                const string expectedStringValue = "string";
                const int expectedIntValue = 42;

                var mock = new Mock<IRecord>();


                mock.Setup(x => x.Keys).Returns(new List<string> { stringIdentifier, intIdentifier });
                mock.Setup(x => x.Values).Returns(new Dictionary<string, object>
                {
                    {stringIdentifier, expectedStringValue},
                    {intIdentifier, expectedIntValue}
                });

                mock.Object.GetValueStrict<int>(intIdentifier).Should().Be(expectedIntValue);
                mock.Object.GetValueStrict<string>(stringIdentifier).Should().Be(expectedStringValue);
            }
        }
    }
}