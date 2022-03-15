using System.Runtime.CompilerServices;
using Inflow.Modules.Payments.Core.DAL;
using Inflow.Modules.Payments.Core.DAL.Repositories;
using Inflow.Modules.Payments.Core.Deposits.Domain.Factories;
using Inflow.Modules.Payments.Core.Deposits.Domain.Repositories;
using Inflow.Modules.Payments.Core.Deposits.Domain.Services;
using Inflow.Modules.Payments.Core.Withdrawals.Domain.Repositories;
using Inflow.Modules.Payments.Core.Withdrawals.Services;
using Inflow.Modules.Payments.Infrastructure.Clients;
using Inflow.Modules.Payments.Infrastructure.Repositories;
using Inflow.Shared.Infrastructure.Messaging.Outbox;
using Inflow.Shared.Infrastructure.Postgres;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Inflow.Modules.Payments.Api")]
namespace Inflow.Modules.Payments.Core;

internal static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
        => services
            .AddSingleton<ICustomerApiClient, CustomerApiClient>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IDepositRepository, DepositRepository>()
            .AddScoped<IDepositAccountRepository, DepositAccountRepository>()
            .AddScoped<IWithdrawalRepository, WithdrawalRepository>()
            .AddScoped<IWithdrawalAccountRepository, WithdrawalAccountRepository>()
            .AddSingleton<ICurrencyResolver, CurrencyResolver>()
            .AddSingleton<IDepositAccountFactory, DepositAccountFactory>()
            .AddSingleton<IWithdrawalMetadataResolver, WithdrawalMetadataResolver>()
            .AddPostgres<PaymentsDbContext>()
            .AddOutbox<PaymentsDbContext>()
            .AddUnitOfWork<PaymentsUnitOfWork>();
}