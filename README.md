# 🛡️ NetworkManager (Auto VLESS Tunnel)

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![Framework](https://img.shields.io/badge/.NET-8.0-purple)
![Status](https://img.shields.io/badge/Status-Active-success)

**NetworkManager** is a sophisticated Windows desktop utility that automates the creation of secure VLESS tunnels using **Xray-core** and **Cloudflare Quick Tunnels**. It eliminates the need for a VPS or personal domain by leveraging Cloudflare's infrastructure. The application automatically generates connection links and sends them directly to your **Telegram** via a bot.

---

## ✨ Key Features

* 🚀 **No VPS/Domain Required:** Utilizes Cloudflare Quick Tunnels (TryCloudflare) for free, secure connectivity.
* 🤖 **Smart Telegram Integration:** Automatically sends the generated VLESS connection key to your private Telegram chat upon startup.
* 🔒 **Security First:** Built on the powerful Xray-core, supporting VLESS + WS + TLS protocols.
* 🏗️ **Clean Architecture:** Professional codebase structure separating Core, Infrastructure, Application, and Presentation layers.
* ⚡ **Modern UI:** Designed with **WPF UI** and the MVVM pattern for a smooth user experience.
* 💾 **Local Database:** Uses SQLite with Entity Framework Core for managing user logs and configuration history.
* 🔄 **Auto-Recovery:** Detects tunnel URL changes and updates the connection link automatically.

---

## 🛠️ Technical Architecture

This project follows the **Clean Architecture** principles to ensure maintainability and scalability:

1.  **Domain/Core:** Contains Entities, Value Objects, and Repository Interfaces.
2.  **Infrastructure:** Implements external services (Xray Wrapper, Cloudflare API, Telegram Bot, EF Core Repositories).
3.  **Application:** Contains Business Logic, Use Cases (e.g., `StartVpnHandler`), and DTOs.
4.  **Presentation (WPF):** The User Interface layer using CommunityToolkit.Mvvm.

---

## 📋 Prerequisites

Before running or building the application, ensure you have the following installed:

* **OS:** Windows 10 or Windows 11 (64-bit).
* **Runtime:** [.NET Desktop Runtime 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (or higher).
* **Dependencies:** [Visual C++ Redistributable x64](https://aka.ms/vs/17/release/vc_redist.x64.exe) (Required for Xray and Cloudflared).

---

## 🚀 Installation & Setup (For Developers)

### 1. Clone the Repository
```bash
git clone [https://github.com/your-username/NetworkManager.git](https://github.com/your-username/NetworkManager.git)
cd NetworkManager