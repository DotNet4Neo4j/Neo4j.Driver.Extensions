namespace Neo4j.Drivers.Extensions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Neo4j.Driver;
    using Neo4j.Driver.Extensions;
    using Xunit;

    public class AsyncSessionExtensionsTests
    {
        public class RunReadTransactionForObjectsMethod
        {
            [Fact]
            public async Task ThrowsArgumentNullException_WhenSessionIsNull()
            {
                var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => await AsyncSessionExtensions.RunReadTransactionForObjects<Foo>(null, "query", null, "identifier"));
                ex.Should().NotBeNull();
            }

            [Fact]
            public async Task ThrowsArgumentNullException_WhenQueryIsNull()
            {
                var mock = new Mock<IAsyncSession>();
                var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => await mock.Object.RunReadTransactionForObjects<Foo>(null, null, "identifier"));
                ex.Should().NotBeNull();
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            public async Task ThrowsArgumentException_WhenQueryIsEmptyOrWhitespace(string query)
            {
                var mock = new Mock<IAsyncSession>();
                var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await mock.Object.RunReadTransactionForObjects<Foo>(query, null, "identifier"));
                ex.Should().NotBeNull();
            }

            [Fact]
            public async Task ThrowsArgumentNullException_WhenIdentifierIsNull()
            {
                var mock = new Mock<IAsyncSession>();
                var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => await mock.Object.RunReadTransactionForObjects<Foo>("query", null, null));
                ex.Should().NotBeNull();
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            public async Task ThrowsArgumentException_WhenIdentifierIsEmptyOrWhitespace(string identifier)
            {
                var mock = new Mock<IAsyncSession>();
                var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await mock.Object.RunReadTransactionForObjects<Foo>("query", null, identifier));
                ex.Should().NotBeNull();
            }

            [Fact]
            public async Task CallsReadTransactionAsync()
            {
                var mock = new Mock<IAsyncSession>();
                await mock.Object.RunReadTransactionForObjects<Foo>("query", null, "id");
                mock.Verify(x => x.ReadTransactionAsync(It.IsAny<Func<IAsyncTransaction, Task<List<Foo>>>>()), Times.Once);
            }

            [Fact]
            public async Task GetsResults()
            {
                const string identifier = "foo";
                const string expectedStringProperty = "string";
                
                var mockSession = new Mock<IAsyncSession>();
                var mockTransaction = new Mock<IAsyncTransaction>();
                var mockCursor = new Mock<IResultCursor>();
                var mockRecord = new Mock<IRecord>();
                
                mockRecord.Setup(x => x.Keys).Returns(new List<string> { identifier });
                mockRecord.Setup(x => x[identifier]).Returns(new Dictionary<string,object>
                {
                    {nameof(Foo.StringProperty), expectedStringProperty}
                });
                mockCursor.Setup(x => x.FetchAsync()).Returns(Task.FromResult(true))
                    .Callback(() =>
                    {
                        mockCursor.Reset();
                        mockCursor.Setup(x => x.Current).Returns(mockRecord.Object);
                        mockCursor.Setup(x => x.FetchAsync()).Returns(Task.FromResult(false));
                    });

                mockTransaction
                    .Setup(x => x.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
                    .Returns(Task.FromResult(mockCursor.Object));

                List<Foo> callBackResponse = null;
                mockSession
                    .Setup(x => x.ReadTransactionAsync(It.IsAny<Func<IAsyncTransaction, Task<List<Foo>>>>()))
                    .Callback(async (Func<IAsyncTransaction, Task<List<Foo>>> func) => callBackResponse = await func(mockTransaction.Object));

                await mockSession.Object.RunReadTransactionForObjects<Foo>("Query", null, identifier);
                callBackResponse.Should().NotBeNull();
                callBackResponse.Should().HaveCount(1);
                callBackResponse[0].StringProperty.Should().Be(expectedStringProperty);
            }
        }


        public class RunReadTransactionMethod
        {
            [Fact]
            public async Task ThrowsArgumentNullException_WhenSessionIsNull()
            {
                var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => await AsyncSessionExtensions.RunReadTransaction<Foo>(null, "query", null, "identifier"));
                ex.Should().NotBeNull();
            }

            [Fact]
            public async Task ThrowsArgumentNullException_WhenQueryIsNull()
            {
                var mock = new Mock<IAsyncSession>();
                var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => await mock.Object.RunReadTransaction<Foo>(null, null, "identifier"));
                ex.Should().NotBeNull();
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            public async Task ThrowsArgumentException_WhenQueryIsEmptyOrWhitespace(string query)
            {
                var mock = new Mock<IAsyncSession>();
                var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await mock.Object.RunReadTransaction<Foo>(query, null, "identifier"));
                ex.Should().NotBeNull();
            }

            [Fact]
            public async Task ThrowsArgumentNullException_WhenIdentifierIsNull()
            {
                var mock = new Mock<IAsyncSession>();
                var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => await mock.Object.RunReadTransaction<Foo>("query", null, null));
                ex.Should().NotBeNull();
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            public async Task ThrowsArgumentException_WhenIdentifierIsEmptyOrWhitespace(string identifier)
            {
                var mock = new Mock<IAsyncSession>();
                var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await mock.Object.RunReadTransaction<Foo>("query", null, identifier));
                ex.Should().NotBeNull();
            }

            [Fact]
            public async Task CallsReadTransactionAsync()
            {
                var mock = new Mock<IAsyncSession>();
                await mock.Object.RunReadTransaction<string>("query", null, "id");
                mock.Verify(x => x.ReadTransactionAsync(It.IsAny<Func<IAsyncTransaction, Task<List<string>>>>()), Times.Once);
            }

            [Fact]
            public async Task ReturnsCorrectValues()
            {
                const string identifier = "foo";
                const string expectedString = "string";

                var mockSession = new Mock<IAsyncSession>();
                var mockTransaction = new Mock<IAsyncTransaction>();
                var mockCursor = new Mock<IResultCursor>();
                var mockRecord = new Mock<IRecord>();

                mockRecord.Setup(x => x.Keys).Returns(new List<string> { identifier });
                mockRecord.Setup(x => x.Values[identifier]).Returns(expectedString);
                mockCursor.Setup(x => x.FetchAsync()).Returns(Task.FromResult(true))
                    .Callback(() =>
                    {
                        mockCursor.Reset();
                        mockCursor.Setup(x => x.Current).Returns(mockRecord.Object);
                        mockCursor.Setup(x => x.FetchAsync()).Returns(Task.FromResult(false));
                    });

                mockTransaction
                    .Setup(x => x.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
                    .Returns(Task.FromResult(mockCursor.Object));

                List<string> callBackResponse = null;
                mockSession
                    .Setup(x => x.ReadTransactionAsync(It.IsAny<Func<IAsyncTransaction, Task<List<string>>>>()))
                    .Callback(async (Func<IAsyncTransaction, Task<List<string>>> func) => callBackResponse = await func(mockTransaction.Object));

                await mockSession.Object.RunReadTransaction<string>("Query", null, identifier);
                callBackResponse.Should().NotBeNull();
                callBackResponse.Should().HaveCount(1);
                callBackResponse[0].Should().Be(expectedString);
            }
        }


        public class ReadTransactionAsListTMethod
        {
            [Fact]
            public async Task Works()
            {
                const string identifier = "foo";
                const string expectedString = "string";

                var mockTransaction = new Mock<IAsyncTransaction>();
                var mockCursor = new Mock<IResultCursor>();
                var mockRecord = new Mock<IRecord>();

                mockRecord.Setup(x => x.Keys).Returns(new List<string> { identifier });
                mockRecord.Setup(x => x.Values[identifier]).Returns(expectedString);
                mockCursor.Setup(x => x.Current).Returns(mockRecord.Object);
                mockCursor.Setup(x => x.FetchAsync()).Returns(Task.FromResult(true))
                    .Callback(() =>
                    {
                        mockCursor.Reset();
                        mockCursor.Setup(x => x.FetchAsync()).Returns(Task.FromResult(false));
                    });

                mockTransaction
                    .Setup(x => x.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
                    .Returns(Task.FromResult(mockCursor.Object));


                var results = await mockTransaction.Object.ReadTransactionAsList<string>("Query", null, cursor => expectedString);
                results.Should().NotBeNull();

                var resultsList = results.ToList();
                resultsList.Should().HaveCount(1);
                resultsList[0].Should().Be(expectedString);
            }
        }
    }
}