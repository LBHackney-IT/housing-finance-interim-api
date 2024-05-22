using System.Linq;
using System.Collections.Generic;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class OpBalTransactionFactory
    {
        public static IList<PRNTransactionDomain> ToDomain(this IList<PRNTransactionEntity> dbEntityCollection)
        {
            if (dbEntityCollection is null)
                return null;

            return dbEntityCollection
                .Select(t => t.ToDomain())
                .ToList();
        }

        public static PRNTransactionDomain ToDomain(this PRNTransactionEntity dbEntity)
        {
            if (dbEntity is null)
                return null;

            var extensionModel = BaseToDomain<PRNTransactionDomain>(dbEntity);

            extensionModel.RentAccount = dbEntity.RentAccount;

            return extensionModel;
        }

        public static TDomain BaseToDomain<TDomain>(this BaseOperatingBalanceTransactionEntity dbEntity)
            where TDomain : BaseOperatingBalanceTransactionDomain, new()
        {
            if (dbEntity is null)
                return null;

            return new TDomain {
                RentGroup = dbEntity.RentGroup,
                Year = dbEntity.Year,
                TotalCharged = dbEntity.TotalCharged,
                TotalPaid = dbEntity.TotalPaid,
                TotalHBPaid = dbEntity.TotalHBPaid,
                Section20Rebate = dbEntity.Section20Rebate,
                Section125Rebate = dbEntity.Section125Rebate,
                AssignmentSCTrans = dbEntity.AssignmentSCTrans,
                BasicRentNoVAT = dbEntity.BasicRentNoVAT,
                MWBalanceTransfer = dbEntity.MWBalanceTransfer,
                CPreliminaries = dbEntity.CPreliminaries,
                CProvisionalSums = dbEntity.CProvisionalSums,
                CContingencySums = dbEntity.CContingencySums,
                CProfessionalFees = dbEntity.CProfessionalFees,
                CAdministration = dbEntity.CAdministration,
                CleaningBlock = dbEntity.CleaningBlock,
                CourtCosts = dbEntity.CourtCosts,
                CleaningEstate = dbEntity.CleaningEstate,
                ContentsInsurance = dbEntity.ContentsInsurance,
                Concierge = dbEntity.Concierge,
                CarPort = dbEntity.CarPort,
                CommunalDigitalTV = dbEntity.CommunalDigitalTV,
                GarageAttached = dbEntity.GarageAttached,
                GroundsMaintenance = dbEntity.GroundsMaintenance,
                GroundRent = dbEntity.GroundRent,
                HostAmenity = dbEntity.HostAmenity,
                Heating = dbEntity.Heating,
                HeatingMaintenance = dbEntity.HeatingMaintenance,
                HotWater = dbEntity.HotWater,
                Interest = dbEntity.Interest,
                ArrangementInterest = dbEntity.ArrangementInterest,
                LostKeyFobs = dbEntity.LostKeyFobs,
                LegacyDebit = dbEntity.LegacyDebit,
                LostKeyCharge = dbEntity.LostKeyCharge,
                LandlordLighting = dbEntity.LandlordLighting,
                LatePaymentCharge = dbEntity.LatePaymentCharge,
                MajorWorksCapital = dbEntity.MajorWorksCapital,
                TAManagementFee = dbEntity.TAManagementFee,
                MWJudgementTrans = dbEntity.MWJudgementTrans,
                MajorWorksLoan = dbEntity.MajorWorksLoan,
                MajorWorksRevenue = dbEntity.MajorWorksRevenue,
                ParkingPermits = dbEntity.ParkingPermits,
                ParkingAnnualChg = dbEntity.ParkingAnnualChg,
                RPreliminaries = dbEntity.RPreliminaries,
                RProvisionalSums = dbEntity.RProvisionalSums,
                RContingencySums = dbEntity.RContingencySums,
                RProfessionalFees = dbEntity.RProfessionalFees,
                RAdministrationFee = dbEntity.RAdministrationFee,
                RechgRepairsnoVAT = dbEntity.RechgRepairsnoVAT,
                RechargeableRepairs = dbEntity.RechargeableRepairs,
                SCAdjustment = dbEntity.SCAdjustment,
                SCBalancingCharge = dbEntity.SCBalancingCharge,
                ServiceCharges = dbEntity.ServiceCharges,
                SCJudgementDebit = dbEntity.SCJudgementDebit,
                SharedOwnersRent = dbEntity.SharedOwnersRent,
                ReserveFund = dbEntity.ReserveFund,
                Storage = dbEntity.Storage,
                BasicRentTempAcc = dbEntity.BasicRentTempAcc,
                TravellersCharge = dbEntity.TravellersCharge,
                TenantsLevy = dbEntity.TenantsLevy,
                TelevisionLicense = dbEntity.TelevisionLicense,
                VATCharge = dbEntity.VATCharge,
                WaterRates = dbEntity.WaterRates,
                WaterStandingChrg = dbEntity.WaterStandingChrg,
                WatersureReduction = dbEntity.WatersureReduction,
                BailiffPayment = dbEntity.BailiffPayment,
                BankPayment = dbEntity.BankPayment,
                PayPointPostOfficeRBR = dbEntity.PayPointPostOfficeRBR,
                RepCashIncentive = dbEntity.RepCashIncentive,
                CashOfficePayments = dbEntity.CashOfficePayments,
                DebitCreditCard = dbEntity.DebitCreditCard,
                MWCreditTransfer = dbEntity.MWCreditTransfer,
                DirectDebit = dbEntity.DirectDebit,
                DirectDebitUnpaid = dbEntity.DirectDebitUnpaid,
                DeductionWorkP = dbEntity.DeductionWorkP,
                BACSRefund = dbEntity.BACSRefund,
                DeductionSalary = dbEntity.DeductionSalary,
                DSSTransfer = dbEntity.DSSTransfer,
                TenantRefund = dbEntity.TenantRefund,
                InternalTransfer = dbEntity.InternalTransfer,
                MWLoanPayment = dbEntity.MWLoanPayment,
                OpeningBalance = dbEntity.OpeningBalance,
                PromptPayDiscount = dbEntity.PromptPayDiscount,
                PostalOrder = dbEntity.PostalOrder,
                PayPointPostOfficeRPY = dbEntity.PayPointPostOfficeRPY,
                ChequePayments = dbEntity.ChequePayments,
                ReturnedCheque = dbEntity.ReturnedCheque,
                RechargeRepCredit = dbEntity.RechargeRepCredit,
                SCJudgementTrans = dbEntity.SCJudgementTrans,
                StandingOrder = dbEntity.StandingOrder,
                TMOReversal = dbEntity.TMOReversal,
                UniversalCreditRec = dbEntity.UniversalCreditRec,
                Rentwaiver = dbEntity.Rentwaiver,
                WriteOff = dbEntity.WriteOff,
                WriteOn = dbEntity.WriteOn,
                HBAdjustment = dbEntity.HBAdjustment,
                HousingBenefit = dbEntity.HousingBenefit
            };
        }
    }
}
