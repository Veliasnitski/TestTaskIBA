using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebManagerTasks.Data.Repositories;

namespace WebManagerTasks.Services
{
    public class EntityService<T> : IService<T>
    {
        private readonly IRepository<T> repository;

        public EntityService(IRepository<T> repository)
        {
            this.repository = repository;
        }
        public async Task AddAsync(T item)
        {
            await repository.AddAsync(item);
        }

        public async Task DeleteAsync(int id)
        {
            await repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await repository.GetAllAsync(predicate); 
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await repository.GetAsync(predicate);
        }

        public async Task UpdateAsync(T item)
        {
            await repository.UpdateAsync(item);
        }
    }
}
