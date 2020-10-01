namespace Neo4j.Drivers.Extensions.Tests
{
    using Neo4j.Driver.Extensions;

    internal class Foo
    {
        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedMember.Global
        public string StringProperty { get; set; }
        [Neo4jProperty(Name = "stringPropertyWithAttribute")]
        public string StringPropertyWithAttribute { get; set; }
        [Neo4jProperty(Ignore = true)]
        public string StringPropertyIgnored { get; set; }
        // ReSharper restore UnusedMember.Global
        // ReSharper restore UnusedMember.Local
    }
}