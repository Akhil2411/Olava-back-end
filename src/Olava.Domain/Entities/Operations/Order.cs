using Olava.Domain.Common;
using Olava.Domain.Enums;

namespace Olava.Domain.Entities.Operations;

public sealed class Order : TenantEntity
{
    public Guid? WeddingId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string CurrencyCode { get; set; } = "INR";
    public long AmountMinor { get; set; }                     // paise: never store money as float
    public string? Provider { get; set; }
    public string? ProviderReference { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
}
