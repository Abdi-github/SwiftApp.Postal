# SwiftApp.Postal — Additional Features & Roadmap

> Features not yet implemented. Ordered by business value and implementation dependency.
> Ordered by business value and implementation dependency.

---

## Table of Contents

1. [Phase A — Operational Data & Realism](#1-phase-a--operational-data--realism)
2. [Phase B — Core Business Features](#2-phase-b--core-business-features)
3. [Phase C — UI/UX Enhancements](#3-phase-c--uiux-enhancements)
4. [Phase D — Real-Time & Communication](#4-phase-d--real-time--communication)
5. [Phase E — Reporting & Analytics](#5-phase-e--reporting--analytics)
6. [Phase F — Advanced Features](#6-phase-f--advanced-features)
7. [Phase G — DevOps & Production Readiness](#7-phase-g--devops--production-readiness)
8. [Phase H — Full Localization](#8-phase-h--full-localization)
9. [Implementation Priority Matrix](#9-implementation-priority-matrix)

---

## 1. Phase A — Operational Data & Realism

### A1. Rich Seed Data Migration
**What**: New EF migration seeding customers, parcels (all statuses), tracking histories, delivery routes/slots, pickup requests, notifications, audit logs
**Why**: The platform currently starts empty — no way to demo the system or test UI with realistic data
**Reference**: Java `la-poste-modular` has V3-V6 Flyway migrations with complete operational data
**Effort**: Medium
**Dependencies**: Fix data type issues first (double→decimal)

### A2. Demo Data Reset Command
**What**: A CLI command or admin endpoint that resets the database to the seeded state
**Why**: Useful for demos, testing, and development iterations
**Implementation**: `dotnet ef database update 0 && dotnet ef database update` or a custom `IHostedService`
**Effort**: Small

### A3. Bulk Address Import (Swiss PTT Database)
**What**: Import full Swiss address database from PTT (Post, Telephone, Telegraph) open data
**Why**: Current 20 addresses are insufficient for real address validation
**Source**: Swiss Post publishes ~4,000 ZIP codes as open data
**Implementation**: CSV import as a separate migration or admin upload page
**Effort**: Medium

---

## 2. Phase B — Core Business Features

### B1. Parcel Status Machine (Full Implementation)
**What**: Enforce valid status transitions with a state machine pattern
**Why**: Currently any status can be set to any other status — no business rule enforcement
**Implementation**:
```csharp
public static class ParcelStatusMachine
{
    private static readonly Dictionary<ParcelStatus, ParcelStatus[]> _transitions = new()
    {
        [ParcelStatus.Created] = [ParcelStatus.PickedUp, ParcelStatus.Cancelled],
        [ParcelStatus.PickedUp] = [ParcelStatus.InTransit],
        [ParcelStatus.InTransit] = [ParcelStatus.OutForDelivery, ParcelStatus.Returned],
        [ParcelStatus.OutForDelivery] = [ParcelStatus.Delivered, ParcelStatus.DeliveryFailed],
        [ParcelStatus.DeliveryFailed] = [ParcelStatus.OutForDelivery, ParcelStatus.Returned],
    };

    public static bool CanTransition(ParcelStatus from, ParcelStatus to)
        => _transitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
}
```
**Effort**: Small

### B2. Parcel Label Generation (PDF)
**What**: Generate a Swiss Post-style shipping label with QR/barcode
**Why**: Core postal function — every parcel needs a printable label
**Implementation**: Use `QuestPDF` or `iText7` to generate PDF with:
- Tracking number as Code128 barcode (or DataMatrix)
- Sender + recipient addresses
- Service type badge
- Weight and dimensions
- Branch code
**Endpoint**: `GET /api/v1/parcels/{id}/label` → returns PDF
**Blazor**: "Print Label" button on ParcelDetail page
**Effort**: Medium

### B3. Estimated Delivery Calculation
**What**: Calculate and display estimated delivery date based on service type and origin/destination
**Why**: `TrackingRecord.EstimatedDelivery` field exists but is never populated
**Business Rules**:
| Service Type | Business Days |
|-------------|--------------|
| Express | 1 |
| Priority | 1-2 |
| Standard | 2-3 |
| Registered | 2-3 |
| Bulky | 3-5 |
**Implementation**: Service method considering Swiss public holidays + weekends
**Effort**: Small-Medium

### B4. Delivery Route Optimization
**What**: Auto-assign parcels to routes based on destination address ZIP codes and branch proximity
**Why**: Currently manual slot assignment only
**Implementation**: Simple ZIP code range mapping per route, with manual override
**Effort**: Medium

### B5. Pickup Request Workflow
**What**: Full lifecycle management of customer pickup requests
**Why**: Currently can only create — no confirm, assign to driver, mark as picked up
**Endpoints needed**:
- `PATCH /api/v1/deliveries/pickups/{id}/confirm` — BranchManager
- `PATCH /api/v1/deliveries/pickups/{id}/assign` — Assign to employee + route
- `PATCH /api/v1/deliveries/pickups/{id}/complete` — Mark picked up
- `PATCH /api/v1/deliveries/pickups/{id}/cancel` — Cancel request
**Effort**: Medium

### B6. Parcel Price Calculator (Public API)
**What**: Public endpoint to calculate shipping price before booking
**Why**: Customers need to know the price before creating a parcel
**Endpoint**: `POST /api/v1/parcels/calculate-price` [AllowAnonymous]
**Input**: Weight, dimensions, service type, origin ZIP, destination ZIP
**Output**: Price in CHF, estimated delivery date, available service types
**Effort**: Small

### B7. Parcel Batch Import
**What**: Upload CSV/Excel file to create multiple parcels at once
**Why**: Business customers ship hundreds of parcels daily
**Implementation**: File upload endpoint + background processing with Quartz
**Effort**: Medium-Large

---

## 3. Phase C — UI/UX Enhancements

### C1. Enhanced DataTables
**What**: Server-side sorting, filtering, and search for all list pages
**Why**: Current tables only have pagination — no sorting or filtering
**Implementation**:
1. Add to `PagedResult<T>` request: `SortBy`, `SortDirection`, `SearchTerm`, `Filters`
2. Service layer applies sorting/filtering to EF query
3. Reusable `<SortableHeader>` Blazor component
4. Search bar component with debounce
5. Column-specific filters (dropdowns for enums, date ranges)
**Effort**: Medium (reusable component, then apply to all pages)

### C2. Dashboard with Charts
**What**: Replace stat cards with interactive charts and real-time data
**Why**: Admin dashboard should show trends, not just counts
**Charts**:
- Parcels by status (doughnut chart)
- Daily parcel volume (line chart, 30-day trend)
- Deliveries by branch (bar chart)
- Top routes by volume (horizontal bar)
- Failed delivery rate (KPI gauge)
**Library**: Chart.js via JS interop, or Blazor-native (e.g., `Radzen.Blazor` or `MudBlazor`)
**Effort**: Medium

### C3. Parcel Timeline View
**What**: Visual timeline showing parcel journey from creation to delivery
**Why**: Tracking page should show a visual representation, not just a table
**Implementation**: Vertical timeline component with tracking events, timestamps, branch names, and status indicators
**Effort**: Small-Medium

### C4. Branch Map View
**What**: Map showing all branches with clickable pins
**Why**: Visual branch management for admin/managers
**Implementation**: Leaflet.js (open-source) via JS interop, using branch Lat/Lng
**Effort**: Medium

### C5. Multi-Select Bulk Actions
**What**: Select multiple parcels/rows and apply actions (cancel, export, assign to route)
**Why**: Operational efficiency — doing one action at a time is slow
**Implementation**: Checkbox column + bulk action toolbar
**Effort**: Medium

### C6. Notification Bell (Real-Time)
**What**: Bell icon in topbar header with unread count badge, dropdown showing recent notifications
**Why**: SignalR hub exists, `NotificationHub` has `UpdateUnreadCount` — just needs UI
**Implementation**:
1. Blazor component that connects to SignalR hub `/hubs/notifications`
2. Badge counter updates in real-time
3. Dropdown shows latest 5 in-app notifications
4. Click → navigate to `/app/inbox`
**Effort**: Medium

### C7. User Profile / Settings Page
**What**: Page where employees can view their profile, change locale, manage notification preferences
**Route**: `/app/profile`
**Shows**: Name, email, role, branch, hire date, preferred locale selector
**Effort**: Small

### C8. Dark Mode
**What**: Toggle between light and dark themes
**Why**: UI polish, developer preference
**Implementation**: CSS custom properties + toggle in topbar, preference stored in cookie/localStorage
**Effort**: Small-Medium

---

## 4. Phase D — Real-Time & Communication

### D1. Real-Time Parcel Status Updates
**What**: When a parcel status changes, all connected users seeing that parcel get an update
**Why**: Operational awareness — branch employees need real-time status
**Implementation**: `ParcelStatusChangedEventHandler` already publishes to SignalR — just needs Blazor client integration
**Effort**: Small

### D2. SMS Notifications (via Twilio or similar)
**What**: Send SMS for delivery notifications
**Why**: `NotificationType.Sms` enum exists but is unimplemented
**Implementation**: Add `SmsService` using Twilio/Vonage, configure in appsettings
**Effort**: Medium

### D3. Webhook Support for Business Customers
**What**: HTTP webhook callbacks when parcel status changes
**Why**: B2B integration — business customers want automated status updates in their systems
**Implementation**: Webhook registration endpoint, event-driven HTTP calls with retry logic
**Effort**: Medium-Large

### D4. Real-Time Route Tracking
**What**: Live map showing delivery driver's current location on their route
**Why**: Operations management visibility
**Implementation**: Mobile app/API sends GPS coordinates, displayed on Leaflet map
**Effort**: Large

---

## 5. Phase E — Reporting & Analytics

### E1. Parcel Report (Export to CSV/Excel)
**What**: Export filtered parcel data to CSV/Excel
**Why**: Branch managers need reports for operational review
**Implementation**: `GET /api/v1/reports/parcels?from=...&to=...` → CSV download + Blazor "Export" button
**Library**: `ClosedXML` for Excel, or simple CSV generation
**Effort**: Small-Medium

### E2. Branch Performance Report
**What**: Report showing per-branch metrics: parcels processed, delivery success rate, average delivery time
**Why**: HQ needs to compare branch performance
**Implementation**: SQL aggregate queries → DTO response → Blazor report page
**Effort**: Medium

### E3. Delivery Success Rate Dashboard
**What**: Calculate and visualize: successful vs. failed deliveries, common failure reasons, trends
**Why**: Key operational KPI
**Effort**: Small-Medium

### E4. Audit Log Viewer
**What**: Admin page to view all audit log entries with filtering
**Why**: `AuditLog` entity and `AuditService` exist — data is captured but not viewable
**Route**: `/app/audit-logs` (ADMIN only)
**Implementation**: Service + controller + Blazor page with date range and entity type filters
**Effort**: Small-Medium

### E5. Financial Summary Report
**What**: Revenue by service type, branch, date range
**Why**: Business value tracking — how much revenue per branch/period
**Data**: Aggregate `Parcel.Price` with filters
**Effort**: Medium

---

## 6. Phase F — Advanced Features

### F1. Customer Self-Service Portal
**What**: Separate Blazor area (or page group) where customers can:
- Track their parcels
- Request pickups
- View delivery history
- Manage notification preferences
**Why**: Current WebApp is admin/employee only — customers can only use the public tracking page
**Auth**: Keycloak CUSTOMER role (new) → dedicated customer pages
**Effort**: Large

### F2. Barcode/QR Scanning
**What**: Use device camera to scan parcel barcodes for quick status updates
**Why**: Real postal workers scan barcodes at each checkpoint
**Implementation**: JavaScript barcode scanner library (e.g., `QuaggaJS` or `html5-qrcode`) via JS interop
**Effort**: Medium

### F3. Multi-Tenant Support
**What**: Support multiple postal companies or regions with data isolation
**Why**: SaaS model — separate tenants share infrastructure but isolated data
**Implementation**: Tenant ID column + global query filter in AppDbContext
**Effort**: Large

### F4. Rate Limiting & API Keys
**What**: Rate limiting for public tracking API + API key authentication for B2B partners
**Why**: Public API without rate limiting is a DoS risk
**Implementation**: `AspNetCoreRateLimit` package or .NET 8+ `RateLimiter` middleware
**Effort**: Small-Medium

### F5. Customs Declaration (International Parcels)
**What**: Customs form data for international shipments
**Why**: Switzerland is not in the EU — needs customs declarations for cross-border parcels
**Implementation**: New entity `CustomsDeclaration` with contents, value, HS codes
**Effort**: Medium

### F6. Insurance Claims
**What**: Track insurance claims for damaged/lost insured parcels
**Why**: `Insured` service type implies liability — needs claims management
**Effort**: Medium-Large

---

## 7. Phase G — DevOps & Production Readiness

### G1. Integration Tests with Testcontainers
**What**: Repository tests against real PostgreSQL in Docker
**Why**: Package is already referenced — just needs test classes
**Implementation**:
```csharp
public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithDatabase("test_postal")
        .Build();

    public string ConnectionString => _container.GetConnectionString();
    public Task InitializeAsync() => _container.StartAsync();
    public Task DisposeAsync() => _container.DisposeAsync().AsTask();
}
```
**Effort**: Medium

### G2. API Endpoint Tests
**What**: Full HTTP pipeline tests with `WebApplicationFactory<Program>`
**Why**: Verify auth, routing, serialization, validation end-to-end
**Effort**: Medium

### G3. CI/CD Pipeline
**What**: GitHub Actions workflow: build → test → Docker build → push to registry
**Stages**: `dotnet build` → `dotnet test` → `docker build` → push to GHCR
**Includes**: EF migration bundle generation, health check verification
**Effort**: Medium

### G4. Structured Health Checks
**What**: Expand health checks beyond Postgres/Redis
**Add**: Keycloak connectivity, MailKit SMTP, Quartz scheduler status, disk space
**Endpoint**: `/health/ready` (readiness) + `/health/live` (liveness)
**Effort**: Small

### G5. OpenTelemetry Integration
**What**: Distributed tracing and metrics
**Why**: Serilog covers logging — add tracing for request flow visualization
**Export to**: Seq (already running) or Jaeger/Zipkin
**Effort**: Medium

### G6. Database Backup & Restore
**What**: Automated PostgreSQL backup script with retention policy
**Implementation**: Cron job with `pg_dump`, store in S3 or local volume
**Effort**: Small

### G7. SSL/TLS Configuration
**What**: HTTPS for all services in production
**Implementation**: Let's Encrypt via Nginx reverse proxy, or Traefik
**Effort**: Small-Medium

---

## 8. Phase H — Full Localization

### H1. Resource Files for All Modules
**What**: `.resx` files for de-CH, fr-CH, it-CH, en for every module
**Files needed per module**: 4 `.resx` files × 7 modules = 28 resource files
**Content**: All UI strings, validation messages, email templates
**Effort**: Large

### H2. Blazor Page Localization
**What**: Replace all hardcoded English strings with `@L["key"]` localizer calls
**Scope**: 27 Blazor pages + shared components + layout
**Effort**: Large

### H3. Culture Switcher UI
**What**: Language dropdown in topbar (🇩🇪 🇫🇷 🇮🇹 🇬🇧) that sets `postal-locale` cookie
**Implementation**: Blazor component with `NavigationManager` redirect + `?culture=` param
**Effort**: Small

### H4. API Error Message Localization
**What**: Validation errors and ProblemDetails messages in the user's locale
**Implementation**: `IStringLocalizer` in validators and exception middleware
**Effort**: Medium

### H5. Date/Currency Formatting by Locale
**What**: Display dates as `dd.MM.yyyy` (Swiss), currency as `CHF 12.50`
**Implementation**: `CultureInfo` configuration per locale
**Effort**: Small

---

## 9. Implementation Priority Matrix

### 🔴 Immediate (Before Next Demo)
| ID | Feature | Effort | Why Now |
|----|---------|--------|---------|
| A1 | Rich seed data | Medium | Platform looks empty without data |
| B1 | Parcel status machine | Small | Core business logic should enforce rules |
| C1 | Enhanced DataTables | Medium | Tables need sorting/search to be usable |

### 🟠 Short-Term (Next Sprint)
| ID | Feature | Effort | Business Value |
|----|---------|--------|---------------|
| B2 | PDF label generation | Medium | Core postal function |
| B3 | Estimated delivery | Small | TrackingRecord field already exists |
| B5 | Pickup request workflow | Medium | Currently incomplete feature |
| C2 | Dashboard charts | Medium | Visual impact for stakeholders |
| C3 | Parcel timeline | Small-Med | Better UX for tracking |
| C6 | Notification bell | Medium | SignalR hub ready, just needs UI |
| E4 | Audit log viewer | Small-Med | Data already being captured |

### 🟡 Medium-Term (Next Month)
| ID | Feature | Effort | Business Value |
|----|---------|--------|---------------|
| B4 | Route optimization | Medium | Operational efficiency |
| B6 | Public price calculator | Small | Customer-facing value |
| C4 | Branch map view | Medium | Visual management |
| D1 | Real-time status updates | Small | SignalR already configured |
| E1 | CSV/Excel export | Small-Med | Reporting basics |
| E2 | Branch performance report | Medium | Management insight |
| G1 | Integration tests | Medium | Quality gate |
| G2 | API endpoint tests | Medium | Quality gate |
| G3 | CI/CD pipeline | Medium | DevOps foundation |
| H3 | Culture switcher | Small | i18n foundation |

### 🟢 Long-Term (Backlog)
| ID | Feature | Effort | Notes |
|----|---------|--------|-------|
| B7 | Batch parcel import | Med-Large | B2B feature |
| C8 | Dark mode | Small-Med | Polish |
| D2 | SMS notifications | Medium | Need Twilio account |
| D3 | Webhooks | Med-Large | B2B integration |
| F1 | Customer portal | Large | Separate frontend concern |
| F2 | Barcode scanning | Medium | Mobile-oriented |
| F4 | Rate limiting | Small-Med | Security hardening |
| F5 | Customs declaration | Medium | International scope |
| G5 | OpenTelemetry | Medium | Observability |
| H1-H4 | Full localization | Large | 28+ resource files |

---

## Quick Reference — Java Feature Parity Check

| Java Feature | .NET Status | Gap |
|-------------|------------|-----|
| Parcel CRUD + status machine | ⚠️ CRUD yes, machine no | Need state machine |
| Tracking timeline with full history | ✅ Implemented | Need seed data |
| Delivery routes + slots + attempts | ⚠️ Routes yes, slots/attempts no API | Need endpoints |
| Pickup requests (full lifecycle) | ⚠️ Create only | Need status transitions |
| Email (MailKit + Scriban) | ✅ Implemented | Working |
| In-app notifications (SignalR) | ⚠️ Backend yes, UI no | Need bell + inbox wiring |
| Seed data (V3-V6: parcels, tracking, routes) | ❌ Not ported | Major gap |
| Branch translations (4 locales) | ✅ Implemented | Working |
| Customer management | ✅ Implemented | Need seed data |

---

*End of Additional Features Document*
