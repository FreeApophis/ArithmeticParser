﻿using ArithmeticParser.Visitors;

namespace ArithmeticParser.Nodes
{
    public class DivisionOperator : BinaryOperator
    {
        internal DivisionOperator(IParseNode leftOperand, IParseNode rightOperand) :
            base(leftOperand, rightOperand)
        {
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return "/";
        }
    }
}