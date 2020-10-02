namespace Neo4j.Drivers.Extensions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Neo4j.Driver;
    using Neo4j.Driver.Extensions;
    using Xunit;
    using ResultCursorExtensions = Neo4j.Driver.Extensions.ResultCursorExtensions;

    public class ResultCursorExtensionsTests
    {
        public class GetValueTMethod
        {
            [Fact]
            public void ThrowsNullReferenceException_WhenCurrentIsNull()
            {
                var mock = new Mock<IResultCursor>();
                mock.Setup(x => x.Current).Returns((IRecord) null);
                var ex = Assert.Throws<NullReferenceException>(() => mock.Object.GetValue<string>("foo"));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ThrowsArgumentNullException_WhenCursorIsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => ResultCursorExtensions.GetValue<string>(null, "identifier"));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ThrowsArgumentNullException_WhenIdentifierIsNull()
            {
                var mock = new Mock<IResultCursor>();
                var ex = Assert.Throws<ArgumentNullException>(() => mock.Object.GetValue<string>(null));
                ex.Should().NotBeNull();
            }


            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            public void ThrowsArgumentException_WhenIdentifierIsEmptyOrWhitespace(string identifier)
            {
                var mock = new Mock<IResultCursor>();
                var ex = Assert.Throws<ArgumentException>(() => mock.Object.GetValue<string>(identifier));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ThrowsFormatException_WhenAttemptingToCastIncorrectly()
            {
                const string identifier = "foo";
                var mock = new Mock<IResultCursor>();
                var mockRecord = new Mock<IRecord>();
                mockRecord.Setup(x => x.Keys).Returns(new List<string> {identifier});
                mockRecord.Setup(x => x.Values[identifier]).Returns("string");
                mock.Setup(x => x.Current).Returns(mockRecord.Object);

                var ex = Assert.Throws<FormatException>(() => mock.Object.GetValue<int>(identifier));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ReturnsCorrectValue()
            {
                const string stringIdentifier = "foo";
                const string intIdentifier = "bar";
                const string expectedStringValue = "string";
                const int expectedIntValue = 42;

                var mock = new Mock<IResultCursor>();
                var mockRecord = new Mock<IRecord>();
                mockRecord.Setup(x => x.Keys).Returns(new List<string> {stringIdentifier, intIdentifier});
                mockRecord.Setup(x => x.Values[stringIdentifier]).Returns(expectedStringValue);
                mockRecord.Setup(x => x.Values[intIdentifier]).Returns(expectedIntValue);
                mock.Setup(x => x.Current).Returns(mockRecord.Object);

                mock.Object.GetValue<string>(stringIdentifier).Should().Be(expectedStringValue);
                mock.Object.GetValue<int>(intIdentifier).Should().Be(expectedIntValue);
            }

            [Fact]
            public void ReturnsDefaultValue_WhenCurrentDoesNotHaveIdentifier()
            {
                const string stringIdentifier = "foo";
                const string intIdentifier = "bar";

                var mock = new Mock<IResultCursor>();
                mock.Setup(x => x.Current.Keys).Returns(new List<string>());

                mock.Object.GetValue<string>(stringIdentifier).Should().Be(default);
                mock.Object.GetValue<int>(intIdentifier).Should().Be(default);
            }
        }

        public class GetValueStrictTMethod
        {
            [Fact]
            public void ThrowsNullReferenceException_WhenCurrentIsNull()
            {
                var mock = new Mock<IResultCursor>();
                mock.Setup(x => x.Current).Returns((IRecord) null);
                var ex = Assert.Throws<NullReferenceException>(() => mock.Object.GetValueStrict<string>("foo"));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ThrowsArgumentNullException_WhenCursorIsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => ResultCursorExtensions.GetValueStrict<string>(null, "identifier"));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ThrowsArgumentNullException_WhenIdentifierIsNull()
            {
                var mock = new Mock<IResultCursor>();
                var ex = Assert.Throws<ArgumentNullException>(() => mock.Object.GetValueStrict<string>(null));
                ex.Should().NotBeNull();
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            public void ThrowsArgumentException_WhenIdentifierIsEmptyOrWhitespace(string identifier)
            {
                var mock = new Mock<IResultCursor>();
                var ex = Assert.Throws<ArgumentException>(() => mock.Object.GetValueStrict<string>(identifier));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ThrowsFormatException_WhenAttemptingToCastIncorrectly()
            {
                const string identifier = "foo";
                var mock = new Mock<IResultCursor>();
                var mockRecord = new Mock<IRecord>();
                mockRecord.Setup(x => x.Keys).Returns(new List<string> {identifier});
                mockRecord.Setup(x => x.Values[identifier]).Returns("string");
                mock.Setup(x => x.Current).Returns(mockRecord.Object);

                var ex = Assert.Throws<FormatException>(() => mock.Object.GetValueStrict<int>(identifier));
                ex.Should().NotBeNull();
            }

            [Fact]
            public void ReturnsCorrectValue()
            {
                const string stringIdentifier = "foo";
                const string intIdentifier = "bar";
                const string expectedStringValue = "string";
                const int expectedIntValue = 42;

                var mock = new Mock<IResultCursor>();
                var mockRecord = new Mock<IRecord>();
                mockRecord.Setup(x => x.Keys).Returns(new List<string> {stringIdentifier, intIdentifier});
                mockRecord.Setup(x => x.Values[stringIdentifier]).Returns(expectedStringValue);
                mockRecord.Setup(x => x.Values[intIdentifier]).Returns(expectedIntValue);
                mock.Setup(x => x.Current).Returns(mockRecord.Object);

                mock.Object.GetValueStrict<string>(stringIdentifier).Should().Be(expectedStringValue);
                mock.Object.GetValueStrict<int>(intIdentifier).Should().Be(expectedIntValue);
            }

            [Fact]
            public void ThrowsKeyNotFoundException_WhenCurrentDoesNotHaveIdentifier()
            {
                const string stringIdentifier = "foo";

                var mock = new Mock<IResultCursor>();
                mock.Setup(x => x.Current.Keys).Returns(new List<string>());

                var ex = Assert.Throws<KeyNotFoundException>(() => mock.Object.GetValueStrict<string>(stringIdentifier).Should().Be(default));
                ex.Should().NotBeNull();
            }
        }

        public class GetContentTMethod
        {
            [Fact]
            public async Task ThrowsArgumentNullException_WhenCursorIsNull()
            {
                var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                {
                    await foreach (var _ in ResultCursorExtensions.GetContent<string>(null, "identifier"))
                    {
                    }
                });

                ex.Should().NotBeNull();
            }

            [Fact]
            public async Task ThrowsArgumentNullException_WhenIdentifierIsNull()
            {
                var mock = new Mock<IResultCursor>();
                var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                {
                    await foreach (var _ in mock.Object.GetContent<string>(null))
                    {
                    }
                });

                ex.Should().NotBeNull();
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            public async Task ThrowsArgumentException_WhenIdentifierIsEmptyOrWhitespace(string identifier)
            {
                var mock = new Mock<IResultCursor>();
                var ex = await Assert.ThrowsAsync<ArgumentException>(async () =>
                {
                    await foreach (var _ in mock.Object.GetContent<string>(identifier))
                    {
                    }
                });

                ex.Should().NotBeNull();
            }

            [Fact]
            public async Task ThrowsFormatException_WhenIdentifierCanNotBeCastToTheTypeGiven()
            {
                const string identifier = "foo";
                const string expectedString = "string";

                var mock = new Mock<IResultCursor>();
                var mockRecord = new Mock<IRecord>();
                mockRecord.Setup(x => x.Keys).Returns(new List<string> { identifier });
                mockRecord.Setup(x => x.Values[identifier]).Returns(expectedString);

                mock.Setup(x => x.Current).Returns(mockRecord.Object);
                mock.Setup(x => x.FetchAsync()).Returns(Task.FromResult(true));

                var ex = await Assert.ThrowsAsync<FormatException>(async () =>
                {
                    await foreach (var _ in mock.Object.GetContent<int>(identifier))
                    {
                    }
                });
                ex.Should().NotBeNull();
            }

            [Fact]
            public async Task YieldsAllTheItems_WhenTheyCanBeConvertedToT()
            {
                const string identifier = "foo";
                const string expectedString = "string";

                var mock = new Mock<IResultCursor>();
                var mockRecord = new Mock<IRecord>();
                mockRecord.Setup(x => x.Keys).Returns(new List<string> { identifier });
                mockRecord.Setup(x => x.Values[identifier]).Returns(expectedString);

                mock.Setup(x => x.Current).Returns(mockRecord.Object);
                mock.Setup(x => x.FetchAsync()).Returns(Task.FromResult(true));
                
                await foreach (var item in mock.Object.GetContent<string>(identifier))
                {
                    item.Should().Be(expectedString);
                    mock.Reset();
                    mock.Setup(x => x.FetchAsync()).Returns(Task.FromResult(false));
                }
            }
        }

        public class GetRecordsMethod
        {
            [Fact]
            public async Task ThrowsArgumentNullException_WhenCursorIsNull()
            {
                var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                {
                    await foreach (var _ in ResultCursorExtensions.GetRecords(null))
                    {
                    }
                });
                ex.Should().NotBeNull();
            }

            [Fact]
            public async Task WillYieldAllTheRecords()
            {
                var mock = new Mock<IResultCursor>();
                var mockRecord = new Mock<IRecord>();
                mock.Setup(x => x.FetchAsync()).Returns(Task.FromResult(true));
                mock.Setup(x => x.Current).Returns(mockRecord.Object);

                int count = 0;
                await foreach (var _ in mock.Object.GetRecords())
                {
                    count++;
                    mock.Reset();
                    mock.Setup(x => x.FetchAsync()).Returns(Task.FromResult(false));
                }

                count.Should().Be(1);
            }
        }
    }
}