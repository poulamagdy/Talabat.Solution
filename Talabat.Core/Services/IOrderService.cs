using Talabat.Core.Entities.Order_Aggregation;

namespace Talabat.Core.Services
{
	public interface IOrderService
	{
		Task<Entities.Order_Aggregation.Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress);
		Task<IReadOnlyList<Entities.Order_Aggregation.Order>> GetOrdersForSpecificUserAsync(string buyerEmail);
		Task<Entities.Order_Aggregation.Order> GetOrdersByIdForSpecificUserAsync(string buyerEmail, int orderId);
		Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync();
	}
}
