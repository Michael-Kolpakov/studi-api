using AutoMapper;
using Teachio.BLL.Dto.Users;
using Teachio.BLL.Dto.Users.Create;
using Teachio.BLL.Dto.Users.Update;
using AppUserEntity = Teachio.DAL.Entities.Users.AppUser;

namespace Teachio.BLL.Mapping.User;

public class AppUserProfile : Profile
{
    public AppUserProfile()
    {
        CreateMap<AppUserCreateDto, AppUserEntity>();
        CreateMap<AppUserUpdateDto, AppUserEntity>();
        CreateMap<AppUserEntity, AppUserResponseDto>();
    }
}
