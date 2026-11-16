using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregation;

namespace Talabat.Repository.DbContexts.Configuration
{
	public class OrderConfiguration : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.Property(o => o.Status)
				   .HasConversion(status => status.ToString(),
								  statusString => (OrderStatus) Enum.Parse(typeof(OrderStatus), statusString));

			builder.Property(o => o.SubTotal)
				   .HasColumnType("decimal(18,2)");

			builder.OwnsOne(o => o.ShippingAddress, x => x.WithOwner());

			builder.HasOne(o => o.DeliveryMethod)
				   .WithMany()
				   .OnDelete(DeleteBehavior.NoAction);
		}
	}
}
