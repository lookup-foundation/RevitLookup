# Documentation

These rules govern every piece of prose the project ships: XML doc comments, `README.md`, `CHANGELOG.md`, and the wiki.
Each format adds its own rules on top of the shared set.

A user-facing change updates the README, the CHANGELOG, the wiki, and the affected XML docs in the same commit.
Documentation that lags the code is a defect.

## Shared Prose Rules

* **State what, not how.** Describe observable behavior and contract, never the implementation. A summary survives an implementation rewrite unchanged.
* **Plain technical English.** No corporate jargon, no marketing tone outside the README.
* **No filler.** Omit obvious statements. State only what a reader cannot infer from the signature.
* **Third-person present indicative.** Write "Decomposes the object", not "Decomposing the object". No `-ing` verb form for what a member does.
* **One sentence per line.** Break at sentence boundaries, never at a fixed character width.
* **No dashes or semicolons.** Use separate sentences or commas.

## XML Doc Comments

* Document every public member with a `<summary>` that states what it does.
* **`<summary>` describes the member, not its parameters.** Parameters belong in `<param>`, the return value in `<returns>`, and thrown exceptions in `<exception>`. Do not restate the signature in prose.
* Add `<remarks>` for a non-trivial constraint or a threading note.
* Reference another type or member with `<see cref="..."/>` so renames stay tracked.

## README

The README is the marketing front page.
It carries the badges, the screenshots, the install methods, and links onward.
It defers the usage detail to the wiki rather than carry it inline.

* Keep the install methods and their links current.
* Send a reader who wants usage detail to the wiki, not to a long inline section.

## CHANGELOG

The CHANGELOG reads as a feature narrative aimed at users, grouped by release version.

* Add a section for the new version with the version in its heading.
* Describe each user-facing change in plain language, and lead a major release with a short summary of the headline features.
* Link the issue or pull request for a change where one exists.

## Wiki

The repository publishes a wiki from the `wiki` folder.
A push to the wiki sources updates the published wiki automatically.
The wiki holds the user-facing usage guides: feature walkthroughs, the version and install reference, keyboard shortcuts, and the extension and visualization guides.

* Update the matching wiki page when a feature's behavior or usage changes.
* Keep the usage detail in the wiki, and keep the README pointing to it.