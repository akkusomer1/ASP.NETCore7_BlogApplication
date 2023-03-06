using Microsoft.Extensions.DependencyInjection;
using ProgrammersBlog.Core.Abstract.İnterfaces.Repository;
using ProgrammersBlog.Core.Abstract.Interfaces.Services;
using ProgrammersBlog.Core.Abstract.İnterfaces.UnitOfWork;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Data.Context;
using ProgrammersBlog.Data.Repositories;
using ProgrammersBlog.Data.UnitofWork;
using ProgrammersBlog.Services.Mapping;
using ProgrammersBlog.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.Extantion
{
    public static class ServiceCollectionExtantion
    {

        public static void LoadMyService(this IServiceCollection Services)
        {
            Services.AddScoped<IUnitOfWork, UnitOfWork>();

            Services.AddIdentity<User, Role>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 5;
                opt.Password.RequiredUniqueChars = 0;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;

                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                opt.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ProgrammersBlogContext>();



            Services.AddScoped<ICategoryService, CategoryService>();
            Services.AddScoped<ICategoryRepository, CategoryRepository>();
            Services.AddScoped<IArticleService, ArticleService>();
            Services.AddScoped<IArticleRepository, ArticleRepository>();
            Services.AddScoped<ICommentRepository, CommentRepository>();
            Services.AddScoped<ICommentService, CommentService>();
            Services.AddSingleton<IMailService, MailService>();
            Services.AddAutoMapper(typeof(CategoryMapProfile));
        }
    }
}
