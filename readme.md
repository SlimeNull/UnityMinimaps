# UnityMinimaps

Allows for quick creation of mini-maps in Unity. It supports static textures, dynamic camera rendering, target following, rotation, scaling, landmarks, and click navigation functionalities.

## Usage

To create a minimap, you can click the "Tools/Minimap" in menu bar, set minimap type and other options, then select any object under 'Canvas' in hierarchy, and click "Create" button.

### Static texture minimap

The display of "Static texture minimap" is based on pre-captured terrain images, which are then manipulated through rotation, scaling, and translation to achieve the desired display. During creation, you also need to specify a "texture area size," which represents how large of an area the texture covers on the map.

To create map textures, you can do so in the "Texture tool" section within the minimap tools. Specify the texture size, camera height, mini-map area size, and the layers you want to capture. Finally, click on "Generate Mini-Map Texture."

### Render texture minimap

The texture of "Render texture minimap" comes from the output of the rendering camera, and the area size and rotation are automatically obtained from the camera. However, it must be ensured that the camera is facing vertically downward from above.

The position of the camera can be anywhere, so you can also directly attach the camera as a child object of the player.

### Minimap modifier

The minimap modifier allows users to zoom in and out of the map by hovering the pointer over the minimap and scrolling the mouse wheel.

### Minimap renderer

The minimap renderer is used to draw the content of the map, it draws based on the size, texture source, and origin set by the minimap component. If you need to use a minimap with polygon shapes, you can delete the default renderer that comes with the mini-map created by the Wizard, and manually add "MinimapPolygonRenderer". It will render the mini-map in polygon form, and with enough vertices, it can also be used as a circle.

### Minimap navigator

The minimap navigator is used to provide the function of clicking on the map for navigation. In the Inspector, specify the "NavMeshAgent" to navigate, click the mouse, and automatically set the target point of the specified "NavMeshAgent" to the location clicked on the map by the mouse.

### Minimap indicator

A minimap indicator is used to mark objects in world coordinates on a minimap. Specify the object to be marked and the icon prefab in the Inspector. When the object enters the display range of the minimap, the component will create an icon based on the prefab and display it at the corresponding position on the minimap. When "Apply Rotation" is enabled for the item being displayed, the direction of the icon will follow that of the object.

### Minimap navigation path renderer

The minimap navigation path renderer is used to display the path line for a specified "NavMeshAgent" on the map.
