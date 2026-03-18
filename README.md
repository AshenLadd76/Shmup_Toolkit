## Overview

This shmup toolkit is designed to help developers create Shmup and bullet-hell style games quickly and efficiently. It provides a modular and flexible foundation for building projectile patterns, gameplay systems, and levels, while remaining lightweight and easy to integrate. The project is ongoing and actively evolving.


### Completed
- **Optimized Code** – High-performance systems designed to handle large numbers of projectiles efficiently.  
- **Custom Collision System** – Supports pluggable collision algorithms via ScriptableObjects, allowing flexible and extensible hit detection.  
- **Configurable Spatial Partitioning** – Optimizes collision checks by dividing the game space into customizable partitions, fully compatible with the collision system.  
- **Centralized Data-Driven Updates** – Projectiles are fully data-driven, with position and rotation updated in batches through a single central update loop for performance and clarity.  
- **Shmup-Focused Tilemap Editor** – Imports an image to generate unique tiles, creates a sliced sprite sheet, builds a JSON map of the image, and instantiates a fully placed Tilemap in the scene, streamlining level design.
