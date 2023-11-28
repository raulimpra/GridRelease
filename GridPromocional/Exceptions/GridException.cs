using System.Text;

namespace GridPromocional.Exceptions
{
    [Serializable]
    public class GridException : Exception
    {
        private const int MAX_EXCEPTIONS = 10;

        public GridException() { }

        public GridException(string message) : base(message) { }

        public GridException(string message, Exception inner): base(message, inner) { }

        public string ToStringGrid()
        {
            StringBuilder sb = new();
            Exception? ex = this;

            for (int i = 0; ex != null && i < MAX_EXCEPTIONS; i++, ex = ex.InnerException)
            {
                if (ex is GridException)
                {
                    sb.Append(ex.Message);
                    sb.Append(' ');
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}