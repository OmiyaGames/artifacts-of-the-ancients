---------------------------------------------------------
---------- Line2D by Anthony Beyer (@Sinok426) ----------
---------------------------------------------------------

Version 0.1
Line2D is a more tweakable and artist friendly linerenderer than unity built-in LineRenderer (Dynamic or Static, Custom UVs, pos/width/colors for points).
Line2D is only for 2D view (XY axis). Support for other 2D axis will be added.
Note : you can safely delete the "Demo" folder. No important assets in it, just shaders/materials/textures/scenes.

----------------------------

To create a new line -> GameObject/2D Object/Line2D 
or you can just add a Line2DRenderer to an empty gameobject (mesh filter and mesh renderer will be added) -> AddComponent/Line2D/Line2DRenderer

----------------------------

The component Line2D.Line2DRenderer is the main class, you can tweak it in Editor and Runtime (if it's not static) :

GLOBAL ---------------------
bool Line2DRenderer.useWorldSpace : Use world space or local space for points position
bool Line2DRenderer.useStraightTangent : A variant for line's corners
bool Line2DRenderer.isStatic : Will disable Line2DRenderer component on Start (keep meshfilter/meshrenderer active) 
if (!isStatic) -> bool Line2DRenderer.updateVerts = Allow Runtime Update for Vertices
if (!isStatic) -> bool Line2DRenderer.updateUvs = Allow Runtime Update for UVs
if (!isStatic) -> bool Line2DRenderer.updateColors : Allow Runtime Update for Colors
Line2DRenderer.meshRenderer : public access to mesh renderer

UVS ------------------------
To tweak mesh's uvs :
float Line2DRenderer.offsetU
float Line2DRenderer.offsetV
float Line2DRenderer.tilingU
float Line2DRenderer.tilingV

POINTS ---------------------
List<Line2DPoint> Line2DRenderer.points : The list of points for the line
new Line2DPoint(Vector3 _pos, float _width, Color _color) : Constructor for a new point
Vector3 Line2DPoint.pos : local or world position
float Line2DPoint.width : width between 2 vertices
Color32 Line2DPoint.color : color of 2 vertices