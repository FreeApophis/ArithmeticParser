﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using apophis.Lexer.Tokens;

namespace apophis.Lexer
{
    public class TokenWalker
    {
        private readonly Tokenizer _tokenizer;
        private readonly Func<IToken> _newEpsilonToken;
        private readonly LinePositionCalculator.Factory _newLinePositionCalculator;
        private List<Lexem> _lexems = new List<Lexem>();
        private ILinePositionCalculator? _linePositionCalculator;

        private int _currentIndex;

        public TokenWalker(Tokenizer tokenizer, Func<IToken> newEpsilonToken, LinePositionCalculator.Factory newLinePositionCalculator)
        {
            _tokenizer = tokenizer;
            _newEpsilonToken = newEpsilonToken;
            _newLinePositionCalculator = newLinePositionCalculator;
        }

        public void Scan(string expression)
        {
            Scan(expression, t => t);
        }

        public void Scan(string expression, Func<IEnumerable<Lexem>, IEnumerable<Lexem>> postProcessTokens)
        {
            _currentIndex = 0;
            var unfilteredLexems = _tokenizer.Scan(expression);
            _linePositionCalculator = _newLinePositionCalculator(unfilteredLexems);
            _lexems = postProcessTokens(unfilteredLexems).ToList();
        }

        public Lexem Pop()
        {
            return ValidToken()
                ? _lexems[_currentIndex++]
                : new Lexem(_newEpsilonToken(), new Position(0, 0));
        }

        public Lexem Peek(int lookAhead = 0)
        {
            Debug.Assert(lookAhead >= 0, "a negative look ahead is not supported");

            return ValidToken(lookAhead)
                ? _lexems[_currentIndex + lookAhead]
                : new Lexem(_newEpsilonToken(), new Position(0, 0));
        }

        public LinePosition CalculateLinePosition(Lexem lexem)
        {
            if (_linePositionCalculator == null)
            {
                throw new Exception("Call Scan first before you try to calculate a position.");
            }

            return _linePositionCalculator.CalculateLinePosition(lexem);
        }

        private bool ValidToken(int lookAhead = 0) => _currentIndex + lookAhead < _lexems.Count;
    }
}
