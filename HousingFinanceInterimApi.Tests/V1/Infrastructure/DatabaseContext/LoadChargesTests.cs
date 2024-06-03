using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;


namespace HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "This is a test class")]


public class LoadChargesTests : IClassFixture<BaseContextTest>
{
    private readonly HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext _context;
    private readonly Fixture _fixture;
    private readonly List<Action> _cleanups;
    private readonly Action<string> _executeProcedure;

    public LoadChargesTests(BaseContextTest baseContextTest)
    {
        _context = baseContextTest._context;
        _fixture = baseContextTest._fixture;
        _cleanups = baseContextTest._cleanups;
        _executeProcedure = baseContextTest.ExecuteProcedure;
    }

    private string GeneratePropRef()
    {
        // 8 Digit random number that leads with 9 to prevent clashes e.g. "90023871"
        return string.Concat("9", _fixture.Create<int>().ToString().PadLeft(7, '0').AsSpan(0, 7));
    }

    [Fact]
    public void LoadChargesWorks()
    {
        var expectedChargeCount = 49; // This many types of charge in ChargesAux

        // Arrange
        var newChargeAux = _fixture.Build<ChargesAux>()
            .Without(chargeAux => chargeAux.Id)
            .With(chargeAux => chargeAux.RentGroup, "HousingRevenue")
            .With(chargeAux => chargeAux.PropertyRef, GeneratePropRef())
            .With(chargeAux => chargeAux.Year, DateTime.Now.Year).Create();

        _context.ChargesAux.Add(newChargeAux);
        _context.SaveChanges();

        _cleanups.Add(() =>
        {
            _context.ChargesAux.Remove(newChargeAux);
            _context.SaveChanges();
        });

        // Act
        _executeProcedure("usp_LoadCharges");

        // Fetch
        var relatedCharges = _context.Charges.Where(
            charge => charge.PropertyRef == newChargeAux.PropertyRef
        ).ToList();

        _cleanups.Add(() =>
        {
            // Massively speed up cleanup by temporarily disabling FK constraint from Charges History
            // There will be no FK violations because the charges have not been loaded to CH yet
            _context.Database.ExecuteSqlRaw(
                "ALTER TABLE dbo.ChargesHistory NOCHECK CONSTRAINT FK__ChargesHi__Charg__2AF6222B");
            _context.Charges.RemoveRange(relatedCharges);
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw(
                "ALTER TABLE dbo.ChargesHistory CHECK CONSTRAINT FK__ChargesHi__Charg__2AF6222B");
        });

        // Assert
        Assert.NotNull(relatedCharges);
        Assert.Equal(relatedCharges.Count, expectedChargeCount);

        // Ensure correct charges are loaded (correct amount per charge type)
        PropertyInfo[] properties = typeof(ChargesAux).GetProperties();

        foreach (var charge in relatedCharges)
        {
            var matchingProperty = properties.FirstOrDefault(property => property.Name == charge.ChargeType);
            Assert.NotNull(matchingProperty);
            var value = matchingProperty.GetValue(newChargeAux);
            Assert.Equal(value, charge.Amount);
        }
    }
}
