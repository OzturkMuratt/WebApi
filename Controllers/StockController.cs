using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IStockRepository _stockrepo;
        public StockController(ApplicationDBContext context,IStockRepository stockrepo)
        {
            _stockrepo = stockrepo;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var stocks = await _stockrepo.GetAllAsync(query);
            var stockDto=stocks.Select(s=>s.ToStockDto());
            return Ok(stocks);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var stock = await _stockrepo.GetByIdAsync(id);
            if (stock == null) { return NotFound(); }
            return Ok(stock.ToStockDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            var stockmodel = stockDto.ToStockFromCreateDto();
            await _stockrepo.CreateAsync(stockmodel);
            return CreatedAtAction(nameof(GetById), new { id = stockmodel.Id }, stockmodel.ToStockDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var stockModel = await _stockrepo.UpdateAsync(id, updateDto);
            if (stockModel == null) { return NotFound(); }
            return Ok(stockModel.ToStockDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var stockModel = await _stockrepo.DeleteAsync(id);
            if (stockModel == null) { return NotFound(); }
             return NoContent();
         }
    }
}
