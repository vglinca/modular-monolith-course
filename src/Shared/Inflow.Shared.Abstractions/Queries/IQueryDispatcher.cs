﻿using System.Threading;
using System.Threading.Tasks;

namespace Inflow.Shared.Abstractions.Queries;

public interface IQueryDispatcher
{
    Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, 
        CancellationToken cancellationToken = default);
}