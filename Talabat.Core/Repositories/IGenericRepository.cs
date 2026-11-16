using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specification;

namespace Talabat.Core.Repositories
{
	public interface IGenericRepository<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
	{
		Task<IReadOnlyList<TEntity>> GetAllAsync();

		Task<TEntity> GetByIdAsync(Tkey id);
		
		Task<IReadOnlyList<TEntity>> GetAllAsyncWithSpec(ISpecification<TEntity, Tkey> spec);

		Task<TEntity> GetByEntityAsyncWithSpec(ISpecification<TEntity, Tkey> spec);

		Task<int> GetCountWithSpecAsync(ISpecification<TEntity, Tkey> spec);

		Task AddAsync(TEntity item);

		void Delete(TEntity item);
		void Update(TEntity item);
	}
}
