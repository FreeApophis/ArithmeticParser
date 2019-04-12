﻿using System.IO;
using ArithmeticParser.Tokens;
using Funcky.Monads;

namespace ArithmeticParser.Lexing
{
    public interface ILexerRule
    {
        int Weight { get; }
        Option<IToken> Match(TextReader reader);
    }
}