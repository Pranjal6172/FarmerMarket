using Billing.DTOModels;
using Billing.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Offers.Interfaces;
using Offers.Models;
using Product.Interfaces;
using Product.Models;

namespace Billing.Services
{
    /// <summary>
    /// Class for billing service.
    /// </summary>
    public class BillingService : IBillingService
    {
        /// <summary>
        /// The bill cache key..
        /// </summary>
        private static readonly string BillCacheKey = "Bill";

        /// <summary>
        /// The offers cache key.
        /// </summary>
        private static readonly string OffersCacheKey = "Offers";

        /// <summary>
        /// The product cache key.
        /// </summary>
        private static readonly string ProductsCacheKey = "Products";

        /// <summary>
        /// Gets or sets the memory cache
        /// </summary>
        private IMemoryCache MemoryCache { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BillingService" /> class.
        /// </summary>
        /// <param name="memoryCache">The memory cache</param>
        public BillingService(IMemoryCache memoryCache)
        {
            this.MemoryCache = memoryCache;
        }

        /// <summary>
        /// get bill items
        /// </summary>
        /// <param name="productCode">The product code</param>
        /// <returns>all the bill items (includes offers)</returns>
        public IEnumerable<BillItemDTO> GetBillItems(string productCode)
        {
            var offers = this.MemoryCache.Get<List<ProductOffer>>(OffersCacheKey);
            var products = this.MemoryCache.Get<List<Product.Models.Product>>(ProductsCacheKey);
            var existingBillItems = new List<BillItemDTO>();
            if (this.MemoryCache.TryGetValue(BillCacheKey, out List<BillItemDTO> billItems))
            {
                existingBillItems = this.MemoryCache.Get<List<BillItemDTO>>(BillCacheKey);
                MemoryCache.Remove(BillCacheKey);
            }

            billItems = this.BuildBillItems(productCode, offers, products, existingBillItems);
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(30),
                Size = 1024,
            };
            MemoryCache.Set(BillCacheKey, billItems, cacheEntryOptions);
            return billItems;
        }

