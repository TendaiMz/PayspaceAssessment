using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaxCalculator.UI.Model;

namespace TaxCalculator.UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }


        public async Task<IActionResult> OnPostAsync(IncomeModel incomeModel)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var taxCalculation = new StringContent(
            JsonSerializer.Serialize(incomeModel), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient("CalculateIncomeTaxApi");

            using var httpResponse = await client.PostAsync("/api/IncomeTaxCalculator", taxCalculation);

            httpResponse.EnsureSuccessStatusCode();

            return RedirectToPage("./Index");
        }

    }
}
