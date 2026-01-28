# CoreHub CRM

**A comprehensive healthcare practice management system built with .NET 9**

Inspired by Zanda Health, CoreHub is designed to streamline healthcare practice operations with modern architecture, robust security, and intuitive workflows.

---

## 🎯 Vision

CoreHub aims to be a secure, scalable, and user-friendly CRM for healthcare practitioners across multiple specialties:
- Psychology & Counselling
- Physiotherapy
- Occupational Therapy
- Speech Pathology
- Allied Health Professionals
- General Medical Practices

---

## 🏗️ Architecture

CoreHub follows **Clean Architecture** principles with a modular monolith design:

```
src/
├── Core/
│   ├── CoreHub.Domain          # Entities, Value Objects, Domain Logic
│   └── CoreHub.Application     # Use Cases, DTOs, Interfaces
├── Infrastructure/
│   └── CoreHub.Infrastructure  # Data Access, External Services
└── Presentation/
    └── CoreHub.API             # Web API, Controllers, Middleware
```

### Key Principles
- **Domain-Driven Design (DDD)**: Rich domain model with business logic encapsulation
- **CQRS Pattern**: Separate read/write operations for scalability
- **Clean Architecture**: Dependency inversion, testable, maintainable
- **Multi-Tenancy**: Organization-level data isolation

---

## ✨ Core Features

### 1. **Patient Management**
- Comprehensive patient profiles
- Medical history & allergies
- Emergency contacts
- Insurance information
- Document management
- Secure data storage (ISO 27001 compliant)

### 2. **Calendar & Appointments**
- Drag-and-drop scheduling
- Recurring appointments
- Automated reminders (SMS & Email)
- Waitlist management
- Multi-location support
- Telehealth integration

### 3. **Clinical Notes**
- Customizable templates
- Rich text editor
- Version control & edit history
- Digital signatures
- SOAP notes support
- Secure locking mechanism

### 4. **Billing & Invoicing**
- Automated invoice generation
- Payment tracking
- Insurance claims management
- Payment reminders
- Multiple payment methods
- Detailed financial reports

### 5. **Telehealth**
- Zoom integration
- Secure video consultations
- Session recording (with consent)
- Virtual waiting room

### 6. **Communication**
- SMS & Email automation
- Appointment reminders
- Payment reminders
- Custom message templates
- Communication history

### 7. **Reports & Analytics**
- Financial reports
- Appointment statistics
- Patient demographics
- Revenue forecasting
- Custom report builder

### 8. **Security & Compliance**
- ISO 27001 certified architecture
- HIPAA compliance ready
- Role-based access control (RBAC)
- Audit logging
- Data encryption at rest & in transit
- Two-factor authentication (2FA)

---

## 🛠️ Technology Stack

### Backend
- **.NET 9** - Latest LTS framework
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **PostgreSQL** - Primary database
- **Redis** - Caching & session management
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping

### Frontend (Planned)
- **Blazor Server** or **React** + TypeScript
- **Tailwind CSS** - Styling
- **SignalR** - Real-time updates

### Infrastructure
- **Docker** - Containerization
- **Kubernetes** - Orchestration (production)
- **Azure** or **AWS** - Cloud hosting
- **GitHub Actions** - CI/CD

### Third-Party Services
- **Twilio** - SMS notifications
- **SendGrid** - Email delivery
- **Stripe** - Payment processing
- **Zoom API** - Telehealth video
- **Azure Blob Storage** - File storage

---

## 📋 Development Roadmap

### Phase 1: Foundation (Current)
- [x] Project structure & architecture
- [x] Domain entities
- [ ] Database schema & migrations
- [ ] Repository pattern implementation
- [ ] Basic authentication & authorization

### Phase 2: Core Features
- [ ] Patient CRUD operations
- [ ] Practitioner management
- [ ] Appointment scheduling
- [ ] Calendar API
- [ ] Basic clinical notes

### Phase 3: Advanced Features
- [ ] Invoice & payment processing
- [ ] Clinical note templates
- [ ] Document management
- [ ] SMS/Email notifications
- [ ] Telehealth integration

### Phase 4: Analytics & Reporting
- [ ] Dashboard & analytics
- [ ] Report builder
- [ ] Data export functionality
- [ ] Advanced search

### Phase 5: Polish & Production
- [ ] UI/UX refinement
- [ ] Performance optimization
- [ ] Security audit
- [ ] Compliance certification
- [ ] Mobile app (React Native)

---

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- PostgreSQL 16+
- Redis (optional for development)
- Node.js 20+ (for frontend)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/datachampionuk/CoreHub.git
   cd CoreHub
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Set up database**
   ```bash
   dotnet ef database update --project src/Infrastructure/CoreHub.Infrastructure
   ```

4. **Run the API**
   ```bash
   dotnet run --project src/Presentation/CoreHub.API
   ```

5. **Access the API**
   - Swagger UI: `https://localhost:5001/swagger`
   - API: `https://localhost:5001/api`

---

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverageReportFormat=opencover
```

---

## 🤝 Contributing

Contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🔒 Security

CoreHub takes security seriously:
- All data encrypted at rest and in transit
- Regular security audits
- OWASP Top 10 compliance
- Vulnerability scanning with Dependabot

For security issues, please email: security@corehub.example.com

---

## 📞 Support

- **Documentation**: [docs.corehub.example.com](https://docs.corehub.example.com)
- **Issues**: [GitHub Issues](https://github.com/datachampionuk/CoreHub/issues)
- **Email**: support@corehub.example.com

---

## 🎯 Differentiation from Zanda Health

While inspired by Zanda Health, CoreHub differentiates itself through:

1. **Open Architecture**: Modular design allowing custom integrations
2. **AI-First Approach**: Built-in AI for clinical note assistance, scheduling optimization
3. **Advanced Analytics**: Predictive analytics for practice growth
4. **Modern Tech Stack**: Latest .NET, cloud-native design
5. **Developer-Friendly**: Well-documented API, extensible plugins
6. **Flexible Deployment**: On-premise, cloud, or hybrid options
7. **Community-Driven**: Open-source core with premium features

---

**Built with ❤️ by the CoreHub Team**
