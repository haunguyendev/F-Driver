using F_Driver.API.Middleware;
using Microsoft.AspNetCore.Mvc.Filters;

namespace F_Driver.API.Extensions
{
    public static class ApplicationExtensions
    {
        public static void UseInfrastructure(this WebApplication app)
        {
           

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

            });



            app.UseCors("CORS");

            app.UseHttpsRedirection();
            app.UseCookiePolicy();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseAuthentication();

            app.UseAuthorization();

            

            app.MapControllers();
            


        }
    }
}
