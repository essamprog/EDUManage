using AutoMapper;
using EduManage.Application.DTOs.Common;
using EduManage.Application.DTOs.Financial;
using EduManage.Application.Interfaces;
using EduManage.Core.Entities;
using EduManage.Core.Enums;
using EduManage.Core.Interfaces;

namespace EduManage.Application.Services.Financial;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    // ── Cart ─────────────────────────────────────────────
    public async Task<CartDto> GetCartAsync(int studentId)
    {
        var items = await _uow.CartItems
            .FindAsync(c => c.StudentId == studentId);

        var cartItems = new List<CartItemDto>();
        decimal total = 0;

        foreach (var item in items)
        {
            var course = await _uow.Courses.GetByIdAsync(item.CourseId);
            if (course is null) continue;

            cartItems.Add(new CartItemDto
            {
                CourseId = course.Id,
                Title = course.Title,
                ThumbnailUrl = course.ThumbnailUrl,
                Price = course.Price,
                AddedAt = item.AddedAt,
            });
            total += course.Price;
        }

        return new CartDto
        {
            Items = cartItems,
            TotalPrice = total,
        };
    }

    public async Task AddToCartAsync(int studentId, int courseId)
    {
        // تحقق إنه مش مسجل أصلاً
        var enrolled = await _uow.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        if (enrolled)
            throw new InvalidOperationException("You are already enrolled in this course.");

        // تحقق إنه مش في السلة أصلاً
        var inCart = await _uow.CartItems
            .AnyAsync(c => c.StudentId == studentId && c.CourseId == courseId);
        if (inCart)
            throw new InvalidOperationException("The course is already in your cart.");

        await _uow.CartItems.AddAsync(new CartItem
        {
            StudentId = studentId,
            CourseId = courseId,
            AddedAt = DateTime.UtcNow,
        });
        await _uow.SaveChangesAsync();
    }

    public async Task RemoveFromCartAsync(int studentId, int courseId)
    {
        var items = await _uow.CartItems
            .FindAsync(c => c.StudentId == studentId && c.CourseId == courseId);

        var item = items.FirstOrDefault()
            ?? throw new KeyNotFoundException("The course is not in the basket.");

        _uow.CartItems.Delete(item);
        await _uow.SaveChangesAsync();
    }

    // ── Coupon ────────────────────────────────────────────
    public async Task<bool> ApplyCouponAsync(int studentId, string couponCode)
    {
        var coupons = await _uow.Coupons
            .FindAsync(c =>
                c.Code == couponCode &&
                c.IsActive == true &&
                (c.ExpiresAt == null || c.ExpiresAt > DateTime.UtcNow) &&
                (c.MaxUses == null || c.UsedCount < c.MaxUses));

        return coupons.Any();
    }

    // ── Checkout ──────────────────────────────────────────
    public async Task<OrderDto> CheckoutAsync(int studentId, CheckoutDto dto)
    {
        var cartItems = await _uow.CartItems
            .FindAsync(c => c.StudentId == studentId);

        if (!cartItems.Any())
            throw new InvalidOperationException("The basket is empty");

        decimal total = 0;
        decimal discount = 0;

        // حساب الإجمالي
        var courseIds = cartItems.Select(c => c.CourseId).ToList();
        var prices = new Dictionary<int, decimal>();

        foreach (var cartItem in cartItems)
        {
            var course = await _uow.Courses.GetByIdAsync(cartItem.CourseId);
            if (course is null) continue;
            prices[course.Id] = course.Price;
            total += course.Price;
        }

        // تطبيق الكوبون
        if (!string.IsNullOrEmpty(dto.CouponCode))
        {
            var coupons = await _uow.Coupons
                .FindAsync(c =>
                    c.Code == dto.CouponCode &&
                    c.IsActive == true &&
                    (c.ExpiresAt == null || c.ExpiresAt > DateTime.UtcNow) &&
                    (c.MaxUses == null || c.UsedCount < c.MaxUses));

            var coupon = coupons.FirstOrDefault();
            if (coupon is not null)
            {
                discount = coupon.Type == CouponType.Percent
                    ? total * (coupon.Value / 100)
                    : coupon.Value;

                coupon.UsedCount++;
                _uow.Coupons.Update(coupon);
            }
        }

        // إنشاء الـ Order
        var order = new Order
        {
            StudentId = studentId,
            TotalAmount = total - discount,
            Discount = discount,
            CouponCode = dto.CouponCode,
            PaymentMethod = dto.PaymentMethod,
            TransactionId = dto.TransactionId,
            Status = OrderStatus.Completed,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        await _uow.Orders.AddAsync(order);
        await _uow.SaveChangesAsync();

        // إنشاء OrderItems + Enrollments
        foreach (var cartItem in cartItems)
        {
            var price = prices.GetValueOrDefault(cartItem.CourseId, 0);

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                CourseId = cartItem.CourseId,
                Price = price,
            };
            await _uow.OrderItems.AddAsync(orderItem);
            await _uow.SaveChangesAsync();

            // تسجيل الطالب في الكورس أوتوماتيك
            var alreadyEnrolled = await _uow.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == cartItem.CourseId);

            if (!alreadyEnrolled)
            {
                await _uow.Enrollments.AddAsync(new EduManage.Core.Entities.Enrollment
                {
                    StudentId = studentId,
                    CourseId = cartItem.CourseId,
                    Status = EnrollmentStatus.InProgress,
                    EnrolledAt = DateTime.UtcNow,
                });

                // تحديث عدد الطلاب في الكورس
                var course = await _uow.Courses.GetByIdAsync(cartItem.CourseId);
                if (course is not null)
                {
                    course.TotalStudents++;
                    _uow.Courses.Update(course);
                }
            }

            // تحويل نسبة للـ Instructor عن طريق WalletService
            // عضو 3 هيربطه بعد ما يخلص WalletService
        }

        // مسح السلة بعد الـ Checkout
        foreach (var cartItem in cartItems)
            _uow.CartItems.Delete(cartItem);

        await _uow.SaveChangesAsync();

        return _mapper.Map<OrderDto>(order);
    }
}