﻿using System;
using ArithmeticParser.Nodes;
using ArithmeticParser.Parsing;
using ArithmeticParser.Tokens;

namespace ArithmeticParser
{
    /// <summary>
    /// This is a Recursive Descent Parser for arithmetic expressions with real numbers with the following grammar in EBNF
    ///
    /// Expression := [ "-" ] Term { ("+" | "-") Term }
    /// Term       := Factor { ( "*" | "/" ) Factor }
    /// Factor     := RealNumber | "(" Expression ") | Variable | Function "
    /// Function   := Identifier "(" Expression { "," Expression } ")"
    /// Variable   := Identifier
    /// Identifier := Non-digit character { Any non whitespace }
    /// RealNumber := Digit{Digit} | [Digit] "." {Digit}
    /// Digit      := "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
    /// </summary>
    public class Parser : IParser
    {
        private readonly string _expression;
        private TokenWalker _walker;
        private IParseNode _parseTree;

        public Parser(string expression)
        {
            _expression = expression;
        }

        public IParseNode Parse()
        {
            var tokens = new Tokenizer().Scan(_expression);
            _walker = new TokenWalker(tokens);

            _parseTree = ParseExpression();
            return _parseTree;
        }

        // Expression := [ "-" ] Term { ("+" | "-") Term }
        public IParseNode ParseExpression()
        {
            IParseNode result;
            if (NextIs<MinusToken>())
            {
                _walker.Pop();
                result = new UnaryMinusOperator(ParseTerm());
            }
            else
            {
                result = ParseTerm();
            }
            while (NextIsMinusOrPlus())
            {
                var op = _walker.Pop();
                switch (op)
                {
                    case MinusToken _:
                        result = new MinusOperator(result, ParseTerm());
                        break;
                    case PlusToken _:
                        result = new PlusOperator(result, ParseTerm());
                        break;
                }
            }
            return result;
        }

        // Term       := Factor { ( "*" | "/" ) Factor }
        private IParseNode ParseTerm()
        {
            var result = ParseFactor();
            while (NextIsMultiplicationOrDivision())
            {
                var op = _walker.Pop();
                switch (op)
                {
                    case DivideToken _:
                        result = new DivisionOperator(result, ParseFactor());
                        break;
                    case MultiplicationToken _:
                        result = new MultiplicationOperator(result, ParseFactor());
                        break;
                }
            }

            return result;
        }

        // Factor     := RealNumber | "(" Expression ") | Variable | Function "
        private IParseNode ParseFactor()
        {
            if (NextIs<NumberToken>())
            {
                return new NumberNode(GetNumber());
            }

            if (NextIs<IdentifierToken>())
            {
                if (NextIs<OpenParenthesisToken>(1))
                {
                    return ParseFunction();
                }
                else
                {
                    return ParseVariable();
                }
            }

            ExpectOpeningParenthesis();
            var result = ParseExpression();
            ExpectClosingParenthesis();

            return result;
        }

        // Function   := Identifier "(" Expression { "," Expression } ")"
        private IParseNode ParseFunction()
        {
            if (_walker.Pop() is IdentifierToken identifier)
            {

                FunctionNode function = new FunctionNode(identifier.Name);

                // Pop opening parenthesis
                Consume(typeof(OpenParenthesisToken));

                function.Parameters.Add(ParseExpression());
                while (NextIs<CommaToken>())
                {
                    Consume(typeof(CommaToken));
                    function.Parameters.Add(ParseExpression());
                }

                Consume(typeof(ClosedParenthesisToken));
                return function;
            }

            throw new ArgumentNullException();
        }

        // Variable   := Identifier
        private IParseNode ParseVariable()
        {
            if (_walker.Pop() is IdentifierToken identifier)
            {
                return new VariableNode(identifier.Name);
            }

            throw new ArgumentNullException();
        }


        private void ExpectClosingParenthesis()
        {
            if (!(NextIs<ClosedParenthesisToken>()))
            {
                throw new Exception("Expecting ')' in expression, instead got: " + (PeekNext() != null ? PeekNext().ToString() : "End of expression"));
            }
            _walker.Pop();
        }

        private void ExpectOpeningParenthesis()
        {
            if (!NextIs<OpenParenthesisToken>())
            {
                throw new Exception("Expecting Real number or '(' in expression, instead got : " + (PeekNext() != null ? PeekNext().ToString() : "End of expression"));
            }
            _walker.Pop();
        }

        private IToken PeekNext()
        {
            return _walker.Peek();
        }

        private double GetNumber()
        {
            var next = _walker.Pop();

            if (next is NumberToken nr)
            {
                return nr.Value;

            }

            throw new Exception("Expecting Real number but got " + next);
        }

        private void Consume(Type type)
        {
            var token = _walker.Pop();
            if (token.GetType() != type)
            {
                throw new Exception($"Expecting {type} but got {token} ");
            }
        }

        private bool NextIs<TType>(int lookAhead = 0)
        {
            return _walker.Peek(lookAhead) is TType;
        }

        private bool NextIsMinusOrPlus(int lookAhead = 0)
        {
            return NextIs<MinusToken>(lookAhead) || NextIs<PlusToken>(lookAhead);
        }

        private bool NextIsMultiplicationOrDivision(int lookAhead = 0)
        {
            return NextIs<MultiplicationToken>(lookAhead) || NextIs<DivideToken>(lookAhead);
        }
    }
}
