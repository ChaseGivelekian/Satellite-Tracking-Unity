# Satellite Tracking and Visualization

This repository contains a Unity project that uses public APIs to track satellites in real-time and visualize their positions on a 3D globe.

## Features
- Real-time satellite tracking using public APIs from [Where The ISS At](https://wheretheiss.at/w/developer) and [N2YO](https://www.n2yo.com/api/).
- Converts the satellite's position from longitude, latitude, and altitude to Unity's 3D coordinates orbiting a globe.
- Semi-realistic Earth model using imagery from [NASA](https://visibleearth.nasa.gov/collection/1484/blue-marble) and [Michael Englyst](https://maps.drsys.eu/).
- Satellite visualization using 3D models and textures from [NASA](https://nasa3d.arc.nasa.gov/models).

## Limitations
- The project is mainly limited regarding the rate limit from the [N2YO](https://www.n2yo.com/api/) API.

## Getting Started
1. Clone the repository:
   ```bash
   git clone https://github.com/ChaseGivelekian/Satellite-Tracking-Unity
   ```
2. Open the project in Unity.
3. Build the project or run it in the Unity editor.
4. Update the API key in the [SatellitesLocation.cs](Assets/Scripts/SatellitesLocation/GetLocation/SatellitesLocation.cs) script.