This demo application was initially developed for code assessment purposes. However, I hope it can also serve as a useful resource for new programmers learning how to use SQL Server, .NET Core for database connectivity, and Entity Framework with AngularJS.

### Project Overview
- Goal: Develop a demonstration system to showcase the integration between a modern frontend and backend.

- Technology Stack:

  - Frontend: Angular

  - Backend: .NET Core

  - Data Handling: Entity Framework for data communication.

📅 Timeline & Scope
- Initial Release: 3-day development window.

- Modification/Refinement: 1-2 days allocated for post-release adjustments.

- Purpose: The entire program is for demonstration purposes only (i.e., a Minimum Viable Product or Proof of Concept, not a production-ready system).


### Technologies Used:

Database: MSSQL 2022 Developer Edition

Frontend: Angular v20 (developed using Visual Studio Code)

Backend: C# .NET Core 10.0

Development Tools: Visual Studio 2026 Community Edition

Web Server: Nginx Reverse Proxy

Other: HTML, JavaScript, Certbot SSL


### Features:

Users can search for books on the Book page, with results returning 100,000 records in less than one second thanks to the caching technology implemented.
Borrowed books are displayed on the Borrowed page.


### Installation procedure:

Download and Install Node.js (Latest LTS version is recommended)

```bash
https://nodejs.org/en/download/current
```

Verify Node.js and npm Installation (Run node -v and npm -v)

```bash
node -v
npm -v
```

Install Angular CLI (Latest Stable Version)
```
npm install -g @angular/cli@latest
```

Restore MSSQL Database (Using the provided database [file](https://edtlib.vagweb.com/sqlbackup.7z) )
```bash
https://edtlib.vagweb.com/sqlbackup.7z
```

Execute SQL Script (To create database user and set required permissions)
```bash
USE [master]
GO

CREATE LOGIN [EdtBooking] WITH PASSWORD = '3dt@B00k1ng';
GO

Use EdtBooking;
ALTER USER [EdtBooking] WITH LOGIN = [EdtBooking];
ALTER Login [EdtBooking] with DEFAULT_DATABASE = [EdtBooking];
GO

select * from users;

```

Git Clone Backend Source (.NET Project)
```bash
git clone https://github.com/edwinwcw2021/EdtDotNet.git
```

Git Clone Frontend Source (Angular Project) 
```
git clone https://github.com/edwinwcw2021/edtangular.git
```

Install Frontend Dependencies (Navigate to the Angular directory and run npm install)
```
cd edtangular
npm -i
```

Build/Run Backend Project (Open the .NET solution and Angular Project and press F5 to run/debug)


Follow External Links (Check the YouTube and Rumble links for additional context/instructions)

[Youtube]()

[Rumble]() 



First Release 31/01/2025
Last Update 22/11/2025

Click [here](https://edtlib.vagweb.com/) to visit the live website.

Click [here](https://edtlib.vagweb.com/sqlbackup.7z) to download database backup.
