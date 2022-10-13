using Mango.Service.OrderAPI.DbContexts;
using Mango.Service.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Service.OrderAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;

        public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            await using var _db = new ApplicationDbContext(_dbContext);
            _db.OrderHeaders.Add(orderHeader);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
        {
            await using var _db = new ApplicationDbContext(_dbContext);
            var orderHeaderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(x => x.OrderHeaderId == orderHeaderId);
            if (orderHeaderFromDb == null) return;

            orderHeaderFromDb.PaymentStatus = paid;
            await _db.SaveChangesAsync();
        }
    }
}
