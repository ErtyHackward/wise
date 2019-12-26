namespace WiseDomain
{
    public class ServerResponse<T>
    {
        public bool Success { get; set; }
        public string ErrorText { get; set; }
        public int? ErrorCode { get; set; }
        public T Response { get; set; }
    }
}