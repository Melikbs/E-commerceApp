using Core.Entities;

namespace Core.Statements
{
    public class ProductsWithTypesAndBrandsStatement : BaseStatement<Product>
    {
        public ProductsWithTypesAndBrandsStatement()
        {
            AddInclude(x => x.ProductType);
            AddInclude(x => x.ProductBrand);
        }
    }
}
