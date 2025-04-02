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
4. When you run the app for the first time, the database will be populated with sample data.
5. To log in, choose any of the following username / password combos:
   - admin / admin (Full Access)
   - attorney / attorney (Limited Access)
   - paralegal / paralegal (Limited Access)
  
### Cases ### 
| Feature              | Admin           | Attorney      | Paralegal        | Complete    | 
|----------------------|-----------------|---------------|------------------|-------------|
| View Cases           | &check;         | &check;       | &check;          | &check;     |
| Search Cases         | &check;         | &check;       | &check;          | &cross;     |
| Add Case             | &check;         | &check;       | &check;          | &cross;     |
| Edit Case            | &check;         | &check;       | &check;          | &cross;     |
| Delete Case          | &check;         | &cross;       | &cross;          | &cross;     |
| View Case Comments   | &check;         | &check;       | &check;          | &check;     |
| Add Case Comment     | &check;         | &check;       | &check;          | &cross;     |
| Edit Case Comment    | &check;         | &check;       | &check;          | &cross;     |
| Delete Case Comment  | &check;         | &cross;       | &cross;          | &cross;     |
| View Case Documents  | &check;         | &check;       | &check;          | &check;     |
| Add Case Document    | &check;         | &check;       | &check;          | &cross;     |
| Upload Case Document | &check;         | &check;       | &check;          | &cross;     |
| Edit Case Document   | &check;         | &check;       | &check;          | &cross;     |
| Delete Case Document | &check;         | &cross;       | &cross;          | &cross;     |
| View Case Notes      | &check;         | &check;       | &check;          | &check;     |
| Add Case Note        | &check;         | &check;       | &check;          | &cross;     |
| Edit Case Note       | &check;         | &check;       | &check;          | &cross;     |
| Delete Case Note     | &check;         | &cross;       | &cross;          | &cross;     |
| View Case Users      | &check;         | &check;       | &check;          | &check;     |
| Add Case User        | &check;         | &check;       | &check;          | &cross;     |
| Delete Case User     | &check;         | &cross;       | &cross;          | &cross;     |

### Clients ### 
| Feature              | Admin           | Attorney      | Paralegal        | Complete    | 
|----------------------|-----------------|---------------|------------------|-------------|
| View Clients         | &check;         | &check;       | &check;          | &check;     |
| Search Clients       | &check;         | &check;       | &check;          | &cross;     |
| Add Client           | &check;         | &check;       | &check;          | &cross;     |
| Edit Client          | &check;         | &check;       | &check;          | &cross;     |
| Delete Client        | &check;         | &cross;       | &cross;          | &cross;     |

### Users ### 
| Feature              | Admin           | Attorney      | Paralegal        | Complete    | 
|----------------------|-----------------|---------------|------------------|-------------|
| View Users           | &check;         | &check;       | &check;          | &check;     |
| Add User             | &check;         | &cross;       | &cross;          | &cross;     |
| Edit User            | &check;         | &cross;       | &cross;          | &cross;     |
| Delete User          | &check;         | &cross;       | &cross;          | &cross;     |
| View User Accounts   | &check;         | &cross;       | &cross;          | &check;     |
| Add User Account     | &check;         | &cross;       | &cross;          | &cross;     |
| Edit User Account    | &check;         | &cross;       | &cross;          | &cross;     |
| Delete User Account  | &check;         | &cross;       | &cross;          | &cross;     |

### Misc. ### 
| Feature              | Admin           | Attorney      | Paralegal        | Complete    | 
|----------------------|-----------------|---------------|------------------|-------------|
| Login / Logout       | &check;         | &check;       | &check;          | &check;     |
| View Reports         | &check;         | &check;       | &check;          | &cross;     |
| Add Court Dates      | &check;         | &check;       | &check;          | &cross;     |
| Edit Court Dates     | &check;         | &check;       | &check;          | &cross;     |
| Delete Court Dates   | &check;         | &check;       | &check;          | &cross;     |

---

## Key Requirements ##

### 1. Authentication & Authorization ###
- [x] Implement user authentication with login/logout functionality.
- [x] Use role-based authorization (e.g., Admin, Attorney, Paralegal).

### 2. Core Features ###

#### 2.1 Case Management ####
- [x] Create, edit, and delete cases.
- [x] Associate cases with clients and attorneys.
- [x] Track case status (e.g., Open, Closed, In Progress).
- [x] Assign a case type (e.g., Criminal, Civil, Family Law).

#### 2.2 Client Management ####
- [x] Add, edit, and delete clients.
- [x] Store client contact details.
- [x] Associate clients with one or multiple cases.

#### 2.3 Document Management ####
- [ ] Upload, view, and delete case-related documents.
- [x] Associate documents with specific cases.
- [x] Display metadata (e.g., uploaded by, date uploaded).

#### 2.4 Notes & Communications ####
- [x] Attorneys can add case notes.
- [x] Support an internal commenting system on cases.

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
