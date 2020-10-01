namespace Neo4j.Drivers.Extensions.Tests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Neo4j.Driver.Extensions;
    using Xunit;

    public class AsyncSessionExtensionsTests
    {
        public class RunReadTransactionForNodeResultsMethod
        {
            [Fact]
            public async Task ThrowsArgumentNullException_WhenSessionIsNull()
            {
                var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => await AsyncSessionExtensions.RunReadTransactionForNodeResults<Foo>(null, "query", null, "identifier"));
                ex.Should().NotBeNull();
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
        }

        public class RunReadTransactionEnumerableMethod
        {
            [Fact]
            public async Task ThrowsArgumentNullException_WhenSessionIsNull()
            {
                var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => await AsyncSessionExtensions.RunReadTransactionEnumerable<Foo>(null, "query", null, "identifier"));
                ex.Should().NotBeNull();
            }
        }
    }
}