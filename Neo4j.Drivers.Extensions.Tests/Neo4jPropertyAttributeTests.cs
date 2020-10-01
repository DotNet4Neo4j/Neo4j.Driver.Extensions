namespace Neo4j.Drivers.Extensions.Tests
{
    using FluentAssertions;
    using Neo4j.Driver.Extensions;
    using Xunit;

    public class Neo4jPropertyAttributeTests
    {
        [Theory]
        [InlineData(true,true)]
        [InlineData(false,false)]
        public void SetsIgnoreToWhatItShouldBe(bool setting, bool expected)
        {
            var attribute = new Neo4jPropertyAttribute{Ignore = setting};
            attribute.Ignore.Should().Be(expected);
        }

        [Fact]
        public void SetsNameToWhatItShouldBe()
        {
            const string expected = "foo";
            var attribute = new Neo4jPropertyAttribute { Name=expected };
            attribute.Name.Should().Be(expected);
        }
    }
}