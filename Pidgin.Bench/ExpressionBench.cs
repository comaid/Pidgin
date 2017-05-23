using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Pidgin.Expression;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Pidgin.Bench
{
    [Config(typeof(Config))]
    public class ExpressionBench
    {
        private string _bigExpression;
        private Parser<char, int> _leftAssoc;
        private Parser<char, int> _rightAssoc;

        [Setup]
        public void Setup()
        {
            var numbers = Enumerable.Range(1, 1000);
            _bigExpression = "0" + numbers.Aggregate("", (x, y) => x + "+" + y);

            var term = Parser.Digit.Many().Select(cs => int.Parse(string.Concat(cs)));
            var infixL = Operator.InfixL(Parser.Char('+').Then(Return<Func<int, int, int>>((x, y) => x + y)));
            _leftAssoc = ExpressionParser.Build(
                term,
                new[] { new[] { infixL } }
            );
            var infixR = Operator.InfixR(Parser.Char('+').Then(Return<Func<int, int, int>>((x, y) => x + y)));
            _rightAssoc = ExpressionParser.Build(
                term,
                new[] { new[] { infixR } }
            );
        }
    
        [Benchmark]
        public void InfixL()
        {
            _leftAssoc.ParseOrThrow(_bigExpression);
        }
        [Benchmark]
        public void InfixR()
        {
            _rightAssoc.ParseOrThrow(_bigExpression);
        }
    }
}