using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregation;

namespace Talabat.Core.Specification.Order_Spec
{
	public class OrderWithPaymentSpec : BaseSpecification<Order, int>
	{
		public OrderWithPaymentSpec(string paymentIntentId) : base(o => o.PaymentIntentId == paymentIntentId)
		{

		}
	}
}
