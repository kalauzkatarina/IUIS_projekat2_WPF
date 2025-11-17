# Infrastructure System Simulator (CG3 – T3 – G2 – P2)

This project is a WPF MVVM desktop application developed as part of the course **Applied Software Engineering – Usability Engineering in Infrastructure Systems**.  

The system implements a *NetworkService* application that communicates with the given *MeteringSimulator*, receives measurement data in real time, and provides visualization, monitoring, and manipulation of infrastructure entities.


The project was created according to the assigned combination:  

**CG3 (Mobile Users) – T3 (Daily Traffic) – G2 (Bar Graph) – P2 (Combined Filtering).**

---

## Target User Group: CG3 — Mobile Phone Users

The application emulates a mobile user interface displayed in **portrait mode**, optimized for pointer-based interaction.

Key CG3-specific features:

- Custom-made **virtual keyboard** (not OS-based, no NuGet packages)

- Navigation through on-screen menus and buttons (Home, Undo)

- **Undo** functionality for sequential reversal of actions and returning to previous views

- Clear, touchscreen-friendly layout with larger UI elements

- Built-in Help section and contextual ToolTips

- Simplified and intuitive interaction flow

---

## Entity Theme: T3 — Daily Traffic Monitoring

The system works with infrastructure entities representing **daily traffic flow** on roads of category IA and IB.

### Entity attributes:

  - `ID` — unique integer

  - `Name`

  - `Type` — *IA* or *IB*
  
  - Last measured value

### Valid measurement ranges:

  - IA roads → up to **15000**

  - IB roads → up to **7000**


Values above these limits are considered **invalid measurements**.

---

## Graph Type: G2 — Bar Chart Visualization


A real-time bar chart is implemented to visualize the last **five received measurements** per selected entity.



Graph properties:

  - Fully **custom drawn** (no chart libraries)
  
  - Marked X and Y axes
  
  - Color distinction between valid and invalid values
  
  - Automatic live updates as new measurements arrive and are logged

---

## Filtering Module: P2 — Combined Type & ID Filtering

The **Network Entities View** includes advanced filtering:

### Type filtering
  
  - ComboBox with: IA, IB

### ID filtering
  
  - Operators: `<`, `>`, `=`
  
  - Numerical input field

### Combined filter (P2)
  
  - Apply type + operator + value simultaneously
  
  - Results update dynamically
  
  - Ability to reset filters to show all entities

---

## Application Features

### Network Entities View
  
  - Adding new entities (with full field validation)

  - Deleting entities (with confirmation dialog)
  
  - Table-based display of all entities
  
  - Display of incoming measurements
  
  - Searching & filtering (P2 specification)
  
  - Undo support for reversible actions
  
  - UI feedback (ToolTips, cursor changes, toast/message notifications)
  
  - Automatic restart of MeteringSimulator when entity list changes

---

### Network Display View
  
  - Drag & Drop placement of entities on a grid (12+ canvas slots)
  
  - Visual representation of each entity (ID + latest value)
  
  - Highlighting invalid states (color/icon change)
  
  - Entities listed in a TreeView when not placed on the grid
  
  - Lines connecting entities (updated as they move)
  
  - Prevention of duplicate connections
  
  - Lines removed automatically when entities are deleted or removed from the grid
  
  - View state preserved during navigation

---

### Measurement Graph View
  
  - Real-time bar chart (G2)
  
  - Entity selection via ComboBox
  
  - Display of last 5 measurements for the chosen entity
  
  - Clear separation of valid and invalid values through color coding
  
  - Data pulled from Log file as it updates

---

## Architecture & Technologies

  - **C# / .NET / WPF**
  
  - **MVVM architecture**
  
  - Full **DataBinding** support  
  
  - Separation into Model, ViewModel, View layers  
  
  - Measurement logging system with timestamped entries

---

## Running the Application

  1. Start the **NetworkService** application.

  2. It automatically establishes communication with the provided **MeteringSimulator**.
  
  3. Load predefined initial entities (minimum 3 valid examples).
  
  4. Navigate using the mobile-styled UI (portrait layout, menu buttons).

---

## Author
Project created for the **Faculty of Technical Sciences – Applied Software Engineering** course.
