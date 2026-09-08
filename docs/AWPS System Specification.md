# AWPS — Auto-Watering Plant System
## System Specification v0.1

**Document status:** Draft  
**Version:** 0.1  
**Project type:** Diploma project / IoT system  
**Primary language:** C#  
**Target platforms:** Web, cross-platform application, IoT device

---

# 1. Introduction

## 1.1 Purpose

AWPS (Auto-Watering Plant System) is an IoT-based system designed for monitoring and automatic watering of plants grown in pots.

The system collects environmental and soil data using sensors connected to an IoT device, stores historical measurements, automatically waters the plant according to configured parameters, and provides the user with a graphical interface for monitoring and configuration.

The system is intended as a diploma project while following production-ready architectural principles where practical.

## 1.2 Main objectives

The system shall:

- monitor the condition of a plant and its environment;
- periodically collect sensor measurements;
- automatically water the plant based primarily on soil moisture;
- operate autonomously when network connectivity is unavailable;
- preserve telemetry and configuration locally on the IoT device;
- synchronize locally stored data when connectivity becomes available;
- provide historical telemetry to the user;
- allow the user to configure device and watering parameters;
- support multiple plants/devices per user;
- support operation with a central server;
- support a fully local installation where the user's device acts as the server.

---

# 2. Scope

## 2.1 In scope

The first version of AWPS includes:

- user accounts;
- plants;
- IoT devices;
- sensor telemetry;
- automatic watering;
- watering events;
- configurable measurement interval;
- configurable sleep mode;
- local device storage;
- offline device operation;
- synchronization;
- central server;
- local server;
- web and cross-platform GUI;
- telemetry history.

## 2.2 Out of scope for the initial version

The following are not currently required:

- automatic identification of plant species;
- machine-learning-based watering;
- automatic plant disease detection;
- computer vision;
- weather-based watering;
- multi-plant watering from a single physical device;
- advanced prediction of plant water consumption.

These features may be considered in future versions.

---

# 3. System Overview

AWPS consists of three major logical components:

1. **IoT Device**
2. **Backend / Server**
3. **GUI**

Conceptually:

```text
                    ┌─────────────────────┐
                    │        GUI          │
                    │                     │
                    │ Blazor MAUI         │
                    │ Blazor WebAssembly  │
                    │ Interactive Server  │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │       Backend       │
                    │                     │
                    │ API                 │
                    │ Authentication      │
                    │ Device management   │
                    │ Telemetry           │
                    │ Synchronization     │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │     IoT Device      │
                    │                     │
                    │ Sensors             │
                    │ Watering            │
                    │ Local storage       │
                    │ Sleep               │
                    └─────────────────────┘
```

The exact communication protocol and deployment topology are TBD.

---

# 4. Domain Model

## 4.1 User

A user owns and manages plants and their associated IoT devices.

One user may have multiple plants and devices.

```text
User
 ├── Plant
 │    └── Device
 │
 ├── Plant
 │    └── Device
 │
 └── Plant
      └── Device
```

## 4.2 Plant

The plant in a pot is the primary object of the AWPS domain.

A plant is associated with:

- a user;
- an IoT device;
- telemetry;
- watering events;
- watering configuration.

The exact plant properties are TBD.

## 4.3 IoT Device

An IoT device is a physical device responsible for monitoring and watering one plant.

The initial system assumes:

> One physical IoT device is associated with one plant.

Support for multiple plants per physical device is out of scope for v0.1.

---

# 5. IoT Device Specification

## 5.1 Purpose

The IoT device performs autonomous monitoring and watering of a plant.

The device shall be capable of performing its primary functionality without continuous communication with a server.

---

## 5.2 Hardware

The initial device consists of:

| Component | Purpose |
|---|---|
| ESP32 / compatible MCU | Device control and execution |
| Resistive moisture sensor | Soil moisture measurement |
| DHT11 | Air temperature and humidity |
| Photo sensor | Light measurement |
| Relay | Pump switching |
| Mini pump | Plant watering |

The exact hardware revisions and electrical characteristics are TBD.

---

# 6. Telemetry

