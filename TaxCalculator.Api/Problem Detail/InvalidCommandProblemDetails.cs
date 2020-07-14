using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaxCalculator.Exceptions;

namespace TaxCalculator.Problem_Detail
{
    public class InvalidCommandProblemDetails : ProblemDetails
    {
        public InvalidCommandProblemDetails(InvalidCommandException exception)
        {
            this.Title = exception.Message;
            this.Status = StatusCodes.Status400BadRequest;
            this.Detail = exception.Details;            
        }
    }
}
