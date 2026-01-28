using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplyRegion.Model;

namespace SupplyRegion.Data
{
    
    public class Repository
    {
        private static DatabaseContext CreateContext()
        {
            var ctx = new DatabaseContext();
            ctx.Database.EnsureCreated();
            return ctx;
        }

        public async Task<List<PurchaseRequest>> GetAllRequestsAsync()
        {
            using var context = CreateContext();
            return await context.PurchaseRequests
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<PurchaseRequest?> GetRequestByIdAsync(int id)
        {
            using var context = CreateContext();
            return await context.PurchaseRequests.FindAsync(id);
        }

        public async Task AddRequestAsync(PurchaseRequest request)
        {
            using var context = CreateContext();
            request.CreatedDate = DateTime.Now;
            await context.PurchaseRequests.AddAsync(request);
            await context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, string newStatus)
        {
            using var context = CreateContext();
            var entity = await context.PurchaseRequests.FindAsync(id);
            if (entity == null)
            {
                return;
            }

            entity.Status = newStatus;
            await context.SaveChangesAsync();
        }

        public async Task DeleteRequestAsync(int id)
        {
            using var context = CreateContext();
            var entity = await context.PurchaseRequests.FindAsync(id);
            if (entity != null)
            {
                context.PurchaseRequests.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<PurchaseRequest>> GetRequestsByStatusAsync(string status)
        {
            using var context = CreateContext();
            return await context.PurchaseRequests
                .AsNoTracking()
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<PurchaseRequest>> SearchByProductNameAsync(string searchText)
        {
            using var context = CreateContext();
            return await context.PurchaseRequests
                .AsNoTracking()
                .Where(r => r.ProductName.Contains(searchText) ||
                           r.Department.Contains(searchText) ||
                           r.Initiator.Contains(searchText))
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }
    }
}
