using GeolocationAPI.Application.BackgroundGeolocationIP.Interfaces;
using GeolocationAPI.Domain;
using GeolocationAPI.Infrastructure.DB.MSSQLExpress.Models;

namespace GeolocationAPI.Infrastructure.DB.MSSQLExpress.Repositories
{
    public class BatchProcessItemRepository : IBatchProcessItemBackgroundRepository
    {
        private readonly GeolocationAPIDbContext _context;

        public BatchProcessItemRepository(GeolocationAPIDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BatchProcessItem item)
        {
            var modelItem = BatchProcessItemModel.FromDomain(item);
            _context.BatchProcessItems.Add(modelItem);
            await _context.SaveChangesAsync();
        }
    }

}