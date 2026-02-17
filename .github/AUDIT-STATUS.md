# SwiftApp.Postal — Project Audit & Status Report

> **Generated**: 2026-04-12
> **Last Verified**: All containers healthy, RBAC working, 3 Keycloak users tested

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Phase 0 — Foundation (SharedKernel)](#2-phase-0--foundation-sharedkernel)
3. [Phase 1 — Auth Module](#3-phase-1--auth-module)
4. [Phase 2 — Branch Module](#4-phase-2--branch-module)
5. [Phase 3 — Address Module](#5-phase-3--address-module)
6. [Phase 4 — Parcel Module](#6-phase-4--parcel-module)
7. [Phase 5 — Delivery Module](#7-phase-5--delivery-module)
8. [Phase 6 — Tracking Module](#8-phase-6--tracking-module)
9. [Phase 7 — Notification Module](#9-phase-7--notification-module)
10. [Phase 8 — WebApi Host](#10-phase-8--webapi-host)
11. [Phase 9 — WebApp Host (Blazor SSR)](#11-phase-9--webapp-host-blazor-ssr)
12. [Phase 10 — Docker & Infrastructure](#12-phase-10--docker--infrastructure)
13. [Phase 11 — Testing](#13-phase-11--testing)
14. [Phase 12 — Seed Data](#14-phase-12--seed-data)
15. [Cross-Cutting Issues](#15-cross-cutting-issues)
16. [Auth/OIDC Configuration (Working State)](#16-authoidc-configuration-working-state)

---

## 1. Executive Summary

| Area | Status | Completeness |
|------|--------|-------------|
| SharedKernel | ✅ Complete | 95% — minor cleanup needed |
| Auth Module | ✅ Working | 85% — Role/Permission CRUD missing |
| Branch Module | ✅ Working | 90% — Lat/Lng type issue |
| Address Module | ✅ Working | 70% — no update/delete, no events |
| Parcel Module | ✅ Working | 75% — type mismatches vs spec |
| Delivery Module | ✅ Working | 70% — slot/attempt APIs missing |
| Tracking Module | ✅ Working | 75% — FK types, no estimated delivery |
| Notification Module | ✅ Working | 60% — many unused entities/features |
| WebApi Host | ✅ Working | 85% — event handlers done, SignalR done |
| WebApp (Blazor) | ✅ Working | 80% — 27 pages built, RBAC sidebar done |
| Docker/Infra | ✅ Working | 80% — prod compose needs Keycloak update |
| Tests | ✅ Passing | 40% — ~63 unit tests, no integration/e2e |
| Seed Data | ⚠️ Partial | 30% — reference only, no operational data |
| EF Migrations | ✅ Working | 3 migrations applied |

**Overall: ~70% complete for a functional MVP**

---

## 2. Phase 0 — Foundation (SharedKernel)

### Status: ✅ COMPLETE

### Files
```
SwiftApp.Postal.SharedKernel/
├── Domain/
│   ├── AuditLog.cs              ← Standalone entity (not BaseEntity)
│   ├── BaseEntity.cs            ← Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, DeletedAt, Version(xmin), IsDeleted
│   ├── BaseTranslation.cs       ← Id, Locale, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Version(xmin)
│   ├── PagedResult.cs           ← sealed record PagedResult<T>(IReadOnlyList<T>, int, int, long, int)
│   ├── SupportedLocales.cs      ← De, Fr, It, En; Normalize("de-CH"→"de")
│   ├── Entities/                ← (empty — intentional)
│   └── Enums/                   ← (empty — intentional)
├── Events/
│   └── IDomainEvent.cs          ← interface IDomainEvent : INotification
├── Exceptions/
│   ├── BusinessRuleException.cs ← sealed, .Rule property
│   ├── ConcurrencyException.cs  ← sealed, .EntityName + .EntityId
│   └── EntityNotFoundException.cs ← sealed, .EntityName + .EntityId
├── Interfaces/
│   ├── ICacheService.cs         ← GetAsync, SetAsync, RemoveAsync, RemoveByPrefixAsync, GetOrSetAsync
│   ├── ICurrentUserService.cs   ← UserId, Username, IsAuthenticated, Roles, IsInRole()
│   └── IModuleInstaller.cs      ← void Install(IServiceCollection, IConfiguration)
├── Persistence/
│   ├── AppDbContext.cs           ← ConfigurationAssemblies, ApplyAuditFields(), soft-delete global filter
│   ├── BaseEntityConfiguration.cs ← ValueGeneratedNever, xmin, snake_case columns
│   └── BaseTranslationConfiguration.cs ← Same pattern for translations
└── Services/
    ├── AuditService.cs           ← LogAsync(entityType, entityId, action, performedBy, details?)
    ├── RedisCacheService.cs      ← ICacheService impl, graceful fallback, camelCase JSON
    └── TranslationResolver.cs    ← Resolve<T>(translations, locale) with de fallback
```

### Issues Found
| # | Severity | Issue |
|---|----------|-------|
| 1 | Low | `RemoveByPrefixAsync` in `RedisCacheService` is a no-op stub — logs debug and returns |
| 2 | Low | `AuditService` has no `IAuditService` interface — not easily testable |
| 3 | Info | `ICacheService.GetOrSetAsync<T>` missing `where T : class` constraint (inconsistent with `GetAsync<T>`) |
| 4 | Info | Empty `Domain/Entities/` and `Domain/Enums/` folders — can be removed |

---

## 3. Phase 1 — Auth Module

### Status: ✅ WORKING (85%)

### Entities
| Entity | Key Properties |
|--------|---------------|
| `Employee : BaseEntity` | EmployeeNumber, FirstName, LastName, Email, Phone?, Role(enum), Status(enum), AssignedBranchId?, HireDate(DateOnly), PreferredLocale, KeycloakUserId? |
| `Customer : BaseEntity` | CustomerNumber, FirstName, LastName, Email, Phone?, Status(enum), PreferredLocale, KeycloakUserId? |
| `Role : BaseEntity` | Name, Description?, Permissions(ICollection) |
| `Permission : BaseEntity` | Name, Description?, Roles(ICollection) |

### Enums
- `EmployeeRole`: Admin, BranchManager, Employee
- `EmployeeStatus`: Active, OnLeave, Terminated
- `CustomerStatus`: Active, Inactive, Suspended

### Services Available
- `EmployeeService`: GetPaged, GetById, Create, Update, Delete, SyncKeycloakUser
- `CustomerService`: GetPaged, GetById, Create, Update, Delete, SyncKeycloakUser

### API Endpoints
| Method | Route | Auth |
|--------|-------|------|
| GET | `/api/v1/employees` | ADMIN, BRANCH_MANAGER |
| GET | `/api/v1/employees/{id}` | ADMIN, BRANCH_MANAGER |
| POST | `/api/v1/employees` | ADMIN |
| PUT | `/api/v1/employees/{id}` | ADMIN, BRANCH_MANAGER |
| DELETE | `/api/v1/employees/{id}` | ADMIN |
| POST | `/api/v1/employees/sync` | ⚠️ No role restriction! |
| GET | `/api/v1/customers` | ADMIN, BRANCH_MANAGER |
| GET | `/api/v1/customers/{id}` | Authorized |
| POST | `/api/v1/customers` | Authorized |
| PUT | `/api/v1/customers/{id}` | ADMIN, BRANCH_MANAGER |
| DELETE | `/api/v1/customers/{id}` | ADMIN |
| POST | `/api/v1/customers/sync` | ⚠️ No role restriction! |

### Issues
| # | Severity | Issue |
|---|----------|-------|
| 1 | **HIGH** | `/sync` endpoints have no role restriction — should be ADMIN only |
| 2 | Medium | Role/Permission entities exist but have NO service, controller, or DTOs — no CRUD |
| 3 | Low | `EmployeeService.CreateAsync` uses `DateTime.UtcNow` — should be `DateTimeOffset.UtcNow` |
| 4 | Info | `Application/Mappings/` folder empty — Mapster not configured |

---

## 4. Phase 2 — Branch Module

### Status: ✅ WORKING (90%)

### Entities
| Entity | Key Properties |
|--------|---------------|
| `Branch : BaseEntity` | BranchCode, Type(enum), Status(enum), Street, ZipCode, City, Canton?, Phone?, Email?, Latitude?(double), Longitude?(double), Translations(ICollection) |
| `BranchTranslation : BaseTranslation` | Name, Description?, BranchId, Branch(nav) |

### Enums
- `BranchType`: PostOffice, SortingCenter, DistributionCenter, Agency
- `BranchStatus`: Active, Inactive, UnderConstruction

### API Endpoints
| Method | Route | Auth |
|--------|-------|------|
| GET | `/api/v1/branches` | Authorized |
| GET | `/api/v1/branches/{id}` | Authorized |
| POST | `/api/v1/branches` | ADMIN |
| PUT | `/api/v1/branches/{id}` | ADMIN, BRANCH_MANAGER |
| DELETE | `/api/v1/branches/{id}` | ADMIN |

### Issues
| # | Severity | Issue |
|---|----------|-------|
| 1 | Medium | `Latitude`/`Longitude` are `double` — should be `decimal` for geo precision |
| 2 | Info | Mapster mappings folder empty |

---

## 5. Phase 3 — Address Module

### Status: ✅ WORKING (70%)

### Entities
| Entity | Key Properties |
|--------|---------------|
| `SwissAddress : BaseEntity` | ZipCode, City, Canton(enum), Municipality? |

### Enum
`Canton`: All 26 Swiss cantons (ZH, BE, LU, ... JU)

### API Endpoints
| Method | Route | Auth |
|--------|-------|------|
| GET | `/api/v1/addresses` | AllowAnonymous |
| GET | `/api/v1/addresses/{id}` | AllowAnonymous |
| GET | `/api/v1/addresses/search/zip/{zipCode}` | AllowAnonymous |
| GET | `/api/v1/addresses/search/canton/{canton}` | AllowAnonymous |
| POST | `/api/v1/addresses` | ADMIN |

### Issues
| # | Severity | Issue |
|---|----------|-------|
| 1 | Medium | No UpdateAsync or DeleteAsync in service or controller |
| 2 | Medium | No Domain/Events/ — no domain events for address changes |
| 3 | Low | Repository has UpdateAsync but service doesn't expose it |
| 4 | Low | No MediatR usage in service |

---

## 6. Phase 4 — Parcel Module

### Status: ✅ WORKING (75%)

### Entities
| Entity | Key Properties |
|--------|---------------|
| `Parcel : BaseEntity` | TrackingNumber, Status(enum), Type(enum), WeightKg?(double), LengthCm?(double), WidthCm?(double), HeightCm?(double), Price(decimal), SenderCustomerId?, OriginBranchId?, Sender/Recipient address fields |

### Enums
- `ParcelStatus`: Created, LabelGenerated, PickedUp, InTransit, OutForDelivery, Delivered, Returned, Cancelled
- `ParcelType`: Standard, Express, Registered, Bulky

### Tracking Number Format
Generated as `99.XX.XXX.XXX.XXX.XX` (numeric Swiss Post format)
> ⚠️ copilot-instructions say `CHE-XXXXXXXXXXXXXXX` — mismatch (current format is actually more realistic)

### Price Calculation
| Type | Base Price (CHF) |
|------|-----------------|
| Standard | 7.50 |
| Express | 12.80 |
| Registered | 9.60 |
| Bulky | 16.00 |

### API Endpoints
| Method | Route | Auth |
|--------|-------|------|
| GET | `/api/v1/parcels` | Authorized |
| GET | `/api/v1/parcels/{id}` | Authorized |
| GET | `/api/v1/parcels/tracking/{trackingNumber}` | AllowAnonymous |
| POST | `/api/v1/parcels` | Authorized |
| POST | `/api/v1/parcels/{id}/cancel` | ADMIN, BRANCH_MANAGER |

### Issues
| # | Severity | Issue |
|---|----------|-------|
| 1 | **Medium** | `WeightKg`, `LengthCm`, `WidthCm`, `HeightCm` are `double` — instructions mandate `decimal` |
| 2 | Medium | Status enum differs from instructions — missing ACCEPTED, SORTED, DELIVERY_FAILED |
| 3 | Medium | ParcelType differs — missing PRIORITY, INSURED; has Bulky |
| 4 | Medium | No UpdateAsync endpoint — can only Create and Cancel |
| 5 | Low | `ParcelConfiguration`: Price uses `HasPrecision(19,4)` — should be `HasColumnType("NUMERIC(19,4)")` |
| 6 | Low | `IParcelModuleApi` missing `GetCountByStatusAsync`, `GetRecentAsync` |

---

## 7. Phase 5 — Delivery Module

### Status: ✅ WORKING (70%)

### Entities
| Entity | Key Properties |
|--------|---------------|
| `DeliveryRoute : BaseEntity` | RouteCode, BranchId, AssignedEmployeeId?, Status(enum), Date(DateOnly), Slots(ICollection) |
| `DeliverySlot : BaseEntity` | DeliveryRouteId, TrackingNumber, SequenceOrder(int), Status(enum), RecipientSignature?, Attempts(ICollection) |
| `DeliveryAttempt : BaseEntity` | DeliverySlotId, Result(enum), Notes?, AttemptTimestamp(DateTimeOffset) |
| `PickupRequest : BaseEntity` | CustomerId, PickupStreet/ZipCode/City, PreferredDate(DateOnly), PreferredTimeFrom/To?(TimeOnly), Status(enum) |

### Enums
- `RouteStatus`: Planned, InProgress, Completed, Cancelled
- `SlotStatus`: Pending, Delivered, Failed, Skipped
- `AttemptResult`: Delivered, Absent, Refused, WrongAddress
- `PickupStatus`: Pending, Requested, Confirmed, Assigned, PickedUp, Cancelled

### API Endpoints
| Method | Route | Auth |
|--------|-------|------|
| GET | `/api/v1/deliveries/routes` | Authorized |
| GET | `/api/v1/deliveries/routes/{id}` | Authorized |
| POST | `/api/v1/deliveries/routes` | ADMIN, BRANCH_MANAGER |
| POST | `/api/v1/deliveries/routes/{id}/start` | Authorized |
| POST | `/api/v1/deliveries/routes/{id}/complete` | Authorized |
| DELETE | `/api/v1/deliveries/routes/{id}` | ADMIN |
| GET | `/api/v1/deliveries/pickups` | ⚠️ No auth! |
| GET | `/api/v1/deliveries/pickups/{id}` | ⚠️ No auth! |
| POST | `/api/v1/deliveries/pickups` | ⚠️ No auth! |

### Issues
| # | Severity | Issue |
|---|----------|-------|
| 1 | **HIGH** | `PickupRequestController` missing `[Authorize]` — all endpoints are public! |
| 2 | Medium | No dedicated service for DeliverySlot/DeliveryAttempt — can't record individual delivery attempts |
| 3 | Medium | `CompleteRouteAsync` publishes events for ALL slots even if some failed |
| 4 | Low | PickupRequestService has no Update, Delete, or status transition methods |

---

## 8. Phase 6 — Tracking Module

### Status: ✅ WORKING (75%)

### Entities
| Entity | Key Properties |
|--------|---------------|
| `TrackingRecord : BaseEntity` | TrackingNumber, CurrentStatus, CurrentBranchId?, EstimatedDelivery?(DateTimeOffset) |
| `TrackingEvent : BaseEntity` | TrackingNumber, EventType(enum), BranchId?, Location?, DescriptionKey?, EventTimestamp(DateTimeOffset), ScannedByEmployeeId?(string ⚠️) |

### Enum
`TrackingEventType`: Registered, PickedUp, ArrivedAtSorting, DepartedSorting, ArrivedAtBranch, OutForDelivery, Delivered, DeliveryFailed, Returned

### API Endpoints
| Method | Route | Auth |
|--------|-------|------|
| GET | `/api/v1/tracking/{trackingNumber}` | AllowAnonymous |
| POST | `/api/v1/tracking/{trackingNumber}/events` | Authorized |

### Issues
| # | Severity | Issue |
|---|----------|-------|
| 1 | Medium | `ScannedByEmployeeId` is `string?` — should be `Guid?` for FK to Auth.Employee |
| 2 | Medium | TrackingRecord and TrackingEvent linked only by TrackingNumber string — no FK relationship |
| 3 | Medium | `EstimatedDelivery` is never calculated or set |
| 4 | Low | `RecordEvent` returns `StatusCode(201)` instead of `CreatedAtAction` |
| 5 | Low | No `SoftDeleteAsync` in repository |

---

## 9. Phase 7 — Notification Module

### Status: ✅ WORKING (60%)

### Entities
| Entity | Key Properties |
|--------|---------------|
| `NotificationLog : BaseEntity` | RecipientEmail?, RecipientPhone?, Type(enum), Status(enum), Subject?, Body?, ReferenceId?, EventType?, RetryCount(int) |
| `NotificationTemplate : BaseEntity` | TemplateCode, Type(enum), EventType?, Translations(ICollection) |
| `NotificationTemplateTranslation : BaseTranslation` | NotificationTemplateId, Subject, Body |
| `NotificationPreference : BaseEntity` | CustomerId, EmailEnabled(true), SmsEnabled, InAppEnabled(true), PreferredLocale("de") |
| `InAppNotification : BaseEntity` | TargetEmployeeId?, TargetRole?, TargetBranchId?, Title, Message, Category(enum), ReferenceUrl?, SenderEmployeeId?, ReadReceipts(ICollection) |
| `InAppNotificationRead` (NOT BaseEntity) | Id, InAppNotificationId, EmployeeId, ReadAt |

### Enums
- `NotificationStatus`: Pending, Sent, Failed, Retrying, PermanentlyFailed
- `NotificationType`: Email, Sms, InApp
- `NotificationCategory`: Info, Warning, Urgent, System

### Infrastructure
- **EmailService** (MailKit + Scriban) — template rendering with locale fallback
- **DailyNotificationDigestJob** (Quartz) — daily at 07:00 Europe/Zurich → admin digest
- **RetryFailedNotificationsJob** (Quartz) — every 5 min, retries up to 3 times

### API Endpoints
| Method | Route | Auth |
|--------|-------|------|
| GET | `/api/v1/notifications/logs` | ADMIN |
| GET | `/api/v1/notifications/templates` | ADMIN |

### Issues
| # | Severity | Issue |
|---|----------|-------|
| 1 | **HIGH** | `INotificationHubPusher` not registered in module installer — registered only in WebApi host |
| 2 | Medium | No in-app notification API endpoints (GET for employee, POST, mark-read) |
| 3 | Medium | `NotificationPreference` entity is completely unused — no repo, service, or API |
| 4 | Medium | No validators at all in this module |
| 5 | Medium | `Domain/Events/` folder is empty — no domain events |
| 6 | Low | `InAppNotificationRead` doesn't inherit BaseEntity — no audit fields |
| 7 | Low | `DailyNotificationDigestJob` hardcodes `admin@swiftapp.ch` |
| 8 | Low | Quartz jobs not registered in module installer (registered in host Program.cs) |

---

## 10. Phase 8 — WebApi Host

### Status: ✅ WORKING (85%)

### Files
```
SwiftApp.Postal.WebApi/
├── Program.cs                    ← Full composition root
├── appsettings.json              ← Postgres:5433, Redis:6380, Keycloak:8090, Mailpit:1029
├── Infrastructure/
│   ├── CurrentUserService.cs     ← ICurrentUserService impl (sub, preferred_username, ClaimTypes.Role)
│   └── NotificationHubPusher.cs  ← INotificationHubPusher impl → SignalR IHubContext
│   └── EventHandlers/
│       ├── EmployeeCreatedEventHandler.cs
│       ├── CustomerCreatedEventHandler.cs
│       ├── ParcelCreatedEventHandler.cs
│       ├── ParcelStatusChangedEventHandler.cs
│       └── DeliveryEventHandlers.cs   ← DeliveryCompleted + DeliveryFailed
├── Hubs/
│   ├── INotificationClient.cs    ← ReceiveNotification, UpdateUnreadCount
│   └── NotificationHub.cs        ← [Authorize], user/role groups, RequestUnreadCount
└── Migrations/
    ├── 20260411202548_InitialCreate.cs
    ├── 20260411211528_AddKeycloakUserIdIndexes.cs
    └── 20260411232403_SeedReferenceData.cs
```

### Program.cs Configuration
1. Serilog (Console + Seq)
2. AppDbContext (Npgsql, ConfigurationAssemblies from all 7 modules)
3. All 7 module installers
4. MediatR (all module + WebApi assemblies)
5. Redis distributed cache
6. Quartz (2 notification jobs)
7. SignalR (+`/hubs/notifications`)
8. Controllers (ApplicationParts from all 7 modules)
9. FluentValidation
10. Swagger with Keycloak OAuth2 Authorization Code flow
11. CORS (localhost:5101)
12. ProblemDetails
13. Health checks (Postgres + Redis)
14. Keycloak JWT Bearer auth with `OnTokenValidated` realm role extraction
15. Authorization policies: AdminOnly, ManagerOrAdmin, Authenticated

### Issues
| # | Severity | Issue |
|---|----------|-------|
| 1 | Medium | CORS hardcodes `http://localhost:5101` — needs configuration for prod |
| 2 | Low | No global exception handling middleware to map BusinessRuleException/EntityNotFoundException to ProblemDetails |

---

## 11. Phase 9 — WebApp Host (Blazor SSR)

### Status: ✅ WORKING (80%)

### Pages (27 total)
| Category | Pages | Route(s) | Status |
|----------|-------|----------|--------|
| Dashboard | Dashboard.razor | `/app` | ✅ Working — 7 stat cards |
| Parcels | List, Detail, Form | `/app/parcels`, `/{id}`, `/new` | ✅ Working — no edit route |
| Branches | List, Detail, Form | `/app/branches`, `/{id}`, `/new`, `/{id}/edit` | ✅ Working |
| Employees | List, Detail, Form | `/app/employees`, `/{id}`, `/new`, `/{id}/edit` | ✅ Working |
| Customers | List, Detail, Form | `/app/customers`, `/{id}`, `/new`, `/{id}/edit` | ✅ Working |
| Delivery | List, Detail, Form | `/app/delivery`, `/{id}`, `/new` | ✅ Working |
| Pickups | List, Form | `/app/pickups`, `/new` | ✅ Working |
| Tracking | ScanPage | `/app/tracking` | ✅ Working |
| Addresses | List | `/app/addresses` | ✅ Working |
| Notifications | List, Inbox, Compose | `/app/notifications`, `/inbox`, `/send` | ⚠️ Inbox not wired |
| Public Tracking | Search | `/tracking/{number?}` | ✅ Working (anonymous) |
| Error pages | AccessDenied, Error | `/access-denied`, `/error` | ✅ Working |

### Shared Components
- `ConfirmDialog.razor` — Bootstrap modal with confirm/cancel
- `Pagination.razor` — Page controls (prev/next/page numbers)
- `StatusBadge.razor` — Colored Bootstrap badge by status string

### Layout
- `MainLayout.razor` — Sidebar (260px, navy #1a237e) + topbar + body
  - RBAC sidebar: Operations (all users), Management (ADMIN/MANAGER), System (ADMIN)
  - Topbar: display name from `name` → `preferred_username` → `Identity.Name` claims
- `App.razor` — Root HTML with Bootstrap 5.3 CDN, Bootstrap Icons, app.css
- `Routes.razor` — Blazor router with `CascadingAuthenticationState`

### Infrastructure
- `CurrentUserService.cs` — ICurrentUserService for Blazor (HttpContextAccessor)
- `NullNotificationHubPusher.cs` — No-op INotificationHubPusher (WebApp doesn't push via SignalR)
- `DashboardService.cs` — Aggregates data from all IModuleApi interfaces for dashboard

### Issues
| # | Severity | Issue |
|---|----------|-------|
| 1 | **HIGH** | Sidebar link `/app/deliveries` doesn't match route `/app/delivery` — **broken link** |
| 2 | Medium | No sidebar links for `/app/pickups` or `/app/inbox` |
| 3 | Medium | `NotificationInbox.razor` has `_currentUserId = Guid.Empty` — not wired to auth |
| 4 | Medium | No edit route for Parcels (`/app/parcels/{id}/edit` missing) |
| 5 | Medium | **Zero `IStringLocalizer` usage** — all strings hardcoded in English |
| 6 | Medium | No logout button in the topbar — user can't sign out |
| 7 | Low | CSS loads before Bootstrap CDN in App.razor — overrides may not apply |
| 8 | Info | `wwwroot/js/` and `wwwroot/lib/` are empty |

---

## 12. Phase 10 — Docker & Infrastructure

### Status: ✅ WORKING (80%)

### compose.yaml (Development) — 8 Services
| Service | Image | Host Port | Status |
|---------|-------|-----------|--------|
| postgres | postgres:17 | 5433 | ✅ Healthy |
| pgadmin | dpage/pgadmin4 | 5052 | ✅ |
| mailpit | axllent/mailpit | SMTP:1029, UI:8029 | ✅ |
| seq | datalust/seq | API:5342, UI:8082 | ✅ |
| keycloak | keycloak:26.2 | 8090 | ✅ In-memory DB |
| redis | redis:7-alpine | 6380 | ✅ AOF, 256MB |
| webapi | Dockerfile.dev | 5100 | ✅ Healthy, hot-reload |
| webapp | Dockerfile.dev | 5101 | ✅ Healthy, hot-reload |

### Keycloak Provisioning (`docker/keycloak/provision.sh`)
- Realm: `swiftapp-postal`
- Roles: ADMIN, BRANCH_MANAGER, EMPLOYEE
- Client: `swiftapp-postal-client` (confidential, code+token auth)
- Protocol mapper: realm roles → ID/access tokens
- **10 test users** with real Swiss names

| Username | Role | Real Name |
|----------|------|-----------|
| admin-postal | ADMIN | Hans Müller |
| manager1-postal | BRANCH_MANAGER | Marie Dupont |
| manager2-postal | BRANCH_MANAGER | Luca Rossi |
| manager3-postal | BRANCH_MANAGER | Sophie Meier |
| emp1-postal | EMPLOYEE | Thomas Keller |
| emp2-postal | EMPLOYEE | Anna Fischer |
| emp3-postal | EMPLOYEE | Marco Bianchi |
| emp4-postal | EMPLOYEE | Elena Weber |
| emp5-postal | EMPLOYEE | David Brunner |
| emp6-postal | EMPLOYEE | Laura Schmid |

All passwords: `pass123`

### Issues
| # | Severity | Issue |
|---|----------|-------|
| 1 | **HIGH** | compose.prod.yaml still has `Jwt__SecretKey` env vars — should be Keycloak config |
| 2 | **HIGH** | compose.prod.yaml missing Keycloak service entirely |
| 3 | Medium | Nginx `limit_req_zone` directives inside `server {}` — should be in `http {}` (Nginx won't start) |
| 4 | Low | `.env` has multiple Keycloak URL vars that may confuse |

---

## 13. Phase 11 — Testing

### Status: ⚠️ PARTIAL (40%)

### Test Summary
| Project | Test Class | # Tests | Type |
|---------|-----------|---------|------|
| Architecture.Tests | ModuleBoundaryTests | 7 | Arch (NetArchTest) |
| Architecture.Tests | NamingConventionTests | 5+Theory | Arch (NetArchTest) |
| Auth.Tests | EmployeeServiceTests | 8 | Unit (Moq) |
| Branch.Tests | BranchServiceTests | 5 | Unit (Moq) |
| Address.Tests | SwissAddressServiceTests | 7 | Unit (Moq) |
| Parcel.Tests | ParcelServiceTests | 12 | Unit (Moq) |
| Delivery.Tests | DeliveryRouteServiceTests | 8 | Unit (Moq) |
| Tracking.Tests | TrackingServiceTests | 5 | Unit (Moq) |
| Notification.Tests | NotificationServiceTests | 6 | Unit (Moq) |
| **WebApi.Tests** | *(empty)* | **0** | — |
| **WebApp.Tests** | *(empty)* | **0** | — |
| **TOTAL** | 9 classes | **~63** | |

### What's Missing
1. **CustomerService tests** — Auth module only tests EmployeeService
2. **PickupRequestService tests** — Delivery module only tests DeliveryRouteService
3. **Integration tests** — Testcontainers package referenced but unused
4. **Controller/API tests** — WebApplicationFactory available but no tests
5. **Blazor/bUnit tests** — WebApp.Tests project empty
6. **Validator tests** — FluentValidation rules not tested
7. **Event handler tests** — 5 event handlers in WebApi not tested

---

## 14. Phase 12 — Seed Data

### Current State: ⚠️ REFERENCE DATA ONLY

### What's Seeded (Migration: SeedReferenceData)
| Table | Count | Notes |
|-------|-------|-------|
| roles | 3 | ADMIN, BRANCH_MANAGER, EMPLOYEE |
| permissions | 21 | PARCEL_*, BRANCH_*, DELIVERY_*, TRACKING_*, EMPLOYEE_*, CUSTOMER_* |
| role_permissions | ~45 | Full matrix |
| branches | 5 | Zurich, Bern, Geneva, Basel, Lugano — with 4-locale translations (20 rows) |
| employees | 10 | Linked to Keycloak users via keycloak_user_id |
| swiss_addresses | 20 | Major Swiss cities |
| notification_templates | 3 | PARCEL_CREATED, PARCEL_DELIVERED, DELIVERY_FAILED — with 4-locale translations (12 rows) |

### What's NOT Seeded (vs Java Reference V3-V6)
| Table | Java Seed Count | .NET Status |
|-------|----------------|-------------|
| customers | 5 | ❌ Not seeded |
| parcels | 15 (all statuses) | ❌ Not seeded |
| tracking_records | 15 | ❌ Not seeded |
| tracking_events | ~55 (full lifecycle histories) | ❌ Not seeded |
| delivery_routes | 4 | ❌ Not seeded |
| delivery_slots | 9 | ❌ Not seeded |
| pickup_requests | 10 | ❌ Not seeded |
| notification_preferences | 5 | ❌ Not seeded |
| notification_logs | 12 (historical) | ❌ Not seeded |
| in_app_notifications | 7 (broadcast, role, individual) | ❌ Not seeded |
| in_app_notification_reads | 4 | ❌ Not seeded |
| audit_logs | 0 | ❌ Not seeded |

---

## 15. Cross-Cutting Issues

### CRITICAL (Must Fix)
| # | Module | Issue |
|---|--------|-------|
| 1 | Delivery | `PickupRequestController` missing `[Authorize]` — all endpoints public |
| 2 | Auth | `/sync` endpoints have no role restriction |
| 3 | WebApp | Sidebar link `/app/deliveries` broken — route is `/app/delivery` |
| 4 | Docker | compose.prod.yaml has stale JWT config, missing Keycloak |

### MEDIUM (Should Fix)
| # | Module | Issue |
|---|--------|-------|
| 5 | All | `Application/Mappings/` folders empty everywhere — Mapster never configured |
| 6 | WebApp | No `IStringLocalizer` usage — hardcoded English everywhere |
| 7 | WebApp | No logout button in layout |
| 8 | WebApp | Inbox page `_currentUserId = Guid.Empty` |
| 9 | Parcel | Weight/dimensions are `double` instead of `decimal` |
| 10 | Tracking | `ScannedByEmployeeId` is `string?` instead of `Guid?` |
| 11 | Notification | `NotificationPreference` entity completely unused |
| 12 | WebApi | No global exception middleware for BusinessRuleException/EntityNotFoundException |

---

## 16. Auth/OIDC Configuration (Working State)

### WebApp OIDC (Cookie-Based)
```
Scheme: OpenIdConnect → Cookie (postal.session)
Authority: http://keycloak:8080/realms/swiftapp-postal (internal)
Public Authority: http://127.0.0.1:8090/realms/swiftapp-postal (browser)
ClientId: swiftapp-postal-client
Scopes: openid, profile, email
GetClaimsFromUserInfoEndpoint: false (avoids back-channel issuer mismatch)
ValidIssuers: [internal authority, public authority]
NameClaimType: preferred_username
RoleClaimType: ClaimTypes.Role
Kestrel MaxRequestHeadersTotalSize: 128KB (for large Keycloak cookies)
```

### Role Extraction (OnTokenValidated)
Keycloak puts `realm_access.roles` in the **access token** (not ID token). The `OnTokenValidated` handler:
1. Checks ID token claims for `realm_access` (usually not there)
2. Falls back to decoding `context.TokenEndpointResponse.AccessToken` via `JwtSecurityTokenHandler`
3. Parses JSON `realm_access.roles` array
4. Adds each role as `ClaimTypes.Role` to the `ClaimsIdentity`

### WebApi JWT Bearer
Same realm role extraction from `realm_access` in the JWT token. Swagger configured with Keycloak OAuth2 Authorization Code flow.

### Authorization Policies (Both Hosts)
- `AdminOnly`: RequireRole("ADMIN")
- `ManagerOrAdmin`: RequireRole("ADMIN", "BRANCH_MANAGER")
- `Authenticated`: RequireAuthenticatedUser()

### RBAC Sidebar (MainLayout.razor)
- **OPERATIONS** (Dashboard, Parcels, Tracking, Deliveries): All authenticated users
- **MANAGEMENT** (Branches, Employees, Customers): ADMIN + BRANCH_MANAGER
- **SYSTEM** (Addresses, Notifications): ADMIN only

---

*End of Audit Report*
