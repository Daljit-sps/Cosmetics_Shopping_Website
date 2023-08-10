using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Services
{
    public class GetOrderServices : IGetOrderServices
    {
        public IGenericRepository _genericRepository;

        public GetOrderServices(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<IEnumerable<Order_Items_PaymentVM>> GetAllOrdersListAsync()
        {
            try
            {
                var getAllOrdersList = await _genericRepository.GetFromMultipleTableBasedOnConditions<Order>(e => e.IsDelete == false, e => e.User);
                return getAllOrdersList.Select(e => new Order_Items_PaymentVM
                {
                    OrderId = e.Id,
                    TotalPrice = e.TotalPrice,
                    UserName = e.User.FirstName + " " + e.User.LastName,
                    PaymentStatus = e.PaymentStatus,
                    DeliveryDate = e.DeliveryDate.ToString("MM-dd-yyyy"),
                    OrderPlacedOn = e.CreatedOn.ToString("MM-dd-yyyy"),
                    OrderStatus = e.OrderStatus
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Order_Items_PaymentVM> GetOrderDetailsByIdAsync(int orderId)
        {
            try
            {
                if (orderId > 0)
                {
                    var objOrder = await _genericRepository.GetByIdFromMultipleTable<Order>(orderId, e=>e.User);
                    var getOrderItem = await _genericRepository.Get<OrderItem>(e => e.OrderId == orderId);
                    var getUsertAddress = await _genericRepository.GetByIdFromMultipleTable<OrderItem>(getOrderItem.Id, e => e.UserAddress);
                    Order_Items_PaymentVM result = new()
                    {
                        UserName =  objOrder.User.FirstName+" "+objOrder.User.LastName,
                        TotalPrice = objOrder.TotalPrice,
                        UserAddress = getOrderItem.UserAddress.Address,
                        DeliveryDate = objOrder.DeliveryDate.ToString("MM-dd-yyyy"),
                        OrderPlacedOn = objOrder.CreatedOn.ToString("MM-dd-yyyy"),
                        OrderId = orderId,
                        PaymentStatus = objOrder.PaymentStatus,
                        OrderStatus = objOrder.OrderStatus,
                        City = getOrderItem.UserAddress.City,
                    };
                    return result;

                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Order_Items_PaymentVM> UpdateOrderStatusAsync(int orderId, string orderStatus, int logedUser)
        {
            try
            {
                var getOrder = await _genericRepository.GetByIdFromMultipleTable<Order>(orderId, e => e.User);
                if(getOrder != null && getOrder.IsDelete == false)
                {
                    getOrder.OrderStatus = orderStatus;
                    getOrder.UpdatedOn = DateTime.Now;
                    getOrder.UpdatedBy = logedUser;
                    await _genericRepository.Put(getOrder);

                    var result = new Order_Items_PaymentVM
                    {
                        OrderId = getOrder.Id,
                        OrderStatus = getOrder.OrderStatus,

                    };
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteOrder(int id, int logedUser)
        {
            try
            {
                if (id > 0)
                {
                    var productvariantDetails = await _genericRepository.GetById<Order>(id);
                    if (productvariantDetails != null)
                    {
                        productvariantDetails.IsDelete = true;
                        productvariantDetails.UpdatedBy = logedUser;
                        productvariantDetails.UpdatedOn = DateTime.Now;
                        var result = _genericRepository.Save();

                        if (result > 0)
                        {
                            return true;
                        }

                    }
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
