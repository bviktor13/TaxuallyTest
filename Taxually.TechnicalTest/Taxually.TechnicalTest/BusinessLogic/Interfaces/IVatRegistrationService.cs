using Taxually.TechnicalTest.Models;

namespace Taxually.TechnicalTest.BusinessLogic.Interfaces
{
    public interface IVatRegistrationService
    {
        Task Register(VatRegistrationRequest request);
    }
}
