using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specification;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
	public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
	{
		private readonly StoreContext _context;

		public GenericRepository(StoreContext context)
        {
			_context = context;
		}

		public async Task<IReadOnlyList<TEntity>> GetAllAsync()
			=> await _context.Set<TEntity>().ToListAsync();
		public async Task<TEntity> GetByIdAsync(TKey id)
			=> await _context.Set<TEntity>().FindAsync(id);
		public async Task<IReadOnlyList<TEntity>> GetAllAsyncWithSpec(ISpecification<TEntity, TKey> spec)
			=> await ApllaySpecifiction(spec).ToListAsync();
		public async Task<TEntity> GetByEntityAsyncWithSpec(ISpecification<TEntity, TKey> spec)
			=> await ApllaySpecifiction(spec).FirstOrDefaultAsync();

		public async Task<int> GetCountWithSpecAsync(ISpecification<TEntity, TKey> spec)
			=> await ApllaySpecifiction(spec).CountAsync();

		private IQueryable<TEntity> ApllaySpecifiction(ISpecification<TEntity, TKey> spec)
		 => SpecifictionEvalutor<TEntity, TKey>.GetQuery(_context.Set<TEntity>(), spec);

		public async Task AddAsync(TEntity item)
			=> await _context.AddAsync(item);

		public void Delete(TEntity item)
			=> _context.Set<TEntity>().Remove(item);

		public void Update(TEntity item)
			=> _context.Set<TEntity>().Update(item);
	}
}
