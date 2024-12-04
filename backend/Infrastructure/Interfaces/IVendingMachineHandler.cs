using backend.Domain;

namespace backend.Infrastructure
{
    public interface IVendingMachineHandler
    {
        public List<CoinModel> GetCoins();
        public List<CoffeeModel> GetCoffees();
        public int AddOrder(OrderModel order);
    }
}
