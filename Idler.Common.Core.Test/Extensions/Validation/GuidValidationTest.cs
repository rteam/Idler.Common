using System.Collections;
using JetBrains.Annotations;

namespace Idler.Common.Core.Test.Extensions.Validation
{
    [TestClass]
    [TestSubject(typeof(GuidValidation))]
    public class GuidValidationTest
    {

        [TestMethod]
        public void TestGuidByStringSingle()
        {
            var validGuidString = Guid.NewGuid().ToString();
            Assert.AreEqual(Guid.Parse(validGuidString), validGuidString.GuidByString(),
                "Expect the GUID to match when string is valid GUID");

            var invalidGuidString = "invalid";
            Assert.AreEqual(Guid.Empty, invalidGuidString.GuidByString(),
                "Expect Guid.Empty when string is not a valid GUID");
        }

        [TestMethod]
        public void TestGuidByStringList()
        {
            var validGuidStringList = new List<string> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var expectedGuidList = validGuidStringList.Select(x => Guid.Parse(x)).ToList();

            var actualGuidList = validGuidStringList.GuidByString();
            CollectionAssert.AreEqual(expectedGuidList, (ICollection?)actualGuidList, "Expect the GUID list to match when list contains valid GUID strings");

            var invalidGuidStringList = new List<string> { "invalid", Guid.NewGuid().ToString() };
            var expectedGuidListForInvalid = invalidGuidStringList
                .Where(x => Guid.TryParse(x, out _))
                .Select(x => Guid.Parse(x)).ToList();

            var actualGuidListForInvalid = invalidGuidStringList.GuidByString();
            CollectionAssert.AreEqual(expectedGuidListForInvalid, (ICollection?)actualGuidListForInvalid, "Expect the GUID list to filter out invalid GUID strings");
        }

        [TestMethod]
        public void TestGuidByStringArray()
        {
            var validGuidStringArray = new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var expectedGuidArray = validGuidStringArray.Select(x => Guid.Parse(x)).ToArray();

            var actualGuidArray = validGuidStringArray.GuidByString();
            CollectionAssert.AreEqual(expectedGuidArray, actualGuidArray,
                "Expect the GUID array to match when array contains valid GUID strings");

            var invalidGuidStringArray = new string[] { "invalid", Guid.NewGuid().ToString() };
            var expectedGuidArrayForInvalid = invalidGuidStringArray
                .Where(x => Guid.TryParse(x, out _))
                .Select(x => Guid.Parse(x)).ToArray();

            var actualGuidArrayForInvalid = invalidGuidStringArray.GuidByString();
            CollectionAssert.AreEqual(expectedGuidArrayForInvalid, actualGuidArrayForInvalid,
                "Expect the GUID array to filter out invalid GUID strings");
        }

        [TestMethod]
        public void TestIsGuid()
        {
            var validGuidString = Guid.NewGuid().ToString();
            Assert.IsTrue(validGuidString.IsGuid(), "Expect IsGuid to be true when string is a valid GUID");

            var invalidGuidString = "invalid";
            Assert.IsFalse(invalidGuidString.IsGuid(), "Expect IsGuid to be false when string is not a valid GUID");
        }
        
          [TestMethod]
        public void TestThrowIfNullSingle()
        {
            // Test when inputGuid is valid (not empty)
            Guid validGuid = Guid.NewGuid();
            Assert.AreEqual(validGuid, validGuid.ThrowIfNull("validGuid"));
            
            // Test when inputGuid is empty
            Guid emptyGuid = Guid.Empty;
            Assert.ThrowsException<ArgumentNullException>(() => emptyGuid.ThrowIfNull("emptyGuid"));
        }

        [TestMethod]
        public void TestThrowIfNullMultiple()
        {
            // Test when all the Guids in the collection are valid (not empty)
            var validGuids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            CollectionAssert.AreEqual(validGuids, validGuids.ThrowIfNull("validGuids").ToList());

            // Test when the Guids collection contains an empty Guid
            var invalidGuids = new List<Guid> { Guid.Empty, Guid.NewGuid() };
            Assert.ThrowsException<ArgumentNullException>(() => invalidGuids.ThrowIfNull("invalidGuids"));
        }

        [TestMethod]
        public void TestIsEmptySingle()
        {
            // Test when inputGuid is valid (not empty)
            Guid validGuid = Guid.NewGuid();
            Assert.IsFalse(validGuid.IsEmpty());

            // Test when inputGuid is empty
            Guid emptyGuid = Guid.Empty;
            Assert.IsTrue(emptyGuid.IsEmpty());
        }

        [TestMethod]
        public void TestIsEmptyMultiple()
        {
            // Test when all the Guids in the collection are valid (not empty)
            var validGuids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            Assert.IsFalse(validGuids.IsEmpty());

            // Test when the Guids collection is null or contains no elements
            List<Guid> noGuids = null;
            Assert.IsTrue(noGuids.IsEmpty());

            List<Guid> emptyGuids = new List<Guid>();
            Assert.IsTrue(emptyGuids.IsEmpty());

            // Test when the Guids collection contains an empty Guid
            var invalidGuids = new List<Guid> { Guid.Empty, Guid.NewGuid() };
            Assert.IsTrue(invalidGuids.IsEmpty());
        }
    }
}