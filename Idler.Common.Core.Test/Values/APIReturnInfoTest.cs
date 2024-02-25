using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Idler.Common.Core;
using JetBrains.Annotations;

namespace Idler.Common.Core.Test.Values
{
    [TestClass]
    [TestSubject(typeof(APIReturnInfo<>))]
    public class APIReturnInfoTest
    {
        [TestMethod]
        public void Should_Return_Error_State()
        {
            var state = APIReturnInfo<int>.Error("error message");
            Assert.IsFalse(state.State);
            Assert.AreEqual("error message", state.Message);
            Assert.AreEqual(BaseStateCode.STATE_CODE_ERROR, state.StateCode);
        }

        [TestMethod]
        public void Should_Return_Success_State()
        {
            var state1 = APIReturnInfo<int>.Success();
            Assert.IsTrue(state1.State);
            Assert.AreEqual("ok", state1.Message);
            Assert.AreEqual(BaseStateCode.STATE_CODE_SUCCESS, state1.StateCode);

            string message = "Hello, World!";
            var state2 = APIReturnInfo<int>.Success(message);
            Assert.IsTrue(state2.State);
            Assert.AreEqual(message, state2.Message);
            Assert.AreEqual(BaseStateCode.STATE_CODE_SUCCESS, state2.StateCode);
        }

        [TestMethod]
        public void Should_Return_Authorization_Failed_State()
        {
            var state = APIReturnInfo<int>.AuthorizationFailed("error message");
            Assert.IsFalse(state.State);
            Assert.IsFalse(state.Authorization);
            Assert.AreEqual("error message", state.Message);
            Assert.AreEqual(BaseStateCode.STATE_CODE_AuthorizationFailed, state.StateCode);
        }

        [TestMethod]
        public void Should_Return_Success_State_With_Data()
        {
            int data = 100;
            var state = APIReturnInfo<int>.Success(data);
            Assert.IsTrue(state.State);
            Assert.AreEqual(data, state.Data);
            Assert.AreEqual(BaseStateCode.STATE_CODE_SUCCESS, state.StateCode);
        }

        [TestMethod]
        public void Should_Return_Message_With_Args()
        {
            string message = "Hello, {0}!";
            string name = "John";
            var state = APIReturnInfo<int>.Success(message, name);
            Assert.AreEqual("Hello, John!", state.Message);
        }
    }
}