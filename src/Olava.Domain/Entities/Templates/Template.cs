using Olava.Domain.Common;
using Olava.Domain.Enums;

namespace Olava.Domain.Entities.Templates;

/// <summary>Platform-owned (NOT tenant-owned): shared by all customers.</summary>
public sealed class Template : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public bool IsPremium { get; set; }
    public string? PreviewImageUrl { get; set; }
    public TemplateStatus Status { get; set; } = TemplateStatus.Draft;
    public ICollection<TemplateVersion> Versions { get; } = [];
}