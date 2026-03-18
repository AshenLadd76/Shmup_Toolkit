## Overview

The Shmup Toolkit is designed to help developers create Shmup and bullet-hell style games quickly and efficiently. It provides a modular and flexible foundation for building projectile patterns, gameplay systems, and levels, while remaining lightweight and easy to integrate. The toolkit features a **custom collision detection system** and a **centralized update loop for moving projectiles**, enabling high-performance handling of thousands of bullets without relying on Unity’s physics or colliders. The project is ongoing and actively evolving.

<br>


### ⚡ Optimized for Performance

 The Shmup Toolkit is designed from the ground up to handle high-performance bullet hell scenarios efficiently:

- **Data-driven architecture:** Minimal GameObjects, no Unity physics or colliders, and batch-updated projectile positions.  
- **High-performance collision:** Spatial partitioning system reduces O(n²) checks; collisions are calculated per grid cell only.  
- **Cache-friendly updates:** Central movement loop and scratch buffers avoid per-frame allocations and GC overhead.  
- **Efficient memory management:** Swap-remove pattern for active projectiles ensures O(1) removal and keeps position buffers aligned.  
- **Scalable design:** Capable of handling tens of thousands of projectiles at 60 FPS on standard hardware.  
- **Render-aware optimization:** Prepares for GPU instancing to minimize draw calls and transform overhead.  
- **Forward-looking architecture:** Jobs + Burst support considered for future multi-threaded computation when scaling beyond 50k projectiles.  

<br>

### ✨ Features

- ✅ Custom collision system with pluggable algorithms  
- ✅ Configurable spatial partitioning system  
- ✅ Central update loop with batch projectile movement
- ✅ Tilemap builder editor tool (work in progress)   
- 🔄 Pattern generator tool (work in progress)
- ⏳ GPU instancing (pending implementation)   






