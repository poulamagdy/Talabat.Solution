using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specification;

namespace Talabat.Repository
{
	public static class SpecifictionEvalutor<TEntity, TKey> where TEntity : BaseEntity<TKey>
	{
		public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity,TKey> spec)
		{
			var query = inputQuery; // context.product

			if (spec.Criteria is not null)
				query = query.Where(spec.Criteria); // context.product.where(p => p.id)

			if (spec.OrderBy is not null)
				query = query.OrderBy(spec.OrderBy);	

			if (spec.OrderByDesc is not null)
				query = query.OrderByDescending(spec.OrderByDesc);

			if (spec.IsPaginationEnabled)
				query = query.Skip(spec.Skip).Take(spec.Take);

			query = spec.Includes.Aggregate(query, (cq, iq) => cq.Include(iq)); // context.product.where(p => p.id).include()

			return query;
		}
	}
}
