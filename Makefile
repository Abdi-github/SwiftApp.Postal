# ──────────────────────────────────────────────────────────
# SwiftApp Postal — Makefile
# ──────────────────────────────────────────────────────────
.DEFAULT_GOAL := help
SHELL := /bin/bash

SLN := SwiftApp.Postal.slnx
WEBAPI := src/SwiftApp.Postal.WebApi
WEBAPP := src/SwiftApp.Postal.WebApp

# ── Build ─────────────────────────────────────────────────
.PHONY: build restore clean

build: ## Build entire solution
	dotnet build $(SLN)

restore: ## Restore NuGet packages
	dotnet restore $(SLN)

clean: ## Clean build artifacts
	dotnet clean $(SLN)

# ── Run ───────────────────────────────────────────────────
.PHONY: run-api run-app

run-api: ## Run WebApi locally (port 5100)
	dotnet watch run --project $(WEBAPI)

run-app: ## Run WebApp locally (port 5101)
	dotnet watch run --project $(WEBAPP)

# ── EF Core ──────────────────────────────────────────────
.PHONY: ef-add ef-update ef-remove

ef-add: ## Add migration: make ef-add NAME=InitialCreate
	dotnet ef migrations add $(NAME) --project $(WEBAPI)

ef-update: ## Apply pending migrations
	dotnet ef database update --project $(WEBAPI)

ef-remove: ## Remove last migration
	dotnet ef migrations remove --project $(WEBAPI)

# ── Docker ────────────────────────────────────────────────
.PHONY: up down up-infra up-build logs

up: ## Start all Docker services
	docker compose up -d

down: ## Stop all Docker services
	docker compose down

up-infra: ## Start infrastructure only (DB, pgAdmin, Mailpit, Seq, Redis)
	docker compose up -d postgres pgadmin mailpit seq redis

up-build: ## Rebuild and start all services
	docker compose up -d --build

logs: ## Tail all container logs
	docker compose logs -f

# ── Test ──────────────────────────────────────────────────
.PHONY: test test-arch

test: ## Run all tests
	dotnet test $(SLN)

test-arch: ## Run architecture tests only
	dotnet test tests/SwiftApp.Postal.Architecture.Tests

# ── Help ──────────────────────────────────────────────────
.PHONY: help
help: ## Show this help
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-15s\033[0m %s\n", $$1, $$2}'
