using Backend.WorkItem.BackgroundServices;
using Backend.WorkItem.Repository.Utility;
using Backend.WorkItem.Repository.Utility.Interface;
using Backend.WorkItem.Repository.WorkItem;
using Backend.WorkItem.Repository.WorkItem.Interface;
using Backend.WorkItem.Service.WorkItem;
using Backend.WorkItem.Service.WorkItem.Interface;
using Confluent.Kafka;

namespace Backend.WorkItem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IRedisConnection, RedisConnection>();
            builder.Services.AddSingleton<IProducer<Null, string>>(sp =>
            {
                var kafkaBootstrap = builder.Configuration["Kafka:BootstrapServers"];
                var config = new ProducerConfig
                {
                    BootstrapServers = kafkaBootstrap
                };
                return new ProducerBuilder<Null, string>(config).Build();
            });
            builder.Services.AddSingleton<IConnectionString, ConnectionString>();
            builder.Services.AddScoped<IWorkItemRepository, WorkItemRepository>();
            builder.Services.AddScoped<IWorkItemService, WorkItemService>();
            builder.Services.AddHostedService<WorkItemKafkaConsumer>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
