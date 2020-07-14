namespace TaxCalculator.Utils
{
    internal class SwaggerOptions
    {
        public string SwaggerDocName { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public Contact Contact { get; set; }
        public string SwaggerEndPoint { get; set; }
        public string EndPointName { get; set; }
    }

    internal class Contact
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }


}