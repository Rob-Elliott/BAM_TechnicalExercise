using Microsoft.EntityFrameworkCore;
using System.Data;

namespace StargateAPI.Business.Data
{
    public class StargateContext : DbContext
    {
        public IDbConnection Connection => Database.GetDbConnection();
        public DbSet<Person> People { get; set; }
        public DbSet<AstronautDetail> AstronautDetails { get; set; }
        public DbSet<AstronautDuty> AstronautDuties { get; set; }

        public StargateContext(DbContextOptions<StargateContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StargateContext).Assembly);

            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            //add seed data
            modelBuilder.Entity<Person>()
                .HasData(
                    new Person
                    {
                        Id = 1,
                        Name = "John Doe"
                    },
                    new Person
                    {
                        Id = 2,
                        Name = "Jane Doe"
                    },
                    new Person
                    {
                        Id = 3,
                        Name = "John Young"
                    },
                    new Person
                    {
                        Id = 4,
                        Name = "Michael Collins"
                    },
                    new Person
                    {
                        Id = 5,
                        Name = "Virgil Grissom"
                    }
                );

            modelBuilder.Entity<AstronautDetail>()
                .HasData(
                    new AstronautDetail
                    {
                        Id = 1,
                        PersonId = 1,
                        CurrentRank = "1LT",
                        CurrentDutyTitle = "Commander",
                        CareerStartDate = new DateTime(2020,02,03)
                    },
                    new AstronautDetail
                    {
                        Id = 2,
                        PersonId = 3,
                        CurrentRank = "LCL",
                        CurrentDutyTitle = "Commander",
                        CareerStartDate = new DateTime(1969, 01, 01)
                    },
                    new AstronautDetail
                    {
                        Id = 3,
                        PersonId = 4,
                        CurrentRank = "CPT",
                        CurrentDutyTitle = "RETIRED",
                        CareerStartDate = new DateTime(1967, 01, 01),
                        CareerEndDate = new DateTime(1969, 01, 02),
                    },
                    new AstronautDetail
                    {
                        Id = 4,
                        PersonId = 5,
                        CurrentRank = "1LT",
                        CurrentDutyTitle = "RETIRED",
                        CareerStartDate = new DateTime(1960, 01, 01),
                        CareerEndDate = new DateTime(1966, 01, 02),
                    }
                );

            modelBuilder.Entity<AstronautDuty>()
                .HasData(
                    new AstronautDuty
                    {
                        Id = 1,
                        PersonId = 1,
                        DutyStartDate = new DateTime(2025, 04, 05),
                        DutyTitle = "Commander",
                        Rank = "1LT"
                    },
                    new AstronautDuty
                    {
                        Id = 2,
                        PersonId = 3,
                        DutyStartDate = new DateTime(1962, 01, 01),
                        DutyEndDate = new DateTime(1966, 01, 01),
                        DutyTitle = "Pilot",
                        Rank = "1LT"
                    },
                    new AstronautDuty
                    {
                        Id = 3,
                        PersonId = 3,
                        DutyStartDate = new DateTime(1966, 01, 02),
                        DutyEndDate = new DateTime(1967, 01, 01),
                        DutyTitle = "Command Pilot",
                        Rank = "CPT"
                    },
                    new AstronautDuty
                    {
                        Id = 4,
                        PersonId = 3,
                        DutyStartDate = new DateTime(1967, 01, 02),
                        DutyEndDate = new DateTime(1969, 01, 01),
                        DutyTitle = "Command Module Pilot",
                        Rank = "MAJ"
                    },
                    new AstronautDuty
                    {
                        Id = 5,
                        PersonId = 3,
                        DutyStartDate = new DateTime(1969, 01, 02),
                        DutyEndDate = new DateTime(1983, 01, 01),
                        DutyTitle = "Commander",
                        Rank = "LCL"
                    },

                    new AstronautDuty
                    {
                        Id = 6,
                        PersonId = 4,
                        DutyStartDate = new DateTime(1962, 01, 01),
                        DutyEndDate = new DateTime(1967, 01, 01),
                        DutyTitle = "Pilot",
                        Rank = "1LT"
                    },
                    new AstronautDuty
                    {
                        Id = 7,
                        PersonId = 4,
                        DutyStartDate = new DateTime(1967, 01, 02),
                        DutyEndDate = new DateTime(1969, 01, 01),
                        DutyTitle = "Command Module Pilot",
                        Rank = "CPT"
                    },
                    new AstronautDuty
                    {
                        Id = 8,
                        PersonId = 5,
                        DutyStartDate = new DateTime(1960, 01, 02),
                        DutyEndDate = new DateTime(1966, 01, 01),
                        DutyTitle = "Command Pilot",
                        Rank = "1LT"
                    },
                    new AstronautDuty
                    {
                        Id = 9,
                        PersonId = 4,
                        DutyStartDate = new DateTime(1969, 01, 02),
                        DutyTitle = "RETIRED",
                        Rank = "CPT"
                    },
                    new AstronautDuty
                    {
                        Id = 10,
                        PersonId = 5,
                        DutyStartDate = new DateTime(1966, 01, 02),
                        DutyTitle = "RETIRED",
                        Rank = "1LT"
                    }
                );
        }
    }
}
