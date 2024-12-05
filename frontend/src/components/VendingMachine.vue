<template>
  <div class="bg-primary d-flex gap-2 p-2 ff-agrandir">
    <div class="p-3 d-flex flex-column flex-fill" style="height: 98vh;">
      <div>
        <div
        v-show="successMessage.length > 0"
          class="alert alert-success ff-agrandirbold fw-bold fs-4" role="alert"
        >
          Compra realizada!<br>
          <span class="fw-normal ff-agrandir fs-6" v-text="successMessage"></span>
          <div class="fw-normal ff-agrandir fs-6" v-for="changeCoin in receiptDetails.changeCoins" v-bind:key="changeCoin">
            <div>{{ changeCoin.quantity }} x ₡ {{ coins.find(coin => coin.id === changeCoin.id).value }}</div>
          </div>
        </div>
        <div
          v-show="errorMessage.length > 0"
          class="alert alert-danger ff-agrandirbold fw-bold fs-4" role="alert"
        >
          Ocurrió un error<br>
          <span class="fw-normal ff-agrandir fs-6" v-text="errorMessage"></span>
        </div>
      </div>
      <div v-for="coffee in coffees" v-bind:key="coffee.name"
        class="my-2"
      >
        <div class="bg-secondary rounded-3 p-3 d-flex gap-4">
          <div class="ff-agrandirbold fw-bold fs-2">
            {{ coffee.name }}
          </div>
          <div class="">
            <span>Unidades Disponibles: {{ coffee.units }}</span><br>
            <span>₡ {{ coffee.price }}</span>
          </div>
        </div>
      </div>
    </div>
    <div class="flex-fill p-4" style="height: 98vh;">
      <div>
        <h3 class="ff-agrandirbold fw-bold">Seleccion cafes por comprar</h3>
        <label class="form-label">Tipo de cafe</label>
        <select v-model="orderedCoffeeName"
          class="form-select form-select-sm border-0 bg-secondary"
        >
          <option class="ff-agrandir" v-for="coffee in coffees" v-bind:key="coffee.name">{{ coffee.name }}</option>
        </select>
        <label class="form-label">Cantidad de cafes</label>
        <input class="form-control form-control-sm border-0 bg-secondary" type="text" v-model="quantityOrdered">
        <div class="d-flex">
          <button @click="addToOrder"
            class="btn btn-ternary ff-agrandirbold fw-bold text-primary my-2 flex-fill"
          >
            Agregar
          </button>
        </div>
      </div><hr>
      <div>
        <h3 class="ff-agrandirbold fw-bold">Vista de la Orden</h3>
        <div class="container text-center">
          <div class="row">
            <div class="col">
              Cantidad
            </div>
            <div class="col">
              Tipo de Cafe
            </div>
            <div class="col">
              Precio Unitario
            </div>
            <div class="col">
              Precio por cantidad
            </div>
          </div>
          <div class="rounded-2 bg-secondary">
            <div v-for="orderedCoffee in orderedCoffees" v-bind:key="orderedCoffee.id" class="row">
              <div class="col">{{ orderedCoffee.quantity }}</div>
              <div class="col">{{ this.coffees.find(coffee => coffee.id === orderedCoffee.id).name }}</div>
              <div class="col">₡ {{ this.coffees.find(coffee => coffee.id === orderedCoffee.id).price }}</div>
              <div class="col">₡ {{ this.coffees.find(coffee => coffee.id === orderedCoffee.id).price * orderedCoffee.quantity }}</div>
            </div>
          </div>
          <div class="row ff-agrandirbold fw-bold">
            <div class="col">
              {{ this.sumQuantities() }}
            </div>
            <div class="col">
              TOTAL
            </div>
            <div class="col"></div>
            <div class="col">
              ₡ {{ this.calculateTotal() }}
            </div>
          </div>
        </div>
      </div><hr>
      <div>
        <h3 class="ff-agrandirbold fw-bold">Realizar el pago</h3>
        <label class="form-label">Moneda o billete</label>
        <select v-model="cashValueSelected"
          class="form-select form-select-sm border-0 bg-secondary"
        >
          <option v-for="coin in coins" v-bind:key="coin.value">₡ {{ coin.value }}</option>
        </select>
        <label class="form-label">Cantidad</label>
        <input class="form-control form-control-sm border-0 bg-secondary" type="text" v-model="quantityCash">
        <div class="d-flex flex-column">
          <button @click="addCash"
            class="btn btn-ternary ff-agrandirbold fw-bold text-primary my-2 flex-fill"
          >
            Agregar
          </button>
          <span>Falta de cancelar: ₡ {{ this.receiptDetails.remainingToPay }}</span><br>
          <span>Vuelto: ₡ {{ this.receiptDetails.change }}</span>
          <button class="btn btn-success ff-agrandirbold fw-bold text-primary my-2 flex-fill" @click="pay">Pagar</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
