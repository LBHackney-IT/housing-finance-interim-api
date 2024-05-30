using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;
using HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext.TestFactories;


namespace HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "This is a test class")]


public class LoadChargesTests
{
    private readonly HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext _context;
    private readonly Fixture _fixture;

    public LoadChargesTests()
    {
        _context = HfsDbContextFactory.Create();
        _fixture = new Fixture();
    }

    private async Task ExecuteProcedure(string procedureName)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);
        _context.Database.SetCommandTimeout(900);
        await _context.Database.ExecuteSqlRawAsync(procedureName).ConfigureAwait(false);
        await transaction.CommitAsync().ConfigureAwait(false);
    }

    private string GeneratePropRef()
    {
        // Lead with 9 to prevent clashes
        return string.Concat("9", _fixture.Create<int>().ToString().PadLeft(7, '0').AsSpan(0, 7));
    }

    [Fact]
    public async Task LoadChargesWorks()
    {
        var expectedChargeCount = 49; // The types of charge
        var cleanups = new List<Action>();
        try {
            // Arrange
            var newChargeAux = _fixture.Build<ChargesAux>()
                .Without(chargeAux => chargeAux.Id)
                .With(chargeAux => chargeAux.RentGroup, "HousingRevenue")
                .With(chargeAux => chargeAux.PropertyRef, GeneratePropRef())
                .With(chargeAux => chargeAux.Year, DateTime.Now.Year).Create();

            await _context.ChargesAux.AddAsync(newChargeAux).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            cleanups.Add(() => {
                _context.ChargesAux.Remove(newChargeAux);
                _context.SaveChanges();
            });

            // Act
            await ExecuteProcedure("usp_LoadCharges").ConfigureAwait(false);

            // Fetch
            var relatedCharges = await _context.Charges.Where(
                charge => charge.PropertyRef == newChargeAux.PropertyRef
            ).ToListAsync().ConfigureAwait(false);

            cleanups.Add(() => {
                _context.Charges.RemoveRange(relatedCharges);
            });

            // Assert
            Assert.NotNull(relatedCharges);
            Assert.Equal(relatedCharges.Count, expectedChargeCount);

            // Ensure correct charges are loaded
            PropertyInfo[] properties = typeof(ChargesAux).GetProperties();

            foreach (var charge in relatedCharges)
            {
                var matchingProperty = properties.FirstOrDefault(property => property.Name == charge.ChargeType);
                Assert.NotNull(matchingProperty);
                var value = matchingProperty.GetValue(newChargeAux);
                Assert.Equal(value, charge.Amount);
            }

        } finally {
            foreach (var cleanup in cleanups)
            {
                cleanup();
            }
        }
    }
}
