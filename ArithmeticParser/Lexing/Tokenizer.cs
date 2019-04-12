﻿using System;
using System.Collections.Generic;
using System.Linq;
using ArithmeticParser.Tokens;
using Funcky.Monads;

namespace ArithmeticParser.Lexing
{
    public class Tokenizer
    {
        private readonly ILexerRules _lexerRules;
        private readonly Func<string, ILexerReader> _newLexerReader;

        public Tokenizer(ILexerRules lexerRules, Func<string, ILexerReader> newLexerReader)
        {
            _lexerRules = lexerRules;
            _newLexerReader = newLexerReader;
        }

        public IEnumerable<IToken> Scan(string expression)
        {
            var reader = _newLexerReader(expression);

            while (reader.Peek().Match(false, c => true))
            {
                yield return SelectLexerRule(reader)
                    .Match(
                        none: () => throw new UnknownTokenException(),
                        some: t => t
                        );
            }
        }

        private Option<IToken> SelectLexerRule(ILexerReader reader)
        {
            return _lexerRules
                .GetRules()
                .OrderByDescending(rule => rule.Weight)
                .Select(rule => rule.Match(reader))
                .First(mt => mt.Match(none: false, some: t => true));
        }
    }
}
