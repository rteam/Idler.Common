using System;
using System.Linq.Expressions;
using Idler.Common.Core.Specification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Idler.Common.Core.Test.Specification
{
    [TestClass]
    public class NotSpecificationTest
    {
        [TestMethod]
        public void NotSpecification_IsSatisfiedBy_FalseTest()
        {
            var mockSpecification = new Mock<ISpecification<int>>();
            mockSpecification.Setup(x => x.Expressions).Returns(x => x > 2);
            var notSpecification = new NotSpecification<int>(mockSpecification.Object);
            Assert.IsFalse(notSpecification.IsSatisfiedBy(3));
        }

        [TestMethod]
        public void NotSpecification_IsSatisfiedBy_TrueTest()
        {
            var mockSpecification = new Mock<ISpecification<int>>();
            mockSpecification.Setup(x => x.Expressions).Returns(x => x > 2);
            var notSpecification = new NotSpecification<int>(mockSpecification.Object);
            Assert.IsTrue(notSpecification.IsSatisfiedBy(2));
        }

        [TestMethod]
        public void NotSpecification_AndTest()
        {
            var mockSpecification = new Mock<ISpecification<int>>();
            mockSpecification.Setup(x => x.Expressions).Returns(x => x > 2);
            var notSpecification = new NotSpecification<int>(mockSpecification.Object);
            var andSpecification = notSpecification.And(mockSpecification.Object);
            Assert.IsFalse(andSpecification.IsSatisfiedBy(3));
        }

        [TestMethod]
        public void NotSpecification_OrTest()
        {
            var mockSpecification = new Mock<ISpecification<int>>();
            mockSpecification.Setup(x => x.Expressions).Returns(x => x > 2);
            var notSpecification = new NotSpecification<int>(mockSpecification.Object);
            var orSpecification = notSpecification.Or(mockSpecification.Object);
            Assert.IsTrue(orSpecification.IsSatisfiedBy(2));
            Assert.IsTrue(orSpecification.IsSatisfiedBy(3));
        }

        [TestMethod]
        public void NotSpecification_NotTest()
        {
            var mockSpecification = new Mock<ISpecification<int>>();
            mockSpecification.Setup(x => x.Expressions).Returns(x => x > 2);
            var notSpecification = new NotSpecification<int>(mockSpecification.Object);
            var notNotSpecification = notSpecification.Not();
            Assert.IsTrue(notNotSpecification.IsSatisfiedBy(3));
            Assert.IsFalse(notNotSpecification.IsSatisfiedBy(2));
        }
    }
}