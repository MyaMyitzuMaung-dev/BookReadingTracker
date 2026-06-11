using BookReadingTracker.Database.AppDbContextModels;
using BookReadingTracker.Domain.Features.Books;
using BookReadingTracker.Domain.Features.Dashboard;
using BookReadingTracker.Domain.Features.ReadingLists;
using BookReadingTracker.Domain.Features.ReadingProgress;
using BookReadingTracker.Domain.Features.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookReadingTracker.Domain
{
    public static class FeatureManager
    {
        public static WebApplicationBuilder AddDomain(this WebApplicationBuilder builder)
        {
            // Register AppDbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<BookService>();
            builder.Services.AddScoped<DashboardService>();
            builder.Services.AddScoped<ReadingListService>();
            builder.Services.AddScoped<ReadingProgressService>();

            return builder;
        }
    }
}
