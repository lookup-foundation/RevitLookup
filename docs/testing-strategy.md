# Testing Strategy

RevitLookup uses a combination of unit tests and a playground environment for rapid UI development.

## 1. Unit Testing

* **Project:** `RevitLookup.Tests`.
* **Framework:** TUnit.
* **Configuration:** Use `Debug.R26` or any latest with `.R` suffix.
* **Revit Integration:** Use `Nice3point.TUnit.Revit` to run tests within the Revit context.
* **Scope:** Test API logic, decomposition, and core services without Revit UI references.

## 2. UI Testing (Playground)

* **Project:** `RevitLookup.UI.Playground`.
* **Configuration:** Use `Debug`.
* **Purpose:** Test UI changes, themes, and interactions without launching Revit.
* **Mocking:** Uses mock data to simulate Revit objects and documents.
