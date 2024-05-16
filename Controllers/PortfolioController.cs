using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController: ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepository;
        private readonly IPortfolioRepository _portfolioRepository;

        public PortfolioController(UserManager<AppUser> userManager,IStockRepository stockRepository, IPortfolioRepository portfolioRepository)
        {
            _stockRepository = stockRepository;
            _userManager = userManager;
            _portfolioRepository = portfolioRepository;

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUsername();
            var appuser = await _userManager.FindByNameAsync(username);
           var userportfolio =await _portfolioRepository.GetUserPortfolio(appuser);
            return Ok(userportfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {

            var username = User.GetUsername();
            var appuser = await _userManager.FindByNameAsync(username);
            var stock = await _stockRepository.GetBySymbolAsync(symbol);

            if (stock == null) { return NotFound("Stock not found");}

            var userportfolio = await _portfolioRepository.GetUserPortfolio(appuser);

            if (userportfolio.Any(x => x.Symbol.ToLower() == symbol.ToLower())) { return BadRequest("Stock already in portfolio"); }

            var portfolio = new Portfolio
            {
                StockId = stock.Id,
                AppUserID = appuser.Id
            };
            await _portfolioRepository.CreateAsync(portfolio);
            if (portfolio == null) { return BadRequest("Failed to add stock to portfolio"); }
            else { return Created(); }

        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol)
        {
            var username = User.GetUsername();
            var appuser = await _userManager.FindByNameAsync(username);

            var userportfolio = await _portfolioRepository.GetUserPortfolio(appuser);

            var filterstock = userportfolio.Where(x => x.Symbol.ToLower() == symbol.ToLower()).ToList();
            if (filterstock.Count == 1) 
            { 
                await _portfolioRepository.DeletePortfolio(appuser, symbol);
                return Ok("Stock removed from portfolio");
            }
            else
            {
                   return BadRequest("Stock not found in portfolio");
            }

            
        }
    }
}
