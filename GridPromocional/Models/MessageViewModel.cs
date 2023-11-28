namespace GridPromocional.Models
{
    public class MessageViewModel
    {
        public MessageViewModel(string message, bool isError = false, string? detail = null)
        {
            Message = message;
            IsError = isError;
            Detail = detail;
        }

        public Boolean IsError { get; set; }
        public string Message { get; set; }
        public string? Detail {  get; set; }
    }
}
