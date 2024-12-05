using backend.Application;
using backend.Domain;
using backend.Infrastructure;
using Moq;

namespace tests
{
    public class VendingMachineCommandTest
    {
        private IVendingMachineHandler handler;
        private VendingMachineCommand command;

        [SetUp]
        public void Setup()
        {
            this.handler = new VendingMachineHandler();
            this.command = new VendingMachineCommand(handler);
        }

        [Test]
        public void AddNullOrderTest()
        {
            OrderModel orderModel = null;
            var exception = Assert.Throws<NullReferenceException>(() => this.command.AddOrder(orderModel));
            Assert.That(exception.Message, Is.EqualTo("La orden no puede ser nula."));
        }

        [Test]
        public void AddOrderWithInvalidId()
        {
            List<IdentifierAndQuantityModel> coffees = new List<IdentifierAndQuantityModel>();
            coffees.Add(new IdentifierAndQuantityModel()
            {
                Id = -1,
                quantity = 1
            });
            List<IdentifierAndQuantityModel> coins = new List<IdentifierAndQuantityModel>();
            coins.Add(new IdentifierAndQuantityModel()
            {
                Id = 2,
                quantity = 1
            });
            OrderModel orderModel = new OrderModel()
            {
                coffees = coffees,
                moneyAdded = coins
            };
            var exception = Assert.Throws<ArgumentException>(() =>  this.command.AddOrder(orderModel));
            Assert.That(exception.Message, Is.EqualTo("Uno de los identificadores es invalido."));
        }

        [Test]
        public void AddOrderWithNegativeQuantitiesInMoneyAdded()
        {
            List<IdentifierAndQuantityModel> coffees = new List<IdentifierAndQuantityModel>();
            coffees.Add(new IdentifierAndQuantityModel()
            {
                Id = 1,
                quantity = 1
            });
            List<IdentifierAndQuantityModel> coins = new List<IdentifierAndQuantityModel>();
            coins.Add(new IdentifierAndQuantityModel()
            {
                Id = 2,
                quantity = -56
            });
            OrderModel orderModel = new OrderModel()
            {
                coffees = coffees,
                moneyAdded = coins
            };
            var exception = Assert.Throws<ArgumentException>(() => this.command.AddOrder(orderModel));
            Assert.That(exception.Message, Is.EqualTo("Debe ingresar una cantidad mayor a cero para la cantidad de monedas."));
        }

        [Test]
        public void AddOrderWithNegativeQuantitiesInCoffees()
        {
            List<IdentifierAndQuantityModel> coffees = new List<IdentifierAndQuantityModel>();
            coffees.Add(new IdentifierAndQuantityModel()
            {
                Id = 1,
                quantity = -5
            });
            List<IdentifierAndQuantityModel> coins = new List<IdentifierAndQuantityModel>();
            coins.Add(new IdentifierAndQuantityModel()
            {
                Id = 2,
                quantity = 1
            });
            OrderModel orderModel = new OrderModel()
            {
                coffees = coffees,
                moneyAdded = coins
            };
            var exception = Assert.Throws<ArgumentException>(() => this.command.AddOrder(orderModel));
            Assert.That(exception.Message, Is.EqualTo("Debe ingresar una cantidad mayor a cero para la cantidad de cafes."));
        }

        [Test]
        public void AddOrderOfHugeQuantitiesOfCoffees()
        {
            List<IdentifierAndQuantityModel> coffees = new List<IdentifierAndQuantityModel>();
            coffees.Add(new IdentifierAndQuantityModel()
            {
                Id = 1,
                quantity = 100000000
            });
            List<IdentifierAndQuantityModel> coins = new List<IdentifierAndQuantityModel>();
            coins.Add(new IdentifierAndQuantityModel()
            {
                Id = 2,
                quantity = 1
            });
            OrderModel orderModel = new OrderModel()
            {
                coffees = coffees,
                moneyAdded = coins
            };
            var exception = Assert.Throws<ArgumentException>(() => this.command.AddOrder(orderModel));
            Assert.That(exception.Message, Is.EqualTo("No hay suficientes unidades de cafe de las que pide."));
        }
    }
}