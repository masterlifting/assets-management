<!-- @format -->

# ASSETS MANAGEMENT

### Designing

## Overview

This pet project, initiated in April 2018, is dedicated to managing various assets including cash, shares of public companies, crypto assets, and real estate. It represents a modern application version that consolidates all my accrued experience in asset management as well as software development. The backend of the project is developed in C# and .NET 8 and is based on the microservices architecture. The frontend is developed in React and TypeScript.

### Key Features

#### Portfolio

- Manages and records asset transactions.
- Parses transaction reports from providers.
- Offers data access via REST API.
- Calculates summary data.
- Integrates with the Recommendations service for personalized buying or selling advice.

#### Market

- Aggregates data on public companies, currency and cryptocurrency exchange rates, and real estate values.
- Analyzes and processes data to compute coefficients and asset ratings.
- Provides data and ratings through REST API.
- Supplies rating data to the Recommendations service.

#### Recommendations

- Generates buying or selling recommendations by analyzing data from other services.
- Offers insights on public company shares and other assets.
- Accessible via REST API.
- Includes a Telegram bot for price-triggered notifications.

#### Web Client

- Developed as a Single Page Application (SPA) using React.
- Also available as a Progressive Web App (PWA) with Blazor technology in a separate repository.
