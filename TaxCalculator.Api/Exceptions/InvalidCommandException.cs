using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaxCalculator.Exceptions
{
    public class InvalidCommandException : Exception
    {
        public string Details { get; }

        public InvalidCommandException(string message, string details) : base(message)
        {
            this.Details = details;
        }
    }
}
