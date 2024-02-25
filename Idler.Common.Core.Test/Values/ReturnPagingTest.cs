using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Idler.Common.Core;

namespace Idler.Common.Core.Test.Values
{
    [TestClass]
    public class ReturnPagingTest
    {
        [TestMethod]
        public void ReturnPaging_CalculatePagingInfo_TestZeroTotal()
        {
            var returnPaging = new ReturnPaging<object>(5, 10, 0);
            Assert.AreEqual(1, returnPaging.PageCount);
            Assert.AreEqual(10, returnPaging.Take);
        }

        [TestMethod]
        public void ReturnPaging_CalculatePagingInfo_TestNonZeroTotalNoRemainder()
        {
            var returnPaging = new ReturnPaging<object>(5, 5, 50);
            Assert.AreEqual(10, returnPaging.PageCount);
            Assert.AreEqual(20, returnPaging.Skip);
            Assert.AreEqual(5, returnPaging.Take);
        }

        [TestMethod]
        public void ReturnPaging_CalculatePagingInfo_TestNonZeroTotalWithRemainder()
        {
            var returnPaging = new ReturnPaging<object>(7, 5, 55);
            Assert.AreEqual(11, returnPaging.PageCount);
            Assert.AreEqual(30, returnPaging.Skip);
            Assert.AreEqual(5, returnPaging.Take);
        }

        [TestMethod]
        public void ReturnPaging_CalculatePagingInfo_TestPageNumGreaterPageCount()
        {
            var returnPaging = new ReturnPaging<object>(15, 5, 20);
            Assert.AreEqual(4, returnPaging.PageCount);
            Assert.AreEqual(4, returnPaging.PageNum);
            Assert.AreEqual(15, returnPaging.Skip);
            Assert.AreEqual(5, returnPaging.Take);
        }
    }
}