using GridPromocional.Exceptions;
using GridPromocional.Models;
using GridPromocional.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text;

namespace GridPromocional.Services
{
    public interface IUploadService<T, S> where T : class where S : UploadError
    {
        public ICsvService<T, S> CsvService { get; }
        public PgLogUploadHistory UploadStats { get; }

        public IQueryable<S> GetPreviousErrors();

        public void RemovePreviousErrors();

        public Task<PgLogUploadHistory> GetRegisters(IFormFile file, Encoding? encoding = null);

        public Task<List<PgLogUploadHistory>> GetUploadHistory(DateTime start, DateTime next, string? entity = null);
        public Task<List<PgLogUploadHistory>> GetUploadHistoryFull(DateTime start, DateTime next);

        public Task UploadTarget(BulkOperation operation);

        public Task UploadSource();

        public FileStreamResult? DownloadErrors(Encoding? encoding = null);

        public bool ValidateDuplicates(Expression<Func<T, object>> keys);
        public bool ValidateExisting(Expression<Func<T, object>> keys);

        public string GetDisplayName();

        public string GetSourceModelName();

        public Task<List<int>> UpdateUserList(bool truncate, Func<T, string> selectorName
            , Func<Register<T, S>, bool> selectorA, Func<Register<T, S>, bool> selectorB);

        public GridException LogException(Exception ex, string archivo);

        public Task LogUploadActivity();
    }
}