export default {
  setup () {
    

    return {}
  },

  data() {
    return {
      successMessage: '',
      errorMessage: '',
      coffees: [],
      orderedCoffees: [],
      orderedCoffeeName: '',
      quantityOrdered: 0,
      coins: [],
      receiptDetails: {
        remainingToPay: 0,
        change: 0,
        moneyAdded: [],
        changeCoins: [],
      },
      cashValueSelected: '',
      quantityCash: 0,
    }
  },

  mounted() {
    this.getCoffees();
    this.getCoins();
  },

  methods: {
    getCoins() {
      var instance = this;
      axios.get("https://localhost:7170/api/VendingMachine/GetCoins")
      .then((response) => {
        this.coins = response.data;
      })
      .catch((error) => {
        instance.manageBackendError(error)
      });
    },

    getCoffees() {
      var instance = this;
      axios.get("https://localhost:7170/api/VendingMachine/GetCoffees")
      .then((response) => {
        this.coffees = response.data;
      })
      .catch((error) => {
        instance.manageBackendError(error)
      });
    },

    calculateTotal() {
      var totalToPay = 0;
      for (let i = 0; i < this.orderedCoffees.length; ++i) {
        totalToPay += this.coffees.find(coffee => coffee.id === this.orderedCoffees[i].id).price * this.orderedCoffees[i].quantity;
      }
      return totalToPay;
    },

    sumQuantities() {
      var sumQuantity = 0;
      for (let i = 0; i < this.orderedCoffees.length; ++i) {
        sumQuantity += this.orderedCoffees[i].quantity;
      }
      return sumQuantity;
    },

    calculateChange() {
      let totalToPay = this.calculateTotal();
      let totalMoneyAdded = this.calculateMoneyAdded();

      this.receiptDetails.remainingToPay = totalMoneyAdded > totalToPay ? 0 : totalToPay - totalMoneyAdded;

      let change = totalMoneyAdded - totalToPay;
      if (change < 0) {
        change = 0;
      }
      return change;
    },

    calculateMoneyAdded() {
      let totalMoneyAdded = 0;
      for (let i = 0; i < this.receiptDetails.moneyAdded.length; ++i) {
        totalMoneyAdded += this.coins.find(coin => coin.id === this.receiptDetails.moneyAdded[i].id).value * this.receiptDetails.moneyAdded[i].quantity;
      }
      return totalMoneyAdded;
    },

    addToOrder() {
      if (this.validateAddingCoffee()) {
        let coffeeId = this.coffees.find(coffee => coffee.name === this.orderedCoffeeName).id;
        let coffeeOrder = this.orderedCoffees.find(coffee => coffee.id === coffeeId);

        if (coffeeOrder != undefined) {
          coffeeOrder.quantity += Number(this.quantityOrdered);
        } else {
          this.orderedCoffees.push({
            id:       coffeeId,
            quantity: Number(this.quantityOrdered)
          });
        }
        this.receiptDetails.change = this.calculateChange();
        
        this.orderedCoffeeName = '';
        this.quantityOrdered = 0;
      }
    },

    validateAddingCoffee() {
      let canOrder = false;
      
      if (this.orderedCoffeeName.length <= 0) {
        this.errorMessage = "Debe seleccionar un cafe para añadir a la orden.";
      } else if (this.quantityOrdered <= 0) {
        this.errorMessage = "Debe ingresar una cantidad valida de cafes.";
      } else if (!this.validateHasEnoughCoffeeUnits()) {
        this.errorMessage = "No hay suficientes unidades del cafe que se esta pidiendo.";
      } else {
        canOrder = true;
      }

      return canOrder;
    },

    validateHasEnoughCoffeeUnits() {
      let canOrder = false;
      let coffee = this.coffees.find(coffee => coffee.name === this.orderedCoffeeName);
      let coffeeOrder = this.orderedCoffees.find(coffeeOrdered => coffeeOrdered.id === coffee.id);
      let alreadyOrderedUnits = coffeeOrder != undefined ? coffeeOrder.quantity : 0;

      if (coffee.units >= Number(this.quantityOrdered) + alreadyOrderedUnits) {
        canOrder = true;
      }
      return canOrder;
    },

    addCash() {
      if (this.validateAddingCash()) {
        let coinId = this.coins.find(coin => coin.value === Number(this.cashValueSelected.substring(2))).id;
        let alreadyAddedCoin = this.receiptDetails.moneyAdded.find(coin => coin.id === coinId);

        if (alreadyAddedCoin != undefined) {
          alreadyAddedCoin.quantity += Number(this.quantityCash);
        } else {
          this.receiptDetails.moneyAdded.push({
            id:       coinId,
            quantity: Number(this.quantityCash)
          });
        }
        this.receiptDetails.change = this.calculateChange();

        this.cashValueSelected = '';
        this.quantityCash = 0;
      }
    },

    validateAddingCash() {
      let canAddCash = false;

      if (this.cashValueSelected.length <= 0) {
        this.errorMessage = "Debe seleccionar una moneda o billete como fondos para realizar la compra.";
      } else if (isNaN(this.quantityCash)) {
        this.errorMessage = "Solo puede ingresar numeros en la cantidad de monedas o billetes.";
      } else if (this.quantityCash <= 0) {
        this.errorMessage = "Debe ingresar una cantidad mayor de monedas o billetes.";
      } else if (this.orderedCoffees.length <= 0) {
        this.errorMessage = "Debe ordenar algo antes de ingresar fondos.";
      } else {
        canAddCash = true;
      }

      return canAddCash;
    },

    validateHasEnoughForChange() {
      let changeAmount = this.calculateChange();

      // Coins array must be sorted in descending order
      this.coins.sort((a, b) => {
        if (a.value > b.value) {
          return 1;
        } else if (a.value == b.value) {
          return 0;
        }
        return -1;
      });
      this.coins.reverse();

      // For each coin, if it is small enough, substracts from the change the value times the quantity it has including what the user gave the machine.
      for (let i = 0; i < this.coins.length && changeAmount > 0; i++) {
        if (changeAmount >= this.coins[i].value) {
          let addedCashWithSameValue = this.receiptDetails.moneyAdded.find(coin => coin.value === this.coins[i].value);
          let addedCashAmount = addedCashWithSameValue != undefined ? addedCashWithSameValue.quantity : 0;
          changeAmount -= this.coins[i].value * (this.coins[i].units + addedCashAmount);
        }
      }

      return changeAmount <= 0;
    },

    pay() {
      if (this.validatePayingOrder()) {
        let instance = this;
        axios.post("https://localhost:7170/api/VendingMachine/PostOrder",
        {
          coffees: this.orderedCoffees,
          moneyAdded: this.receiptDetails.moneyAdded
        })
        .then((response) => {
          this.receiptDetails.changeCoins = response.data;
          this.successMessage = `La compra se realizó con éxito. Su vuelto fue de ₡ ${this.receiptDetails.change}. Desglose:`;
          this.errorMessage = '';

          this.orderedCoffees = [];
          this.receiptDetails.change = 0;
          this.receiptDetails.remainingToPay = 0;
          this.receiptDetails.moneyAdded = [];

          this.getCoffees();
          this.getCoins();
        })
        .catch((error) => {
          instance.manageBackendError(error);
        })
      }
    },

    validatePayingOrder() {
      let canPay = false;

      if (this.receiptDetails.remainingToPay > 0) {
        this.errorMessage = "Debe primero ingresar suficientes fondos para cancelar la orden.";
      } else if (!this.validateHasEnoughForChange()) {
        this.errorMessage = "Fallo al realizar la compra";
      } else if (this.orderedCoffees.length <= 0) {
        this.errorMessage = "Debe ordenar algo antes de pagar.";
      } else {
        canPay = true;
      }

      return canPay;
    },

    manageBackendError(error) {
      var errorMsg = "";
      var errorStatus = 0;
      if (error.response == undefined) {
        errorMsg = "No hay conexión con el servidor.";
        errorStatus = 408;
      } else if (error.response.data) {
        errorMsg = error.response.data;
        errorStatus = error.response.status;
      } else if (error.response.data.title) {
        errorMsg = error.response.data.title;
        errorStatus = error.response.status;
      } else if (error.request) {
        errorMsg = error.message;
        errorStatus = error.response.status;
      }
      
      console.log("ERROR " + errorStatus + " ----> " + errorMsg);
      this.errorMessage = errorMsg;
    }
  }
}
</script>

<style lang="scss" scoped></style>
