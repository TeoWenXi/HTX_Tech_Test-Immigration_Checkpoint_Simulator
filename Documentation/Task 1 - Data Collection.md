# Task 1 – Data Collection

## Objective
Extract key pedestrian behavioural parameters from a top-down video of people walking to define baseline inputs for a crowd simulation system representing checkpoint conditions.

## Approach
- Observed a top-down pedestrian video  
- Identified movement patterns such as speed, spacing, flow, and grouping  
- Used visual sampling of representative frames and time intervals to estimate behavioural parameters  

## Observed Behavioural Parameters

| Variable        | Description                                                  | Influence on Simulation Design          |
|----------------|--------------------------------------------------------------|----------------------------------------|
| Walking Speed  | Average speed at which pedestrians move through the environment | Controls agent movement speed          |
| Arrival Rate   | Number of pedestrians entering the environment over time     | Agent spawning controller              |
| Personal Space | Average spacing maintained between pedestrians               | Collision avoidance / crowd spacing    |
| Group Size     | Number of people travelling together                         | Group movement behaviour              |

## Key Observations
- Pedestrians generally move at steady, moderate speeds with minor variation  
- Movement is continuous rather than burst-based entry  
- Majority of pedestrians are individuals, with occasional pairs  
- In immigration contexts, higher proportions of group travel are expected  
- Flow direction is generally consistent with mild bidirectional movement  