        /// <summary>
        /// build bill line items
        /// </summary>
        /// <param name="productCode">The product code</param>
        /// <param name="offers">The offers</param>
        /// <param name="products">The products</param>
        /// <param name="existingBillItems">The existing bill items</param>
        /// <returns>invoice line items</returns>
        private List<BillItemDTO> BuildBillItems(string productCode, IEnumerable<ProductOffer> offers, IEnumerable<Product.Models.Product> products, List<BillItemDTO> existingBillItems)
        {
            var billItems = new List<BillItemDTO>();
            this.BuildExistingBillItem(existingBillItems, billItems);
            try
            {
                var productDetails = products.SingleOrDefault(pc => string.Equals(pc.Code, productCode));
                if (productDetails is null)
                {
                    throw new Exception("Product doesn't exist");
                }
                var applicableOffers = offers.Where(o => string.Equals(o.ProductId, productDetails.Id) || (o.OfferedProductId.HasValue && string.Equals(o.OfferedProductId.Value, productDetails.Id)));
                var productBillItem = new BillItemDTO
                {
                    Id = Guid.NewGuid(),
                    Code = productCode,
                    ItemId = productDetails.Id,
                    Type = BillItemType.Product,
                    Price = productDetails.Price
                };
                billItems.Add(productBillItem);

                // when product is acting as parent in offer.
                if (applicableOffers.Any(ao => string.Equals(ao.ProductId, productDetails.Id)))
                {
                    var appliedOffer = applicableOffers.SingleOrDefault(ao => string.Equals(ao.ProductId, productDetails.Id));
                    var existingProductBillItems = existingBillItems.Where(item => item.Type == BillItemType.Product && string.Equals(item.ItemId, productDetails.Id)).ToList();
                    if (this.CheckOfferValidity(appliedOffer, existingBillItems.Count(eb => eb.Type == BillItemType.Offer && string.Equals(eb.ItemId, appliedOffer.Id))))
                    {
                        if (!appliedOffer.OfferedProductId.HasValue)
                        {
                            // inserting offer like buy on get one free (BOGO)
                            if (appliedOffer.OfferedProductQty.HasValue && appliedOffer.PurchaseQty == appliedOffer.OfferedProductQty.Value)
                            {
                                var existingProductBillItemCount = existingProductBillItems.Count;
                                if (existingProductBillItemCount > 0 && 
                                    ((existingProductBillItemCount + 1) % (appliedOffer.PurchaseQty + appliedOffer.OfferedProductQty.Value)) == 0)
                                {
                                    billItems.Add(this.BuildOfferBillItem(appliedOffer, productDetails, productBillItem.Id));
                                }
                            }

                            // inserting offer like APPL (buy 3 and more get 50 % off)
                            if (!appliedOffer.OfferedProductQty.HasValue)
                            {
                                var existingProductBillItemCount = existingProductBillItems.Count;
                                if (existingProductBillItemCount > 0 && (existingProductBillItemCount + 1) >= appliedOffer.PurchaseQty)
                                {
                                    // inserting offers for all the items
                                    existingProductBillItems.ForEach(item =>
                                    {
                                        billItems.Add(this.BuildOfferBillItem(appliedOffer, productDetails, item.Id));
                                    });

                                    billItems.Add(this.BuildOfferBillItem(appliedOffer, productDetails, productBillItem.Id));
                                }
                            }
                        }
                        else
                        {
                            // If chils came into basket first and than parent inserted into basket.
                            var existingChildProductBillItems = existingBillItems.Where(billItem => billItem.Type == BillItemType.Product && string.Equals(billItem.ItemId, appliedOffer.OfferedProductId.Value)).ToList();
                            if (existingChildProductBillItems.Any() && existingChildProductBillItems.Count >= (existingProductBillItems.Count + 1))
                            {
                                for (int index = 0; index < appliedOffer.OfferedProductQty; index++)
                                {
                                    var childProductDetails = products.SingleOrDefault(p => string.Equals(p.Id, existingChildProductBillItems[index].ItemId));
                                    billItems.Add(this.BuildOfferBillItem(appliedOffer, childProductDetails, existingChildProductBillItems[index].Id));
                                }
                            }
                        }
                    }
                }

                // when product is acting as supporting/child in offer.
                if (applicableOffers.Any(o => o.OfferedProductId.HasValue && string.Equals(o.OfferedProductId.Value, productDetails.Id)))
                {
                    var appliedOffer = applicableOffers.SingleOrDefault(ao => ao.OfferedProductId.HasValue && string.Equals(ao.OfferedProductId.Value, productDetails.Id));
                    if (this.CheckOfferValidity(appliedOffer, existingBillItems.Count(eb => eb.Type == BillItemType.Offer && string.Equals(eb.ItemId, appliedOffer.Id))))
                    {
                        var existingParentProductBillItems = existingBillItems.Where(billItem => billItem.Type == BillItemType.Product && string.Equals(billItem.ItemId, appliedOffer.ProductId)).ToList();
                        var existingProductBillItems = existingBillItems.Where(billItem => billItem.Type == BillItemType.Product && string.Equals(billItem.ItemId, appliedOffer.OfferedProductId.Value)).ToList();
                        if (existingParentProductBillItems.Any() && existingParentProductBillItems.Count > existingProductBillItems.Count)
                        {
                            billItems.Add(this.BuildOfferBillItem(appliedOffer, productDetails, productBillItem.Id));
                        }
                    }
                }
                return billItems;
            }
            catch(Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Build existing bill item
        /// </summary>
        /// <param name="existingBillItems"></param>
        /// <param name="billItems"></param>
        private void BuildExistingBillItem(List<BillItemDTO> existingBillItems, List<BillItemDTO> billItems)
        {
            existingBillItems.ForEach(billItem =>
            {
                var newBillItem = new BillItemDTO
                {
                    Id = billItem.Id,
                    ItemId = billItem.ItemId,
                    Code = billItem.Code,
                    Type = billItem.Type,
                    Price = billItem.Price,
                    ParentId = billItem.ParentId,
                };
                billItems.Add(billItem);
            });
        }

        /// <summary>
        /// Build offer bill item
        /// </summary>
        /// <param name="appliedOffer">The applied offer</param>
        /// <param name="productDetails">The product details</param>
        /// <param name="parentId">The parent id</param>
        /// <returns>bill items</returns>
        private BillItemDTO BuildOfferBillItem(ProductOffer appliedOffer, Product.Models.Product productDetails, Guid parentId)
        {
            return new BillItemDTO
            {
                Id = Guid.NewGuid(),
                ItemId = appliedOffer.Id,
                Code = appliedOffer.Code,
                Type = BillItemType.Offer,
                Price = appliedOffer.DiscountType == DiscountType.Value ? appliedOffer.OfferPrice : (productDetails.Price * appliedOffer.OfferPrice) / 100,
                ParentId = parentId,
            };
        }

        /// <summary>
        /// Check offer validity
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="appliedOfferCount"></param>
        /// <returns>true of offer is valid</returns>
        private bool CheckOfferValidity(ProductOffer offer, int appliedOfferCount)
        {
            return !offer.IsLimitAvailable || (offer.IsLimitAvailable && appliedOfferCount < offer.LimtPerPurchase);
        }
    }
}
