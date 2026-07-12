// Application/Services/Financial/WalletService.cs
using AutoMapper;
using EduManage.Application.DTOs.Financial;
using EduManage.Application.Interfaces;
using EduManage.Core.Entities;
using EduManage.Core.Enums;
using EduManage.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EduManage.Application.Services.Financial;

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private readonly INotificationService _notificationService;
    private readonly UserManager<ApplicationUser> _userManager;

    public WalletService(
        IUnitOfWork uow,
        IMapper mapper,
        IConfiguration config,
        INotificationService notificationService,
        UserManager<ApplicationUser> userManager)
    {
        _uow = uow;
        _mapper = mapper;
        _config = config;
        _notificationService = notificationService;
        _userManager = userManager;
    }

    public async Task<WalletDto> GetWalletAsync(int instructorId)
    {
        var wallet = await GetOrCreateWalletAsync(instructorId);
        return _mapper.Map<WalletDto>(wallet);
    }

    public async Task<WalletDto> GetFullWalletAsync(int instructorId)
    {
        var wallet = await GetOrCreateWalletAsync(instructorId);
        var dto    = _mapper.Map<WalletDto>(wallet);

        var transactions = await _uow.Transactions.Query()
            .Include(t => t.OrderItem)
                .ThenInclude(oi => oi!.Course)
            .Where(t => t.InstructorId == instructorId && t.Type == TransactionType.Sale)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        dto.Transactions = transactions.Select(t => new TransactionRecordDto
        {
            Id          = t.Id,
            GrossAmount = t.GrossAmount,
            NetAmount   = t.NetAmount,
            CourseName  = t.OrderItem?.Course?.Title ?? "---",
            Status      = t.Status.ToString(),
            CreatedAt   = t.CreatedAt,
        }).ToList();

        var withdrawals = await _uow.Withdrawals.Query()
            .Where(w => w.InstructorId == instructorId)
            .OrderByDescending(w => w.RequestedAt)
            .ToListAsync();

        dto.Withdrawals = withdrawals.Select(w => new WithdrawalRecordDto
        {
            Id            = w.Id,
            Amount        = w.Amount,
            Method        = w.Method,
            AccountDetail = w.AccountDetail,
            Status        = w.Status.ToString(),
            RequestedAt   = w.RequestedAt,
            PaidAt        = w.Status == WithdrawalStatus.Paid ? w.UpdatedAt : null,
        }).ToList();

        return dto;
    }

    public async Task ProcessSaleAsync(int instructorId, int orderItemId, decimal grossAmount)
    {
        var feePercent = decimal.Parse(_config["AppSettings:PlatformFeePercent"] ?? "20");
        var fee        = grossAmount * (feePercent / 100);
        var netAmount  = grossAmount - fee;

        await _uow.Transactions.AddAsync(new Transaction
        {
            InstructorId = instructorId,
            OrderItemId  = orderItemId,
            GrossAmount  = grossAmount,
            NetAmount    = netAmount,
            Type         = TransactionType.Sale,
            Status       = TransactionStatus.Completed,
            Description  = "Course sale earnings",
            CreatedAt    = DateTime.UtcNow,
        });

        var wallet = await GetOrCreateWalletAsync(instructorId);
        wallet.AvailableBalance += netAmount;
        wallet.LifetimeEarnings += netAmount;
        wallet.UpdatedAt = DateTime.UtcNow;
        _uow.Wallets.Update(wallet);

        await _uow.SaveChangesAsync();
    }

    public async Task<bool> RequestWithdrawalAsync(int instructorId, WithdrawalDto dto)
    {
        var wallet = await GetOrCreateWalletAsync(instructorId);
        if (wallet.AvailableBalance < dto.Amount)
            throw new InvalidOperationException("Insufficient available funds");

        wallet.AvailableBalance -= dto.Amount;
        wallet.UpdatedAt = DateTime.UtcNow;
        _uow.Wallets.Update(wallet);

        await _uow.Withdrawals.AddAsync(new Withdrawal
        {
            InstructorId  = instructorId,
            Amount        = dto.Amount,
            Method        = dto.Method,
            AccountDetail = dto.AccountDetail,
            Status        = WithdrawalStatus.Pending,
            RequestedAt   = DateTime.UtcNow,
            CreatedAt     = DateTime.UtcNow,
            UpdatedAt     = DateTime.UtcNow,
        });

        await _uow.SaveChangesAsync();

        // Notify all Admins
        var instructorUser = await _userManager.FindByIdAsync(instructorId.ToString());
        var instructorName = instructorUser?.FullName ?? $"Instructor #{instructorId}";
        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        foreach (var admin in admins)
        {
            await _notificationService.SendAsync(
                admin.Id,
                NotificationType.WithdrawalRequest,
                "Withdrawal Request",
                $"{instructorName} requested a withdrawal of {dto.Amount:N0} EGP via {dto.Method}.",
                "/Admin/Finance");
        }

        return true;
    }

    public async Task<bool> ApproveWithdrawalAsync(int withdrawalId)
    {
        var withdrawal = await _uow.Withdrawals.GetByIdAsync(withdrawalId);
        if (withdrawal is null) return false;

        withdrawal.Status    = WithdrawalStatus.Paid;
        withdrawal.UpdatedAt = DateTime.UtcNow;
        _uow.Withdrawals.Update(withdrawal);

        var wallet = await GetOrCreateWalletAsync(withdrawal.InstructorId);
        wallet.TotalWithdrawn += withdrawal.Amount;
        wallet.UpdatedAt       = DateTime.UtcNow;
        _uow.Wallets.Update(wallet);

        await _uow.SaveChangesAsync();

        await _notificationService.SendAsync(
            withdrawal.InstructorId,
            NotificationType.Payment,
            "Payment Processed",
            $"Your withdrawal of {withdrawal.Amount:N0} EGP via {withdrawal.Method} has been paid successfully.",
            "/Instructor/Financials");

        return true;
    }

    public async Task<bool> RejectWithdrawalAsync(int withdrawalId)
    {
        var withdrawal = await _uow.Withdrawals.GetByIdAsync(withdrawalId);
        if (withdrawal is null) return false;

        var wallet = await GetOrCreateWalletAsync(withdrawal.InstructorId);
        wallet.AvailableBalance += withdrawal.Amount;
        wallet.UpdatedAt         = DateTime.UtcNow;
        _uow.Wallets.Update(wallet);

        withdrawal.Status    = WithdrawalStatus.Rejected;
        withdrawal.UpdatedAt = DateTime.UtcNow;
        _uow.Withdrawals.Update(withdrawal);

        await _uow.SaveChangesAsync();

        await _notificationService.SendAsync(
            withdrawal.InstructorId,
            NotificationType.Payment,
            "Withdrawal Rejected",
            $"Your withdrawal request of {withdrawal.Amount:N0} EGP has been rejected. The amount has been returned to your balance.",
            "/Instructor/Financials");

        return true;
    }

    private async Task<Wallet> GetOrCreateWalletAsync(int instructorId)
    {
        var wallets = await _uow.Wallets.FindAsync(w => w.InstructorId == instructorId);
        var wallet  = wallets.FirstOrDefault();
        if (wallet is null)
        {
            wallet = new Wallet
            {
                InstructorId     = instructorId,
                AvailableBalance = 0,
                PendingBalance   = 0,
                TotalWithdrawn   = 0,
                LifetimeEarnings = 0,
                UpdatedAt        = DateTime.UtcNow,
            };
            await _uow.Wallets.AddAsync(wallet);
            await _uow.SaveChangesAsync();
        }
        return wallet;
    }
}