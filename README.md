# Simple Time Tracker

## Overview
This project was implemented as a code kata for the second stage of an interview with CMap, a provider of software as a service to the architect, engineering, and consulting industries. It creates a simple timesheet application for tracking hours worked by users on various projects.

## Features Implemented
- **Add Timesheet Entry**: Users can submit their username, date, project, description, and hours worked.
- **CSV Download**: Allows users to download all timesheet entries in a CSV format.
- **Basic Error Handling**: Validation is performed at both the controller and service levels. The UI includes front-end validation to prevent common errors.
- **Simple UI**: A basic interface for adding entries and downloading the CSV file.

## Key Design Choices
- **Validation Strategy**:
  - Data validation is handled at the service level, though the controller itself includes some input validation to ensure relevant messages are returned for nonsensical inputs like invalid dates, without passing these errors to the service.
  - The UI also includes front-end validation to prevent bad data being entered by the user.
- **Scoped Services**:
  - The `ITimesheetRepository` is registered as a singleton to persist data for the lifetime of the application. The `ITimesheetService` is registered as scoped to allow for service-level logic to process each request independently.
- **Error Handling**:
  - The application handles invalid user input gracefully by returning error messages to the front-end. Unexpected errors are also managed to prevent application crashes, with user-friendly error messages displayed.
- **UI Design**:
  - The UI is simple but functional, focusing on meeting the requirements of the kata. It includes validation feedback and basic functionality for submitting timesheets and downloading the CSV output.

## Limitations
- **Data Persistence**: The current implementation uses an in-memory repository, meaning all data is lost when the application restarts. Future improvements could introduce a database for long-term storage.
- **Basic UI**: The current UI is minimal. Improved layout, branding, and additional pages for viewing and managing existing timesheets could significantly enhance usability.
- **Entry Duplication**: Currently, duplicate entries are allowed. Introducing unique identifiers for entries would be required to enable features like editing or deletion.
- **Testing**: Expanding test coverage to validate edge cases (e.g., leap years, overlapping timesheet entries) would help ensure robust behavior and make the system more resilient to changes in requirements or edge-case user data. A test has been included to ensure that entries do not exceed 24 hours per day per user, but this functionality is not implemented due to time constraints.

## Future Enhancements
- **UI Improvements**:
  - Enhance the layout and branding for better user adoption.
  - Add pages for viewing and filtering timesheet entries by user or project, with options to edit or delete entries.
- **Data Validation**:
  - Ensure entries cannot exceed a total of 24 hours per day per user.
- **Repository Enhancements**:
  - Replace the in-memory repository with a database for persistent storage.
  - Introduce unique identifiers for entries to enable editing and deletion functionality.
  - Adding unique identifiers to each timesheet entry would additionally improve database querying efficiency and data integrity in a persistent storage system, should one be implemented in the future
- **CSV Features**:
  - Allow users to filter entries by date, user, or project before downloading the CSV file.

## Notable Limitations
1. **Error Feedback**  
   Error messages are presented as plain text. A more user-friendly feedback mechanism (e.g., inline field validation) could improve usability.

2. **Basic UI**  
   The current UI is minimal and may not scale well with additional features. Future iterations could consider adopting a more robust frontend framework.

## Instructions to Run the Project
1. Clone the repository:
   ```bash
   git clone https://github.com/TheWelshbrit/SimpleTimeTracker.git
   cd <project-folder>
   cd SimpleTimeTracker
   ```
2. Build the project:
   ```bash
   dotnet build
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. Access the application in your browser at `http://localhost:5242` (or the alternate URL provided by your terminal window's output after running the application).

## Conclusion
This project demonstrates core functionality for a simple timesheet application. While the basic requirements are fulfilled, there is significant potential for enhancing usability, robustness, and maintainability. The project's structure and frequent check-ins highlight a clear development process focused on scalability and maintainability. The thought and development process have been clearly documented via the check-in history throughout the completion of this task.