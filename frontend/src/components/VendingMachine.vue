<template>
  <div class="bg-primary d-flex gap-2 p-2 ff-agrandir">
    <div class="p-3 d-flex flex-column" style="height: 98vh;">
      <div>
        <div
        v-show="isOrderMade"
          class="alert alert-success ff-agrandirbold fw-bold fs-4" role="alert"
        >
          Compra realizada!<br>
          <span class="fw-normal ff-agrandir fs-6">La compra se realizó con éxito. Su vuelto fue de ₡ {{ receiptDetails.change }}</span>
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
        <form>
          <label>Tipo de cafe</label>
          <select>
            <option v-for="coffee in coffees" v-bind:key="coffee.name">{{ coffee.name }}</option>
          </select>
          <label>Cantidad de cafes</label>
          <input type="text" value="0">
          <button type="submit">Agregar</button>
        </form>
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
            <div v-for="orderedCoffee in orderCoffees" v-bind:key="orderedCoffee.id" class="row">
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
        <form>
          <label>Moneda o billete</label>
          <select>
            <option v-for="coin in coins" v-bind:key="coin.value">₡ {{ coin.value }}</option>
          </select>
          <label>Cantidad</label>
          <input type="text" value="0">
          <button type="submit">Agregar</button>
        </form>
        <div>
          <span>Falta de cancelar: ₡ {{ this.receiptDetails.remainingToPay }}</span><br>
          <span>Vuelto: ₡ {{ calculateChange() }}</span>
        </div>
        <button>Pagar</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  setup () {
    

    return {}
  },

  data() {
    return {
      isOrderMade: false,
      errorMessage: '',
      coffees: [
        {
          id: 1,
          name: 'Americano',
          units: 10,
          price: 950.00,
        },
        {
          id: 2,
          name: 'Capuccino',
          units: 10,
          price: 1200.00,
        }
      ],
      orderCoffees: [
        { id: 1, quantity: 2 },
        { id: 2, quantity: 1 },
      ],
      receiptDetails: {
        remainingToPay: 0,
        moneyAdded: [ { id: 2, quantity: 1 } ],
      },
      coins: [
        { id: 1, value: 100, available: 10 },
        { id: 2, value: 500, available: 10 },
        { id: 3, value: 50, available: 10 },
        { id: 4, value: 25, available: 10 },
      ],
    }
  },

  methods: {
    calculateTotal() {
      var totalToPay = 0;
      for (let i = 0; i < this.orderCoffees.length; ++i) {
        totalToPay += this.coffees.find(coffee => coffee.id === this.orderCoffees[i].id).price * this.orderCoffees[i].quantity;
      }
      return totalToPay;
    },

    sumQuantities() {
      var sumQuantity = 0;
      for (let i = 0; i < this.orderCoffees.length; ++i) {
        sumQuantity += this.orderCoffees[i].quantity;
      }
      return sumQuantity;
    },

    calculateChange() {
      let totalToPay = this.calculateTotal();
      let totalMoneyAdded = this.calculateMoneyAdded();

      this.receiptDetails.remainingToPay = totalMoneyAdded > totalToPay ? 0 : totalToPay - totalMoneyAdded;

      let change = totalMoneyAdded - this.receiptDetails.remainingToPay;
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
    }
  }
}
</script>

<style lang="scss" scoped></style>
