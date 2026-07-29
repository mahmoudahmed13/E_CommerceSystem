using E_Commerce.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace E_Commerce.API.Attributes
{
    // Action Filter Attribute
    public class RedisCashAttribute : /*IActionFilter or IAsyncActionFilter*/ ActionFilterAttribute
    //Base Class it implement /*IActionFilter or IAsyncActionFilter*/
    {
        private readonly int _durationInSec;

        public RedisCashAttribute(int durationInSec = 60)
        {
            _durationInSec = durationInSec;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Get Cash Service From Container
            
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

            var cacheKey = CreateCacheKey(context.HttpContext.Request);

            var data = await cacheService.GetDataAsync(cacheKey);

            //If data exists in Cash => Get data from cash + skip endpoint

            if (!string.IsNullOrEmpty(data))
            {
                context.Result = new ContentResult() // set result
                {
                    Content = data,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;
            }

            //If data not exists in Cash => Execute endpoint + Store Result In Cash if Result is 200OK + Data
            
            var executedContext = await next.Invoke();// Execute the next action filter or the action itself(Go to EndPoind)
            if (executedContext.Result is OkObjectResult { Value: not null } ok)
            {
                await cacheService.SetDataAsync(cacheKey, ok.Value, TimeSpan.FromSeconds(_durationInSec));
            } 


        }
        // request => https://localhost:7228/api/Products?TypeId=1&BrandId=2
        private static string CreateCacheKey(HttpRequest request) // request
        {
            var Key = new StringBuilder();
            Key.Append(request.Path); // api/Products

            if(request.Query.Any()) // ?TypeId=1&BrandId=2
            {
                Key.Append("?");
                foreach(var (k, v) in request.Query.OrderBy(x => x.Key))
                {
                    Key.Append(k).Append('=').Append(v).Append("&");
                }
            }
            return Key.ToString();
        }
    }
}
