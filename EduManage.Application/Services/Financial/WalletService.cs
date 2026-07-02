// Application/Services/Financial/WalletService.cs
using AutoMapper;
using EduManage.Application.DTOs.Financial;
using EduManage.Application.Interfaces;
using EduManage.Core.Entities;
using EduManage.Core.Enums;
using EduManage.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EduManage.Application.Services.Financial;

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public WalletService(IUnitOfWork uow, IMapper mapper, IConfiguration config)
    {
        _uow = uow;
        _mapper = mapper;
        _config = config;
    }

    public async Task<WalletDto> GetWalletAsync(int instructorId)
    {
        var wallets = await _uow.Wallets
            .FindAsync(w => w.InstructorId == instructorId);

        var wallet = wallets.FirstOrDefault();
        if (wallet is null)
            throw new KeyNotFoundException("Wallet not found");

        return _mapper.Map<WalletDto>(wallet);
    }

    public async Task ProcessSaleAsync(int instructorId, int orderItemId, decimal grossAmount)
    {
        var feePercent = decimal.Parse(_config["AppSettings:PlatformFeePercent"] ?? "20");
        var fee = grossAmount * (feePercent / 100);
        var netAmount = grossAmount - fee;

        // إنشاء Transaction
        await _uow.Transactions.AddAsync(new Transaction
        {
            InstructorId = instructorId,
            OrderItemId = orderItemId,
            GrossAmount = grossAmount,
            NetAmount = netAmount,
            Type = TransactionType.Sale,
            Status = TransactionStatus.Pending,   // يتحول Matured بعد Hold Days
            Description = "Course sales commission",
            CreatedAt = DateTime.UtcNow,
        });

        // تحديث الـ Wallet — المبلغ يروح PendingBalance الأول
        var wallets = await _uow.Wallets
            .FindAsync(w => w.InstructorId == instructorId);

        var wallet = wallets.FirstOrDefault();
        if (wallet is null)
        {
            // إنشاء Wallet لو مش موجودة
            wallet = new Wallet
            {
                InstructorId = instructorId,
                AvailableBalance = 0,
                PendingBalance = netAmount,
                LifetimeEarnings = netAmount,
                UpdatedAt = DateTime.UtcNow,
            };
            await _uow.Wallets.AddAsync(wallet);
        }
        else
        {
            wallet.PendingBalance += netAmount;
            wallet.LifetimeEarnings += netAmount;
            wallet.UpdatedAt = DateTime.UtcNow;
            _uow.Wallets.Update(wallet);
        }

        await _uow.SaveChangesAsync();
    }

    public async Task<bool> RequestWithdrawalAsync(int instructorId, WithdrawalDto dto)
    {
        var wallets = await _uow.Wallets
            .FindAsync(w => w.InstructorId == instructorId);

        var wallet = wallets.FirstOrDefault();
        if (wallet is null || wallet.AvailableBalance < dto.Amount)
            throw new InvalidOperationException("Insufficient available funds");

        // تجميد المبلغ
        wallet.AvailableBalance -= dto.Amount;
        wallet.UpdatedAt = DateTime.UtcNow;
        _uow.Wallets.Update(wallet);

        await _uow.Withdrawals.AddAsync(new Withdrawal
        {
            InstructorId = instructorId,
            Amount = dto.Amount,
            Method = dto.Method,
            AccountDetail = dto.AccountDetail,
            Status = WithdrawalStatus.Pending,
            RequestedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        });

        await _uow.SaveChangesAsync();
        return true;
    }
}