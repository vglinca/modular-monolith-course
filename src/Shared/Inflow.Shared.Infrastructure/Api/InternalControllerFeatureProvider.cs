using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Inflow.Shared.Infrastructure.Api;

internal class InternalControllerFeatureProvider : ControllerFeatureProvider
{
    private const string ControllerTypeNameSuffix = "Controller";
    
    protected override bool IsController(TypeInfo typeInfo)
    {
        if (!typeInfo.IsClass)
        {
            return false;
        }

        if (typeInfo.IsAbstract)
        {
            return false;
        }

        if (typeInfo.ContainsGenericParameters)
        {
            return false;
        }

        if (typeInfo.IsDefined(typeof(NonControllerAttribute)))
        {
            return false;
        }

        return typeInfo.Name.EndsWith(ControllerTypeNameSuffix, StringComparison.OrdinalIgnoreCase) ||
               typeInfo.IsDefined(typeof(ControllerAttribute));
    }
}