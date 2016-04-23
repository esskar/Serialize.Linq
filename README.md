# Serialize.Linq

Serialize.Linq is a c# library for serializing linq expressions. 
Formats it supports (but it is not limited to) are JSON, XML and BINARY.

## Installation
Simple install via [NuGet][1]:

    PM> Install-Package Serialize.Linq

## Building the source

    git clone https://github.com/esskar/Serialize.Linq.git

After cloning the repository, run 

    cd src\
    build.cmd

__NOTE__: Opening the solution requires VS 2012.

## Examples
There is an working WCF and REST API example included in the [examples folder][5].
You may also find some examples on [esskar's][2] [blog][3].

## Help and Support
If you have a feature request, a bug or any other question, just create an [issue][4].
For bugs: make sure you create a unit test, so it is easier for me to reproduce and fix it.

## Supported Platforms (or known to work with)
* .NET 4.0
* .NET 4.5
* .NET 4.5.x
* .NET 4.6.x
* DNX 4.5.x
* DNXCORE 5.0
* DOTNET 5.4


* Silverlight 5.0
* Windows Phone 8.0

[1]: http://nuget.org/packages/Serialize.Linq
[2]: https://github.com/esskar
[3]: http://blog.esskar.de/tags/serialize.linq.html
[4]: https://github.com/esskar/Serialize.Linq/issues
[5]: https://github.com/esskar/Serialize.Linq/tree/master/src/Serialize.Linq.Examples
