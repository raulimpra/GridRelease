using GridPromocional.Models;

namespace GridPromocional.Services.Interfaces
{
    public interface ILogServices
    {
        public bool saveLogActivity(PgLogActivity element);
        public bool saveUploadHistory(PgLogUploadHistory element);
    }
}
