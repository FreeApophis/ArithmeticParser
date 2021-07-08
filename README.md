# ArithmeticParser

Very Simple Proof of Concept Parser for Simple Arithmetic Expressions.

[![Build Status](https://travis-ci.org/FreeApophis/ArithmeticParser.svg?branch=master)](https://travis-ci.org/FreeApophis/ArithmeticParser) 
[![Parser NuGet package](https://buildstats.info/nuget/SimpleArithmeticParser)](https://www.nuget.org/packages/SimpleArithmeticParser)
[![Lexer NuGet package](https://buildstats.info/nuget/apophis.Lexer)](https://www.nuget.org/packages/apophis.Lexer/)

This is an example of a simple but clean recursive descent parser which builds an abstract syntax tree.

It illustrates the usage of the visitor pattern to traverse the syntax tree.

Supported Operations:

* Binary Operations
  * Addition
  * Subtraction
  * Multiplication
  * Division
  * Modula
  * Power
* Function Calls
* Variables
  * Constants

Sample vistors include:

* CalculateVisitor: calculates the result of the arithmetic expression as a double.
* Parenthesizer: several visitors to create infix notation from a syntax tree.
* RPN: Output as a postfix syntax tree also known as reverse polish notation
* GraphViz: visualize parse tree with GraphViz

![Example 1](https://raw.githubusercontent.com/FreeApophis/ArithmeticParser/master/example/parsetree.png)

Some Simple Tests are included, but no exhaustive test suite, the parser is intendet only as an academic example with a well designed architecture.
