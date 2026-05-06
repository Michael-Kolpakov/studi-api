using AutoMapper;
using Teachio.BLL.DTOs.Users.Request.Create;
using Teachio.BLL.DTOs.Users.Request.Update;
using Teachio.BLL.DTOs.Users.Response;
using AppUserEntity = Teachio.DAL.Entities.Users.AppUser;

namespace Teachio.BLL.Mappings.User;

public class AppUserProfile : Profile
{
    public AppUserProfile()
    {
        CreateMap<AppUserCreateRequestDto, AppUserEntity>();
        CreateMap<AppUserUpdateRequestDto, AppUserEntity>();
        CreateMap<AppUserEntity, AppUserResponseDto>();
    }
}
