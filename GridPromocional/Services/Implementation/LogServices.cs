using GridPromocional.Data;
using GridPromocional.Models;
using GridPromocional.Services.Interfaces;

namespace GridPromocional.Services.Implementation
{
    public class LogServices : ILogServices
    {
        private GridContext _context;
        public LogServices(GridContext context)
        { 
            _context = context;
        }
        public bool saveLogActivity(PgLogActivity element)
        {
            try
            {
                _context.PgLogActivity.Add(element);
                _context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public bool saveUploadHistory(PgLogUploadHistory element)
        {
            try
            {
                _context.PgLogUploadHistory.Add(element);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
