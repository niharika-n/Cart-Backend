using Data.Interfaces;
using Data.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;
using Service.Logic;
using System.Security.Principal;

namespace WebAPIs
{
    public static class BuildUnityContainer
    {
        public static IServiceCollection RegisterAddTransient(IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IPrincipal>(
                provider => provider.GetService<IHttpContextAccessor>().HttpContext.User);

            #region Repository
            services.AddTransient<ICartRepository, CartRepository>();
            services.AddTransient<ILoginRepository, LoginRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IProductAttributeRepository, ProductAttributeRepository>();
            services.AddTransient<IDashboardRepository, DashboardRepository>();
            services.AddTransient<ITemplateRepository, TemplateRepository>();
            #endregion

            #region Services
            services.AddTransient<ICartService, CartService>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddTransient<IProductAttributeService, ProductAttributeService>();
            services.AddTransient<ITemplateService, TemplateService>();
            #endregion

            return services;
        }
    }
}
