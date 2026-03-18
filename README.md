## Overview

This shmup toolkit is designed to help developers create Shmup and bullet-hell style games quickly and efficiently. It provides a modular and flexible foundation for building projectile patterns, gameplay systems, and levels, while remaining lightweight and easy to integrate. The project is ongoing and actively evolving.


### ⚡ Optimized for Performance

The Shmup Toolkit is designed from the ground up to handle high-performance bullet hell scenarios efficiently:

- **Data-driven architecture:** Minimal GameObjects, no Unity physics or colliders, and batch-updated projectile positions.  
- **High-performance collision:** Spatial partitioning system reduces O(n²) checks; collisions are calculated per grid cell only.  
- **Cache-friendly updates:** Central movement loop and scratch buffers avoid per-frame allocations and GC overhead.  
- **Efficient memory management:** Swap-remove pattern for active projectiles ensures O(1) removal and keeps position buffers aligned.  
- **Scalable design:** Capable of handling tens of thousands of projectiles at 60 FPS on standard hardware.  
- **Render-aware optimization:** Prepares for GPU instancing to minimize draw calls and transform overhead.  
- **Forward-looking architecture:** Jobs + Burst support considered for future multi-threaded computation when scaling beyond 50k projectiles.  



### Completed
- **Optimized Code** – High-performance systems designed to handle large numbers of projectiles efficiently.  
- **Custom Collision System** – Supports pluggable collision algorithms via ScriptableObjects, allowing flexible and extensible hit detection.  
- **Configurable Spatial Partitioning** – Optimizes collision checks by dividing the game space into customizable partitions, fully compatible with the collision system.  
- **Centralized Data-Driven Updates** – Projectiles are fully data-driven, with position and rotation updated in batches through a single central update loop for performance and clarity.  
- **Shmup-Focused Tilemap Editor** – Imports an image to generate unique tiles, creates a sliced sprite sheet, builds a JSON map of the image, and instantiates a fully placed Tilemap in the scene, streamlining level design.

### In Progress
- **Pattern Generator Tool** – Editor tool that lets developers compose bullet patterns modularly, like building with Lego blocks.  

### Pending
- **GPU Instancing** – Designed with GPU instancing in mind, making high-volume projectile rendering straightforward.





