using Microsoft.AspNetCore.Mvc;
using backend.Application;
using backend.Domain;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VendingMachineController : Controller
    {
        private readonly VendingMachineQuery vendingMachineQuery;
        private readonly VendingMachineCommand vendingMachineCommand;

        public VendingMachineController(
            VendingMachineQuery vendingMachineQuery,
            VendingMachineCommand vendingMachineCommand)
        {
            this.vendingMachineQuery = vendingMachineQuery;
            this.vendingMachineCommand = vendingMachineCommand;
        }

        [HttpGet]
        public IActionResult GetCoffees()
        {
            try
            {
                var response = vendingMachineQuery.ObtainCoffees();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet]
        public IActionResult GetCoins()
        {
            try
            {
                var response = vendingMachineQuery.ObtainCoins();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost]
        public IActionResult PostOrder(OrderModel order)
        {
            try
            {
                var response = vendingMachineCommand.AddOrder(order);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
