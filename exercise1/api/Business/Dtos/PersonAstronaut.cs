using System;
using System.Diagnostics;
using StargateAPI.Business.Data;

namespace StargateAPI.Business.Dtos
{
    /// <summary>
    /// DTO representing a Person and their AstronautDetail
    /// </summary>
    [DebuggerDisplay("PersonAstronaut[{PersonId}] {Name}")]
    public class PersonAstronaut
    {
        public PersonAstronaut() { }

        public PersonAstronaut(int personId, string name, string currentRank, string currentDutyTitle, DateTime? careerStartDate, DateTime? careerEndDate)
        {
            PersonId = personId;
            Name = name;
            CurrentRank = currentRank;
            CurrentDutyTitle = currentDutyTitle;
            CareerStartDate = careerStartDate;
            CareerEndDate = careerEndDate;
        }

        public PersonAstronaut(Person person, AstronautDetail detail)
            : this(person.Id, person.Name, detail?.CurrentRank, detail?.CurrentDutyTitle, detail?.CareerStartDate, detail?.CareerEndDate)
        { }


        public int PersonId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string CurrentRank { get; set; } = string.Empty;

        public string CurrentDutyTitle { get; set; } = string.Empty;

        public DateTime? CareerStartDate { get; set; }

        public DateTime? CareerEndDate { get; set; }
    }
}
