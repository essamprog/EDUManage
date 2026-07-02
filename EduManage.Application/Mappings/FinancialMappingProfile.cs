using AutoMapper;
using EduManage.Application.DTOs.Financial;
using EduManage.Application.DTOs.System;
using EduManage.Core.Entities;

namespace EduManage.Application.Mappings;

public class FinancialMappingProfile : Profile
{
    public FinancialMappingProfile()
    {
        // Order → OrderDto
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Status,
                       o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Items,
                       o => o.MapFrom(s => s.Items));

        // OrderItem → OrderItemDto
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.CourseTitle,
                       o => o.MapFrom(s => s.Course != null
                           ? s.Course.Title
                           : string.Empty));

        // Wallet → WalletDto
        CreateMap<Wallet, WalletDto>();

        // Withdrawal → WithdrawalDto
        CreateMap<Withdrawal, WithdrawalDto>()
            .ForMember(d => d.Status,
                       o => o.MapFrom(s => s.Status.ToString()));

        // Notification → NotificationDto
        CreateMap<Notification, NotificationDto>()
            .ForMember(d => d.Type,
                       o => o.MapFrom(s => s.Type.ToString()));
    }
}