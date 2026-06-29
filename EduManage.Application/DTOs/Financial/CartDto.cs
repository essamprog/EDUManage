using System.Collections.Generic;

namespace EduManage.Application.DTOs.Financial
{
    public class CartDto
    {
        public int StudentId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<int> CourseIds { get; set; } = new();
        public List<CartItemDto> Items { get; set; } = [];
    }
}