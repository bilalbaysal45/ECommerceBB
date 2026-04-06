namespace ECommerce.Gateway.API
{
    public class ClaimToHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimToHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                Console.WriteLine("[Gateway-Error] HttpContext is NULL! Accessor cannot find context.");
            }
            else if (httpContext.User?.Identity?.IsAuthenticated != true)
            {
                Console.WriteLine("[Gateway-Error] User is NOT authenticated in Handler!");
            }
            else
            {
                // Tüm claimleri tek tek yazdırıp hangisi "userId" görelim
                foreach (var claim in httpContext.User.Claims)
                {
                    Console.WriteLine($"[Gateway-Debug] Found Claim: {claim.Type} = {claim.Value}");
                }

                var userId = httpContext.User.FindFirst("userId")?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    request.Headers.Remove("X-User-Id");
                    request.Headers.Add("X-User-Id", userId);
                    Console.WriteLine($"[Gateway-Success] HEADER ADDED: X-User-Id = {userId}");
                }
                else
                {
                    Console.WriteLine("[Gateway-Error] userId claim found but it's EMPTY or NULL!");
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
