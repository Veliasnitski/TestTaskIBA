using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebManagerTasks.Data.Repositories
{
    public class EntityRepository<T> : IRepository<T> where T : class
    {
        private readonly ProjectsManagerContext db;
        public EntityRepository(ProjectsManagerContext context)
        {
            db = context;
        }

        public async Task AddAsync(T item)
        {
            await db.Set<T>().AddAsync(item);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            T existing = db.Set<T>().Find(id);
            if (existing != null)
            {
                db.Set<T>().Remove(existing);
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null)
        {
            return (predicate is null)
                ? await db.Set<T>().ToListAsync()
                : await db.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return (predicate is null)
                ? throw new ArgumentNullException("Filter empty! ")
                : await db.Set<T>().LastOrDefaultAsync(predicate);
        }

        public async Task UpdateAsync(T item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }
    }
}
