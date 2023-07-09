using Billing.DTOModels;
using Billing.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmerMarket.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BillingController : ControllerBase
    {
        private IBillingService BillingService { get; set; }

        public BillingController(IBillingService billingService)
        {
            this.BillingService = billingService; 
        }

        [HttpGet("billitems/{productCode}")]
        public IEnumerable<BillItemDTO> GetBillItems(string productCode)
        {
            return this.BillingService.GetBillItems(productCode);
        }
    }
}