using System.Linq.Expressions;

namespace MidAssignment.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        IQueryable<T> GetQueryable(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(object id);
        public Task<T?> GetByIdWithIncludesAsync(object id, params Expression<Func<T, object>>[] includes);
        public void Update(T entity);
        public Task Delete(object id);
        Task<bool> SaveChangesAsync();
    }

}
