using System.Threading.Tasks;
using Marketplace.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Marketplace.UserProfile
{
    [Route("/profile")]
    public class UserProfileCommandsApi : Controller
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<UserProfileCommandsApi>();
        private readonly UserProfileApplicationService _applicationService;

        public UserProfileCommandsApi(UserProfileApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpPost]
        public Task<IActionResult> Post(Contracts.V1.RegisterUser request)
        {
            return RequestHandler.HandleCommand(request, _applicationService.Handle, Log);
        }

        [Route("fullname")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.V1.UpdateUserFullName request)
        {
            return RequestHandler.HandleCommand(request, _applicationService.Handle, Log);
        }

        [Route("displayname")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.V1.UpdateUserDisplayName request)
        {
            return RequestHandler.HandleCommand(request, _applicationService.Handle, Log);
        }

        [Route("photo")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.V1.UpdateUserProfilePhoto request)
        {
            return RequestHandler.HandleCommand(request, _applicationService.Handle, Log);
        }
    }
}