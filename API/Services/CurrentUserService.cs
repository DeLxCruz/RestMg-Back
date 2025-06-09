using Application.Common.Interfaces;
using System.Security.Claims;

namespace API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");
                return Guid.TryParse(value, out var guid) ? guid : null;
            }
        }

        public Guid? RestaurantId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue("restaurantId");
                return Guid.TryParse(value, out var guid) ? guid : null;
            }
        }

        public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
    }
}