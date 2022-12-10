using System.Text;
using System.Xml.Serialization;
using Taxually.TechnicalTest.BusinessLogic.Interfaces;
using Taxually.TechnicalTest.Models;

namespace Taxually.TechnicalTest.BusinessLogic
{
    public class VatRegistrationService : IVatRegistrationService
    {
        public async Task Register(VatRegistrationRequest request)
        {
            switch (request.Country)
            {
                case "GB":
                    await RegisterVatNumberForGB(request);
                    break;
                case "FR":
                    await RegisterVatNumberForFR(request);
                    break;
                case "DE":
                    await RegisterVatNumberForDE();
                    break;
                default:
                    throw new Exception("Country not supported");
            }
        }

        private async Task RegisterVatNumberForDE()
        {
            // Germany requires an XML document to be uploaded to register for a VAT number
            using (var stringwriter = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(VatRegistrationRequest));
                serializer.Serialize(stringwriter, this);
                var xml = stringwriter.ToString();
                var xmlQueueClient = new TaxuallyQueueClient();
                // Queue xml doc to be processed
                xmlQueueClient.EnqueueAsync("vat-registration-xml", xml).Wait();
            }
        }

        private async Task RegisterVatNumberForFR(VatRegistrationRequest request)
        {
            // France requires an excel spreadsheet to be uploaded to register for a VAT number
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("CompanyName,CompanyId");
            csvBuilder.AppendLine($"{request.CompanyName}{request.CompanyId}");
            var csv = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            var excelQueueClient = new TaxuallyQueueClient();
            // Queue file to be processed
            excelQueueClient.EnqueueAsync("vat-registration-csv", csv).Wait();
        }

        private async Task RegisterVatNumberForGB(VatRegistrationRequest request)
        {
            // UK has an API to register for a VAT number
            var httpClient = new TaxuallyHttpClient();
            httpClient.PostAsync("https://api.uktax.gov.uk", request).Wait();
        }
    }
}
