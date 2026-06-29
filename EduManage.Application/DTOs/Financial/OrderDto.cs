using System;

namespace EduManage.Application.DTOs.Financial
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}