The device shall periodically collect the following measurements:

| Parameter | Source | Current role |
|---|---|---|
| Soil moisture | Moisture sensor | Watering algorithm |
| Temperature | DHT11 | Telemetry |
| Air humidity | DHT11 | Telemetry |
| Light | Photo sensor | Telemetry |

At this stage, **soil moisture is the only environmental parameter used by the automatic watering algorithm**.

Temperature, air humidity and light shall be collected and stored as telemetry but shall not directly influence watering decisions.

This decision may be revised in a future version after experimental research.

## 6.1 Telemetry record

A telemetry record shall contain at least:

```text
Timestamp
SoilMoisture
Temperature
AirHumidity
Light
```

The exact data types, units and serialization format are TBD.

---

# 7. Measurement Cycle

The device shall operate periodically.

The measurement interval shall be configurable by the user.

The basic cycle is:

```text
Wake
  ↓
Read sensors
  ↓
Create telemetry
  ↓
Store telemetry locally
  ↓
Evaluate soil moisture
  ↓
Execute watering if required
  ↓
Store watering event
  ↓
Attempt synchronization
  ↓
Deep Sleep
  ↓
Wait configured interval
  ↓
Wake
```

## 7.1 MeasurementInterval

The device shall provide a configurable parameter:

```text
MeasurementInterval
```

It determines the period between measurement cycles.

The exact allowed values and minimum/maximum limits are TBD and shall be determined based on:

- device power consumption;
- sensor characteristics;
- required telemetry resolution;
- watering behavior;
- deep sleep characteristics.

---

# 8. Automatic Watering

## 8.1 General behavior

Automatic watering shall be based on soil moisture.

When the measured soil moisture indicates that the plant requires watering, the device shall activate the pump.

The watering procedure shall be **portion-based** rather than one continuous long pump activation.

## 8.2 Watering procedure

Conceptually:

```text
Measure moisture
      ↓
Below required level?
   ┌──┴──┐
  No    Yes
   │      │
   │   Pump ON
   │      ↓
   │   Watering
   │    portion
   │      ↓
   │   Pump OFF
   │      ↓
   │  Wait several
   │    minutes
   │      ↓
   │ Measure again
   │      ↓
   │  Sufficient?
   │   ┌──┴──┐
   │  Yes   No
   │   │     │
   │   │   Next portion
   │   │     │
   └───┴─────┘
```

The device shall re-measure soil moisture between watering portions.

## 8.3 Watering parameters

The watering algorithm is expected to contain configurable parameters such as:

```text
MinimumMoisture
TargetMoisture
WateringPortionDuration
PauseBetweenPortions
MaximumWateringPortions
```

These parameters are currently **TBD**.

Their values shall be determined through research and experimentation.

## 8.4 Local execution

The watering algorithm shall execute locally on the IoT device.

The central or local server shall not be required for the device to make an automatic watering decision.

Therefore:

```text
No network
   ↓
Measure moisture
   ↓
Watering algorithm
   ↓
Pump
```

shall remain possible.

---

# 9. Watering Events

Each automatic watering operation shall produce a watering event.

A watering event shall contain at least:

```text
Timestamp
Duration
NumberOfPortions
Reason
```

The exact representation is TBD.

The event shall be stored locally and eventually synchronized with the server.

---

# 10. Device Sleep

## 10.1 Default behavior

Deep sleep shall be the default operating mode of the IoT device.

After completing its current cycle, the device shall enter deep sleep.

```text
Wake
 ↓
Measure
 ↓
Process
 ↓
Water
 ↓
Synchronize
 ↓
Deep Sleep
```

## 10.2 SleepEnabled

The device shall expose a configurable parameter:

```text
SleepEnabled
```

Default:

```text
SleepEnabled = true
```

When enabled, the device enters deep sleep after completing its cycle.

When disabled, the device remains active.

The active mode is intended primarily for:

- device configuration;
- provisioning;
- debugging;
- development;
- testing;
- situations where continuous interaction is required.

## 10.3 Remote configuration while sleeping

A sleeping device cannot be expected to receive arbitrary commands immediately.

