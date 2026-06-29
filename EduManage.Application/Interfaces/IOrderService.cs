using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EduManage.Application.DTOs.Financial;

namespace EduManage.Application.Interfaces;

public interface IOrderService
{
    Task<CartDto> GetCartAsync(int studentId);
    Task AddToCartAsync(int studentId, int courseId);
    Task RemoveFromCartAsync(int studentId, int courseId);
    Task<OrderDto> CheckoutAsync(int studentId, CheckoutDto dto);
    Task<bool> ApplyCouponAsync(int studentId, string couponCode);
}