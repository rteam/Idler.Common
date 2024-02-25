using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Idler.Common.Core;

namespace Idler.Common.Core.Test.Values
{
    [TestClass]
    [TestCategory("ComplexId")]
    public class ComplexIdTest
    {
        [TestMethod]
        public void ConcatId_ValidInput_ShouldReturnConcatenatedId()
        {
            // Arrange
            string mainId = "mainId";
            string limitId = "limitId";

            // Act
            string result = ComplexId.ConcatId(mainId, limitId);

            // Assert
            Assert.AreEqual("mainId@limitId", result);
        }

        [TestMethod]
        public void ConcatId_EmptyInput_ShouldReturnConcatenatedId()
        {
            // Arrange
            string mainId = "";
            string limitId = "";

            Assert.ThrowsException<ArgumentNullException>(() => ComplexId.ConcatId(mainId, limitId));
        }

        [TestMethod]
        public void ConcatId_NullInput_ShouldThrowArgumentNullException()
        {
            // Arrange
            string mainId = null;
            string limitId = null;

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => ComplexId.ConcatId(mainId, limitId));
        }

        [TestMethod]
        public void Equals_ValidInput_ShouldReturnTrue()
        {
            // Arrange
            ComplexId id1 = new ComplexId("mainId", "limitId");
            ComplexId id2 = new ComplexId("mainId", "limitId");

            // Act
            bool result = id1.Equals(id2);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_InvalidInput_ShouldReturnFalse()
        {
            // Arrange
            ComplexId id1 = new ComplexId("mainId1", "limitId1");
            ComplexId id2 = new ComplexId("mainId2", "limitId2");

            // Act
            bool result = id1.Equals(id2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ToString_ValidInput_ShouldReturnConcatenatedId()
        {
            // Arrange
            ComplexId id = new ComplexId("mainId", "limitId");

            // Act
            string result = id.ToString();

            // Assert
            Assert.AreEqual("mainId@limitId", result);
        }

        [TestMethod]
        public void ComplexId_ConstructorWithSingleParameter_ValidInput_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            string complexId = "mainId@limitId";

            // Act
            ComplexId id = new ComplexId(complexId);

            // Assert
            Assert.AreEqual("mainId", id.MainId);
            Assert.AreEqual("limitId", id.LimitId);
        }

        [TestMethod]
        public void ComplexId_ConstructorWithSingleParameter_InvalidInput_ShouldThrowException()
        {
            // Arrange
            string complexId = "mainId";

            // Act & Assert
            Assert.ThrowsException<AggregateException>(() => new ComplexId(complexId));
        }
    }
}