Therefore, configuration changes shall be persisted by the server and applied by the device during its next available communication/wake cycle.

The exact mechanism is TBD.

---

# 11. Local Device Storage

The device shall maintain local storage for operation without a server.

At minimum, local storage shall contain:

### Configuration

```text
MeasurementInterval
SleepEnabled
WateringSettings
ServerEndpoint
...
```

### Telemetry

```text
Timestamp
SoilMoisture
Temperature
AirHumidity
Light
```

### Watering events

```text
Timestamp
Duration
NumberOfPortions
Reason
```

The exact storage technology and capacity are TBD.

The implementation shall account for finite local storage capacity.

---

# 12. Connectivity Modes

AWPS shall support three logical connectivity states.

## 12.1 Central Server

The device communicates with the central AWPS server through a network/Internet connection.

```text
IoT Device
    │
    ▼
Internet
    │
    ▼
Central AWPS Server
    │
    ▼
Database
```

The device may:

- send telemetry;
- send watering events;
- receive configuration;
- synchronize locally stored data.

The exact protocol is TBD.

---

## 12.2 Local Server

The system shall support a fully local installation.

In this mode, a device belonging to the user may act as the AWPS server.

```text
              Local Network

       ┌───────────────────────┐
       │ User's Computer       │
       │                       │
       │ AWPS Server           │
       │ Database              │
       │ GUI                   │
       └───────────┬───────────┘
                   │
                   ▼
              IoT Device
```

The local server shall not require Internet connectivity for normal operation.

The exact implementation and deployment mechanism are TBD.

---

## 12.3 No Server / Autonomous Mode

The IoT device shall be capable of operating when neither a central nor local server is available.

```text
IoT Device
 ├── Local configuration
 ├── Local telemetry
 ├── Local watering algorithm
 └── Local watering events
```

The device shall continue its monitoring and watering functionality.

When a server becomes available, locally stored data shall be synchronized.

---

# 13. Offline Operation

Offline operation means that temporary loss of server connectivity does not stop the device from performing its primary function.

The device shall:

1. continue measuring sensors;
2. continue executing the watering algorithm;
3. store telemetry locally;
4. store watering events locally;
5. retain the required configuration locally;
6. periodically attempt to restore connectivity.

Example:

```text
Server available
      ↓
Measure
      ↓
Synchronize
      ↓
Sleep

Server unavailable
      ↓
Measure
      ↓
Store locally
      ↓
Water if required
      ↓
Sleep
      ↓
Repeat
```

---

# 14. Synchronization

When connectivity becomes available, the device shall synchronize locally stored information with the available server.

At minimum, synchronization shall include:

- telemetry;
- watering events;
- relevant device state/configuration.

The synchronization mechanism shall follow a store-and-forward model:

```text
Measurement
    ↓
Local Storage
    ↓
Connectivity unavailable?
    ├── Yes → Keep locally
    └── No  → Synchronize
```

The exact synchronization protocol, batching strategy, acknowledgement mechanism and conflict resolution strategy are TBD.

## 14.1 Timestamp

Telemetry shall preserve the time at which the measurement was performed.

The server may additionally store the time at which the record was received.

This distinction is important for correctly reconstructing historical telemetry after an offline period.

---

# 15. Configuration

Device configuration shall be controlled through the AWPS GUI where applicable.

Initial configuration categories:

### Measurement

```text
MeasurementInterval
```

### Power

```text
SleepEnabled
```

### Watering

```text
MinimumMoisture
TargetMoisture
WateringPortionDuration
PauseBetweenPortions
MaximumWateringPortions
```

Not all parameters are required to be exposed to the user in the first implementation.

The final configuration model is TBD.

---

# 16. Device Lifecycle

The expected device lifecycle is:

```text
New Device
    ↓
Provisioning
    ↓
Network Configuration
    ↓
Device Registration
    ↓
Plant Association
    ↓
Operational
    ↓
Configuration Updates
    ↓
Normal Operation
    ↓
Offline / Online transitions
    ↓
Reset / Reconfiguration
```

The exact provisioning mechanism is TBD.

---

# 17. Backend Requirements — Preliminary

