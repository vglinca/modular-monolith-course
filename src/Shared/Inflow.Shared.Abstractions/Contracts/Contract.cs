using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Inflow.Shared.Abstractions.Contracts;

public abstract class Contract<T> : IContract where T : class
{
    private readonly ISet<string> _required = new HashSet<string>();
    public Type Type { get; } = typeof(T);
    public string Module { get; }
    public IEnumerable<string> Required => _required;

    protected void Require(Expression<Func<T, object>> expression) => _required.Add(GetName(expression));
    protected void Ignore(Expression<Func<T, object>> expression) => _required.Remove(GetName(expression));

    protected string GetName(Expression<Func<T, object>> expression)
    {
        if (!(expression.Body is MemberExpression memberExpression))
        {
            memberExpression = ((UnaryExpression) expression.Body).Operand as MemberExpression;
        }

        if (memberExpression is null)
        {
            throw new InvalidOperationException("Invalid member expression");
        }

        var parts = expression.ToString().Split(",")[0].Split(".").Skip(1);
        var name = string.Join(".", parts);

        return name;
    }

    protected void RequireAll() => RequireAll(typeof(T));

    protected void RequireAll(Type type, string parent = null)
    {
        var originalContract = FormatterServices.GetUninitializedObject(type);
        var originalContractType = originalContract.GetType();
        foreach (var propertyInfo in originalContractType.GetProperties())
        {
            var propertyName = string.IsNullOrWhiteSpace(parent) ? propertyInfo.Name : $"{parent}.{propertyInfo.Name}";
            _required.Add(propertyName);
            if (propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType != typeof(string))
            {
                RequireAll(propertyInfo.PropertyType, propertyName);
            }
        }
    }

    protected void IgnoreAll() => _required.Clear();
}