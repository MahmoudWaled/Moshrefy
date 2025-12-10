using Microsoft.AspNetCore.Authorization;
using Moshrefy.Application.Authorization.Requirements;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.Authorization.Handlers
{
    public class CenterAccessHandler(ITenantContext _tenantContext) : AuthorizationHandler<CenterAccessRequirement>
    {
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CenterAccessRequirement requirement)
        {
            // Have all permissions if SuperAdmin
            if (_tenantContext.IsSuperAdmin())
            {
                context.Succeed(requirement);
                return;
            }

            var centerId = _tenantContext.GetCurrentCenterId();

            // User must be assigned to a center otherwise fail
            if (centerId == null)
            {
                context.Fail();
                return;
            }

            var roles = await _tenantContext.GetCurrentUserRolesAsync();

            // Check permissions based on roles
            if (HasPermission(roles, requirement.Entity, requirement.Action))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }

        private bool HasPermission(string[] roles, string entity, string action)
        {
            var role = roles.FirstOrDefault();

            // تحويل الـ string role إلى RolesNames enum للمقارنة
            if (!Enum.TryParse<RolesNames>(role, out var roleEnum))
                return false;

            return (entity, action, roleEnum) switch
            {
                // Student permissions
                ("Student", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Student", "Add", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Student", "Update", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Student", "Delete", RolesNames.Admin or RolesNames.Manager) => true,

                // Teacher permissions
                ("Teacher", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Teacher", "Add", RolesNames.Admin or RolesNames.Manager) => true,
                ("Teacher", "Update", RolesNames.Admin or RolesNames.Manager) => true,
                ("Teacher", "Delete", RolesNames.Admin) => true,

                // Course permissions
                ("Course", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Course", "Add", RolesNames.Admin or RolesNames.Manager) => true,
                ("Course", "Update", RolesNames.Admin or RolesNames.Manager) => true,
                ("Course", "Delete", RolesNames.Admin) => true,

                // Classroom permissions
                ("Classroom", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Classroom", "Add", RolesNames.Admin or RolesNames.Manager) => true,
                ("Classroom", "Update", RolesNames.Admin or RolesNames.Manager) => true,
                ("Classroom", "Delete", RolesNames.Admin) => true,

                // AcademicYear permissions
                ("AcademicYear", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("AcademicYear", "Add", RolesNames.Admin) => true,
                ("AcademicYear", "Update", RolesNames.Admin) => true,
                ("AcademicYear", "Delete", RolesNames.Admin) => true,

                // Enrollment permissions
                ("Enrollment", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Enrollment", "Add", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Enrollment", "Update", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Enrollment", "Delete", RolesNames.Admin or RolesNames.Manager) => true,

                // TeacherCourse permissions
                ("TeacherCourse", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("TeacherCourse", "Add", RolesNames.Admin or RolesNames.Manager) => true,
                ("TeacherCourse", "Update", RolesNames.Admin or RolesNames.Manager) => true,
                ("TeacherCourse", "Delete", RolesNames.Admin or RolesNames.Manager) => true,

                // TeacherItem permissions
                ("TeacherItem", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("TeacherItem", "Add", RolesNames.Admin or RolesNames.Manager) => true,
                ("TeacherItem", "Update", RolesNames.Admin or RolesNames.Manager) => true,
                ("TeacherItem", "Delete", RolesNames.Admin or RolesNames.Manager) => true,

                // Session permissions
                ("Session", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Session", "Add", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Session", "Update", RolesNames.Admin or RolesNames.Manager) => true,
                ("Session", "Delete", RolesNames.Admin or RolesNames.Manager) => true,

                // Exam permissions
                ("Exam", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Exam", "Add", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Exam", "Update", RolesNames.Admin or RolesNames.Manager) => true,
                ("Exam", "Delete", RolesNames.Admin or RolesNames.Manager) => true,

                // ExamResult permissions
                ("ExamResult", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("ExamResult", "Add", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("ExamResult", "Update", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("ExamResult", "Delete", RolesNames.Admin or RolesNames.Manager) => true,

                // Attendance permissions
                ("Attendance", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Attendance", "Add", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Attendance", "Update", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Attendance", "Delete", RolesNames.Admin or RolesNames.Manager) => true,

                // Invoice permissions
                ("Invoice", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Invoice", "Add", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Invoice", "Update", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Invoice", "Delete", RolesNames.Admin or RolesNames.Manager) => true,

                // Payment permissions
                ("Payment", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Payment", "Add", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Payment", "Update", RolesNames.Admin or RolesNames.Manager) => true,
                ("Payment", "Delete", RolesNames.Admin or RolesNames.Manager) => true,

                // Item permissions
                ("Item", "View", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Item", "Add", RolesNames.Admin or RolesNames.Manager or RolesNames.Employee) => true,
                ("Item", "Update", RolesNames.Admin or RolesNames.Manager) => true,
                ("Item", "Delete", RolesNames.Admin or RolesNames.Manager) => true,

                _ => false
            };
        }
    }
}