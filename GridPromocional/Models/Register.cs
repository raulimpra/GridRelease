#nullable disable
namespace GridPromocional.Models
{
    public class Register<T, S> where T : class where S : UploadError
    {
        public Dictionary<string, Exception> Errors { get; } = new();
        public T Target { get; set; }
        public S Source { get; set; }

        public bool HasErrors()
        {
            return Errors.Any();
        }

        public Register<T, S> AddError(string column, Exception ex)
        {
            if (!Errors.ContainsKey(column))
                Errors.Add(column, ex);
            return this;
        }
    }
}