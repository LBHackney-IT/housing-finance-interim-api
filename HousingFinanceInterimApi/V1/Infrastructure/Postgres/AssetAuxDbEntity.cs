using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure.Postgres
{
    [Table("assets_aux")]
    public class AssetAuxDbEntity
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("asset_id")]
        public string AssetId { get; set; }

        [Column("asset_type")]
        public string AssetType { get; set; }
    }
}
