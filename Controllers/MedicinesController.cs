using Microsoft.AspNetCore.Mvc;
using ABCPharmacyApi.Models;
using ABCPharmacyApi.Services;

namespace ABCPharmacyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicinesController : ControllerBase
    {
        private readonly DataService ds;
        public MedicinesController(DataService dataService) { ds = dataService; }

        [HttpGet]
        public IActionResult Get([FromQuery]string? search)
        {
            var meds = ds.GetMedicines(search);
            return Ok(meds);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Medicine create)
        {
            if (create == null) return BadRequest();
            var added = ds.AddMedicine(create);
            return CreatedAtAction(nameof(Get), new { id = added.Id }, added);
        }

        // [HttpPost("{id}/sell")]
        // public IActionResult Sell(Guid id, [FromBody] dynamic body)
        // {
        //     try
        //     {
        //         if (body == null || body.quantity == null) return BadRequest("Missing quantity");
        //         int q = (int)body.quantity;
        //         var sale = ds.SellMedicine(id, q);
        //         return Ok(sale);
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { error = ex.Message });
        //     }
        // }

        [HttpPost("{id}/sell")]
        public IActionResult Sell(Guid id, [FromBody] SaleDto dto)
        {
            try
            {
                if (dto == null) return BadRequest("Missing request body");
                var sale = ds.SellMedicine(id, dto.Quantity);
                return Ok(sale);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("/api/sales")]
        public IActionResult Sales() => Ok(ds.GetSales());
    }
}