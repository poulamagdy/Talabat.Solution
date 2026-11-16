using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specification
{
	public class ProductWithProductBrandAndTypeSpec : BaseSpecification<Product, int>
	{
		public ProductWithProductBrandAndTypeSpec(ProductSpecParams specParams) :
			base(p =>
				(string.IsNullOrEmpty(specParams.Search) || p.Name.Trim().ToLower().Contains(specParams.Search.Trim().ToLower()))
				&&
				(!specParams.TypeId.HasValue || p.ProductTypeId == specParams.TypeId.Value)
				&&
				(!specParams.BrandId.HasValue || p.ProductBrandId == specParams.BrandId.Value)
			)
		{
			Includes.Add(p => p.ProductBrand);
			Includes.Add(p => p.ProductType);

			if (!string.IsNullOrEmpty(specParams.Sort))
			{
				switch (specParams.Sort) 
				{
					case "priceAsc" :
						AddOrderBy(p => p.Price);
						break;

					case "priceDesc" :
						AddOrderByDesc(p => p.Price); 
						break;
					
					default :
						AddOrderBy(p => p.Name);
						break;
				}
			}

			ApplyPagination(specParams.PageSize, specParams.PageSize * (specParams.PageIndex - 1));
		}

        public ProductWithProductBrandAndTypeSpec(int id) : base(p => p.Id == id)
        {
			Includes.Add(p => p.ProductBrand);
			Includes.Add(p => p.ProductType);
		}
    }
}
