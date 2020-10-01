# Neo4j.Driver.Extensions

This is a collection of extension methods for the [Neo4j.Driver](https://github.com/neo4j/neo4j-dotnet-driver).

The aim is to make it a bit simpler to use, it doesn't cover all use cases at the 
moment, and is a work in progress that I'll update as and when new cases arrive.

This is NetStandard2.1 to allow the use of `IAsyncEnumerable` and as such - can't be 
used in any lower version.

## Current Build: [![Build status](https://ci.appveyor.com/api/projects/status/c0fy341t3a33lp23?svg=true)](https://ci.appveyor.com/project/ChrisSkardon/neo4j-driver-extensions)

Available on [Nuget](https://www.nuget.org/packages/Neo4j.Driver.Extensions/)

## Usage

For the below examples, the `Movie` class looks like:

```
public class Movie
{
    [Neo4jProperty(Name = "title")]
    public string Title { get; set; }

    [Neo4jProperty(Name = "released")]
    public long? Released { get; set; }

    [Neo4jProperty(Name = "tagline")]
    public string Tagline { get; set; }

    public static string Labels => nameof(Movie);
}
```

### No-Extension Approach

If you use the `Neo4j.Driver` to access your data, you have to write code like this:

```
var session = _driver.AsyncSession();
var results = await session.ReadTransactionAsync(async x =>
{
    var cursor = await x.RunAsync($"MATCH (m:{Movie.Labels}) WHERE m.title = $title RETURN m", new { title });
    var fetched = await cursor.FetchAsync();

    while (fetched)
    {
        var node = cursor.Current["m"].As<INode>();
        var movie = new Movie
        {
            Title = node.Properties.ContainsKey("title") ? node.Properties["title"].As<string>() : null,
            Tagline = node.Properties.ContainsKey("tagline") ? node.Properties["tagline"].As<string>() : null,
            Released = node.Properties.ContainsKey("released") ? node.Properties["released"].As<int?>() : null
        };
        return movie;
    }

    return null;
});

return results;
```

There is a lot of boiler plate code around there, and some you'll want to keep, but the aim of this
project is to make that a bit simpler to use. The examples below will take the above code and show how
you can use the extensions to make it easier to read.

### Node - GetValue method

The first thing we can do is make the getting of values better, so, let's take the `Properties` getters:

```
Title = node.Properties.ContainsKey("title") ? node.Properties["title"].As<string>() : null,
```

and replace with:

```
Title = node.GetValue<string>("title"),
```

or

```
Title = node.GetValueStrict<string>("title"),
```

`GetValue<T>` will return the value from the `INode`, or a default, so for `string` the default is `null` for `int` the default is `0`. 
`GetValueStrict<T>` will return the value, _or_ throw a `KeyNotFoundException` exception if the value isn't there. So choose whichever makes
most sense in your code.

**Issues** - This hasn't been tested with `IEnumerable` properties, at the moment, just primitives.

### Node - ToObject<T> method

We can take the entire `Movie` creation code:

```
var movie = new Movie
{
    Title = node.Properties.ContainsKey("title") ? node.Properties["title"].As<string>() : null,
    Tagline = node.Properties.ContainsKey("tagline") ? node.Properties["tagline"].As<string>() : null,
    Released = node.Properties.ContainsKey("released") ? node.Properties["released"].As<int?>() : null
};
```

and change it to:

```
var movie = node.ToObject<Movie>();
```

**Issues** - As with `GetValue<T>` this only copes with primitive properties at the moment!

### Neo4jPropertyAttribute

This allows you to decorate your class' properties with attributes to let the `ToObject<T>` method know how you want to read your data.

To use a different name in the database to your classes, use the `Name` property:

```
[Neo4jProperty(Name = "title")]
public string Title { get; set; }
```

To ignore a property, so no attempt is made to read it, use the `Ignore` property:

```
[Neo4jProperty(Ignore = true)]
public string Title { get; set; }
```