The backend shall provide the infrastructure required to support IoT devices and GUI clients.

At a minimum, the backend is expected to provide:

- authentication;
- authorization;
- user management;
- plant management;
- device management;
- device configuration;
- telemetry ingestion;
- telemetry history;
- watering event storage;
- synchronization;
- device status;
- local-server operation.

The detailed backend specification will be developed separately.

---

# 18. GUI Requirements — Preliminary

The GUI shall provide:

- user authentication;
- plant management;
- device management;
- current sensor values;
- telemetry history;
- watering history;
- device configuration;
- measurement interval configuration;
- sleep mode configuration;
- watering configuration;
- device status.

The GUI shall be implemented using the project's planned Blazor architecture:

- Blazor MAUI;
- Blazor WebAssembly;
- Interactive Server.

The exact distribution of functionality between these technologies is TBD.

---

# 19. Multi-device Support

A user may own multiple plants and IoT devices.

The GUI shall provide a mechanism for switching between devices/plants.

Example:

```text
User
 │
 ├── Monstera
 │     └── Device 001
 │
 ├── Ficus
 │     └── Device 002
 │
 └── Cactus
       └── Device 003
```

The exact UI for device switching is TBD.

---

# 20. Non-functional Requirements

## 20.1 Reliability

Loss of network connectivity shall not prevent autonomous watering.

## 20.2 Data durability

Telemetry collected during offline operation shall not be intentionally discarded merely because synchronization is unavailable.

## 20.3 Recoverability

The device shall attempt to recover network connectivity without requiring manual intervention.

## 20.4 Energy efficiency

The default device behavior shall prioritize low power consumption through deep sleep.

## 20.5 Configurability

Important operational parameters shall be configurable without modifying firmware where technically feasible.

## 20.6 Maintainability

The software architecture shall separate:

- sensor acquisition;
- watering logic;
- storage;
- communication;
- synchronization;
- device configuration;
- power management.

## 20.7 Extensibility

The architecture should allow additional sensors and watering strategies to be introduced without redesigning the entire system.

---

# 21. Security — Preliminary

The final system shall consider:

- device identity;
- authentication;
- authorization;
- secure storage of credentials;
- secure communication;
- protection against unauthorized device configuration;
- protection of user data.

The exact security mechanisms are TBD.

---

# 22. Open Research Questions

The following decisions are deliberately not finalized in v0.1.

## 22.1 Watering algorithm

Research is required to determine:

- optimal moisture threshold;
- target moisture;
- watering portion duration;
- pause duration;
- maximum number of portions;
- minimum interval between watering procedures;
- sensor response after watering;
- conditions for terminating watering.

## 22.2 Moisture sensor calibration

Research is required to determine:

- conversion from sensor reading to meaningful moisture value;
- calibration procedure;
- influence of soil type;
- sensor stability;
- acceptable measurement error;
- effects of corrosion associated with resistive sensors.

## 22.3 Measurement interval

Research is required to determine appropriate minimum and maximum measurement intervals considering:

- plant behavior;
- sensor behavior;
- required history resolution;
- power consumption;
- watering algorithm.

## 22.4 Communication protocol

TBD:

- MQTT;
- HTTP;
- another protocol;
- or a combination.

The decision shall consider:

- ESP32 support;
- offline operation;
- message delivery;
- low power consumption;
- local server support;
- security;
- implementation complexity.

## 22.5 Telemetry format

TBD:

- JSON;
- MessagePack;
- another serialization format.

## 22.6 Local storage

TBD:

- filesystem;
- database;
- key-value storage;
- custom binary storage.

Storage limits and retention policy also require investigation.

## 22.7 Synchronization

TBD:

- batching;
- acknowledgement;
- duplicate detection;
- ordering;
- retry strategy;
- conflict resolution;
- configuration versioning.

## 22.8 Provisioning

TBD:

- Wi-Fi configuration;
- device identification;
- device registration;
- user-device pairing;
- local discovery.

## 22.9 Fully local server

TBD:

- how the local server is deployed;
- how the IoT device discovers it;
- how the GUI connects to it;
- whether the local server can later synchronize with a central server.

