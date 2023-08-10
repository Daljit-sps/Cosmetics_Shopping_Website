using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Interfaces
{
    public interface IPlacingOrderServices
    {
        Task<Order> CreateOrderAsync(decimal totalPrice, int logedUser);

        Task<Order> GetOrderByIdAsync(int orderId);

        Task<Order_Items_PaymentVM> GetOrderDetailsByIdAsync(int orderId, string logedUserFirstName, string logedUserLastName);

        Task<IEnumerable<Order_Items_PaymentVM>> GetUserOrdersListAsync(int logeduser);

       

        Task<IEnumerable<Order_Items_PaymentVM>> GetOrderItemsListOfUserAsync(int orderid);

        void UpdateOrderStatusAsync(int orderId, string orderStatus, string? paymentStatus, string paymentIntentId);

        void UpdateOrderStripePaymentIdAsync(int orderId, string sessionId);

      

        Task<string> CreateOrderItemsAsync(int orderId, IEnumerable<CartItemsVM> variantsList, int userAddressId, int logedUser);
    }
}
