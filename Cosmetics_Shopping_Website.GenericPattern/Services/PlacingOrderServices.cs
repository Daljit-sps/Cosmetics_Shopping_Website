using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.Repositories;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using Cosmetics_Shopping_Website.GenericPattern.StaticDetails_SD;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stripe;
using System.Runtime.Remoting;
using Microsoft.AspNetCore.Mvc;

namespace Cosmetics_Shopping_Website.GenericPattern.Services
{
    public class PlacingOrderServices: IPlacingOrderServices
    {
        public IGenericRepository _genericRepository;
        
        public PlacingOrderServices(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<Order> CreateOrderAsync(decimal totalPrice, int logedUser)
        {
            try
            {
                if (totalPrice>0)
                {
                    Order newOrder = new()
                    {
                        UserId = logedUser,
                        OrderStatus = StaticDetails.OrderStatusPending,
                        PaymentStatus = StaticDetails.PaymentStatusNotPaid,
                        DeliveryDate = DateTime.Now.AddDays(4),
                        TotalPrice = totalPrice,
                        CreatedBy = logedUser,
                        CreatedOn = DateTime.Now,
                        UpdatedBy = logedUser,
                        UpdatedOn = DateTime.Now,
                    };
                    var result= await _genericRepository.Post<Order>(newOrder);
                    
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }


        }

        public async void UpdateOrderStripePaymentIdAsync(int orderId, string sessionId)
        {
            try
            {
                var objOrder = await _genericRepository.GetById<Order>(orderId);
                if (objOrder != null && objOrder.IsDelete == false)
                {
                    objOrder.Id = orderId;
                    objOrder.SessionId = sessionId;
                    objOrder.UpdatedOn = DateTime.Now;

                    await _genericRepository.Put(objOrder);

                }
                
                
            }
            catch (Exception)
            {
                throw;
            }


        }

        public async void UpdateOrderStatusAsync(int orderId, string orderStatus, string? paymentStatus, string paymentIntentId)
        {
            try
            {
                var objOrder = await _genericRepository.GetById<Order>(orderId);
                if (objOrder != null && objOrder.IsDelete == false)
                {
                    objOrder.Id = orderId;
                    objOrder.OrderStatus = orderStatus;
                    if(paymentStatus != null)
                    {
                        objOrder.PaymentStatus = paymentStatus;
                        objOrder.PaymentIntentId = paymentIntentId;
                    }

                    objOrder.UpdatedOn = DateTime.Now;

                    await _genericRepository.Put(objOrder);

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            try
            {
                if (orderId > 0)
                {
                    var objOrder = await _genericRepository.GetById<Order>(orderId);
                    return objOrder;

                }
                return null;
            }
            catch(Exception)
            {
                throw;
            }
        }
        

        public async Task<Order_Items_PaymentVM> GetOrderDetailsByIdAsync(int orderId, string logedUserFirstName, string logedUserLastName)
        {
            try
            {
                if (orderId > 0)
                {
                    var objOrder = await _genericRepository.GetById<Order>(orderId);
                    var getOrderItem = await _genericRepository.Get<OrderItem>(e=>e.OrderId == orderId);
                    var getUsertAddress = await _genericRepository.GetByIdFromMultipleTable<OrderItem>(getOrderItem.Id, e=>e.UserAddress);
                    Order_Items_PaymentVM result = new()
                    {
                        UserName = logedUserFirstName+" "+logedUserLastName,
                        TotalPrice = objOrder.TotalPrice,
                        UserAddress = getOrderItem.UserAddress.Address,
                        DeliveryDate = objOrder.DeliveryDate.ToString("MM-dd-yyyy"),
                        OrderPlacedOn = objOrder.CreatedOn.ToString("MM-dd-yyyy"),
                        OrderId = orderId,
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

        public async Task<string> CreateOrderItemsAsync(int orderId, IEnumerable<CartItemsVM> variantsList, int userAddressId, int logedUser)
        {
            try
            {
                string Response = string.Empty;
                if (variantsList != null)
                {

                    foreach (var variant in variantsList)
                    {

                        OrderItem newOrderItem = new()
                        {
                            OrderId = orderId,
                            UserAddressId = userAddressId,
                            ProductVariantId = variant.ProductVariantId,
                            Quantity = variant.Quantity,
                            CreatedBy = logedUser,
                            CreatedOn = DateTime.Now,
                            UpdatedBy = logedUser,
                            UpdatedOn = DateTime.Now,
                        };
                        var result = await _genericRepository.Post<OrderItem>(newOrderItem);
                    }
                    Response = "pass";
                }
                return Response;
               
               
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<IEnumerable<Order_Items_PaymentVM>> GetOrderItemsListOfUserAsync(int orderid)
        {
            try
            {
                var getOrderItems = await _genericRepository.GetFromMultipleTableBasedOnConditions<OrderItem>(e=>e.OrderId == orderid && e.IsDelete == false, e=>e.Order, e=>e.ProductVariant);
                if(getOrderItems != null)
                {
                    return getOrderItems.Select(e => new Order_Items_PaymentVM
                    {
                        OrderId = e.Id,
                        ProductVariantId = e.ProductVariantId,
                        ProductVariantName = e.ProductVariant.VariantName,
                        Price = e.ProductVariant.Price,
                        VariantImage = e.ProductVariant.VariantImage,
                        Quantity = e.Quantity
                    }).ToList();
                }
                else
                {
                    return Enumerable.Empty<Order_Items_PaymentVM>();
                }
                
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Order_Items_PaymentVM>> GetUserOrdersListAsync(int logeduser)
        {
            try
            {
                if(logeduser > 0)
                {
                    var getUserOrdersList = await _genericRepository.GetFromMultipleTableBasedOnConditions<Order>(e=>e.UserId == logeduser && e.IsDelete == false, e=>e.User);
                    return getUserOrdersList.Select(e=> new Order_Items_PaymentVM
                    {
                        OrderId = e.Id,
                        TotalPrice = e.TotalPrice,
                        UserName = e.User.FirstName+" "+e.User.LastName,
                        PaymentStatus = e.PaymentStatus,
                        DeliveryDate = e.DeliveryDate.ToString("MM-dd-yyyy"),
                        OrderPlacedOn = e.CreatedOn.ToString("MM-dd-yyyy"),
                        OrderStatus = e.OrderStatus
                    }).ToList();
                }
                else
                {
                    return Enumerable.Empty<Order_Items_PaymentVM>();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

       
    }
}
