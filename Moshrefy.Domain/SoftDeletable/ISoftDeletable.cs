namespace Moshrefy.Domain.SoftDeletable
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}
