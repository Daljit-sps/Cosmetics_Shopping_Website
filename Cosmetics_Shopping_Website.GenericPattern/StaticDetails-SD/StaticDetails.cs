using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.StaticDetails_SD
{
    public static class StaticDetails
    {
        public const string OrderStatusPending = "Pending";
        public const string OrderStatusApproved = "Approved";
        public const string OrderStatusOrderPacked = "Order Packed";
        public const string OrderStatusShipped = "Shipped";
        public const string OrderStatusDelivered = "Delivered";
        public const string OrderStatusCancelled = "Cancelled";
        public const string OrderStatusRefunded = "Refunded";


        public const string PaymentStatusPaid = "paid";
        public const string PaymentStatusNotPaid = "NotPaid";

    }
}
