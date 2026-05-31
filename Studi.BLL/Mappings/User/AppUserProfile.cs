using AutoMapper;
using Studi.BLL.DTOs.Users.Account.Request.Update;
using Studi.BLL.DTOs.Users.Account.Response;
using Studi.BLL.DTOs.Users.Auth.Request;
using AppUserEntity = Studi.DAL.Entities.Users.Users.AppUser;

namespace Studi.BLL.Mappings.User;

public class AppUserProfile : Profile
{
    public AppUserProfile()
    {
        CreateMap<AuthRegisterRequestDto, AppUserEntity>();

        CreateMap<AccountUpdateRequestDto, AppUserEntity>();

        CreateMap<AppUserEntity, AppUserResponseDto>();

        CreateMap<AppUserEntity, AppUserShortResponseDto>();
    }
}
