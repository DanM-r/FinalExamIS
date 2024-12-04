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
            return new List<CoffeeModel>();
        }

        public List<CoinModel> ObtainCoins()
        {
            return new List<CoinModel>();
        }
    }
}
