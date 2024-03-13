using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Idler.Common.Core.Tree;
using JetBrains.Annotations;

namespace Idler.Common.Core.Test.Tree
{
    [TestClass]
    [TestSubject(typeof(TreeExtensions))]
    public class TreeExtensionsTest
    {
        // Test data: dummy implementation of ITreeData<int>
        private class TestData : ITreeData<string>
        {
            public TestData(string id, string name, string parentId)
            {
                Id = id;
                Name = name;
                ParentId = parentId;
            }

            public string Id { get; }

            public string Name { get; }

            public string ParentId { get; }
        }

        [TestMethod]
        public void TestBuildFirstLeveNullBaseTree()
        {
            // Arrange
            var data = new List<TestData>
            {
                new TestData("1", "Root1", null),
                new TestData("2", "Root2", null),
                new TestData("1-1", "Child 1", "1"),
                new TestData("1-2", "Child 2", "1"),
                new TestData("1-1-1", "Child of Child 1", "1-1"),
                new TestData("1-2-1", "Child of Child 2", "1-2")
            };

            // Act
            var result = data.BuildBaseTree<string, TestData>(default(string));
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            var baseTree = result.First();
            Assert.AreEqual("1", baseTree.Value);
            Assert.AreEqual("Root1", baseTree.Text);
            Assert.AreEqual("1", baseTree.Data.Id);
            Assert.AreEqual("Root1", baseTree.Data.Name);
            Assert.AreEqual(null, baseTree.Data.ParentId);
            Assert.AreEqual(2, baseTree.Children.Count);
            Assert.AreEqual(1, baseTree.Children.First().Children.Count);

        }
        
        [TestMethod]
        public void TestBuildFirstLeveNullHashTree()
        {
            // Arrange
            var data = new List<TestData>
            {
                new TestData("1", "Root1", null),
                new TestData("2", "Root2", null),
                new TestData("1-1", "Child 1", "1"),
                new TestData("1-2", "Child 2", "1"),
                new TestData("1-1-1", "Child of Child 1", "1-1"),
                new TestData("1-2-1", "Child of Child 2", "1-2")
            };
            // Act
            var result = data.BuildHashTree<string, TestData>(null);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            var baseTree = result.First();
            Assert.AreEqual("1", baseTree.Value);
            Assert.AreEqual("Root1", baseTree.Text);
            Assert.AreEqual("1", baseTree.Data.Id);
            Assert.AreEqual("Root1", baseTree.Data.Name);
            Assert.AreEqual(null, baseTree.Data.ParentId);
            Assert.AreEqual(2, baseTree.Children.Count);
            Assert.AreEqual(1, baseTree.Children.First().Children.Count);

        }

        [DataTestMethod]
        [DataRow("1", "Root", "")]
        [DataRow("2", "Child 1", "1")]
        [DataRow("3", "Child 2", "1")]
        [DataRow("4", "Child of Child 1", "2")]
        [DataRow("5", "Child of Child 2", "3")]
        public void TestBuildBaseTree(string id, string name, string parentId)
        {
            // Arrange
            var data = new List<TestData>
            {
                new TestData(id, name, parentId)
            };

            var parent = new TestData(parentId, "Parent", "");

            // Act
            var result = data.BuildBaseTree<string, TestData>(parent.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            var baseTree = result.First();
            Assert.AreEqual(id.ToString(), baseTree.Value);
            Assert.AreEqual(name, baseTree.Text);
            Assert.AreEqual(id, baseTree.Data.Id);
            Assert.AreEqual(name, baseTree.Data.Name);
            Assert.AreEqual(parentId, baseTree.Data.ParentId);
        }

        [DataTestMethod]
        [DataRow("1", "Root", "")]
        [DataRow("2", "Child 1", "1")]
        [DataRow("3", "Child 2", "1")]
        [DataRow("4", "Child of Child 1", "2")]
        [DataRow("5", "Child of Child 2", "3")]
        public void TestBuildHashTree(string id, string name, string parentId)
        {
            // Arrange
            var data = new List<TestData>
            {
                new TestData(id, name, parentId)
            };

            var parent = new TestData(parentId, "Parent", "");

            // Act
            var result = data.BuildHashTree<string, TestData>(parent.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            var hashTree = result.First();
            Assert.AreEqual(id.ToString(), hashTree.Value);
            Assert.AreEqual(name, hashTree.Text);
            Assert.AreEqual(id, hashTree.Data.Id);
            Assert.AreEqual(name, hashTree.Data.Name);
            Assert.AreEqual(parentId, hashTree.Data.ParentId);
        }
    }
}