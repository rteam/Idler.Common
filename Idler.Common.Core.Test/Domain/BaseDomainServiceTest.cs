using System.Threading.Tasks;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Idler.Common.Core.Test.Domain
{
    [TestClass]
    public class BaseDomainServiceTest
    {
        [TestMethod]
        public async Task SaveChangeAsync_Should_Return_And_Wait_Commit_Task()
        {
            TaskCompletionSource<int> taskSource = new();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(t => t.CommitAsync()).Returns(taskSource.Task);

            var service = new TestDomainService(unitOfWorkMock.Object);
            Task saveTask = service.SaveChangeAsync();

            Assert.IsFalse(saveTask.IsCompleted, "SaveChangeAsync should return the underlying pending commit task.");
            unitOfWorkMock.Verify(t => t.CommitAsync(), Times.Once);

            taskSource.SetResult(0);
            await saveTask;
        }

        private sealed class TestDomainService : BaseDomainService
        {
            public TestDomainService(IUnitOfWork unitOfWork) : base(unitOfWork)
            {
            }
        }
    }
}
