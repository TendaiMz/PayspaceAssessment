using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TaxCalculator.Domain.Commands;
using TaxCalculator.Domain.Dto;


namespace TaxCalculator.Controllers
{
    public class IncomeTaxCalculator : BaseController
    {
       
        
    private readonly IMediator _mediator;

    public IncomeTaxCalculator(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Post([FromBody] IncomeDto incomeDto)
    {
        var command = new CalculateIncomeTaxCommand()
        {
            AnnualIncome = incomeDto.AnnualIncome,
            PostCode = incomeDto.PostCode
        };

        var result = await _mediator.Send(command);
        return result is null ? BadRequest(result) : (IActionResult)Created("Income Tax Calculation created", result);
    }

}
}
