using System;

namespace EventManagerASP.Models
{
    public class ErrorViewModel
    {
        public int StatusCode { get; set; }
        public string RequestId { get; set; }
        public string OriginalPath { get; set; }
        public string Message { get; set; } 

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
