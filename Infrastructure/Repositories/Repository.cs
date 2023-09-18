using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly TodoContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(TodoContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        //create a async Get Method to Get All Todos using generic repository pattern and IRepository interface. implement pagination
        public async Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize) 
        {
            return await _context.Set<T>().Select(x => x).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public Task<T> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
