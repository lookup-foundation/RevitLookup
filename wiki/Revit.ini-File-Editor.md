The **Revit.ini** is a configuration file in Revit that stores settings related to user preferences, system behavior, and project defaults. 

The **Revit.ini File Editor** provides a simple way to manage these settings without manual editing.

![image](https://github.com/user-attachments/assets/701a0a97-1906-419d-950b-b70f9b852966)

## Managing Entries

The **Revit.ini File Editor** allows you to add new settings or update existing ones within the **Revit.ini** file.

To add or update an entry:

- **Select an existing entry** to update it, or click **New Entry** at the top to create a new one.
- **Enter the Category:** Specify the section where the setting belongs (e.g., `UserInterface`, `Snapping`).
- **Enter the Property:** Provide the key name for the setting (e.g., `RecentFiles`, `DefaultTemplate`).
- **Enter the Value:** Set the appropriate value for the key (e.g., `%USERPROFILE%\Documents\` for `ProjectPath` or a list of increments for `SnapIncrements`).
- Click **Create** to create a new entry, or **Update** to save changes to an existing entry.

![image](https://github.com/user-attachments/assets/6d596fb9-dcdf-4456-bd5b-15ffda6a696b)

## Filtering Settings

The **Revit.ini File Editor** includes a filtering tool to help you quickly locate specific settings:

- Click the **Filter** icon in the toolbar.
- Enter search criteria in the **Category**, **Property**, or **Value** fields.
- The results will filter dynamically based on your input, allowing for quick access to the relevant settings.

There is also an option to display only user-defined settings by toggling the **Show only user settings** switch.

![image](https://github.com/user-attachments/assets/b3efdc7c-f365-410b-ae3b-d5a31dbfe52b)

## List Controls

In the **Revit.ini File Editor**, several controls are available for managing settings:

![image](https://github.com/user-attachments/assets/81799c61-1646-4fd1-8365-d7f0546e731a)

- **Restore Button:** restores a modified setting to its default value. The default values are retrieved from the `UserDataCache` directory.
- **Delete Button:** permanently removes the selected entry from the `Revit.ini` file. Once deleted, the setting cannot be restored unless manually added again.
- **Toggle Switch:** temporarily enable or disable a setting. You can toggle it back on at any time to restore the setting without losing its value.

    ![image](https://github.com/user-attachments/assets/7ee0e92b-2bc8-41e8-b609-5e262f87f16e)

## Backing Up the Revit.ini File

Before any edits are made, the **Revit.ini File Editor** automatically creates a backup of the file. 
These backup files are stored in the same directory as the original `Revit.ini` file and follow the naming convention `Revit_RevitLookupBackup_YYYYMMDDHHMMSS`, ensuring that you can easily restore previous settings if necessary.