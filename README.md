# Assets Management System

[![.NET](https://img.shields.io/badge/.NET-6%2F7%2F8-purple)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18.2-blue)](https://reactjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-4.9-blue)](https://www.typescriptlang.org/)
[![Docker](https://img.shields.io/badge/Docker-Containerized-blue)](https://www.docker.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](./LICENSE)

A comprehensive asset management platform built with modern microservices architecture, designed to manage various financial assets including stocks, cryptocurrencies, cash, and real estate.

## 🚀 Overview

This project, initiated in April 2018, represents a modern application that consolidates years of experience in both asset management and software development. The platform features a robust backend built with C#/.NET and a responsive frontend developed with React and TypeScript.

### 🏗️ Architecture

The system follows **Clean Architecture** principles with a **microservices** approach:

- **Domain-Driven Design (DDD)** with clear separation of concerns
- **Repository Pattern** for data access abstraction
- **Dependency Injection** for loose coupling
- **CQRS-like** patterns for read/write operations
- **Event-driven architecture** with message queues

## 🎯 Key Features

### 📊 Portfolio Service
- **Transaction Management**: Record and track all asset transactions
- **Report Parsing**: Automated parsing of broker reports (BCS, Raiffeisen)
- **Data Analytics**: Calculate portfolio performance and metrics
- **REST API**: Comprehensive API for portfolio data access
- **Excel Integration**: Import/export capabilities for transaction data

### 📈 Market Service
- **Data Aggregation**: Real-time market data from multiple sources
  - Moscow Exchange (MOEX)
  - TD Ameritrade
  - Investing.com
- **Asset Analysis**: Calculate coefficients and ratings
- **Multi-Asset Support**: Stocks, currencies, cryptocurrencies, real estate
- **Historical Data**: Price tracking and trend analysis

### 🤖 Recommendations Service
- **AI-Powered Insights**: Generate buy/sell recommendations
- **Risk Assessment**: Analyze investment opportunities
- **Portfolio Optimization**: Suggest rebalancing strategies
- **Telegram Bot**: Real-time notifications and price alerts
- **Custom Alerts**: Price-triggered notifications

### 🌐 Web Client
- **React SPA**: Modern single-page application
- **TypeScript**: Type-safe development
- **Progressive Web App (PWA)**: Mobile-friendly experience
- **Real-time Updates**: Live portfolio and market data
- **Responsive Design**: Works on all device types

## 🛠️ Technology Stack

### Backend
- **Framework**: .NET 6/7/8
- **Language**: C# with nullable reference types
- **Architecture**: Microservices with Clean Architecture
- **Databases**: 
  - PostgreSQL (transactional data)
  - MongoDB (document storage)
- **Message Queue**: RabbitMQ
- **Containerization**: Docker & Docker Compose
- **HTTP Clients**: Polly for resilience (retry, circuit breaker)

### Frontend
- **Framework**: React 18.2
- **Language**: TypeScript 4.9
- **Build Tool**: Create React App
- **Testing**: Jest + React Testing Library

### Infrastructure
- **Deployment**: Docker containers
- **Monitoring**: Seq for logging
- **Environment**: Development/Production configurations
- **CI/CD**: Docker-based deployment pipeline

## 📁 Project Structure

```
src/
├── backend/
│   ├── hosts/vps/portfolio/          # Deployment hosts
│   │   ├── AM.Portfolio.Api/         # REST API host
│   │   └── AM.Portfolio.Worker/      # Background worker
│   └── modules/
│       ├── portfolio/                # Portfolio microservice
│       │   ├── AM.Portfolio.Core/    # Domain logic
│       │   └── AM.Portfolio.Infrastructure/ # Data access
│       ├── market/                   # Market data service
│       ├── recommendation/           # Recommendation engine
│       └── shared/                   # Shared libraries
└── frontend/
    └── web/react/assets_management/  # React application
```

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 16+](https://nodejs.org/)
- [Docker](https://www.docker.com/get-started)
- [PostgreSQL](https://www.postgresql.org/)
- [MongoDB](https://www.mongodb.com/)

### Quick Start with Docker

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/assets-management.git
   cd assets-management
   ```

2. **Set up environment variables**
   ```bash
   cp .env.example .env
   # Edit .env with your configuration
   ```

3. **Start services with Docker Compose**
   ```bash
   cd src/backend/modules/portfolio
   docker-compose up -d
   ```

4. **Run the frontend**
   ```bash
   cd src/frontend/web/react/assets_management
   npm install
   npm start
   ```

### Manual Development Setup

1. **Backend Services**
   ```bash
   cd src/backend/modules/portfolio
   dotnet restore
   dotnet run --project AM.Portfolio.Api
   ```

2. **Frontend Application**
   ```bash
   cd src/frontend/web/react/assets_management
   npm install
   npm start
   ```

## 📊 API Documentation

The REST APIs provide comprehensive access to:
- Portfolio management endpoints
- Market data retrieval
- Investment recommendations
- User account management

**Base URLs:**
- Portfolio API: `http://localhost:8090`
- Market API: `http://localhost:8091`
- Recommendations API: `http://localhost:8092`

## 🧪 Testing

```bash
# Backend tests
dotnet test

# Frontend tests
cd src/frontend/web/react/assets_management
npm test
```

## 🚢 Deployment

The application supports containerized deployment:

```bash
# Build and deploy
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📈 Roadmap

- [ ] Mobile application (React Native)
- [ ] Advanced ML-based recommendations
- [ ] Options and derivatives trading support
- [ ] Multi-language support
- [ ] Advanced charting and technical analysis
- [ ] Social trading features
- [ ] API rate limiting and caching
- [ ] Comprehensive test coverage

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Financial data providers: MOEX, TD Ameritrade, Investing.com
- Open source community for excellent libraries and tools
- Contributors and testers who helped improve the platform

## 📞 Contact

For questions, suggestions, or collaboration opportunities, please open an issue on GitHub.

---

**Note**: This is a personal project developed for educational and portfolio demonstration purposes. Please consult with financial advisors before making investment decisions.
