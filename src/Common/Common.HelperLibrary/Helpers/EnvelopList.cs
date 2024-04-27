namespace Common.HelperLibrary.Helpers
{
    public class EnvelopeList<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public int? TotalCount { get; set; }
        public bool HasData { get; set; }
        public EnvelopeList()
        {

        }
        public EnvelopeList(bool isSuccess, string message, T data, int? totalCount,bool hasData=true)
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
            this.Data = data;
            TotalCount = totalCount;
            HasData= hasData;
        }
    }
}
