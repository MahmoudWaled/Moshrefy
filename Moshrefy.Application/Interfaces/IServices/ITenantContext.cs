namespace Moshrefy.Application.Interfaces.IServices
{
    public interface ITenantContext
    {
        string GetCurrentUserId();
        int? GetCurrentCenterId();
        bool IsSuperAdmin();
        bool IsAdmin();
        Task<string[]> GetCurrentUserRolesAsync();
    }
}