# GPUParticlePlayground

A DirectX 11 based boid flocking simulation.

## Features And Notes
* Requires DirectX 11
* Requires shader model 5.0 or higher.
* Normal map painting via realtime height map sampling.
  * Boids will collide with brush strokes.
  * No inter frame interpolation (fast movements can leave spotty strokes).
  * Also works as a voronoi painter.
* Boid/Physics simulation
  * Simulate up to 16,000 boids at once (for an average fps of 120 on test machine).
    * Could be increased but for ui design reasons this was an ok cap.
  * Runtime configuration to change boid simulation settings.
  * Runtime configuration to change physics simulation settings.
* Input
  * Q: Repel boids.
  * W: Restart Simulation.
  * E: Attract boids.
  * Left mouse button: brush paint.
  
## Source File Appendix
* RootRenderer
  * Executes the render loop.
  * Creates and holds reference of runtime graphics resources.
* SH_CS_BoidCompute
  * Responsible for core boid simulation logic.
  * DirectX11 Compute Shader.
  * CSBoidMain kernel executes boid simulation on the gpu with 32 items per thread group.
* SH_IMG_Drawing
  * Screen space shader for normal map painting.
  * Outputs to an accumulative drawing buffer.
  * Only computes normals for new brush strokes.
* SH_PROC_BoidMesh
  * Procedurally generates quads out of boid instances.
  * Transforms boids from screen space to clip space.
* UIUtilities
  * UI Utility classes and interfaces.
* UISetting
  * UI ISetting interface implementation for setting instantiation.
* UIHandler
  * Handles UI logic and toggles user input.
  * Acts as an interface between RootRenderer and UISetting.
