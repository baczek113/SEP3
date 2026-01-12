# HireFire

A distributed job-matching platform designed to streamline recruitment by enabling fast applications, recruiter decision-making, and real-time communication between matched candidates and recruiters.

HireFire was built as a full-stack, three-tier system with a strong focus on **clear separation of concerns**, **secure communication**, and **real-time interaction**.

---

## Overview

HireFire rethinks traditional job portals by reducing friction on both sides of the hiring process. Applicants can quickly browse and apply to relevant job listings without lengthy forms, while recruiters can efficiently review candidates, accept or decline applications, and immediately communicate with matched applicants via real-time chat.

The system is implemented as a **distributed 3-tier architecture** consisting of:

- A **Blazor-based client** (presentation layer)
- A **.NET application server** (logic tier)
- A **Java Spring Boot data server** (persistence tier)

This architecture allows the system to remain scalable, maintainable, and secure, while supporting future feature expansion.

---

## Key Features

- Role-based system (Applicant, Recruiter, Company Representative, Admin)
- Job listing creation, editing, closing, and removal
- Skill- and location-based job recommendations
- Application tracking with clear status updates
- Real-time recruiter–applicant chat using WebSockets
- Secure authentication with hashed passwords
- Strict separation between UI, business logic, and data access

---

## System Architecture

HireFire follows a **three-tier distributed architecture**:

