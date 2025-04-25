using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MidAssignment.Domain;

namespace MidAssignment.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // validations
            /* book */
            /* cate */
            /* borrowing request */
            /* borrowing request detail */

            // relationships
            /* book vs cate */
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Categories)
                .WithMany(c => c.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookCategory", 
                    b => b
                        .HasOne<Category>()
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade),
                    c => c
                        .HasOne<Book>()
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade),
                    t =>
                    {
                        t.HasKey("BookId", "CategoryId");
                        t.ToTable("BookCategory"); 
                    });
            /* borrowing request vs borrowing request detail */
            modelBuilder.Entity<BookBorrowingRequestDetail>()
                .HasKey(d => d.BookBorrowingRequestId);

            modelBuilder.Entity<BookBorrowingRequestDetail>()
                .HasOne(d => d.BookBorrowingRequest)
                .WithOne(r => r.BookBorrowingRequestDetail)
                .HasForeignKey<BookBorrowingRequestDetail>(d => d.BookBorrowingRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            /* borrowing request vs user */
            modelBuilder.Entity<BookBorrowingRequest>()
                .HasOne(r => r.Requestor)
                .WithMany(u => u.MadeRequests)
                .HasForeignKey(r => r.RequestorId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho phép xoá user nếu còn request

            modelBuilder.Entity<BookBorrowingRequest>()
                .HasOne(r => r.Approver)
                .WithMany(u => u.ApprovedRequests)
                .HasForeignKey(r => r.ApproverId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho phép xoá user nếu còn request
            /* book vs borrowing request detail */
            modelBuilder.Entity<BookBorrowingRequestDetail>()
                .HasMany(d => d.Books)
                .WithMany(b => b.BookBorrowingRequestDetails)
                .UsingEntity<Dictionary<string, object>>(
                    "BookBorrowingRequestDetailBook",
                    d => d
                        .HasOne<Book>()
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Restrict), // ❌ Cấm xóa Book nếu đang được dùng
                    b => b
                        .HasOne<BookBorrowingRequestDetail>()
                        .WithMany()
                        .HasForeignKey("BookBorrowingRequestId")
                        .OnDelete(DeleteBehavior.Cascade), // ✅ Xóa request detail thì tự động gỡ liên kết
                    t =>
                    {
                        t.HasKey("BookId", "BookBorrowingRequestId");
                        t.ToTable("BookBorrowingRequestDetailBook");
                    });


        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<BookBorrowingRequest> BookBorrowingRequests { get; set; }
        public virtual DbSet<BookBorrowingRequestDetail> BookBorrowingRequestDetails { get; set; }
    }
}
