using RoutingExample.CustomConstraints;

namespace RoutingExample
{
    public class Program
    {
        public static void Main(string[] args) 
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRouting(options =>
            {
                options.ConstraintMap.Add("months", typeof(MonthsCustomConstraint));
            });
            var app = builder.Build();

            //app.UseRouting();

            app.Map("/files/{filename}.{extension}", async (httpContext) =>
            {
                string? fileName = httpContext.Request.RouteValues["filename"]?.ToString();
                string? extension = httpContext.Request.RouteValues["extension"]?.ToString();

                await httpContext.Response.WriteAsync($"In files - {fileName}.{extension}");
            });
            app.Map("/employee/profile/{employeename:alpha:length(3, 15)=Jason}", async (httpContext) =>
            {
                if (httpContext.Request.Query.ContainsKey("employeename"))
                {
                    string? employeeName = httpContext.Request.RouteValues["employeename"]?.ToString();
                    await httpContext.Response.WriteAsync($"In Employee profile - {employeeName}");
                }
            });
            app.Map("/products/details/{id:int?}", async (httpContext) =>
            {
                if (httpContext.Request.Query.ContainsKey("id"))
                {
                    bool valid = int.TryParse(httpContext.Request.RouteValues["id"]?.ToString(), out int id);
                    if (valid)
                    {
                        await httpContext.Response.WriteAsync($"Product details - {id}");
                    }
                }
                else
                {
                    await httpContext.Response.WriteAsync("Product details - id is not supplied");
                }
                
            });

            app.Map("/daily-digest-report/{reportdate:datetime}", async (httpContext) =>
            {
                DateTime reportDate = Convert.ToDateTime(httpContext.Request.RouteValues["reportdate"]);

                await httpContext.Response.WriteAsync($"In daily-digest-report - {reportDate.ToShortDateString()}");
            });
            app.Map("cities/{cityid:guid}", async (httpContext) =>
            {
                Guid cityId = Guid.Parse(httpContext.Request.RouteValues["cityid"]?.ToString());
                await httpContext.Response.WriteAsync($"City Information - {cityId}");
            });
            app.Map("sales-report/{year:int:min(1900)}/{month:months}", async (httpContext) =>
            {
                int year = Convert.ToInt32(httpContext.Request.RouteValues["year"]);
                string month = httpContext.Request.RouteValues["month"]?.ToString()?.ToUpper() ?? "";

                
                await httpContext.Response.WriteAsync($"sales report - {year} - {month}");
            });
            app.MapFallback(async (httpContext) =>
            {
                await httpContext.Response.WriteAsync($"No route matched at {httpContext.Request.Path}");
            });
            app.Run();
        }

    }
}
