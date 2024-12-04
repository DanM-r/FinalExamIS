using backend.Domain;
using backend.Infrastructure;

namespace backend.Application
{
    public class VendingMachineQuery
    {
        private readonly IVendingMachineHandler handler;

        public VendingMachineQuery(IVendingMachineHandler handler)
        {
            this.handler = handler;
        }

        public List<CoffeeModel> ObtainCoffees()
        {
            return this.handler.GetCoffees();
        }

        public List<CoinModel> ObtainCoins()
        {
            return this.handler.GetCoins();
        }
    }
}
