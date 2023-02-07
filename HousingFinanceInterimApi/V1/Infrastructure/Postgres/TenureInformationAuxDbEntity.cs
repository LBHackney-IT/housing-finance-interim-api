using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure.Postgres
{
    [Table("tenures_information_aux")]
    public class TenureInformationAuxDbEntity
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("payment_reference")]
        public string PaymentReference { get; set; }
    }
}
