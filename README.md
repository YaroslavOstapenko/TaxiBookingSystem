# 🚕 Taxi Booking Management System

![Status](https://img.shields.io/badge/Status-Active-success)
![Backend](https://img.shields.io/badge/Backend-ASP.NET_Core_8.0-blueviolet)
![Frontend](https://img.shields.io/badge/Frontend-React_18-blue)
![Database](https://img.shields.io/badge/Database-MySQL-orange)

A robust, full-stack taxi management solution featuring a RESTful API, a reactive dashboard, and an automated simulation engine for stress testing.

---

## 🌟 Key Features

### 🧾 Booking Lifecycle
- **Full CRUD Operations:** Seamlessly create, read, update, and delete bookings.
- **Status Workflow:** Real-time tracking of booking states: `PENDING`, `CONFIRMED`, and `CANCELLED`.
- **Auto-Sync:** Frontend UI features a 3-second polling mechanism to keep data fresh.

### 🚀 Simulation Engine (Load Testing)
Built-in tool to mimic real-world system usage:
- Generates **5–30 random bookings** instantly.
- Intelligent route generation using a predefined city list.
- Simulated user behavior with 1–2s delays and a **50% random cancellation rate**.

---

## 🛠 Tech Stack

| Layer | Technology |
| :--- | :--- |
| **Backend** | ASP.NET Core Web API, Entity Framework Core |
| **Frontend** | React (Hooks & Functional Components), Axios |
| **Database** | MySQL 8.0+ |
| **UI/UX** | Lucide React Icons, Modern CSS |

---

## 📡 API Documentation

### Primary Endpoints

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/bookings/create` | Create a new booking request |
| `GET` | `/api/bookings/all` | Fetch all existing bookings |
| `POST` | `/api/bookings/{id}/confirm` | Transition status to `CONFIRMED` |
| `POST` | `/api/bookings/{id}/cancel` | Transition status to `CANCELLED` |
| `DELETE` | `/api/bookings/{id}` | Permanently remove a record |
| `POST` | `/api/bookings/simulate` | Trigger the load testing engine |

---

## 🗄️ Database Schema

The system uses a relational MySQL structure with **Optimistic Concurrency** control via a versioning column.

```sql
CREATE TABLE bookings (
    id CHAR(36) PRIMARY KEY,
    user_phone VARCHAR(50) NOT NULL,
    pickup_location VARCHAR(255) NOT NULL,
    drop_location VARCHAR(255) NOT NULL,
    status VARCHAR(20) DEFAULT 'PENDING',
    version INT DEFAULT 0
);
```

## 📺 Live Demo
[Watch the project walk-through on YouTube](https://youtu.be/w4cQpBo1xz0)
