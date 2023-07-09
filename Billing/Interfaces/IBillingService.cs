using Billing.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Interfaces
{
    /// <summary>
    /// Interface for billing service
    /// </summary>
    public interface IBillingService
    {
        /// <summary>
        /// Gets the bill items.
        /// </summary>
        /// <param name="productCode">The product code.</param>
        /// <returns>list of bill items</returns>
        IEnumerable<BillItemDTO> GetBillItems(string productCode);
    }
}
