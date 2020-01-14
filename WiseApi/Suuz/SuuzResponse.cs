namespace WiseApi
{
    public class SuuzResponse<T>
    {
        public string Result { get; set; }

        public T Data { get; set; }
    }
}