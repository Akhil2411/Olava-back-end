using Olava.Domain.Common;
using Olava.Domain.Enums;

namespace Olava.Domain.Entities.Operations;

public sealed class MediaAsset : TenantEntity
{
    public Guid? WeddingId { get; set; }
    public MediaPurpose Purpose { get; set; }
    public string StorageKey { get; set; } = string.Empty;   // object-storage key, never the bytes
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? Caption { get; set; }
    public int SortOrder { get; set; }
}