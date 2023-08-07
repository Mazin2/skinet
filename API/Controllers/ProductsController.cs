using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
   
    public class ProductsController:BaseApiController
    {
        //for repository
       /* private readonly IProductRepository _repo;   
       
        public ProductsController(IProductRepository repo)
        {
            _repo = repo;
            
        }
        */
        //for generic rep

        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productsRepo,
        IGenericRepository<ProductBrand> productBrandRepo,IGenericRepository<ProductType> productTypeRepo,IMapper mapper)
        {
            _mapper = mapper;
            _productTypeRepo = productTypeRepo;
            _productBrandRepo = productBrandRepo;
            _productsRepo = productsRepo;
            
            
        }


        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts() //ProductToReturnDto was product before dto
        {
            //var products = await _productsRepo.ListAllAsync();  for generic before specification

            var spec=new ProductsWithTypesAndBrandsSpecification();
            var products = await _productsRepo.ListAsync(spec);   //FOR GENERIC WITH SPECIFICATION
           // return Ok(products); before dto
          /* return products.Select(product => new ProductToReturnDto
           {
                Id=product.Id,
                Name=product.Name,
                Description=product.Description,
                PictureUrl=product.PictureUrl,
                Price=product.Price,
                ProductBrand=product.ProductBrand.Name,
                ProductType=product.ProductType.Name
           }).ToList();
           before automapper*/
           return Ok(_mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products));
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id) //ProductToReturnDto was product before dto
        {
            //return await _productsRepo.GetByIdAsync(id); for generic before specification
            var spec=new ProductsWithTypesAndBrandsSpecification(id);
            // return await _productsRepo.GetEntityWithSpec(spec); before dto
            var product = await _productsRepo.GetEntityWithSpec(spec);
           /* return new ProductToReturnDto
            {
                Id=product.Id,
                Name=product.Name,
                Description=product.Description,
                PictureUrl=product.PictureUrl,
                Price=product.Price,
                ProductBrand=product.ProductBrand.Name,
                ProductType=product.ProductType.Name
            };
            before automapper*/
            if(product == null ) return NotFound(new ApiResponse(404));
            return _mapper.Map<Product,ProductToReturnDto>(product);
        }


 [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            
            return Ok(await _productBrandRepo.ListAllAsync());
        }
        
         [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            
            //return Ok(await _repo.GetProductTypesAsync());   example for repository 
            return Ok(await _productTypeRepo.ListAllAsync());
        }
    }
}