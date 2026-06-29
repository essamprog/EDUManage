// Application/Mappings/FinancialMappingProfile.cs
using AutoMapper;
using EduManage.Application.DTOs.Financial;
using EduManage.Application.DTOs.System;
using EduManage.Core.Entities;

namespace EduManage.Application.Mappings;

public class FinancialMappingProfile : Profile
{
    public FinancialMappingProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Status,
                       o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<Wallet, WalletDto>();

        CreateMap<Notification, NotificationDto>()
            .ForMember(d => d.Type,
                       o => o.MapFrom(s => s.Type.ToString()));
    }
}