---

# 23. Requirements and Acceptance Criteria

The following initial requirements are defined.

### REQ-IOT-001 — Periodic measurement

**Requirement:**  
The IoT device shall periodically collect measurements according to `MeasurementInterval`.

**Acceptance criteria:**

- Given the device is operational;
- When `MeasurementInterval` expires;
- Then the device performs a new measurement cycle.

---

### REQ-IOT-002 — Telemetry collection

**Requirement:**  
The device shall collect soil moisture, temperature, air humidity and light measurements.

**Acceptance criteria:**

- Each completed measurement cycle produces a telemetry record;
- The record contains all four measurements and a timestamp.

---

### REQ-IOT-003 — Moisture-based watering

**Requirement:**  
The device shall use soil moisture as the primary input for automatic watering.

**Acceptance criteria:**

- Given soil moisture does not require watering;
- Then the pump remains inactive.

- Given soil moisture requires watering;
- Then the watering procedure is initiated.

---

### REQ-IOT-004 — Portion-based watering

**Requirement:**  
The device shall perform watering in portions with a pause between portions.

**Acceptance criteria:**

- The pump is activated for the configured portion duration;
- The pump is deactivated;
- The device waits for the configured pause;
- The device measures soil moisture again;
- The procedure terminates when the configured stopping condition is met or the maximum number of portions is reached.

---

### REQ-IOT-005 — Autonomous watering

**Requirement:**  
The device shall be capable of executing the watering algorithm without an active server connection.

**Acceptance criteria:**

- Disconnecting the server does not prevent automatic watering;
- The device continues to use its locally stored configuration.

---

### REQ-IOT-006 — Local telemetry storage

**Requirement:**  
The device shall locally store telemetry when synchronization is unavailable.

**Acceptance criteria:**

- Given the server is unavailable;
- When a measurement is performed;
- Then the telemetry record is stored locally.

---

### REQ-IOT-007 — Synchronization

**Requirement:**  
The device shall synchronize locally stored data when server connectivity becomes available.

**Acceptance criteria:**

- Previously unsynchronized telemetry is transmitted;
- Previously unsynchronized watering events are transmitted;
- Successfully synchronized records are not unnecessarily retransmitted.

The exact duplicate-detection strategy is TBD.

---

### REQ-IOT-008 — Deep sleep

**Requirement:**  
The device shall use deep sleep as its default power-saving mode.

**Acceptance criteria:**

- Given `SleepEnabled = true`;
- When the measurement cycle is complete;
- Then the device enters deep sleep.

---

### REQ-IOT-009 — Sleep configuration

**Requirement:**  
The user shall be able to enable or disable sleep mode through configuration.

**Acceptance criteria:**

- Given `SleepEnabled = false`;
- When the active cycle finishes;
- Then the device does not enter deep sleep.

---

### REQ-IOT-010 — Multi-device support

**Requirement:**  
A user shall be able to manage multiple plants and associated IoT devices.

**Acceptance criteria:**

- A user can associate multiple devices with their plants;
- The GUI allows the user to switch between them.

---

# 24. Versioning Policy

This document is a living specification.

Changes shall be reflected by increasing the specification version.

Example:

```text
v0.1 — Initial specification
v0.2 — Backend architecture added
v0.3 — Communication protocol defined
v0.4 — Database and synchronization defined
v0.5 — GUI specification added
v1.0 — Final agreed specification
```

Decisions marked `TBD`, `Research` or `Proposed` may be changed without being considered a contradiction of the specification.

Once a decision is experimentally or architecturally justified, it shall be moved into the defined requirements.

---

# 25. Current Architecture Principle

The central architectural principle of AWPS is:

> **The IoT device must remain capable of performing its primary function independently of server availability.**

Therefore, the server primarily provides:

- persistence;
- history;
- configuration management;
- synchronization;
- user interaction;
- centralized management.

The IoT device remains responsible for:

- sensing;
- local telemetry storage;
- watering decisions;
- pump control;
- offline operation;
- power management.

This separation allows AWPS to operate across central-server, local-server and fully autonomous scenarios.