using System;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class NightlyProcessLogResponse
    {
        public long Id { get; set; }
        public string LogGroupName { get; set; }
        public DateTime? Timestamp { get; set; }
        public bool? IsSuccess { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
