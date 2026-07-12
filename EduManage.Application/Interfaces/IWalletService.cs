using EduManage.Application.DTOs.Financial;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Application.Interfaces;

public interface IWalletService
{
    Task<WalletDto> GetWalletAsync(int instructorId);
    Task<WalletDto> GetFullWalletAsync(int instructorId);   // includes transactions & withdrawals
    Task ProcessSaleAsync(int instructorId, int orderItemId, decimal grossAmount);
    Task<bool> RequestWithdrawalAsync(int instructorId, WithdrawalDto dto);
    Task<bool> ApproveWithdrawalAsync(int withdrawalId);    // pays + notifies instructor
    Task<bool> RejectWithdrawalAsync(int withdrawalId);     // refunds + notifies instructor
}
