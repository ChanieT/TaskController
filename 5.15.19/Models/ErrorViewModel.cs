using Data;
using System;
using System.Collections.Generic;

namespace _5._15._19.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }


}