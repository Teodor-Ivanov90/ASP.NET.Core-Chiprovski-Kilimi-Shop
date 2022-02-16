﻿using ChiprovciCarpetsShop.Data;
using ChiprovciCarpetsShop.Data.Models;
using ChiprovciCarpetsShop.Models.Products;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ChiprovciCarpetsShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ChiprovciCapretsDbContext data;

        public ProductsController(ChiprovciCapretsDbContext data)
            => this.data = data;

        public IActionResult All([FromQuery] AllProductsQueryModel query)
        {
            var productsQuery = this.data.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Type))
            {
                productsQuery = productsQuery
                    .Where(p =>
                    p.Type.Name == query.Type);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                productsQuery = productsQuery
                    .Where(p =>
                    p.Model.ToLower().Contains(query.SearchTerm.ToLower()) ||
                    p.Maker.ToLower().Contains(query.SearchTerm.ToLower()) ||
                    p.Type.Name.ToLower().Contains(query.SearchTerm.ToLower())
                    );
            }

            productsQuery = query.Sorting switch
            {
                ProductSorting.Model => productsQuery.OrderBy(p => p.Model),
                ProductSorting.Type => productsQuery.OrderBy(p => p.Type.Name),
                ProductSorting.DateCreated or _ => productsQuery.OrderByDescending(p => p.Id)
            };

            var products = productsQuery
                .Select(p => new AllProductsViewModel
                {
                    Id = p.Id,
                    Model = p.Model,
                    ImageUrl = p.ImageUrl,
                    ProductType = p.Type.Name
                })
                .ToList();

            var types = this.data.ProductTypes
                .Select(pt => pt.Name)
                .OrderBy(pt => pt)
                .ToList();

            query.Products = products;
            query.Types = types;

            return View(query);
        }

        public IActionResult Add()
        {
            return View(new AddProductFormModel
            {
                Types = GetProductTypes()
            }) ;
        }

        [HttpPost]
        public IActionResult Add(AddProductFormModel product)
        {
            if (!this.data.ProductTypes.Any(pt => pt.Id == product.TypeId))
            {
                this.ModelState.AddModelError(nameof(product.TypeId), "The product type is invalid!");
            }

            if (!ModelState.IsValid)
            {
                product.Types = this.GetProductTypes();

                return View(product);
            }

            var productData = new Product
            {
                Model = product.Model,
                Material = product.Material,
                Maker = product.Maker,
                YearOfMade = product.YearOfMade,
                TypeId = product.TypeId,
                ImageUrl = product.ImageUrl
            };

            this.data.Products.Add(productData);

            this.data.SaveChanges();

            return RedirectToAction(nameof(All));
        }

        private IEnumerable<ProductTypeFormModel> GetProductTypes()
            => this.data
            .ProductTypes
            .Select(pt => new ProductTypeFormModel
            {
                Id = pt.Id,
                Name = pt.Name
            })
            .ToList();
    }
}
