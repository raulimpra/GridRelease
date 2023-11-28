using EFCore.BulkExtensions;
using GridPromocional.Data;
using GridPromocional.Models.Enums;

namespace GridPromocional.Services
{
    public class BulkService : IBulkService
    {
        private readonly GridContext _context;

        public BulkService(GridContext context)
        {
            _context = context;
        }

        public async Task<int> BulkMerge(IEnumerable<object> registers, BulkOperation operation)
        {
            int result = registers.Count();

            switch (operation)
            {
                case BulkOperation.Insert:
                    await _context.BulkInsertAsync(registers);
                    break;
                case BulkOperation.Upsert:
                    await _context.BulkInsertOrUpdateAsync(registers);
                    break;
                case BulkOperation.Replace:
                    await _context.BulkInsertOrUpdateOrDeleteAsync(registers);
                    break;
                case BulkOperation.Delete:
                    await _context.BulkDeleteAsync(registers);
                    break;
                case BulkOperation.Update:
                    await _context.BulkUpdateAsync(registers);
                    break;
            }

            return result;
        }
    }
}