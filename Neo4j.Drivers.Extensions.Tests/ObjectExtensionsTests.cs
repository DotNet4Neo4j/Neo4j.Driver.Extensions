namespace Neo4j.Drivers.Extensions.Tests
{
    using FluentAssertions;
    using Neo4j.Driver.Extensions;
    using Xunit;

    public class ObjectExtensionsTests
    {
        public class TryAsTMethod
        {
            [Fact]
            public void ReturnsFalse_WhenConversionIsNotPossible_FormatException()
            {
                const string expected = "A String";
                object obj = expected;

                var actual = obj.TryAs<int>(out _);
                actual.Should().BeFalse();
            }

            [Fact]
            public void ReturnsFalse_WhenConversionIsNotPossible_InvalidCastException()
            {
                object obj = null;

                var actual = obj.TryAs<int>(out _);
                actual.Should().BeFalse();
            }

            [Fact]
            public void SetsValueToDefault_WhenConversionIsNotPossible_FormatException()
            {
                const string expected = "A String";
                object obj = expected;

                obj.TryAs<int>(out var actual);
                actual.Should().Be(default);
            }

            [Fact]
            public void SetsValueToDefault_WhenConversionIsNotPossible_InvalidCastException()
            {
                object obj = null;

                obj.TryAs<int>(out var actual);
                actual.Should().Be(default);
            }


            [Fact]
            public void SetsTheCorrectValue_WhenAllOk()
            {
                const string expected = "A String";
                object obj = expected;

                obj.TryAs<string>(out var actual);
                actual.Should().Be(expected);
            }

            [Fact]
            public void ReturnsTrue_WhenAllOk()
            {
                const string expected = "A String";
                object obj = expected;

                var actual = obj.TryAs<string>(out _);
                actual.Should().BeTrue();
            }
        }
    }
}