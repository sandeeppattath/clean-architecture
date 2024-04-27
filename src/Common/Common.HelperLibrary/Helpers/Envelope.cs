namespace Common.HelperLibrary.Helpers
{
    public class Envelope<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }     
        public T? Data { get; set; }
        public Envelope(bool isSuccess, string message, T data)
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
            this.Data = data;
        }
        public Envelope(bool isSuccess, string message)
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
        }     
    }
}
