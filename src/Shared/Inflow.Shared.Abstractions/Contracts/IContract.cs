using System;
using System.Collections.Generic;

namespace Inflow.Shared.Abstractions.Contracts;

public interface IContract
{
    Type Type { get; }
    string Module { get; }
    public IEnumerable<string> Required { get; }
}