using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specification
{
	public class BaseSpecification<TEntity, TKey> : ISpecification<TEntity, TKey> where TEntity : BaseEntity<TKey>
	{
		public Expression<Func<TEntity, bool>> Criteria { get ; set ; }
		public List<Expression<Func<TEntity, object>>> Includes { get ; set ; } = new List<Expression<Func<TEntity, object>>> ();
		public Expression<Func<TEntity, object>> OrderBy { get; set; }
		public Expression<Func<TEntity, object>> OrderByDesc { get; set; }
		public int Take { get; set; }
		public int Skip { get; set; }
		public bool IsPaginationEnabled { get; set; }
		public BaseSpecification()
        {
        }
        public BaseSpecification(Expression<Func<TEntity, bool>> criteriaExpression)
        {
            Criteria = criteriaExpression;
        }

		public void AddOrderBy(Expression<Func<TEntity, object>> OrderByExpression)
		{
			OrderBy = OrderByExpression;
		}

		public void AddOrderByDesc(Expression<Func<TEntity, object>> OrderByDescExpression)
		{
			OrderByDesc = OrderByDescExpression;
		}

		public void ApplyPagination(int take, int skip)
		{
			Take = take;
			Skip = skip;
			IsPaginationEnabled = true;
		}
	}
}
