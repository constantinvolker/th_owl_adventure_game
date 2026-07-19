# Contributing / Mitwirken

## Deutsch

Danke, dass du zu diesem Projekt beitragen möchtest.
Dieses Dokument beschreibt unseren Branching- und Review-Workflow.

---

## Branch-Struktur

Wir verwenden folgendes Branch-System:

```text
main -> dev -> feature/... oder fix/...
```

### `main`

Der `main`-Branch enthält ausschließlich produktionsreife und funktionierende Releases.

* `main` entspricht dem Produktionsstand.
* Änderungen auf `main` müssen stabil, getestet und releasefähig sein.
* Direkte Änderungen auf `main` sind nicht erlaubt, außer bei Hotfixes.
* Auf `main` darf nur gemerged werden:

  * von `dev`
  * oder über einen `hotfix/...`-Branch

### `dev`

Der `dev`-Branch enthält den aktuellen Entwicklungsstand.

* Neue Features und Fixes werden zuerst in `dev` integriert.
* `dev` dient als Sammelpunkt für getestete Entwicklungsstände.
* Änderungen an `dev` benötigen ein Approval.

### Feature-Branches

Neue Features werden immer von `dev` aus erstellt.

Namensschema:

```text
feature/name-des-features
```

Beispiele:

```text
feature/user-login
feature/dashboard-redesign
feature/api-authentication
```

### Fix-Branches

Bugfixes werden ebenfalls von `dev` aus erstellt.

Namensschema:

```text
fix/name-des-fixes
```

Beispiele:

```text
fix/login-validation
fix/navbar-overflow
fix/api-error-handling
```

### Hotfix-Branches

Hotfixes sind dringende Fehlerbehebungen für produktionsrelevante Probleme.

Namensschema:

```text
hotfix/name-des-hotfixes
```

Hotfixes dürfen direkt auf `main` gemerged werden, müssen aber anschließend auch nach `dev` übernommen werden, damit die Änderung im Entwicklungsstand enthalten bleibt.

---

## Workflow

### Neues Feature entwickeln

1. Aktuellen Stand von `dev` holen:

```bash
git checkout dev
git pull
```

2. Feature-Branch erstellen:

```bash
git checkout -b feature/name-des-features
```

3. Änderungen umsetzen und committen.

4. Branch pushen:

```bash
git push -u origin feature/name-des-features
```

5. Pull Request nach `dev` erstellen.

6. Nach Approval wird der Branch in `dev` gemerged.

---

### Bugfix entwickeln

1. Aktuellen Stand von `dev` holen:

```bash
git checkout dev
git pull
```

2. Fix-Branch erstellen:

```bash
git checkout -b fix/name-des-fixes
```

3. Fehler beheben und Änderungen committen.

4. Branch pushen:

```bash
git push -u origin fix/name-des-fixes
```

5. Pull Request nach `dev` erstellen.

6. Nach Approval wird der Branch in `dev` gemerged.

---

### Release nach `main`

Wenn `dev` stabil ist und ein Release erstellt werden soll:

1. Pull Request von `dev` nach `main` erstellen.
2. Review und Approval abwarten.
3. Nach erfolgreicher Prüfung wird `dev` in `main` gemerged.
4. `main` enthält danach den neuen produktionsreifen Release-Stand.

---

## Approval-Regeln

Für folgende Branches sind Approvals erforderlich:

* Pull Requests nach `dev`
* Pull Requests nach `main`

Direkte Pushes auf `dev` und `main` sollten vermieden bzw. durch Branch Protection verhindert werden.

---

## Branch-Namenskonventionen

Bitte verwende sprechende und einheitliche Branch-Namen.

Erlaubte Präfixe:

```text
feature/
fix/
hotfix/
```

Beispiele:

```text
feature/add-user-settings
fix/incorrect-date-format
hotfix/production-login-crash
```

---

## Commit-Nachrichten

Commit-Nachrichten sollten kurz, verständlich und beschreibend sein.

