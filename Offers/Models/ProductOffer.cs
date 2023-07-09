using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.Models
{
    /// <summary>
    /// Class for product offer
    /// </summary>
    public class ProductOffer
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the purchase qty.
        /// </summary>
        /// <value>
        /// The purchase qty.
        /// </value>
        public decimal PurchaseQty { get; set; }

        /// <summary>
        /// Gets or sets the type of the qty matching.
        /// </summary>
        /// <value>
        /// The type of the qty matching.
        /// </value>
        public OfferQtyMatchingType QtyMatchingType { get; set; }

        /// <summary>
        /// Gets or sets the type of the discount.
        /// </summary>
        /// <value>
        /// The type of the discount.
        /// </value>
        public DiscountType DiscountType { get; set; }

        /// <summary>
        /// Gets or sets the offered product identifier.
        /// </summary>
        /// <value>
        /// The offered product identifier.
        /// </value>
        public int? OfferedProductId { get; set; }

        /// <summary>
        /// Gets or sets the offer price.
        /// </summary>
        /// <value>
        /// The offer price.
        /// </value>
        public double OfferPrice { get; set; }

        /// <summary>
        /// Gets or sets the offered product qty.
        /// </summary>
        /// <value>
        /// The offered product qty.
        /// </value>
        public decimal? OfferedProductQty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is limit available.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is limit available; otherwise, <c>false</c>.
        /// </value>
        public bool IsLimitAvailable { get; set; }

        /// <summary>
        /// Gets or sets the limt per purchase.
        /// </summary>
        /// <value>
        /// The limt per purchase.
        /// </value>
        public int? LimtPerPurchase { get; set; }
    }
}
