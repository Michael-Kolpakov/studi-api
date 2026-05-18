using AutoMapper;
using Teachio.BLL.DTOs.Users.Account.Request.Create;
using Teachio.BLL.DTOs.Users.Account.Request.Update;
using Teachio.BLL.DTOs.Users.Account.Response;
using Teachio.BLL.DTOs.Users.Auth.Request;
using AppUserEntity = Teachio.DAL.Entities.Users.Users.AppUser;

namespace Teachio.BLL.Mappings.User;

public class AppUserProfile : Profile
{
    public AppUserProfile()
    {
        CreateMap<AppUserCreateRequestDto, AppUserEntity>();

        CreateMap<AppUserUpdateRequestDto, AppUserEntity>();

        CreateMap<AuthRegisterRequestDto, AppUserEntity>();

        CreateMap<AccountUpdateRequestDto, AppUserEntity>();

        CreateMap<AppUserEntity, AppUserResponseDto>();

        CreateMap<AppUserEntity, AppUserShortResponseDto>();
    }
}
