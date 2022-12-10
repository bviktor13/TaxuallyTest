using Taxually.TechnicalTest.Models;

namespace Taxually.TechnicalTest.BusinessLogic.Interfaces
{
    public interface IVatRegistrationService
    {
        void Register(VatRegistrationRequest request);
    }
}
