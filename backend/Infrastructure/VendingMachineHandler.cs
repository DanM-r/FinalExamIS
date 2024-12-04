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
            Console.WriteLine("Se lee el constriuctor");
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
            foreach (var coin in this.configuration.Coins)
            {
                Console.WriteLine(coin.units);
            }
            return this.configuration.Coins;
        }

        public List<CoffeeModel> GetCoffees()
        {
            return this.configuration.Coffees;
        }

        public int AddOrder(OrderModel order)
        {
            double totalToPay = this.UpdateStock(order.coffees);
            return this.CalculateChange(totalToPay, order.moneyAdded);
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

            this.SaveJsonConfiguration("CoffeeConfiguration.json", JsonConvert.SerializeObject(this.configuration.Coffees));
            return totalToPay;
        }

        private int CalculateChange(double totalToPay, List<IdentifierAndQuantityModel> moneyAdded)
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

            this.SaveJsonConfiguration("CoinConfiguration.json", JsonConvert.SerializeObject(this.configuration.Coins));
            return Convert.ToInt32(Math.Abs(remainingToPay));
        }
    }
}
