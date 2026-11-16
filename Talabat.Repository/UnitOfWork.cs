using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly StoreContext _context;
		private Hashtable _respoitory;
		public UnitOfWork(StoreContext context)
        {
			_context = context;
			_respoitory = new Hashtable();
		}
        public async Task<int> CompleteAsync()
			=> await _context.SaveChangesAsync();

		public async ValueTask DisposeAsync()
			=> await _context.DisposeAsync();

		public IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
		{
			var type = typeof(TEntity).Name;

			if(!_respoitory.ContainsKey(type))
			{
				var respoitory = new GenericRepository<TEntity, TKey>(_context);
				_respoitory.Add(type, respoitory);
			}
			
			return _respoitory[type] as IGenericRepository<TEntity, TKey>;
		}
	}
}
