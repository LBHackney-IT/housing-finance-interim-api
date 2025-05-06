using AutoFixture;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Controllers;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Controllers
{
    public class NightlyProcessLogControllerTests
    {
        private readonly NightlyProcessLogController _classUnderTest;
        private readonly Mock<INightlyProcessLogUseCase> _nightlyProcessLogUseCaseMock;
        private readonly Fixture _fixture = new Fixture();

        public NightlyProcessLogControllerTests()
        {
            _nightlyProcessLogUseCaseMock = new Mock<INightlyProcessLogUseCase>();
            _classUnderTest = new NightlyProcessLogController(_nightlyProcessLogUseCaseMock.Object);
        }

        [Fact]
        public async Task GetNightlyProcessLogs_WhenNoLogsFound_ReturnsNotFound()
        {
            // Arrange
            var createdDate = DateTime.UtcNow;
            _nightlyProcessLogUseCaseMock
                .Setup(x => x.ExecuteAsync(createdDate))
                .ReturnsAsync(new List<NightlyProcessLogResponse>());

            // Act
            var response = await _classUnderTest.GetNightlyProcessLogs(createdDate).ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Fact]
        public async Task GetNightlyProcessLogs_WhenLogsFound_ReturnsOkWithLogs()
        {
            // Arrange
            var createdDate = DateTime.UtcNow;
            var logs = _fixture.Create<List<NightlyProcessLogResponse>>();

            _nightlyProcessLogUseCaseMock
                .Setup(x => x.ExecuteAsync(createdDate))
                .ReturnsAsync(logs);

            // Act
            var response = await _classUnderTest.GetNightlyProcessLogs(createdDate).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            var result = (response as OkObjectResult)?.Value;
            result.Should().BeEquivalentTo(logs);
        }

        [Fact]
        public async Task GetNightlyProcessLogs_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var createdDate = DateTime.UtcNow;

            _nightlyProcessLogUseCaseMock
                .Setup(x => x.ExecuteAsync(createdDate))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            Func<Task> act = async () => await _classUnderTest.GetNightlyProcessLogs(createdDate).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Test exception");
        }
    }
}
