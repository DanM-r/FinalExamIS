using backend.Domain;

namespace backend.Infrastructure
{
    public interface IVendingMachineHandler
    {
        public List<CoinModel> GetCoins();
        public List<CoffeeModel> GetCoffees();
        public List<IdentifierAndQuantityModel> AddOrder(OrderModel order);
        public int CalculateChange(double totalToPay, List<IdentifierAndQuantityModel> moneyAdded);
        public double CalculateTotalToPay(List<IdentifierAndQuantityModel> orderedCoffees);
    }
}
