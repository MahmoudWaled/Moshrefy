namespace Moshrefy.Domain.Interfaces
{
    public interface IAuditable
    {
        string? CreatedById { get; set; }
        string? CreatedByName { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        string? ModifiedById { get; set; }
        string? ModifiedByName { get; set; }
        DateTimeOffset? ModifiedAt { get; set; }
    }
}