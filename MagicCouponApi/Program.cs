using MagicCouponApi.Data;
using MagicCouponApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicCouponApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapGet("/api/coupons", (ILogger<Coupon> _logger) =>
            {
                _logger.Log(LogLevel.Information, "Getting all coupons.");
                return Results.Ok(CouponStore.coupons);
            }).WithName("GetCoupons").Produces<IEnumerable<Coupon>>(200);
            
            app.MapGet("/api/coupon/{id:int}", (int id) =>
            {
                return Results.Ok(CouponStore.coupons.FirstOrDefault(x => x.Id == id));
            }).WithName("GetCoupon").Produces<Coupon>(200);
            
            app.MapPost("/api/coupon", ([FromBody] Coupon coupon) =>
            {
                if (coupon.Id != 0 || string.IsNullOrEmpty(coupon.Name))
                {
                    return Results.BadRequest("Invalid Id or Coupon Name");
                }

                if (CouponStore.coupons.FirstOrDefault(x => x.Name.ToLower().Equals(coupon.Name.ToLower())) != null)
                {
                    return Results.BadRequest("Coupon Name already exists.");           
                }

                coupon.Id = CouponStore.coupons.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
                CouponStore.coupons.Add(coupon);
                return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, coupon);
            }).WithName("CreateCoupon").Accepts<Coupon>("application/json").Produces<Coupon>(201).Produces(400);
            
            app.MapPut("/api/coupon", () =>
            {

            });
            
            app.MapDelete("/api/coupon/{id:int}", (int id) =>
            {

            });
            //app.UseAuthorization();

            app.Run();
        }
    }
}