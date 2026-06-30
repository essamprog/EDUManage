using System;
using EduManage.Core.Enums;

namespace EduManage.Core.Entities
{
    public class Coupon : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public CouponType Type { get; set; } = CouponType.Percent;
        public decimal Value { get; set; }
        public int? MaxUses { get; set; }
        public int UsedCount { get; set; } = 0;
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}