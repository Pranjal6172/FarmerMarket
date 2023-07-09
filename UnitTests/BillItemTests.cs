using Billing.DTOModels;
using Billing.Interfaces;
using Billing.Services;
using FarmerMarket.API.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Product.Interfaces;
using Product.Services;

namespace UnitTests
{
    /// <summary>
    /// Class for bill item tests
    /// </summary>
    [TestClass]
    public class BillItemTests
    {
        /// <summary>
        /// The billing service
        /// </summary>
        private readonly IBillingService BillingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BillItemTests"/> class.
        /// </summary>
        public BillItemTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddScoped<IBillingService, BillingService>();
            serviceCollection.AddScoped<IProductService, ProductService>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            ContextService.MemoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
            BillingService = serviceProvider.GetService<IBillingService>();
            ContextService.BuildCache();
        }

        /// <summary>
        /// Tests the bill items.
        /// </summary>
        [TestMethod]
        public void TestBillItems()
        {
            // pass any combination of basket item.
            var productCodes = new List<string> { "CH1", "AP1", "AP1", "AP1", "MK1" };
            var billItems = new List<BillItemDTO>();
            productCodes.ForEach(code =>
            {
                billItems = BillingService.GetBillItems(code).ToList();
            });

            // match exact total with basket total, summing up all the product and subtracting offers.
            Assert.AreEqual(16.61, billItems.Sum(item => item.Type == BillItemType.Product ? item.Price : -item.Price));
        }
    }
}