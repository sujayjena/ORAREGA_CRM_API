using System.ComponentModel;

namespace OraRegaAV.Models
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        [DefaultValue(0)]
        public int TotalCount { get; set; }
        public object Data { get; set; }
    }
}
