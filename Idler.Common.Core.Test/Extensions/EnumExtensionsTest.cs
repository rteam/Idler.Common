using System.Collections;
using System.ComponentModel;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Idler.Common.Core.Test.Extensions;

[TestClass]
[TestSubject(typeof(EnumExtensions))]
public class EnumExtensionsTest
{
    private enum TestEnum
    {
        [System.ComponentModel.Description("Description for Value1")]
        Value1,
        Value2
    }

    [TestMethod]
    public void EnumToDictionary_ShouldReturnCorrectDictionary_WhenEnumWithDescriptionAttributeProvided()
    {
        var expectedDictionary = new Dictionary<string, string>
        {
            { "Value1", "Description for Value1" },
            { "Value2", "No description" }
        };

        var resultDictionary = EnumExtensions.EnumToDictionary<TestEnum>();

        Assert.IsTrue(Enumerable.SequenceEqual(expectedDictionary.OrderBy(t => t.Key),
            resultDictionary.OrderBy(t => t.Key)));
        Assert.IsTrue(Enumerable.SequenceEqual(expectedDictionary.OrderBy(t => t.Value),
            resultDictionary.OrderBy(t => t.Value)));
    }

    [TestMethod]
    public void EnumToDictionary_ShouldBeConsistentAcrossMultipleCalls()
    {
        var resultDictionary1 = EnumExtensions.EnumToDictionary<TestEnum>();
        var resultDictionary2 = EnumExtensions.EnumToDictionary<TestEnum>();

        Assert.IsTrue(Enumerable.SequenceEqual(resultDictionary1.OrderBy(t => t.Key),
            resultDictionary2.OrderBy(t => t.Key)));
        Assert.IsTrue(Enumerable.SequenceEqual(resultDictionary1.OrderBy(t => t.Value),
            resultDictionary2.OrderBy(t => t.Value)));
    }
}