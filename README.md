# Order Management System

ASP.NET Core Web API for order management with JWT authentication, role-based access control, and automated business logic.

## Features

- Customer & order management
- Product catalog with inventory tracking
- JWT authentication with Admin/Customer roles
- Tiered discounts (5% over $100, 10% over $200)
- Invoice generation & email notifications
- RBAC security

## Tech Stack

- ASP.NET Core Web API
- Entity Framework Core + SQL Server
- JWT Bearer Authentication
- BCrypt password hashing
- Swagger documentation

## Project Structure

- **Domain**: Entities, Enums, Interfaces (repository contracts)
- **Persistence**: Data access layer, DbContext, Repository implementations
- **Services**: Business logic layer, Service implementations
- **Services.Abstractions**: Service interfaces
- **Shared**: DTOs, Data Transfer Objects
- **WebAPI**: Controllers, Program.cs, API layer

## Database Location

Database will be created automatically in your local SQL Server instance:
- **Database Name**: `OrderManagementSystemDB`
- **Location**: Local SQL Server (`.` server)
- **Connection**: Windows Authentication (Trusted_Connection=true)
- **Migration**: Run `dotnet ef database update` to create tables

## Quick Setup

1. **Clone & Configure**
```bash
git clone <repo-url>
cd Order_Management_System
```

2. **Create appsettings.json** (excluded from git):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=OrderManagementSystemDB;Trusted_Connection=true;TrustServerCertificate=true"
  },
  "JWT": {
    "SecretKey": "YOUR_32+_CHARACTER_SECRET_KEY_HERE",
    "Issuer": "https://localhost:5001/",
    "Audience": "https://localhost:4200/"
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "From": "your-email@gmail.com"
  }
}
```

3. **Run**
```bash
dotnet ef database update
dotnet run
```

Access: `https://localhost:5001/swagger`

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register user (Admin/Customer)
- `POST /api/auth/login` - Login & get JWT token

### Customers
- `POST /api/customers` - Create customer
- `GET /api/customers/{id}/orders` - Get customer orders 

### Products
- `GET /api/products` - List all products
- `GET /api/products/{id}` - Get product details
- `POST /api/products` - Create product (Admin only)
- `PUT /api/products/{id}` - Update product (Admin only)

### Orders
- `POST /api/orders` - Create order
- `GET /api/orders/{id}` - Get order (own only for customers)
- `GET /api/orders` - List all orders (Admin only)
- `PUT /api/orders/{id}/status` - Update status (Admin only)

### Invoices (Admin only)
- `GET /api/invoices` - List all invoices
- `GET /api/invoices/{id}` - Get invoice details

## Authentication

1. Register: `POST /api/auth/register`
2. Login: `POST /api/auth/login` → Get JWT token
3. Use token: `Authorization: Bearer {token}`

**Password Requirements**: 8+ chars, uppercase, lowercase, digit, special character

## CORS Setup (For Future Frontend Integration)

Uncomment in `Program.cs`:
```csharp
builder.Services.AddCors(Options => {
    Options.AddPolicy("MyPolicy", policy => {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// And in pipeline:
app.UseCors("MyPolicy");
```

## Security Notes

- **Never commit appsettings.json**
- Use strong JWT secret (32+ chars)
- Configure email with app passwords
- Admin/Customer role separation enforced

## Business Logic

- Automatic stock validation
- Tiered discounts applied on order creation
- Invoice auto-generation
- Email notifications on status changes
- Multiple payment methods (Credit Card, PayPal)
