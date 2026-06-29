using EduManage.Application.DTOs.Financial;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Application.Interfaces;

public interface IWalletService
{
    Task<WalletDto> GetWalletAsync(int instructorId);
    Task ProcessSaleAsync(int instructorId, int orderItemId, decimal grossAmount);
    Task<bool> RequestWithdrawalAsync(int instructorId, WithdrawalRequestDto dto);
}
