namespace Neo4j.Drivers.Extensions.Tests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Neo4j.Driver;
    using Neo4j.Driver.Extensions;
    using Xunit;

    public class NodeExtensionsTests
    {
        public class GetValueTMethod
        {
            [Fact]
            public void ThrowsArgumentNullException_WhenNodeIsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => NodeExtensions.GetValue<string>(null, "foo"));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ThrowsArgumentNullException_WhenPropertyNameIsNullOrWhiteSpace()
            {
                var mockNode = new Mock<INode>(MockBehavior.Loose);
                var ex = Assert.Throws<ArgumentNullException>(() => mockNode.Object.GetValue<string>(null));
                ex.Should().NotBeNull();
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            public void ThrowsArgumentException_WhenPropertyNameIsNullOrWhiteSpace(string propertyName)
            {
                var mockNode = new Mock<INode>(MockBehavior.Loose);
                var ex = Assert.Throws<ArgumentException>(() => mockNode.Object.GetValue<string>(propertyName));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ThrowsFormatException_WhenUnableToCastToTheGivenType()
            {
                const string nameOfProperty = "foo";
                var mockNode = new Mock<INode>();

                mockNode
                    .Setup(n => n.Properties)
                    .Returns(new SortedList<string, object> { { nameOfProperty, "not-an-int" } });

                var ex = Assert.Throws<FormatException>(() => mockNode.Object.GetValue<bool>(nameOfProperty));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ReturnsDefault_WhenNodeDoesNotContainKey()
            {
                const string nameOfProperty = "foo";
                var mockNode = new Mock<INode>();

                mockNode
                    .Setup(n => n.Properties)
                    .Returns(new SortedList<string, object>());

                mockNode.Object.GetValue<string>(nameOfProperty).Should().Be(default);
                mockNode.Object.GetValue<int>(nameOfProperty).Should().Be(default);
                mockNode.Object.GetValue<double>(nameOfProperty).Should().Be(default);
            }

            [Fact]
            public void ReturnsConvertedValue_WhenNodeContainsKey()
            {
                const string expectedValue = "string";
                const string nameOfProperty = "foo";
                var mockNode = new Mock<INode>();

                mockNode
                    .Setup(n => n.Properties)
                    .Returns(new SortedList<string, object> {{nameOfProperty, expectedValue}});

                var actual = mockNode.Object.GetValue<string>(nameOfProperty);
                actual.Should().Be(expectedValue);
            }
        }

        public class GetValueStrictTMethod
        {
            [Fact]
            public void ThrowsArgumentNullException_WhenNodeIsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => NodeExtensions.GetValueStrict<string>(null, "foo"));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ThrowsArgumentNullException_WhenPropertyNameIsNullOrWhiteSpace()
            {
                var mockNode = new Mock<INode>(MockBehavior.Loose);
                var ex = Assert.Throws<ArgumentNullException>(() => mockNode.Object.GetValueStrict<string>(null));
                ex.Should().NotBeNull();
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            public void ThrowsArgumentException_WhenPropertyNameIsNullOrWhiteSpace(string propertyName)
            {
                var mockNode = new Mock<INode>(MockBehavior.Loose);
                var ex = Assert.Throws<ArgumentException>(() => mockNode.Object.GetValueStrict<string>(propertyName));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ThrowsFormatException_WhenUnableToCastToTheGivenType()
            {
                const string nameOfProperty = "foo";
                var mockNode = new Mock<INode>();

                mockNode
                    .Setup(n => n.Properties)
                    .Returns(new SortedList<string, object> { { nameOfProperty, "not-an-int" } });

                var ex = Assert.Throws<FormatException>(() => mockNode.Object.GetValueStrict<bool>(nameOfProperty));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ThrowsKeyNotFoundException_WhenNodeDoesNotContainKey()
            {
                const string nameOfProperty = "foo";
                var mockNode = new Mock<INode>();

                mockNode
                    .Setup(n => n.Properties)
                    .Returns(new SortedList<string, object>());

                var ex = Assert.Throws<KeyNotFoundException>(() => mockNode.Object.GetValueStrict<string>(nameOfProperty));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ReturnsConvertedValue_WhenNodeContainsKey()
            {
                const string expectedValue = "string";
                const string nameOfProperty = "foo";
                var mockNode = new Mock<INode>();

                mockNode
                    .Setup(n => n.Properties)
                    .Returns(new SortedList<string, object> { { nameOfProperty, expectedValue } });

                var actual = mockNode.Object.GetValueStrict<string>(nameOfProperty);
                actual.Should().Be(expectedValue);
            }
        }

        public class ToObjectTMethod
        {
            private class Foo
            {
                public string StringProperty { get; set; }
                public int IntProperty { get; set; }
            }

            private class FooWithAttributes
            {
                [Neo4jProperty(Name = "stringProperty")]
                public string StringProperty { get; set; }

                [Neo4jProperty(Ignore = true)]
                public int? IntProperty { get; set; }
            }

            [Fact]
            public void ThrowsArgumentNullException_WhenNodeIsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => NodeExtensions.ToObject<Foo>(null));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ReturnsEmptyDefaults_WhenNoPropertiesExist()
            {
                var mock = new Mock<INode>();
                mock.Setup(x => x.Properties)
                    .Returns(new SortedList<string, object>());

                var foo = mock.Object.ToObject<Foo>();
                foo.IntProperty.Should().Be(default);
                foo.StringProperty.Should().Be(default);
            }

            [Fact]
            public void SetsProperties()
            {
                const string expectedString = "foo";
                const int expectedInt = 42;

                var mock = new Mock<INode>();
                mock.Setup(x => x.Properties)
                    .Returns(new SortedList<string, object>
                    {
                        {nameof(Foo.StringProperty), expectedString},
                        {nameof(Foo.IntProperty), expectedInt}
                    });

                var foo = mock.Object.ToObject<Foo>();
                foo.IntProperty.Should().Be(expectedInt);
                foo.StringProperty.Should().Be(expectedString);
            }

            [Fact]
            public void SetsPropertiesUsingNeo4jPropertyAttribute()
            {
                const string expectedString = "foo";
                const int expectedInt = 42;

                var mock = new Mock<INode>();
                mock.Setup(x => x.Properties)
                    .Returns(new SortedList<string, object>
                    {
                        {"stringProperty", expectedString},
                        {nameof(Foo.IntProperty), expectedInt}
                    });

                var foo = mock.Object.ToObject<FooWithAttributes>();
                foo.StringProperty.Should().Be(expectedString);
            }

            [Fact]
            public void IgnoresPropertiesWithIgnoreSetByNeo4jPropertyAttribute()
            {
                var mock = new Mock<INode>();
                mock.Setup(x => x.Properties)
                    .Returns(new SortedList<string, object>
                    {
                        {"stringProperty", "foo"},
                        {nameof(Foo.IntProperty), 42}
                    });

                var foo = mock.Object.ToObject<FooWithAttributes>();
                foo.IntProperty.Should().Be(null);
            }
        }
    }
}