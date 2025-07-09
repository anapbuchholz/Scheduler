namespace Scheduler.API
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Error { get; set; }
        public T? Data { get; set; }

        public static Response<T> Ok(T data)
        {
            return new Response<T>
            {
                Success = true,
                Data = data
            };
        }

        public static Response<T> Fail(string errorMessage, string? error = null)
        {
            return new Response<T>
            {
                Success = false,
                ErrorMessage = errorMessage,
                Error = error
            };
        }
    }
}
