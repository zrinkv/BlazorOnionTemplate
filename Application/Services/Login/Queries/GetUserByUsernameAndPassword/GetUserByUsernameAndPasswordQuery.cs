using MediatR;
using Models.Login;

namespace Application.Services.Login.Queries.GetUserByUsernameAndPassword
{
    public class GetUserByUsernameAndPasswordQuery : LoginRequestModel, IRequest<LoggedUserViewModel>
    {
    }
}
