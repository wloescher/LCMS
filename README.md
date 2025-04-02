# LCMS - Legal Case Management System

Web application that enables attorneys to manage legal cases, associate clients and documents, and schedule court appearances.
- [x] Blazor Server
- [x] C# / .NET 8 (Core)
- [x] EF Core Power Tools
- [x] Entity Framework
- [x] SQL Express
- [x] Responsive Design (Bootstrap)

---

## Setup ##

1. Run the db script file "lcms-create-db.sql" to create the SQL database.
2. Right click on the Presentation/LCMS.Blazor project and select "Set as Startup Project".
3. To launch the application, hit F5 or selected Debug > Start Debugging.
4. To log in, choose any of the following username / password combos:
   - admin / admin
   - attorney / attorney
   - paralegal / paralegal

---

## Key Requirements ##

### 1. Authentication & Authorization ###
- [x] Implement user authentication with login/logout functionality.
- [x] Use role-based authorization (e.g., Admin, Attorney, Paralegal).

### 2. Core Features ###

#### 2.1 Case Management ####
- [ ] Create, edit, and delete cases.
- [x] Associate cases with clients and attorneys.
- [x] Track case status (e.g., Open, Closed, In Progress).
- [x] Assign a case type (e.g., Criminal, Civil, Family Law).

#### 2.2 Client Management ####
- [ ] Add, edit, and delete clients.
- [x] Store client contact details.
- [x] Associate clients with one or multiple cases.

#### 2.3 Document Management ####
- [ ] Upload, view, and delete case-related documents.
- [x] Associate documents with specific cases.
- [ ] Display metadata (e.g., uploaded by, date uploaded).

#### 2.4 Notes & Communications ####
- [ ] Attorneys can add case notes.
- [ ] Support an internal commenting system on cases.

### 3. UI & User Experience ###
- [x] Implement responsive design for desktop and mobile.
- [x] Use data grids for case, client, and document listings.

### 4. Data Storage & Access ###
- [x] Use Entity Framework Core with a SQL database.
- [x] Implement repository pattern for data access.
- [x] Use Blazor’s dependency injection for services.

### 5. Reporting & Search ###
- [ ] Implement a search functionality for cases and clients.
- [ ] Generate basic reports (e.g., open cases, upcoming court dates).
