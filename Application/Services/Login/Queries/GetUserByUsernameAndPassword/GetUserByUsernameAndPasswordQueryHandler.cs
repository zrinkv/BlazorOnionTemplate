using MediatR;

namespace Application.Services.Login.Queries.GetUserByUsernameAndPassword
{
    public class GetUserByUsernameAndPasswordQueryHandler : IRequestHandler<GetUserByUsernameAndPasswordQuery, LoggedUserViewModel>
    {
        //private readonly dbContext _context; //dodati ako treba
        //private readonly IMapper _mapper; //dodati ako treba

        //public GetUserByUsernameAndPasswordQueryHandler(dbContext context, IMapper mapper)
        //{
        //    _context = context;
        //    _mapper = mapper;
        //}

        public async Task<LoggedUserViewModel> Handle(GetUserByUsernameAndPasswordQuery request, CancellationToken cancellationToken)
        {
            LoggedUserViewModel? user = null;

            //Validate the User Credentials
            //Demo Purpose, I have Passed HardCoded User Information
            if (request.Username == "user")
            {
                List<string> roles = ["Administrator", "EndUser"];
                user = new LoggedUserViewModel {UserId = 1, Username = "User Trivedi", Email = "test.mail@mail.com", Roles = roles };
            }
            return user ?? new LoggedUserViewModel();
        }
    }
}
