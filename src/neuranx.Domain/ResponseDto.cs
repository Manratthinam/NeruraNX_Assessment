namespace neuranx.Domain
{
    public class ResponseDto<T>
    {
        public T? Payload { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; }
        public int Code { get; set; }
        public int ResponseTime { get; set; }
    }
}
