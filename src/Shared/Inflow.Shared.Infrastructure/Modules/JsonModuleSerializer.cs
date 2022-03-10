using System;
using System.Text.Json;

namespace Inflow.Shared.Infrastructure.Modules;

internal sealed class JsonModuleSerializer : IModuleSerializer
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public byte[] Serialize<T>(T value)
        => JsonSerializer.SerializeToUtf8Bytes(value, JsonSerializerOptions);

    public T Deserialize<T>(byte[] value)
        => JsonSerializer.Deserialize<T>(value, JsonSerializerOptions);

    public object Deserialize(byte[] value, Type type)
        => JsonSerializer.Deserialize(value, type, JsonSerializerOptions);
}