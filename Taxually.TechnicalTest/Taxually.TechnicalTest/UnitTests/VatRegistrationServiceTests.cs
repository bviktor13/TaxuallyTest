using Taxually.TechnicalTest.BusinessLogic;
using Taxually.TechnicalTest.Models;
using Xunit;
using FluentAssertions;

namespace Taxually.TechnicalTest.UnitTests
{
    public class VatRegistrationServiceTests
    {
        private readonly VatRegistrationRequest _request;
        private readonly VatRegistrationService _sut;

        public VatRegistrationServiceTests()
        {
            _request = new()
            {
                CompanyId = "1",
                CompanyName = "Taxually",
            };
        }

        [Theory]
        [InlineData("GB")]
        [InlineData("FR")]
        [InlineData("DE")]
        public async Task Register__with_known_country_runs_properly(string country)
        {
            // Arrange
            _request.Country = country;

            // Act
            Action act = async () => await _sut.Register(_request);

            // Assert
            act.Should().NotThrow<Exception>();
        }

        [Fact]
        public async Task Register_with_unknown_country_throws_exception()
        {
            // Arrange
            _request.Country = "NO";

            // Act
            Action act = async () => await _sut.Register(_request);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }
    }
}
