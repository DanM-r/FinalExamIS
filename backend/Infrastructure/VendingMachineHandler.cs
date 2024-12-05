using System.Collections;
using backend.Domain;
using Newtonsoft.Json;

namespace backend.Infrastructure
{
    public class VendingMachineHandler : IVendingMachineHandler
    {
        private VendingMachineConfiguration configuration;

        public VendingMachineHandler()
        {
            configuration = new VendingMachineConfiguration();
            this.configuration.Coffees = JsonConvert.DeserializeObject<List<CoffeeModel>>
                (this.ReadJsonConfigurationFile("CoffeeConfiguration.json"));
            this.configuration.Coins = JsonConvert.DeserializeObject<List<CoinModel>>
                (this.ReadJsonConfigurationFile("CoinConfiguration.json"));
        }

        private string ReadJsonConfigurationFile(string filename)
        {
            string json = "";
            string path = Path.Combine(Environment.CurrentDirectory, "Configuration/", filename);
            using (StreamReader reader = new StreamReader(path))
            {
                json = reader.ReadToEnd();
            }
            return json;
        }

        private void SaveJsonConfiguration(string filename, string json)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "Configuration/", filename);
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(json);
            }
        }

        public List<CoinModel> GetCoins()
        {
            return this.configuration.Coins;
        }

        public List<CoffeeModel> GetCoffees()
        {
            return this.configuration.Coffees;
        }

        public List<IdentifierAndQuantityModel> AddOrder(OrderModel order)
        {
            double totalToPay = this.UpdateStock(order.coffees);
            int change = this.CalculateChange(totalToPay, order.moneyAdded);
            List<IdentifierAndQuantityModel> changeCoins = this.CalculateChangeCoins(change);
            this.SaveCoinsAndCoffeesData();
            return changeCoins;
        }

        private List<IdentifierAndQuantityModel> CalculateChangeCoins(double change)
        {
            double remainingChange = change;
            List<IdentifierAndQuantityModel> changeCoins = new List<IdentifierAndQuantityModel>();
            this.configuration.Coins.OrderByDescending(coin => coin.value);
            foreach (var coin in this.configuration.Coins)
            {
                if (coin.value <= remainingChange)
                {
                    int quantity = Convert.ToInt32(Math.Floor(remainingChange / coin.value));
                    changeCoins.Add(new IdentifierAndQuantityModel()
                    {
                        Id = coin.Id,
                        quantity = quantity
                    });
                    remainingChange -= quantity * coin.value;
                    coin.units -= quantity;
                }
            }
            return changeCoins;
        }

        private double UpdateStock(List<IdentifierAndQuantityModel> orderedCoffees)
        {
            double totalToPay = 0;
            foreach (var orderedCoffee in orderedCoffees)
            {
                var coffee = this.configuration.Coffees.Find(coffee => coffee.Id == orderedCoffee.Id);
                coffee.Units -= orderedCoffee.quantity;
                totalToPay += coffee.Price * orderedCoffee.quantity;
            }
            return totalToPay;
        }

        public double CalculateTotalToPay(List<IdentifierAndQuantityModel> orderedCoffees)
        {
            double totalToPay = 0;
            foreach (var orderedCoffee in orderedCoffees)
            {
                var coffee = this.configuration.Coffees.Find(coffee => coffee.Id == orderedCoffee.Id);
                totalToPay += coffee.Price * orderedCoffee.quantity;
            }
            return totalToPay;
        }

        public int CalculateChange(double totalToPay, List<IdentifierAndQuantityModel> moneyAdded)
        {
            double remainingToPay = totalToPay;
            foreach (IdentifierAndQuantityModel cashAdded in moneyAdded)
            {
                CoinModel coin = this.configuration.Coins.Find(coin => coin.Id == cashAdded.Id);
                remainingToPay -= cashAdded.quantity * coin.value;
            }

            for (int i = 0; i < this.configuration.Coins.Count; i++)
            {
                IdentifierAndQuantityModel cashAdded = moneyAdded.Find(cashAdded => cashAdded.Id == this.configuration.Coins[i].Id);
                int amount = cashAdded != null ? cashAdded.quantity : 0;
                this.configuration.Coins[i].units += amount;
            }

            return Convert.ToInt32(Math.Abs(remainingToPay));
        }

        private void SaveCoinsAndCoffeesData()
        {
            this.SaveJsonConfiguration("CoffeeConfiguration.json", JsonConvert.SerializeObject(this.configuration.Coffees));
            this.SaveJsonConfiguration("CoinConfiguration.json", JsonConvert.SerializeObject(this.configuration.Coins));
        }
    }
}
