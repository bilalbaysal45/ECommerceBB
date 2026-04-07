using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Commons
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Detail { get; set; }
        public string TraceId { get; set; } // Hatanın takibi için loglardaki ID
        public List<string>? Errors { get; set; }
    }
}
