# Serialize.Linq

Serialize.Linq is a .NET library that provides functionality to serialize and deserialize LINQ expressions.
This library is useful in scenarios where you need to send LINQ expressions over the wire or persist them in a database.

## Features

- Serialize and deserialize LINQ expressions to XML, JSON, and binary formats.
- Supports various types of expressions, including binary, unary, member access, lambda, and more.
- Extensible design that allows you to add support for custom expressions.

## Getting Started

### Installation

You can install Serialize.Linq via [NuGet][1]:

```
Install-Package Serialize.Linq
```

### Usage

Here's a simple example of how to use Serialize.Linq:

```csharp
// Create an expression
Expression<Func<int, bool>> expression = num => num < 5;

// Create a serializer
var serializer = new ExpressionSerializer(new JsonSerializer());

// Serialize the expression
string serializedExpression = serializer.SerializeText(expression);

// Deserialize the expression
var deserializedExpression = serializer.DeserializeText(serializedExpression);
```

## Contributing

We welcome contributions to Serialize.Linq!
If you'd like to contribute, please fork the repository, make your changes, and submit a pull request.
If you're not sure where to start, take a look at our open issues.

For bugs: make sure you create a unit test, so it is easier for me to reproduce and fix it.

You can always [buy me a coffee :coffee:][2].

## Testing

Serialize.Linq has a comprehensive test suite. You can run the tests using your preferred test runner.

## Supported Platforms (or known to work with)

- .NET 8.0
- .NET 7.0
- .NET 6.0
- .NET 4.8
- .NET 4.8.1

## License

Serialize.Linq is licensed under the MIT License. See the LICENSE file for more details.

[1]: http://nuget.org/packages/Serialize.Linq
[2]: https://www.buymeacoffee.com/esskar
