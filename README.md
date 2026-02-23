# 🔍 AISA — AI Customer Sentiment Analyzer

> Platformă SaaS de monitorizare automată a reputației online pentru afaceri locale.

## 🏗️ Arhitectură

| Componentă      | Tehnologie                                | Port   |
| --------------- | ----------------------------------------- | ------ |
| **Backend API** | .NET 10, Clean Architecture, MediatR CQRS | `5000` |
| **AI Service**  | Python 3.12, FastAPI, DistilBERT          | `8000` |
| **Frontend**    | Blazor WebAssembly, MudBlazor             | `5010` |
| **Database**    | PostgreSQL 16                             | `5432` |

## 🚀 Quick Start

### Cu Docker Compose (recomandat)
```bash
docker-compose up --build
```

### Manual

**1. Baza de date (PostgreSQL):**
```bash
# Creează DB-ul aisa_db cu user aisa_user
```

**2. AI Service:**
```bash
cd src/ai-service
pip install -r requirements.txt
uvicorn app.main:app --reload --port 8000
```

**3. Backend:**
```bash
cd src/backend
dotnet restore AISA.sln
dotnet run --project AISA.API
```

**4. Frontend:**
```bash
cd src/frontend/AISA.Frontend
dotnet run
```

## 📂 Structura Proiectului

```
src/
├── backend/          # .NET 10 Clean Architecture
│   ├── AISA.Domain/          # Entități, Enums, Interfețe
│   ├── AISA.Application/     # MediatR CQRS, DTOs, Validators
│   ├── AISA.Infrastructure/  # EF Core, Repositories, AI Client
│   └── AISA.API/             # Controllers, Swagger, Middleware
├── ai-service/       # Python FastAPI + Transformers
└── frontend/         # Blazor WASM + MudBlazor
```

## 📋 Status Faze

- ✅ **Faza 1**: Fundația — Schelet complet, flux end-to-end
- 🔲 **Faza 2**: Autentificare, Subscriptions, Grafice interactive
- 🔲 **Faza 3**: AI răspunsuri, Export PDF, SonarQube, CI/CD
- 🔲 **Faza 4**: Cloud deployment (Azure)
