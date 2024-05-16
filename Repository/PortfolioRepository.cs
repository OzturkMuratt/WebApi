using api.Data;
using api.Interfaces;
using api.Models;
using System.Data.Entity;

namespace api.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDBContext _context; 

        public PortfolioRepository(ApplicationDBContext contex
            )
        {
            _context= contex;
        }

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();
            return portfolio;
        }

        public async Task<Portfolio> DeletePortfolio(AppUser user, string symbol)
        {
           var portfolioModel= await _context.Portfolios.FirstOrDefaultAsync(
               x=>x.AppUserID == user.Id && x.Stock.Symbol.ToLower() == symbol.ToLower());
            if (portfolioModel == null) { return null; }
            _context.Portfolios.Remove(portfolioModel);
            await _context.SaveChangesAsync();
            return portfolioModel;
        }

        public async Task<List<Stock>> GetUserPortfolio(AppUser user)
        {
            return await _context.Portfolios
                .Where(x => x.AppUserID == user.Id)
                .Select(x => new Stock {
                    Id=x.Stock.Id,
                    Symbol=x.Stock.Symbol,
                    CompanyName=x.Stock.CompanyName,
                    Purchase=x.Stock.Purchase,
                    LastDiv=x.Stock.LastDiv,
                    Industry=x.Stock.Industry,
                    MarketCap=x.Stock.MarketCap,
                   
                }).ToListAsync();
        }
    }
}
