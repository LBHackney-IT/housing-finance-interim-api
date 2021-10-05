using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("ActionDiaryAux")]
    public class ActionDiaryAux
    {
        [Key]
        public long Id { get; set; }

        public string TenancyAgreementRef { get; set; }

        public string RentAccount { get; set; }

        public string ActionCode { get; set; }

        public string Action { get; set; }

        public DateTime? ActionDate { get; set; }

        public string Username { get; set; }

        public decimal Balance { get; set; }

        public string ActionComment { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset Timestamp { get; private set; }
    }
}
