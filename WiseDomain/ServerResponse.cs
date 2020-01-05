namespace WiseDomain
{
    public class ServerResponse<T> : ServerResponse
    {
        public T Response { get; set; }
    }

    public class ServerResponse
    {
        public bool Success { get; set; }
        public string ErrorText { get; set; }
        public int? ErrorCode { get; set; }
    }
}