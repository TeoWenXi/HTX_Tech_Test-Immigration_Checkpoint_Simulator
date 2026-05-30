# Task 3 – Simulation Implementation and Metrics

## Overview

The simulation is implemented in Unity as a frame-based, manager-driven system representing a multi-stage immigration checkpoint. Travellers progress through Security and Immigration processing stages, with optional secondary inspections determined probabilistically.

## Traveller Flow

Each traveller follows the workflow:

1. Spawn into the system
2. Move towards Security Checkpoint
3. Join Security queue
4. Complete Security processing
5. Optional Security Secondary Inspection
6. Join Immigration queue
7. Complete Immigration processing
8. Optional Immigration Secondary Inspection
9. Exit the system

Traveller movement is handled using NavMesh pathfinding.

## Queue System

### Ordered Queues
Used for Security and Immigration processing.

- Custom node-based FCFS queue system
- Slot-based queue positions
- Holding area used when queues are full

### Secondary Inspection Queues

- Implemented using `List<GameObject>`
- Unordered queue structure
- Used for secondary inspections

## Processing Logic

Service counters operate as shared processing pools.

- Travellers are assigned using a push-based model
- One traveller is processed per counter at a time
- Processing duration depends on traveller attributes
- Secondary inspections are determined probabilistically

## Runtime Configuration

The simulation includes configurable parameters for:

- Random seed
- Queue layouts
- Station configurations
- Processing times
- Secondary inspection probabilities

Parameters can be adjusted through Unity Inspector variables and runtime UI controls.

## User Interface

The simulation includes:

- Scenario Adjustment Panel
- Custom Agent Spawner
- Real-Time Metrics Dashboard

These tools support rapid testing and scenario analysis.

---

# Key Outputs and Metrics

## System Overview
- Active Travellers
- Travellers Processed
- Throughput
- Average Time in System

## Queue Performance
- Current Queue Length
- Average Queue Length
- Peak Queue Length
- Average Wait Time

## Resource Utilisation
- Counter Utilisation
- Idle Counters
- Average Counter Processing Time

## Bottleneck Detection
The bottleneck is identified as the stage with the highest average waiting time. If all average waiting times remain below a threshold, the system reports no significant bottleneck.