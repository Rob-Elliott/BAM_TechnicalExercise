using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StargateAPI.Business.Data
{
    /// <summary>
    /// A person's current astronaut information
    /// </summary>
    [Table("AstronautDetail")]
    [DebuggerDisplay("AstronautDetail[{Id}] [{PersonId}] {CurrentRank}")]
    public class AstronautDetail
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public string CurrentRank { get; set; } = string.Empty;

        public string CurrentDutyTitle { get; set; } = string.Empty;

        public DateTime CareerStartDate { get; set; }

        public DateTime? CareerEndDate { get; set; }

        public virtual Person Person { get; set; }
    }

    public class AstronautDetailConfiguration : IEntityTypeConfiguration<AstronautDetail>
    {
        public void Configure(EntityTypeBuilder<AstronautDetail> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
