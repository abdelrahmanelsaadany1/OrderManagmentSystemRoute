using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using persistence.Data.AppDbContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace persistence.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<TEntity> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task AddAsync(TEntity entity)
            => await _dbSet.AddAsync(entity);

        public void Update(TEntity entity)
            => _dbSet.Update(entity);

        public void Delete(TEntity entity)
            => _dbSet.Remove(entity);

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
