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
            this.validateOrderIsNotNull(order);
            this.validateValidIdsAndQuantities(order);
            this.validateHasEnoughCoffees(order);
            this.validateHasEnoughForChange(order);
            return this.handler.AddOrder(order);
        }

        private void validateOrderIsNotNull(OrderModel order)
        {
            if (order == null)
            {
                throw new NullReferenceException("La orden no puede ser nula.");
            }
        }

        private void validateValidIdsAndQuantities(OrderModel order)
        {
            foreach (IdentifierAndQuantityModel coffee in order.coffees)
            {
                if (coffee.Id < 0)
                {
                    throw new ArgumentException("Uno de los identificadores es invalido.");
                }

                if (coffee.quantity <= 0)
                {
                    throw new ArgumentException("Debe ingresar una cantidad mayor a cero para la cantidad de cafes.");
                }
            }

            foreach (IdentifierAndQuantityModel coffee in order.moneyAdded)
            {
                if (coffee.Id < 0)
                {
                    throw new ArgumentException("Uno de los identificadores es invalido.");
                }

                if (coffee.quantity <= 0)
                {
                    throw new ArgumentException("Debe ingresar una cantidad mayor a cero para la cantidad de monedas.");
                }
            }
        }

        private void validateHasEnoughCoffees(OrderModel order)
        {
            List<CoffeeModel> coffees = this.handler.GetCoffees();

            foreach (IdentifierAndQuantityModel orderedCoffee in order.coffees)
            {
                Console.WriteLine(coffees == null);
                CoffeeModel coffee = coffees.Find(coffee => coffee.Id == orderedCoffee.Id);
                if (coffee.Units < orderedCoffee.quantity)
                {
                    throw new ArgumentException("No hay suficientes unidades de cafe de las que pide.");
                }
            }
        }

        private void validateHasEnoughForChange(OrderModel order)
        {
            List<CoinModel> coins = this.handler.GetCoins();
            int changeAmount = this.handler.CalculateChange(this.handler.CalculateTotalToPay(order.coffees), order.moneyAdded);

            // Coins array must be sorted in descending order
            coins.OrderByDescending(coin => coin.value);

            // For each coin, if it is small enough, substracts from the change the value times the quantity it has including what the user gave the machine.
            for (int i = 0; i < coins.Count && changeAmount > 0; i++)
            {
                if (changeAmount >= coins[i].value)
                {
                    IdentifierAndQuantityModel addedCashWithSameValue = order.moneyAdded.Find(coin => coin.Id == coins[i].Id);
                    int addedCashAmount = addedCashWithSameValue != null ? addedCashWithSameValue.quantity : 0;
                    changeAmount -= coins[i].value * (coins[i].units + addedCashAmount);
                }
            }

            if (changeAmount > 0)
            {
                throw new ArgumentException("Fallo al realizar la compra");
            }
        }
    }
}
