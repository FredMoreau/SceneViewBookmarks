# SceneView Bookmarks
 Scene View shortcuts to bookmark view positions and states (layer visibility and locked state, shading mode, lighting, effects, grid visibility, etc).

# Install through the Package Manager
1. Go to Window/Package Manager.
2. Click on the + icon and select "Add package from git URL...".
2. Copy/paste the following url in the field and click on Add.
- git@github.com:FredMoreau/SceneViewBookmarks.git

# Features
- Quick Access Menu (Ctrl + Right Click in SceneView).
- Save/Load SceneView Bookmarks (position, direction, ortho/persp, 2D, Shading Mode, View States (Skybox, Fog, etc), grid, gizmos, visible and locked layers).
- Built-in Views (Perspective, Top, Front, UI).
- Cameras/Cinemachine Virtual Cameras shortcuts.
- Filters (LOD Group, Collider, Renderer, Metadata).
- Bookmarks Editor Window.
- Link Main Camera (or current Virtual Camera) to Scene View.

# Known Limitations
- If a Camera is Linked and a Bookmark is recalled, the Camera is moved without warning.

# Future Improvements
- Saving to Editor / Project Settings (currently saves to Assets/SceneViewBookmarks.json).
- A better Editor View with Reorderable List.