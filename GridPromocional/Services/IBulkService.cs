using GridPromocional.Models.Enums;

namespace GridPromocional.Services
{
    public interface IBulkService
    {
        public Task<int> BulkMerge(IEnumerable<object> registers, BulkOperation operation);
    }
}