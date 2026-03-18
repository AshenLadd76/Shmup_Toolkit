## Overview

The Shmup Toolkit is designed to help developers create Shmup and bullet-hell style games quickly and efficiently. It provides a modular and flexible foundation for building projectile patterns, gameplay systems, and levels, while remaining lightweight and easy to integrate. The toolkit features a **custom collision detection system** and a **centralized update loop for moving projectiles**, enabling high-performance handling of thousands of bullets without relying on Unity’s physics or colliders. The project is ongoing and actively evolving.

<br>

### ✨ Features

- ✅ Custom collision system with pluggable algorithms  
- ✅ Configurable spatial partitioning system  
- ✅ Central update loop with batch projectile movement
- ✅ Tilemap builder editor tool (work in progress)   
- 🔄 Pattern generator tool (work in progress)
- ⏳ GPU instancing (pending implementation)

### Components

<br>
## ✨ Spatial Partitioning System

- Divides the game world into a grid of cells to efficiently organize collision objects.
- Each cell tracks active objects using `HashSet<ICollisionObject>`, reducing unnecessary collision checks.
- Supports **pluggable collision algorithms**, making it flexible for different projectile types.
- Objects are updated only when they move between cells, minimizing overhead.
- Completely independent of Unity’s physics and colliders — lightweight and optimized.






