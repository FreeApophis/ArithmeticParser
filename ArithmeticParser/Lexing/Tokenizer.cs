﻿using System.Collections.Generic;
using System.IO;
using ArithmeticParser.Tokens;

namespace ArithmeticParser.Lexing
{
    public class Tokenizer
    {
        private readonly LexerRules _lexerRules;

        public Tokenizer(LexerRules lexerRules)
        {
            _lexerRules = lexerRules;
        }

        public IEnumerable<IToken> Scan(string expression)
        {
            var reader = new StringReader(expression);

            while (reader.Peek() != -1)
            {
                foreach (var lexerRule in _lexerRules.GetRules())
                {
                    var token = lexerRule
                        .Match(reader)
                        .Match(() => null as IToken, t => t);

                    if (token != null && token.GetType() != typeof(WhiteSpaceToken))
                    {
                        yield return token;
                    }
                }
            }
        }
    }
}
