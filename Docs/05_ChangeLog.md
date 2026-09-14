# 05 — Change Log

> ثبت تمام تغییرات مهم پروژه به ترتیب زمانی معکوس (جدیدترین بالا).

**فرمت هر ورودی:**

```
## [YYYY-MM-DD] - Session-XXXX / Phase XX
### Added
- 

### Changed
- 

### Fixed
- 

### Removed
- 

### Decisions / ADRs
- 
```

---

## [2026-09-14] - Session-20260914-1040 / Phase 0
### Added
- ایجاد ساختار ریشه پروژه: `Backend/`, `Frontend/`, `.gitignore`, `.editorconfig`, `README.md`
- ایجاد پوشه‌های اسناد: `Docs/adr/`, `Docs/domain/`, `Docs/Architecture/`, `Docs/Phases/`, `Docs/SessionReports/`, `Docs/api-contracts/`
- اسناد عملیاتی `00_ProjectContext.md`, `01_Architecture.md`, `02_CodingStandards.md`, `03_Roadmap.md`, `04_Progress.md`, `05_ChangeLog.md`, `06_DevelopmentRules.md`
- `Docs/PROJECT-BASELINE-v0.1.md` (شارتر، مخصوص، Gap Analysis، Decision Log، Risk Register)
- `Docs/decision-log.md` (DEC-001..DEC-017) و `Docs/open-questions.md` (Q-001..Q-008)
- `Docs/adr/` — ADR-001 تا ADR-009
- `Docs/domain/` — vision-and-problem, bounded-contexts, ubiquitous-language, aggregates-and-entities, domain-events
- `Docs/Phases/Phase00.md` + اسکلت `Phase01..08.md`
- `Docs/Architecture/` — context-map, container-diagram, iam-integration (پیش‌نویس)
- `Docs/api-contracts/README.md`
- `Docs/SessionReports/Session-20260914-1040.md`

### Changed
- (اسناد پایه موجود از نظر سازگاری با تصمیمات بررسی شدند)

### Fixed
- 

### Removed
- 

### Decisions / ADRs
- DEC-001 تا DEC-017 — مرز IAM، Frontend/Backend، Multi-tenancy، Region، Post Tree، Employee، PII، REST-only، Import
- ADR-001 تا ADR-009 (معماری/دامنه)