using EduManage.Application.DTOs.Financial;
using EduManage.Application.Interfaces;
using EduManage.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduManage.Web.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly IOrderService _orderService;

    public CartController(IOrderService orderService)
        => _orderService = orderService;

    private int UserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ── View Cart ─────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        var cart = await _orderService.GetCartAsync(UserId);
        return View(cart);
    }

    // ── Add ───────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int courseId)
    {
        try
        {
            await _orderService.AddToCartAsync(UserId, courseId);
            TempData["Success"] = "Added to cart";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Details", "Courses", new { id = courseId });
    }

    // ── Remove ────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int courseId)
    {
        await _orderService.RemoveFromCartAsync(UserId, courseId);
        return RedirectToAction("Index");
    }

    // ── Apply Coupon ──────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplyCoupon(string couponCode)
    {
        var valid = await _orderService.ApplyCouponAsync(UserId, couponCode);
        TempData[valid ? "Success" : "Error"] = valid
            ? "The coupon was successfully applied."
            : "The discount code is invalid or expired.";

        return RedirectToAction("Index");
    }

    // ── Checkout ──────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cart = await _orderService.GetCartAsync(UserId);
        if (!cart.Items.Any())
            return RedirectToAction("Index");

        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutDto dto)
    {
        try
        {
            var order = await _orderService.CheckoutAsync(UserId, dto);
            TempData["Success"] = "Purchase successful!";
            return RedirectToAction("Index", "Dashboard", new { area = "Student" });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index");
        }
    }
}