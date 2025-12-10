using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Exceptions;

namespace Moshrefy.Application.Services
{
    // Base service providing common tenant context functionalities
    public abstract class BaseService(ITenantContext _tenantContext)
    {

        // validate and get current center id or throw exception
        protected int GetCurrentCenterIdOrThrow()
        {
            if (_tenantContext.IsSuperAdmin())
                throw new BadRequestException("SuperAdmin cannot access center-specific data through this endpoint.");

            var centerId = _tenantContext.GetCurrentCenterId();
            if (centerId == null)
                throw new UnauthorizedAccessException("User must be assigned to a center.");

            return centerId.Value;
        }

        // validate access to entity based on center id
        protected void ValidateCenterAccess(int? entityCenterId, string entityName)
        {
            if (_tenantContext.IsSuperAdmin())
                return; // SuperAdmin can access everything

            var currentCenterId = GetCurrentCenterIdOrThrow();

            if (entityCenterId != currentCenterId)
                throw new ForbiddenException($"You don't have access to this {entityName}.");
        }
    }
}