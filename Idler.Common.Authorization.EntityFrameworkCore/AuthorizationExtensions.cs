using Microsoft.EntityFrameworkCore;

namespace Idler.Common.Authorization.EntityFrameworkCore
{
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// 配置授权上下文
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static ModelBuilder AuthorizationModelBuilderExtensions(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CoreResource>().HasOne(t => t.Parent).WithMany(t => t.Children).HasForeignKey(t => t.ParentId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CorePermission>().HasOne(t => t.Parent).WithMany(t => t.Children).HasForeignKey(t => t.ParentId).OnDelete(DeleteBehavior.Restrict);
            return modelBuilder;
        }
    }
}