Pegasus
=======

Pegasus is a PEG-style parser generator for C# that integrates with MSBuild and Visual Studio.

[![MIT Licensed](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://github.com/otac0n/Pegasus/blob/master/license.md)
[![Appveyor Build](https://img.shields.io/appveyor/ci/otac0n/Pegasus.svg?style=flat-square)](https://ci.appveyor.com/project/otac0n/pegasus)
[![Codecov](https://img.shields.io/codecov/c/github/otac0n/Pegasus.svg?style=flat-square)](https://codecov.io/gh/otac0n/Pegasus)
[![Get it on NuGet](https://img.shields.io/nuget/v/Pegasus.svg?style=flat-square)](http://nuget.org/packages/Pegasus)
[![Join the Chat](https://img.shields.io/badge/chat-join-brightgreen.svg?style=flat-square)](https://gitter.im/otac0n/Pegasus)

Getting Started
---------------

The easiest way to get a copy of Pegasus is to install the [Pegasus NuGet package](http://nuget.org/packages/Pegasus) in Visual Studio.

    PM> Install-Package Pegasus

Due to a limitation in Visual Studio, you will need to reload your project for the 'PegGrammar' build action to be recognized.

Once you have the package installed, files in your project marked as 'PegGrammar' in the properties window will be compiled to their respective `.peg.cs` parser classes before every build.  These parser classes will be automatically included in compilation.

For help with grammar syntax, see [the syntax guide wiki entry](https://github.com/otac0n/Pegasus/wiki/Syntax-Guide)

Example
-------

Here is an example of a simple parser for mathematical expressions:

    @namespace MyProject
    @classname ExpressionParser

    additive <double> -memoize
        = left:additive "+" right:multiplicative { left + right }
        / left:additive "-" right:multiplicative { left - right }
        / multiplicative

    multiplicative <double> -memoize
        = left:multiplicative "*" right:power { left * right }
        / left:multiplicative "/" right:power { left / right }
        / power

    power <double>
        = left:primary "^" right:power { Math.Pow(left, right) }
        / primary

    primary <double> -memoize
        = decimal
        / "-" primary:primary { -primary }
        / "(" additive:additive ")" { additive }

    decimal <double>
        = value:([0-9]+ ("." [0-9]+)?) { double.Parse(value) }

This will take mathematical expressions as strings and evaluate them with the proper order of operations and associativity to produce a result as a decimal.

The above parser would be used like so:

    var parser = new MyProject.ExpressionParser();
    var result = parser.Parse("5.1+2*3");
    Console.WriteLine(result); // Outputs "11.1".
