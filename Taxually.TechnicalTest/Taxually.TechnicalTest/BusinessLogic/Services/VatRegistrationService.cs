using System.Text;
using System.Xml.Serialization;
using Taxually.TechnicalTest.BusinessLogic.Interfaces;
using Taxually.TechnicalTest.Models;

namespace Taxually.TechnicalTest.BusinessLogic
{
    public class VatRegistrationService : IVatRegistrationService
    {
        public void Register(VatRegistrationRequest request)
        {
            switch (request.Country)
            {
                case "GB":
                    // UK has an API to register for a VAT number
                    var httpClient = new TaxuallyHttpClient();
                    httpClient.PostAsync("https://api.uktax.gov.uk", request).Wait();
                    break;
                case "FR":
                    // France requires an excel spreadsheet to be uploaded to register for a VAT number
                    var csvBuilder = new StringBuilder();
                    csvBuilder.AppendLine("CompanyName,CompanyId");
                    csvBuilder.AppendLine($"{request.CompanyName}{request.CompanyId}");
                    var csv = Encoding.UTF8.GetBytes(csvBuilder.ToString());
                    var excelQueueClient = new TaxuallyQueueClient();
                    // Queue file to be processed
                    excelQueueClient.EnqueueAsync("vat-registration-csv", csv).Wait();
                    break;
                case "DE":
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
                    break;
                default:
                    throw new Exception("Country not supported");
            }
        }
    }
}
