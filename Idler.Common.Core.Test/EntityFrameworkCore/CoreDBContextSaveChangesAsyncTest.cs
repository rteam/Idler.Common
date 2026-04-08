using System;
using System.Threading.Tasks;
using Idler.Common.Core.Auditing;
using Idler.Common.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Idler.Common.Core.Test.EntityFrameworkCore
{
    [TestClass]
    public class CoreDBContextSaveChangesAsyncTest
    {
        [TestMethod]
        public async Task SaveChangesAsync_Should_Apply_Create_Audit()
        {
            DbContextOptions<TestCoreDbContext> options = new DbContextOptionsBuilder<TestCoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;

            using var context = new TestCoreDbContext(options);
            var entity = new TestEntity
            {
                Id = Guid.NewGuid()
            };

            context.Entities.Add(entity);
            await context.SaveChangesAsync();

            Assert.IsNotNull(entity.CreateTime);
        }

        private sealed class TestCoreDbContext : CoreDBContext
        {
            public TestCoreDbContext(DbContextOptions options) : base(options)
            {
            }

            public DbSet<TestEntity> Entities { get; set; }
        }

        private sealed class TestEntity : IHasCreateTime
        {
            public Guid Id { get; set; }

            public DateTime? CreateTime { get; set; }
        }
    }
}
