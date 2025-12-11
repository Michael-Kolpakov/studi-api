using AutoMapper;
using Teachio.BLL.Dto.Users.Request.Create;
using Teachio.BLL.Dto.Users.Request.Update;
using Teachio.BLL.Dto.Users.Response;
using AppUserEntity = Teachio.DAL.Entities.Users.AppUser;

namespace Teachio.BLL.Mapping.User;

public class AppUserProfile : Profile
{
    public AppUserProfile()
    {
        CreateMap<AppUserCreateRequestDto, AppUserEntity>();
        CreateMap<AppUserUpdateRequestDto, AppUserEntity>();
        CreateMap<AppUserEntity, AppUserResponseDto>();
    }
}
