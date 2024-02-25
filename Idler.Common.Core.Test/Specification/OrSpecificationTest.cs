using Idler.Common.Core.Specification;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq.Expressions;

namespace Idler.Common.Core.Test.Specification
{
    [TestClass]
    [TestSubject(typeof(OrSpecification<>))]
    public class OrSpecificationTest
    {
        private Mock<ISpecification<object>> _leftSpecification;
        private Mock<ISpecification<object>> _rightSpecification;

        [TestInitialize]
        public void Initialize()
        {
            _leftSpecification = new Mock<ISpecification<object>>();
            _rightSpecification = new Mock<ISpecification<object>>();
        }

        [TestMethod]
        public void Constructor_SetsLeftAndRightSpecifications()
        {
            var orSpecification = new OrSpecification<object>(_leftSpecification.Object, _rightSpecification.Object);

            Assert.AreEqual(_leftSpecification.Object, ((CombinationSpecification<object>)orSpecification).Left);
            Assert.AreEqual(_rightSpecification.Object, ((CombinationSpecification<object>)orSpecification).Right);
        }

        [TestMethod]
        public void Expressions_ReturnsOrExpression()
        {
            const bool leftResult = true;
            const bool rightResult = false;

            _leftSpecification.Setup(left => left.Expressions).Returns(obj => leftResult);
            _rightSpecification.Setup(right => right.Expressions).Returns(obj => rightResult);

            var orSpecification = new OrSpecification<object>(_leftSpecification.Object, _rightSpecification.Object);

            Expression<Func<object, bool>> orExpression = orSpecification.Expressions;
            bool result = orExpression.Compile()(new object());

            Assert.AreEqual(leftResult || rightResult, result);
        }
    }
}