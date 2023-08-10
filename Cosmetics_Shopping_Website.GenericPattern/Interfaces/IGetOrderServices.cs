using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.Repositories;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Interfaces
{
    public interface IGetOrderServices
    {
        Task<IEnumerable<Order_Items_PaymentVM>> GetAllOrdersListAsync();

        Task<Order_Items_PaymentVM> GetOrderDetailsByIdAsync(int orderId);

        Task<Order_Items_PaymentVM> UpdateOrderStatusAsync(int orderId, string orderStatus, int logedUser);
        
        Task<bool> DeleteOrder(int id, int logedUser);
    }
}
