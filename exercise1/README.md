<!--v004-->
# Stargate

***

## Astronaut Career Tracking System (ACTS)

ACTS is used as a tool to maintain a record of all the People that have served as Astronauts. When serving as an Astronaut, your *Job* (Duty) is tracked by your Rank, Title and the Start and End Dates of the Duty.

The People that exist in this system are not all Astronauts. ACTS maintains a master list of People and Duties that are updated from an external service (not controlled by ACTS). The update schedule is determined by the external service.

## Definitions

1. A person's astronaut assignment is the Astronaut Duty.
1. A person's current astronaut information is stored in the Astronaut Detail table.
1. A person's list of astronaut assignments is stored in the Astronaut Duty table.

## Requirements

##### Enhance the Stargate API (Required)

The REST API is expected to do the following:

1. Retrieve a person by name.
	// handled by GetPersonByName
1. Retrieve all people.
	// handled by GetPeople
1. Add/update a person by name.
	// handled by CreatePerson/UpdatePerson
1. Retrieve Astronaut Duty by name.
	// handled by GetAstronautDutiesByName
1. Add an Astronaut Duty.
	// handled by CreateAstronautDuty

##### Implement a user interface: (Encouraged)

The UI is expected to do the following:

1. Successfully implement a web application that demonstrates production level quality. Angular is preferred.
	// created basic ASP.NET Razor pages
1. Implement call(s) to retrieve an individual's astronaut duties.
	// Detail page created
1. Display the progress of the process and the results in a visually sophisticated and appealing manner.
	// no progress shown, but errors shown and successes displayed immediately.
	
## Tasks

Overview
Examine the code, find and resolve any flaws, if any exist. Identify design patterns and follow or change them. Provide fix(es) and be prepared to describe the changes.

1. Generate the database
   * This is your source and storage location
1. Enforce the rules
1. Improve defensive coding
1. Add unit tests
   * identify the most impactful methods requiring tests
   * reach >50% code coverage
1. Implement process logging
   * Log exceptions
   * Log successes
   * Store the logs in the database

## Rules

1. A Person is uniquely identified by their Name.
	// enforced by CreatePersonPreProcessor
1. A Person who has not had an astronaut assignment will not have Astronaut records.
1. A Person will only ever hold one current Astronaut Duty Title, Start Date, and Rank at a time.
	// handled by CreateAstronautDuty
1. A Person's Current Duty will not have a Duty End Date.
	// enforced by CreateAstronautDutyPreProcessor
1. A Person's Previous Duty End Date is set to the day before the New Astronaut Duty Start Date when a new Astronaut Duty is received for a Person.
	// handled by CreateAstronautDutyHandler
1. A Person is classified as 'Retired' when a Duty Title is 'RETIRED'.
	// handled by CreateAstronautDutyHandler
1. A Person's Career End Date is one day before the Retired Duty Start Date.
	// handled by CreateAstronautDutyHandler

Assumptions:
	Tried to keep the existing package versions
	New AstronautDuty records always occur after the latest existing record (no adding Duty records out of chronological order)
	No way to edit a Duty record
	No way to remove any records
	Name uniqueness is case-insensitive
	
Disclaimers/Issues:
	I could not figure out how StarGateContext.SeedData was supposed to be working, so I added it as an actual DB migration step.
	I ignored nullability issues, this should be resolved in production code.
	I ignored DateTime UTC vs Local issues, this should be resolved in production code.
	Security was not considered.
	Unit test coverage was not considered for demo website nor logging.
	Demo website was largely written with AI and manually checked for accuracy.
	Serilog logger used for logging to database, did not setup any filtering to exclude existing logging.
	