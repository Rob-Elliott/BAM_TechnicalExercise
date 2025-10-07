using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;

namespace StarGateTests
{
    public class PersonAstronautTests
    {
        [Fact]
        public void Constructor_WithPrimitives_SetsProperties()
        {
            var dto = new PersonAstronaut(123, "Alice", "CPT", "Pilot", new DateTime(2000, 1, 1), null);

            Assert.Equal(123, dto.PersonId);
            Assert.Equal("Alice", dto.Name);
            Assert.Equal("CPT", dto.CurrentRank);
            Assert.Equal("Pilot", dto.CurrentDutyTitle);
            Assert.Equal(new DateTime(2000, 1, 1), dto.CareerStartDate);
            Assert.Null(dto.CareerEndDate);
        }

        [Fact]
        public void Constructor_WithPersonAndNullDetail_HandlesNullDetail()
        {
            var person = new Person { Id = 42, Name = "Bob" };

            var dto = new PersonAstronaut(person, null);

            Assert.Equal(42, dto.PersonId);
            Assert.Equal("Bob", dto.Name);
            Assert.Null(dto.CurrentRank);
            Assert.Null(dto.CurrentDutyTitle);
            Assert.Null(dto.CareerStartDate);
            Assert.Null(dto.CareerEndDate);
        }

        [Fact]
        public void Constructor_WithPersonAndDetail_MapsValues()
        {
            var person = new Person { Id = 7, Name = "Carol" };
            var detail = new AstronautDetail { PersonId = 7, CurrentRank = "LT", CurrentDutyTitle = "Engineer", CareerStartDate = new DateTime(1999, 1, 1) };

            var dto = new PersonAstronaut(person, detail);

            Assert.Equal(7, dto.PersonId);
            Assert.Equal("Carol", dto.Name);
            Assert.Equal("LT", dto.CurrentRank);
            Assert.Equal("Engineer", dto.CurrentDutyTitle);
            Assert.Equal(new DateTime(1999, 1, 1), dto.CareerStartDate);
            Assert.Null(dto.CareerEndDate);
        }
    }
}
