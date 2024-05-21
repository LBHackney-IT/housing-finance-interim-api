using FluentAssertions;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Infrastructure;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Factories
{
    public class OpBalTransactionFactoryTests
    {
        [Fact]
        public void PRNTransactionCollectionGetsMappedCorrectlyFromEntityToDomain()
        {
            // arrange
            var dbEntityCollection = RandomGen.CreateMany<PRNTransactionEntity>();

            // act
            var mappedDomainCollection = dbEntityCollection.ToDomain();

            // assert
            mappedDomainCollection.Should().HaveSameCount(dbEntityCollection);

            // can afford a looser data map check as the full fields logic is tested elsewhere.
            mappedDomainCollection.Should().BeEquivalentTo(dbEntityCollection);
        }

        [Fact]
        public void PRNTransactionGetsMappedCorrectlyFromEntityToDomain()
        {
            // arrange
            var dbEntity = RandomGen.Create<PRNTransactionEntity>();

            // act
            var mappedDomain = dbEntity.ToDomain();

            // assert
            // check extended fields get mapped
            mappedDomain.RentAccount.Should().Be(dbEntity.RentAccount);

            // check the base field mapping gets triggered
            mappedDomain.RentGroup.Should().Be(dbEntity.RentGroup);
            mappedDomain.SCAdjustment.Should().Be(dbEntity.SCAdjustment);
            mappedDomain.HousingBenefit.Should().Be(dbEntity.HousingBenefit);
        }

        [Fact]
        public void BaseOperatingBalanceTransactionGetsMappedCorrectlyFromEntityToDomain()
        {
            // arrange
            var dbEntity = RandomGen.Create<BaseOperatingBalanceTransactionEntity>();

            // act
            var mappedDomain = dbEntity.BaseToDomain<PRNTransactionDomain>();

            // assert
            mappedDomain.RentGroup.Should().Be(dbEntity.RentGroup);
            mappedDomain.Year.Should().Be(dbEntity.Year);
            mappedDomain.TotalCharged.Should().Be(dbEntity.TotalCharged);
            mappedDomain.TotalPaid.Should().Be(dbEntity.TotalPaid);
            mappedDomain.TotalHBPaid.Should().Be(dbEntity.TotalHBPaid);
            mappedDomain.Section20Rebate.Should().Be(dbEntity.Section20Rebate);
            mappedDomain.Section125Rebate.Should().Be(dbEntity.Section125Rebate);
            mappedDomain.AssignmentSCTrans.Should().Be(dbEntity.AssignmentSCTrans);
            mappedDomain.BasicRentNoVAT.Should().Be(dbEntity.BasicRentNoVAT);
            mappedDomain.MWBalanceTransfer.Should().Be(dbEntity.MWBalanceTransfer);
            mappedDomain.CPreliminaries.Should().Be(dbEntity.CPreliminaries);
            mappedDomain.CProvisionalSums.Should().Be(dbEntity.CProvisionalSums);
            mappedDomain.CContingencySums.Should().Be(dbEntity.CContingencySums);
            mappedDomain.CProfessionalFees.Should().Be(dbEntity.CProfessionalFees);
            mappedDomain.CAdministration.Should().Be(dbEntity.CAdministration);
            mappedDomain.CleaningBlock.Should().Be(dbEntity.CleaningBlock);
            mappedDomain.CourtCosts.Should().Be(dbEntity.CourtCosts);
            mappedDomain.CleaningEstate.Should().Be(dbEntity.CleaningEstate);
            mappedDomain.ContentsInsurance.Should().Be(dbEntity.ContentsInsurance);
            mappedDomain.Concierge.Should().Be(dbEntity.Concierge);
            mappedDomain.CarPort.Should().Be(dbEntity.CarPort);
            mappedDomain.CommunalDigitalTV.Should().Be(dbEntity.CommunalDigitalTV);
            mappedDomain.GarageAttached.Should().Be(dbEntity.GarageAttached);
            mappedDomain.GroundsMaintenance.Should().Be(dbEntity.GroundsMaintenance);
            mappedDomain.GroundRent.Should().Be(dbEntity.GroundRent);
            mappedDomain.HostAmenity.Should().Be(dbEntity.HostAmenity);
            mappedDomain.Heating.Should().Be(dbEntity.Heating);
            mappedDomain.HeatingMaintenance.Should().Be(dbEntity.HeatingMaintenance);
            mappedDomain.HotWater.Should().Be(dbEntity.HotWater);
            mappedDomain.Interest.Should().Be(dbEntity.Interest);
            mappedDomain.ArrangementInterest.Should().Be(dbEntity.ArrangementInterest);
            mappedDomain.LostKeyFobs.Should().Be(dbEntity.LostKeyFobs);
            mappedDomain.LegacyDebit.Should().Be(dbEntity.LegacyDebit);
            mappedDomain.LostKeyCharge.Should().Be(dbEntity.LostKeyCharge);
            mappedDomain.LandlordLighting.Should().Be(dbEntity.LandlordLighting);
            mappedDomain.LatePaymentCharge.Should().Be(dbEntity.LatePaymentCharge);
            mappedDomain.MajorWorksCapital.Should().Be(dbEntity.MajorWorksCapital);
            mappedDomain.TAManagementFee.Should().Be(dbEntity.TAManagementFee);
            mappedDomain.MWJudgementTrans.Should().Be(dbEntity.MWJudgementTrans);
            mappedDomain.MajorWorksLoan.Should().Be(dbEntity.MajorWorksLoan);
            mappedDomain.MajorWorksRevenue.Should().Be(dbEntity.MajorWorksRevenue);
            mappedDomain.ParkingPermits.Should().Be(dbEntity.ParkingPermits);
            mappedDomain.ParkingAnnualChg.Should().Be(dbEntity.ParkingAnnualChg);
            mappedDomain.RPreliminaries.Should().Be(dbEntity.RPreliminaries);
            mappedDomain.RProvisionalSums.Should().Be(dbEntity.RProvisionalSums);
            mappedDomain.RContingencySums.Should().Be(dbEntity.RContingencySums);
            mappedDomain.RProfessionalFees.Should().Be(dbEntity.RProfessionalFees);
            mappedDomain.RAdministrationFee.Should().Be(dbEntity.RAdministrationFee);
            mappedDomain.RechgRepairsnoVAT.Should().Be(dbEntity.RechgRepairsnoVAT);
            mappedDomain.RechargeableRepairs.Should().Be(dbEntity.RechargeableRepairs);
            mappedDomain.SCAdjustment.Should().Be(dbEntity.SCAdjustment);
            mappedDomain.SCBalancingCharge.Should().Be(dbEntity.SCBalancingCharge);
            mappedDomain.ServiceCharges.Should().Be(dbEntity.ServiceCharges);
            mappedDomain.SCJudgementDebit.Should().Be(dbEntity.SCJudgementDebit);
            mappedDomain.SharedOwnersRent.Should().Be(dbEntity.SharedOwnersRent);
            mappedDomain.ReserveFund.Should().Be(dbEntity.ReserveFund);
            mappedDomain.Storage.Should().Be(dbEntity.Storage);
            mappedDomain.BasicRentTempAcc.Should().Be(dbEntity.BasicRentTempAcc);
            mappedDomain.TravellersCharge.Should().Be(dbEntity.TravellersCharge);
            mappedDomain.TenantsLevy.Should().Be(dbEntity.TenantsLevy);
            mappedDomain.TelevisionLicense.Should().Be(dbEntity.TelevisionLicense);
            mappedDomain.VATCharge.Should().Be(dbEntity.VATCharge);
            mappedDomain.WaterRates.Should().Be(dbEntity.WaterRates);
            mappedDomain.WaterStandingChrg.Should().Be(dbEntity.WaterStandingChrg);
            mappedDomain.WatersureReduction.Should().Be(dbEntity.WatersureReduction);
            mappedDomain.BailiffPayment.Should().Be(dbEntity.BailiffPayment);
            mappedDomain.BankPayment.Should().Be(dbEntity.BankPayment);
            mappedDomain.PayPointPostOfficeRBR.Should().Be(dbEntity.PayPointPostOfficeRBR);
            mappedDomain.RepCashIncentive.Should().Be(dbEntity.RepCashIncentive);
            mappedDomain.CashOfficePayments.Should().Be(dbEntity.CashOfficePayments);
            mappedDomain.DebitCreditCard.Should().Be(dbEntity.DebitCreditCard);
            mappedDomain.MWCreditTransfer.Should().Be(dbEntity.MWCreditTransfer);
            mappedDomain.DirectDebit.Should().Be(dbEntity.DirectDebit);
            mappedDomain.DirectDebitUnpaid.Should().Be(dbEntity.DirectDebitUnpaid);
            mappedDomain.DeductionWorkP.Should().Be(dbEntity.DeductionWorkP);
            mappedDomain.BACSRefund.Should().Be(dbEntity.BACSRefund);
            mappedDomain.DeductionSalary.Should().Be(dbEntity.DeductionSalary);
            mappedDomain.DSSTransfer.Should().Be(dbEntity.DSSTransfer);
            mappedDomain.TenantRefund.Should().Be(dbEntity.TenantRefund);
            mappedDomain.InternalTransfer.Should().Be(dbEntity.InternalTransfer);
            mappedDomain.MWLoanPayment.Should().Be(dbEntity.MWLoanPayment);
            mappedDomain.OpeningBalance.Should().Be(dbEntity.OpeningBalance);
            mappedDomain.PromptPayDiscount.Should().Be(dbEntity.PromptPayDiscount);
            mappedDomain.PostalOrder.Should().Be(dbEntity.PostalOrder);
            mappedDomain.PayPointPostOfficeRPY.Should().Be(dbEntity.PayPointPostOfficeRPY);
            mappedDomain.ChequePayments.Should().Be(dbEntity.ChequePayments);
            mappedDomain.ReturnedCheque.Should().Be(dbEntity.ReturnedCheque);
            mappedDomain.RechargeRepCredit.Should().Be(dbEntity.RechargeRepCredit);
            mappedDomain.SCJudgementTrans.Should().Be(dbEntity.SCJudgementTrans);
            mappedDomain.StandingOrder.Should().Be(dbEntity.StandingOrder);
            mappedDomain.TMOReversal.Should().Be(dbEntity.TMOReversal);
            mappedDomain.UniversalCreditRec.Should().Be(dbEntity.UniversalCreditRec);
            mappedDomain.Rentwaiver.Should().Be(dbEntity.Rentwaiver);
            mappedDomain.WriteOff.Should().Be(dbEntity.WriteOff);
            mappedDomain.WriteOn.Should().Be(dbEntity.WriteOn);
            mappedDomain.HBAdjustment.Should().Be(dbEntity.HBAdjustment);
            mappedDomain.HousingBenefit.Should().Be(dbEntity.HousingBenefit);
        }
    }
}
