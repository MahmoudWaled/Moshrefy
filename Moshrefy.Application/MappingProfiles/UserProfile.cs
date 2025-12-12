using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moshrefy.Application.DTOs.User;
using Moshrefy.Domain.Identity;

namespace Moshrefy.Application.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDTO, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());

            CreateMap<UpdateUserDTO, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ApplicationUser, UserResponseDTO>()
                .ForMember(dest => dest.CenterName, opt => opt.MapFrom(src => src.Center != null ? src.Center.Name : null))
                .ForMember(dest => dest.CenterId, opt => opt.MapFrom(src => src.CenterId))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByName))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom<UserRolesResolver>());
        }
    }

    public class UserRolesResolver : IValueResolver<ApplicationUser, UserResponseDTO, string?>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRolesResolver(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string? Resolve(ApplicationUser source, UserResponseDTO destination, string? destMember, ResolutionContext context)
        {
            var roles = _userManager.GetRolesAsync(source).GetAwaiter().GetResult();
            return roles.FirstOrDefault();
        }
    }
}
