using Microsoft.Extensions.Caching.Memory;
using Offers.Interfaces;
using Offers.Models;
using Product.Interfaces;
using System.Reflection;
using ProductModel = Product.Models;

namespace FarmerMarket.API.Core
{
    /// <summary>
    /// Class for context service
    /// </summary>
    public static class ContextService
    {
        /// <summary>
        /// The offers cache key
        /// </summary>
        private static readonly string OffersCacheKey = "Offers";

        /// <summary>
        /// The products cache key
        /// </summary>
        private static readonly string ProductsCacheKey = "Products";

        /// <summary>
        /// Gets or sets the memory cache.
        /// </summary>
        /// <value>
        /// The memory cache.
        /// </value>
        public static IMemoryCache MemoryCache { get; set; }

        /// <summary>
        /// Builds the cache.
        /// </summary>
        public static void BuildCache()
        {
            var offers = GetDefaultOffers();
            var products = GetDefaultProducts();
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(1440),
                SlidingExpiration = TimeSpan.FromMinutes(1440),
                Size = 1024,
            };
            MemoryCache.Set(OffersCacheKey, offers, cacheEntryOptions);

            MemoryCache.Set(ProductsCacheKey, products, cacheEntryOptions);
        }

        /// <summary>
        /// Gets the default offers.
        /// </summary>
        /// <returns>default offers</returns>
        public static List<ProductOffer> GetDefaultOffers()
        {
            var offers = new List<ProductOffer>
            {
                new ProductOffer{ Id = 1, Code = "BOGO", ProductId = 3, PurchaseQty = 1, QtyMatchingType = OfferQtyMatchingType.Equal, DiscountType = DiscountType.Value, OfferedProductId = null, OfferPrice = 11.23, OfferedProductQty = 1, IsLimitAvailable = false, LimtPerPurchase = null },
                new ProductOffer{ Id = 2, Code = "APPL", ProductId = 2, PurchaseQty = 3, QtyMatchingType = OfferQtyMatchingType.EqualAndGreater, DiscountType = DiscountType.Value, OfferedProductId = null, OfferPrice = 1.5, OfferedProductQty = null, IsLimitAvailable = false, LimtPerPurchase = null },
                new ProductOffer{ Id = 3, Code = "CHMK", ProductId = 1, PurchaseQty = 1, QtyMatchingType = OfferQtyMatchingType.Equal, DiscountType = DiscountType.Value, OfferedProductId = 4, OfferPrice = 4.75, OfferedProductQty = 1, IsLimitAvailable = true, LimtPerPurchase = 1 },
                new ProductOffer{ Id = 4, Code = "APOM", ProductId = 5, PurchaseQty = 5, QtyMatchingType = OfferQtyMatchingType.Equal, DiscountType = DiscountType.Percentage, OfferedProductId = 2, OfferPrice = 50, OfferedProductQty = 1, IsLimitAvailable = false, LimtPerPurchase = null },
            };

            return offers;
        }

        /// <summary>
        /// Gets the default products.
        /// </summary>
        /// <returns>default products</returns>
        public static List<ProductModel.Product> GetDefaultProducts()
        {
            var products = new List<ProductModel.Product>
            {
                new ProductModel.Product { Id = 1, Code = "CH1", Name = "Chai", Price = 3.11 },
                new ProductModel.Product { Id = 2, Code = "AP1", Name = "Apples", Price = 6 },
                new ProductModel.Product { Id = 3, Code = "CF1", Name = "Coffee", Price = 11.23 },
                new ProductModel.Product { Id = 4, Code = "MK1", Name = "Milk", Price = 4.75 },
                new ProductModel.Product { Id = 5, Code = "OM1", Name = "OatMeal", Price = 3.69 },
            };

            return products;
        }
    }
}
