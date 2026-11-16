using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specification
{
	public class ProductWithFiltrationForCountAsync : BaseSpecification<Product, int>
	{
        public ProductWithFiltrationForCountAsync(ProductSpecParams specParams) :
			base(p =>
				(string.IsNullOrEmpty(specParams.Search) || p.Name.Trim().ToLower().Contains(specParams.Search.Trim().ToLower()))
				&&
				(!specParams.TypeId.HasValue || p.ProductTypeId == specParams.TypeId.Value)
				&&
				(!specParams.BrandId.HasValue || p.ProductBrandId == specParams.BrandId.Value)
			)
		{
            
        }
    }
}
