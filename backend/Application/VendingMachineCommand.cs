using backend.Domain;
using backend.Infrastructure;

namespace backend.Application
{
    public class VendingMachineCommand
    {
        private readonly IVendingMachineHandler handler;

        public VendingMachineCommand(IVendingMachineHandler handler)
        {
            this.handler = handler;
        }

        public int AddOrder(OrderModel order)
        {
            return 0;
        }
    }
}
