using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.Models
{
    /// <summary>
    /// This is the enum for qty matching of product for offer.
    /// </summary>
    public enum OfferQtyMatchingType
    {
        Equal,
        EqualAndGreater,
        Greater,
    }

    /// <summary>
    /// This enum is for differentiate wheather offer is applicable on bill item.
    /// </summary>
    public enum OfferApplicableType
    {
        SameBillItem,
        AnotherBillItem,
        Both,
    }

    /// <summary>
    /// enum for discount type.
    /// </summary>
    public enum DiscountType
    {
        Percentage,
        Value,
    }
}
