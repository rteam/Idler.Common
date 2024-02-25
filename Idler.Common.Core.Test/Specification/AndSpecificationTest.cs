using System;
using System.Linq.Expressions;
using Idler.Common.Core.Specification;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Idler.Common.Core.Test.Specification
{
    [TestClass]
    public class AndSpecificationTest
    {
        private class SpecTrue : ISpecification<int>
        {
            public bool IsSatisfiedBy(int predicate) => true;
            public ISpecification<int> And(ISpecification<int> other) => new AndSpecification<int>(this, other);
            public ISpecification<int> Or(ISpecification<int> other) => this;
            public ISpecification<int> Not() => new SpecFalse();
            public Expression<Func<int, bool>> Expressions => x => true;
        }

        private class SpecFalse : ISpecification<int>
        {
            public bool IsSatisfiedBy(int predicate) => false;
            public ISpecification<int> And(ISpecification<int> other) => this;
            public ISpecification<int> Or(ISpecification<int> other) => other;
            public ISpecification<int> Not() => new SpecTrue();
            public Expression<Func<int, bool>> Expressions => x => false;
        }

        [TestMethod]
        public void ExpressionsTest()
        {
            var specTrue = new SpecTrue();
            var specFalse = new SpecFalse();

            var andSpecTrueTrue = new AndSpecification<int>(specTrue, specTrue);
            var result = andSpecTrueTrue.Expressions.Compile()(0);
            Assert.IsTrue(result);

            var andSpecTrueFalse = new AndSpecification<int>(specTrue, specFalse);
            result = andSpecTrueFalse.Expressions.Compile()(0);
            Assert.IsFalse(result);

            var andSpecFalseTrue = new AndSpecification<int>(specFalse, specTrue);
            result = andSpecFalseTrue.Expressions.Compile()(0);
            Assert.IsFalse(result);

            var andSpecFalseFalse = new AndSpecification<int>(specFalse, specFalse);
            result = andSpecFalseFalse.Expressions.Compile()(0);
            Assert.IsFalse(result);
        }
    }
}