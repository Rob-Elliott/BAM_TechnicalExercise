using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StargateAPI.Business.Repositories;

namespace StargateAPI.Business.Data
{
    /// <summary>
    /// A person's astronaut assignments
    /// </summary>
    [Table("AstronautDuty")]
    [DebuggerDisplay("AstronautDuty[{Id}] [{PersonId}] {Rank}")]
    public class AstronautDuty : IBaseEntity
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public string Rank { get; set; } = string.Empty;

        public string DutyTitle { get; set; } = string.Empty;

        public DateTime DutyStartDate { get; set; }

        public DateTime? DutyEndDate { get; set; }

        public virtual Person Person { get; set; }
    }

    public class AstronautDutyConfiguration : IEntityTypeConfiguration<AstronautDuty>
    {
        public void Configure(EntityTypeBuilder<AstronautDuty> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
