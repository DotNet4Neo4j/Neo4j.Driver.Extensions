namespace Neo4j.Drivers.Extensions.Tests
{
    using System;
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
                //i.e. a 'string' to an int.
                throw new NotImplementedException();
            }

            [Fact]
            public void ReturnsCorrectValue()
            {
                throw new NotImplementedException();
            }

            [Fact]
            public void ReturnsDefaultValue_WhenCurrentDoesNotHaveIdentifier()
            {
                throw new NotImplementedException();
            }
        }

        public class GetValueStrictTMethod
        {
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
                //i.e. a 'string' to an int.
                throw new NotImplementedException();
            }

            [Fact]
            public void ReturnsCorrectValue()
            {
                throw new NotImplementedException();
            }

            [Fact]
            public void ThrowsKeyNotFoundException_WhenCurrentDoesNotHaveIdentifier()
            {
                throw new NotImplementedException();
            }

        }

        public class GetContentTMethod
        {
            [Fact]
            public async Task ThrowsArgumentNullException_WhenCursorIsNull()
            {
                var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                {
                    await foreach (var _ in ResultCursorExtensions.GetContent<string>(null, "identifier")) { }
                });

                ex.Should().NotBeNull();
            }

            [Fact]
            public async Task ThrowsArgumentNullException_WhenIdentifierIsNull()
            {
                var mock = new Mock<IResultCursor>();
                var ex = await Assert.ThrowsAsync<ArgumentException>(async () =>
                {
                    await foreach (var _ in mock.Object.GetContent<string>(null)) { }
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
                    await foreach (var _ in mock.Object.GetContent<string>(identifier)) { }
                });

                ex.Should().NotBeNull();
            }
        }

        public class GetRecordsMethod
        {
            [Fact]
            public async Task ThrowsArgumentNullException_WhenCursorIsNull()
            {
                var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => {
                    await foreach (var _ in ResultCursorExtensions.GetRecords(null)) {}
                });
                ex.Should().NotBeNull();
            }

           
        }

    }
}