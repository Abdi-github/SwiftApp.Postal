# SwiftApp.Postal — Improvements Needed

> Prioritized list of bugs, fixes, and quality improvements.
> Organized by severity: CRITICAL → HIGH → MEDIUM → LOW

---

## Table of Contents

1. [CRITICAL — Security Vulnerabilities](#1-critical--security-vulnerabilities)
2. [HIGH — Functional Bugs](#2-high--functional-bugs)
3. [MEDIUM — Data Type & Schema Issues](#3-medium--data-type--schema-issues)
4. [MEDIUM — Missing Core Functionality](#4-medium--missing-core-functionality)
5. [MEDIUM — UI/UX Issues](#5-medium--uiux-issues)
6. [LOW — Code Quality & Consistency](#6-low--code-quality--consistency)
7. [Infrastructure Fixes](#7-infrastructure-fixes)
8. [Seed Data Enrichment](#8-seed-data-enrichment)
9. [Testing Gaps to Fill](#9-testing-gaps-to-fill)

---

## 1. CRITICAL — Security Vulnerabilities

### 1.1 PickupRequestController Missing Authorization
**Module**: Delivery
**File**: `Modules.Delivery/Controllers/PickupRequestController.cs`
**Issue**: All 3 endpoints (GET list, GET by ID, POST) have no `[Authorize]` attribute — completely public
**Fix**:
```csharp
[ApiController]
[Route("api/v1/deliveries/pickups")]
[Authorize] // ← ADD THIS
public class PickupRequestController(...)
```

### 1.2 Employee/Customer Sync Endpoints Unprotected
**Module**: Auth
**Files**: `EmployeeController.cs`, `CustomerController.cs`
**Issue**: `POST /api/v1/employees/sync` and `POST /api/v1/customers/sync` accept any authenticated user
**Fix**: Add `[Authorize(Roles = "ADMIN")]` to both sync endpoints

### 1.3 compose.prod.yaml Missing Keycloak
**File**: `compose.prod.yaml`
**Issue**: Still references `Jwt__SecretKey` environment variables (pre-Keycloak), missing Keycloak service entirely
**Fix**: Update to match compose.yaml Keycloak config; add Keycloak service with PostgreSQL backend (not in-memory for prod)

---

## 2. HIGH — Functional Bugs

### 2.1 Broken Sidebar Link — Deliveries
**File**: `WebApp/Components/Layout/MainLayout.razor`
**Issue**: Sidebar href is `/app/deliveries` but the page route is `/app/delivery`
**Fix**: Change `href="/app/deliveries"` → `href="/app/delivery"`

### 2.2 INotificationHubPusher Not in Module Installer
**Module**: Notification
**Issue**: `INotificationHubPusher` registered only in WebApi `Program.cs`, not in `NotificationModuleInstaller.Install()`. WebApp provides a `NullNotificationHubPusher` but it's also hardcoded in WebApp's Program.cs. If a module tries to resolve this without a host, it fails.
**Fix**: Either register in module installer with a default no-op, or document that hosts must provide the implementation (current implicit contract).

### 2.3 NotificationInbox Not Wired to Auth
**File**: `WebApp/Components/Pages/App/Notifications/NotificationInbox.razor`
**Issue**: `_currentUserId = Guid.Empty` — doesn't resolve the current employee's GUID from Keycloak `sub` claim
**Fix**: Inject `ICurrentUserService`, resolve `UserId` from claims, query employee by `KeycloakUserId`

---

## 3. MEDIUM — Data Type & Schema Issues

### 3.1 Weight/Dimensions Use `double` Instead of `decimal`
**Module**: Parcel
**Entity**: `Parcel.cs` — `WeightKg`, `LengthCm`, `WidthCm`, `HeightCm` are all `double?`
**Instructions**: `decimal` with `NUMERIC(10,3)` column type
**Fix**: Change to `decimal?`, update DTOs, configuration (`HasColumnType("NUMERIC(10,3)")`), and migration

### 3.2 Branch Lat/Lng Use `double` Instead of `decimal`
**Module**: Branch
**Entity**: `Branch.cs` — `Latitude`, `Longitude` are `double?`
**Fix**: Change to `decimal?`, update EF config with `HasColumnType("NUMERIC(10,7)")`, migration

### 3.3 ScannedByEmployeeId is `string?` Instead of `Guid?`
**Module**: Tracking
**Entity**: `TrackingEvent.cs` — `ScannedByEmployeeId` is `string?`
**Fix**: Change to `Guid?`, update DTO, configuration, migration

### 3.4 ParcelConfiguration Price Precision
**Module**: Parcel
**File**: `ParcelConfiguration.cs`
**Issue**: Uses `.HasPrecision(19, 4)` — should use `.HasColumnType("NUMERIC(19,4)")` per project standards
**Fix**: Replace with `HasColumnType("NUMERIC(19,4)")`

### 3.5 DateTime.UtcNow Usage
**Module**: Auth
**File**: `EmployeeService.cs`
**Issue**: Uses `DateTime.UtcNow` in one place — project standard is `DateTimeOffset.UtcNow`
**Fix**: Replace with `DateTimeOffset.UtcNow`

---

## 4. MEDIUM — Missing Core Functionality

### 4.1 Role/Permission CRUD
**Module**: Auth
**Issue**: `Role` and `Permission` entities exist with EF configurations, but have NO service, controller, DTOs, or repository
**What's needed**: At minimum, read-only endpoints for ADMIN to view roles/permissions and manage role-permission assignments

### 4.2 Address Update/Delete
**Module**: Address
**Issue**: Service only has `CreateAsync` + read operations. No update or delete (even soft-delete).
**Fix**: Add `UpdateAsync`, `SoftDeleteAsync` to service + controller. Add domain events.

### 4.3 Parcel Update/Edit
**Module**: Parcel
**Issue**: Can only Create and Cancel. No update endpoint for editing parcel details (before shipping).
**Fix**: Add `PUT /api/v1/parcels/{id}`, update service, add edit Blazor page

### 4.4 Delivery Attempt & Slot APIs
**Module**: Delivery
**Issue**: No way to record individual delivery attempts via API. No CRUD for delivery slots.
**What's needed**:
- `POST /api/v1/deliveries/routes/{routeId}/slots/{slotId}/attempts` — record attempt
- `GET /api/v1/deliveries/routes/{routeId}/slots` — list slots with delivery status
- `PATCH /api/v1/deliveries/slots/{slotId}/status` — update slot status

### 4.5 Pickup Request Status Transitions
**Module**: Delivery
**Issue**: Can only create pickup requests. No way to confirm, assign, or complete them.
**What's needed**: Status transition endpoints (confirm, assign, complete, cancel) with role-based access

### 4.6 In-App Notification API Endpoints
**Module**: Notification
**Issue**: No endpoints for employees to view, mark as read, or manage in-app notifications
**What's needed**:
- `GET /api/v1/notifications/inbox` — current user's in-app notifications
- `POST /api/v1/notifications/inbox/{id}/read` — mark as read
- `GET /api/v1/notifications/inbox/unread-count` — for badge counter

### 4.7 NotificationPreference CRUD
**Module**: Notification
**Issue**: Entity exists but is completely unused — no repository, service, or API
**Fix**: Add full CRUD chain, let customers/employees manage their notification preferences

### 4.8 Global Exception Middleware
**Module**: WebApi + WebApp
**Issue**: No centralized mapping of `BusinessRuleException`, `EntityNotFoundException`, `ConcurrencyException` to RFC 7807 `ProblemDetails` responses
**Fix**: Add `GlobalExceptionMiddleware` in SharedKernel that maps:
- `EntityNotFoundException` → 404 ProblemDetails
- `BusinessRuleException` → 422 ProblemDetails
- `ConcurrencyException` → 409 ProblemDetails
- Unhandled → 500 ProblemDetails (no stack trace in prod)

### 4.9 RemoveByPrefixAsync Implementation
**Module**: SharedKernel
**File**: `RedisCacheService.cs`
**Issue**: `RemoveByPrefixAsync` logs debug and returns — never actually removes keys
**Fix**: Implement using `SCAN` + `DEL` pattern on Redis (`server.Keys(pattern)` → `db.KeyDeleteAsync`)

---

## 5. MEDIUM — UI/UX Issues

### 5.1 No Logout Button
**Component**: `MainLayout.razor`
**Issue**: No way for user to sign out in the UI
**Fix**: Add logout link in topbar that hits `/authentication/logout` or custom `/app/logout` endpoint that calls `HttpContext.SignOutAsync`

### 5.2 Missing Sidebar Nav Links
**Component**: `MainLayout.razor`
**Issue**: No sidebar links for `/app/pickups` (under OPERATIONS) and `/app/inbox` (under OPERATIONS or user section)
**Fix**: Add nav items to appropriate sidebar sections

### 5.3 No Parcel Edit Page
**Component**: `WebApp/Components/Pages/App/Parcels/`
**Issue**: `ParcelForm.razor` only handles create (`/app/parcels/new`). No edit route.
**Fix**: Add `@page "/app/parcels/{id:guid}/edit"` parameter and pre-populate form in edit mode

### 5.4 Zero Localization in Blazor Pages
**All Blazor pages**: Hardcoded English strings everywhere
**Impact**: Project spec requires `de-CH`, `fr-CH`, `it-CH`, `en` — none implemented in UI
**Fix**: Add `@inject IStringLocalizer<{Module}Resource> L` and create `.resx` files for all 4 locales. Extract all string literals.
**Estimation**: Large effort — 27 pages with many strings

### 5.5 CSS Load Order
**File**: `App.razor` (or `_Host.cshtml`)
**Issue**: Custom `app.css` loads before Bootstrap CDN → custom overrides may be overridden by Bootstrap
**Fix**: Move `app.css` `<link>` to AFTER Bootstrap CDN link

### 5.6 DataTables — No Sorting/Filtering
**All list pages**: Use basic `<table>` with pagination but no client-side sorting, column filtering, or search
**Fix**: Consider adding a reusable shared component with sort headers (server-side sort via query params `?sortBy=trackingNumber&sortDir=desc`) and a search bar that filters via query params

---

## 6. LOW — Code Quality & Consistency

### 6.1 Empty Mappings Folders
**All modules**: `Application/Mappings/` folders exist but are empty
**Options**: Either implement Mapster `TypeAdapterConfig` for each module or remove the empty folders

### 6.2 Status Enum Alignment with Instructions
**Parcel module**: `ParcelStatus` has `LabelGenerated, PickedUp` but instructions say `Accepted, Sorted, DeliveryFailed`
**ParcelType**: Has `Bulky` but instructions say `Priority, Insured`
**Decision**: Current enums are actually more realistic for Swiss Post. Update `copilot-instructions.md` to match reality, OR align code to instructions.

### 6.3 Tracking Number Format Divergence
**Instructions**: `CHE-XXXXXXXXXXXXXXX` format
**Code**: `99.XX.XXX.XXX.XXX.XX` format (more realistic Swiss Post format)
**Decision**: Code format is better — update instructions to match

### 6.4 IParcelModuleApi Incomplete
**File**: `Domain/Interfaces/IParcelModuleApi.cs`
**Issue**: Instructions show `GetTotalCountAsync`, `GetCountByStatusAsync`, `GetRecentAsync` — implementation may be missing some
**Fix**: Verify and add missing methods needed by Dashboard

### 6.5 AuditService Should Have Interface
**File**: `SharedKernel/Services/AuditService.cs`
**Issue**: No `IAuditService` interface — hard to mock in tests
**Fix**: Extract `IAuditService`, register in DI

### 6.6 InAppNotificationRead Missing Audit Fields
**Module**: Notification
**Entity**: `InAppNotificationRead` inherits nothing (not `BaseEntity`)
**Issue**: No `CreatedAt`, `UpdatedAt`, `Version` fields
**Fix**: Either inherit `BaseEntity` or at minimum add `CreatedAt`

---

## 7. Infrastructure Fixes

### 7.1 compose.prod.yaml Full Rewrite
**Needs**:
- Remove all `Jwt__*` environment variables
- Add `Keycloak__*` environment variables matching dev compose
- Add Keycloak service with PostgreSQL DB (prod: not in-memory H2)
- Add Redis service (if not external)
- Verify health checks and restart policies

### 7.2 Nginx limit_req_zone Placement
**File**: `docker/nginx/conf.d/default.conf`
**Issue**: `limit_req_zone` directive is inside `server {}` block — must be in `http {}` context
**Fix**: Move `limit_req_zone` to `nginx.conf` (at http level) or a separate include

### 7.3 CORS Configuration for Production
**File**: `WebApi/Program.cs`
**Issue**: CORS origins hardcoded to `http://localhost:5101`
**Fix**: Move to `appsettings.json` / `appsettings.Production.json`:
```json
{ "Cors": { "AllowedOrigins": ["https://app.swiftapp.ch"] } }
```

---

## 8. Seed Data Enrichment

### 8.1 Add Operational Seed Data (New Migration)
Create `SeedOperationalData` migration matching Java reference:

**Customers** (5):
- Peter Zürcher, Claire Bonnet, Giovanni Colombo, Martina Bühler, Robert Favre
- With Keycloak user IDs, realistic customer numbers

**Parcels** (15):
- Cover ALL statuses (Created → Delivered, including Returned and Cancelled)
- Various types (Standard, Express, Registered, Bulky)
- Assigned to seeded branches, linked to customer senders
- Realistic weights, dimensions, prices

**Tracking Records + Events** (15 records, ~55 events):
- Full lifecycle histories showing realistic tracking journeys
- Events with timestamps every few hours/days
- Linked to branches and employees (scanned by)

**Delivery Routes** (4):
- Routes for Zurich, Bern, Geneva, Lugano
- Various statuses (Planned, InProgress, Completed)
- Assigned to employees

**Delivery Slots** (9):
- Linked to routes and tracking numbers
- Distribution of Pending, Delivered, Failed statuses

**Pickup Requests** (10):
- Various statuses (Pending → PickedUp)
- Linked to customers, realistic addresses

**Notification Logs** (12):
- Historical sent/failed emails
- Various event types and statuses

**In-App Notifications** (7):
- Broadcast, role-targeted, individual
- Different categories (Info, Warning, Urgent, System)

**Notification Preferences** (5):
- Per-customer preferences (email on, SMS off, etc.)

**Audit Logs** (10+):
- Parcel create/status change, employee login, branch update actions

---

## 9. Testing Gaps to Fill

### Priority 1 — Unit Tests for Missing Services
| Service | Exists | Tests Exist |
|---------|--------|------------|
| EmployeeService | ✅ | ✅ 8 tests |
| CustomerService | ✅ | ❌ **Need ~8 tests** |
| BranchService | ✅ | ✅ 5 tests |
| SwissAddressService | ✅ | ✅ 7 tests |
| ParcelService | ✅ | ✅ 12 tests |
| DeliveryRouteService | ✅ | ✅ 8 tests |
| PickupRequestService | ✅ | ❌ **Need ~8 tests** |
| TrackingService | ✅ | ✅ 5 tests |
| NotificationService | ✅ | ✅ 6 tests |
| InAppNotificationService | ✅ | ❌ **Need ~6 tests** |
| EmailService | ✅ | ❌ **Need ~5 tests** |

### Priority 2 — Validator Tests
Test all `FluentValidation` validators — especially edge cases (empty strings, negative weights, invalid tracking numbers)

### Priority 3 — Event Handler Tests
Test 5 event handlers in WebApi/Infrastructure/EventHandlers:
- `ParcelCreatedEventHandler`, `ParcelStatusChangedEventHandler`
- `EmployeeCreatedEventHandler`, `CustomerCreatedEventHandler`
- `DeliveryEventHandlers` (completed + failed)

### Priority 4 — Integration Tests (Testcontainers)
- Repository tests against real PostgreSQL
- Verify EF configurations, constraints, indexes
- Verify soft-delete global query filter works

### Priority 5 — API Endpoint Tests (WebApplicationFactory)
- Auth flow (JWT Bearer validation)
- Role-based access control (403 for wrong role)
- CRUD operations end-to-end
- Validation error responses (400)

### Priority 6 — Blazor Component Tests (bUnit)
- Dashboard renders with module API data
- Parcel list pagination works
- Form submission and validation display
- RBAC-gated sidebar sections

---

*End of Improvements Document*