Beispiele:

```text
Add user login form
Fix validation error on registration
Update API error handling
```

Optional kann ein konventionelles Format verwendet werden:

```text
feat: add user login form
fix: correct registration validation
docs: update contributing guide
```

---

## Pull Requests

Ein Pull Request sollte enthalten:

* eine kurze Beschreibung der Änderung
* relevante Screenshots, falls UI geändert wurde
* Hinweise zu Tests oder manueller Prüfung
* Verweise auf Issues, falls vorhanden

Bitte stelle sicher, dass der Code vor dem Erstellen des Pull Requests lokal getestet wurde.

---

## English

Thank you for contributing to this project.
This document describes our branching and review workflow.

---

## Branch Structure

We use the following branch system:

```text
main -> dev -> feature/... or fix/...
```

### `main`

The `main` branch contains only production-ready and working releases.

* `main` represents the production state.
* Changes on `main` must be stable, tested, and release-ready.
* Direct changes to `main` are not allowed, except for hotfixes.
* Merges into `main` are only allowed:

  * from `dev`
  * or from a `hotfix/...` branch

### `dev`

The `dev` branch contains the current development state.

* New features and fixes are integrated into `dev` first.
* `dev` acts as the integration branch for tested development work.
* Changes to `dev` require approval.

### Feature Branches

New features must always be created from `dev`.

Naming convention:

```text
feature/feature-name
```

Examples:

```text
feature/user-login
feature/dashboard-redesign
feature/api-authentication
```

### Fix Branches

Bug fixes must also be created from `dev`.

Naming convention:

```text
fix/fix-name
```

Examples:

```text
fix/login-validation
fix/navbar-overflow
fix/api-error-handling
```

### Hotfix Branches

Hotfixes are urgent fixes for production-related issues.

Naming convention:

```text
hotfix/hotfix-name
```

Hotfixes may be merged directly into `main`, but must also be merged back into `dev` afterwards so that the development branch stays up to date.

---

## Workflow

### Developing a New Feature

1. Get the latest version of `dev`:

```bash
git checkout dev
git pull
```

2. Create a feature branch:

```bash
git checkout -b feature/feature-name
```

3. Implement and commit your changes.

4. Push the branch:

```bash
git push -u origin feature/feature-name
```

5. Open a pull request into `dev`.

6. After approval, the branch will be merged into `dev`.

---

### Developing a Bugfix

1. Get the latest version of `dev`:

```bash
git checkout dev
git pull
```

2. Create a fix branch:

```bash
git checkout -b fix/fix-name
```

3. Fix the issue and commit your changes.

4. Push the branch:

```bash
git push -u origin fix/fix-name
```

5. Open a pull request into `dev`.

6. After approval, the branch will be merged into `dev`.

---

### Releasing to `main`

When `dev` is stable and ready for release:

1. Open a pull request from `dev` into `main`.
2. Wait for review and approval.
3. After successful review, merge `dev` into `main`.
4. `main` now contains the new production-ready release state.

---

## Approval Rules

Approvals are required for:

* Pull requests into `dev`
* Pull requests into `main`

Direct pushes to `dev` and `main` should be avoided or prevented using branch protection rules.

---

## Branch Naming Conventions

Please use clear and consistent branch names.

Allowed prefixes:

```text
feature/
fix/
hotfix/
```

Examples:

```text
feature/add-user-settings
fix/incorrect-date-format
hotfix/production-login-crash
```

---

## Commit Messages

Commit messages should be short, clear, and descriptive.

Examples:

```text
Add user login form
Fix validation error on registration
Update API error handling
```

Optionally, a conventional commit format may be used:

```text
feat: add user login form
fix: correct registration validation
docs: update contributing guide
```

---

## Pull Requests

A pull request should include:

* a short description of the change
* relevant screenshots if the UI was changed
* notes about testing or manual verification
* references to related issues, if available

Please make sure your code has been tested locally before opening a pull request.
