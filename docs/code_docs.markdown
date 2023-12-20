---
layout: page
title: Code Documentation
permalink: /codedocs/
---

# Outline

- [Intro and typical workflow](#Typical-workflow)
- [Drawer](#Drawable-elements)
    - [Constructor](#Constructor)
    - [Create an empty sprite](#Create-an-empty-sprite)
    - [Draw](#Draw)
    - [Bucket tool](#Bucket-tool)
- [Drawable elements](#Drawable-elements)
    - [IDrawable interface](#[IDrawable-interface)
      - [Transformations](#Transformations)
      - [Copy](#Copy)
  - [Lines](#Lines)
    - [Broken line](#Broken-line)
    - [Bezier curve](#Bezier-curve)
  - [Shapes](#Shapes)
    - [Circle](#Circle)
    - [Circular sector](#Circular-sector)
    - [Ellipse](#Ellipse)
    - [Ellipse sector](#Ellipse-sector)
    - [Poligon](#Poligon)
    - [Regular poligon](#Regular-poligon)
  - [Textures](#Textures)
- [DrawnSprite component](#DrawnSprite-component)


# Typical workflow

This package contains a few function to draw geometric object or textures over another texture contained in a 
`SpriteRenderer`.
The workflow of **GeomDraw** typically consists in the following steps:
- Create an object of the class `Drawer`, associating it with the `SpriteRenderer` over which you want to draw
- Create a geometric object (or a texture) belonging to the `IDrawable` class
- Use the `Drawer` object to draw the `IDrawable`
- Save the texture as png using from the  `DrawnSprite` class that has been added to the `SpriteRenderer` gameObject.

```csharp
// We imagine that the script is attached to the gameObject containing the SpriteRenderer
SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

// Creating the Drawer object
Drawer drawer = new Drawer(spriteRenderer);

// Creating a new black empty sprite in the SpriteRenderer. Size 4x4 and 100 pixels per unit
drawer.NewEmptySprite(4, 4, 100, Color.black);

// Creating a white pentagon
Vector2 center = new Vector2(2, 2);
Vector2 scale = new Vector2(2, 2);
PoligonRegular pentagon = new PoligonRegular(5, center, scale, Color.white);

// Using the Drawer object to draw
drawer.Draw(pentagon);

// Saving the image as png
DrawnSprite drawn = GetComponent<DrawnSprite>();
drawn.SavePng("pentagon");
```
*Output*

![pentagon](images/pentagon2.png){:style="display:block; margin-left:auto; margin-right:auto" height="200px" width="200px"}

# Drawer

#### Constructor

The `Drawer` class allows you to draw over the texture attached to a `SpriteRenderer`, which has to be specified through the contructors.
```csharp
public Drawer(SpriteRenderer spriteRenderer);
```

#### Create an empty sprite

The function `NewEmptySprite` substitutes the current texture of the `SpriteRenderer` (or creates a new one if empty) with a new mono-color texture of given size and pixels per units.
```csharp
public void NewEmptySprite(
    float width,
    float height,
    float pixelsPerUnity,
    Color backgroundColor
);
```
`width` and `height` are in world units, `pixelsPerUnity` is the factor epressing the number of pixels per world unit, `backgroundColor` sets the color.

#### Draw

The main function `Draw` edits the texture of the `SpriteRenderer` drawing on it a geometric element or a texture. 
These elements are istances of `IDrawable` and can be created as discussed below. 
```csharp
public void Draw(IDrawable drawable, bool updateDrawnSprite = true);
```
updateDrawnSprite flags if the class DrawnSprite should be updated with this operation. This class will be introduced later.


#### Bucket tool

The bucket tool colors all the neighbours pixels of a point having a "similar" color of the pixel at that point.
```csharp
public void Bucket(Vector2 point, Color color, float sensitivity);
```
The point has coordinates in world units, and the origin is the bottom left corner of the texture.
sensitivity is the parameter that sets how much similar is the neighbouring color to be considered as a neighbours. It is normalized between 0 and 1.

As an example, the bucket tool is applied at coordinates `(0,0)` to the first texture. The output is the second image.

![bucket1](images/bucket_exe1.png){:style="display:block; margin-left:auto; margin-right:auto" height="200px" width="200px"} | ![bucket2](images/bucket_exe2.png){:style="display:block; margin-left:auto; margin-right:auto" height="200px" width="200px"}

# Drawable elements

## IDrawable interface

All the drawable elements implement the `IDrawable` interface and share a series of common functions. 
The main group can geometrically transform the shape.

#### Transformations

The translation moves the drawable of an amount and direction specified by the vector in the argument (in world units).
```csharp
public void Translate(Vector2 translation);
```

This rotates the drawable of an angle in radiants with respect to a rotational center. 
If `isRelative` the rotational center is relative to the center of the rectangle that contains the 
drawable, otherwise the rotational center is in world coordinates, with the origin as the bottom left
corner of the modified texture.
```csharp
public void Rotate(float radAngle, Vector2 rotCenter, bool isRelative);
```

Reflection of the drawable with respect to the x or y axis (`Axis` is an enumerator that can be
`Axis.x` or `Axis.y`).
`coord` specifies the y or x coordinate of the reflection axis.
As for the rotation,  `isRelative` sets if this coordinate is relative to the rectangle containing
the drawable or the texture coordinates.
```csharp
public void Reflect(Axis axis, float coord = 0, bool isRelative = true);
```

Deformation of the drawable along the `axis` of a given `factor`.
`coord` specifies the y or x coordinate of the deformation axis, which is the line that 
does not change position during the deformation.
`isRelative` sets if this coordinate is relative to the rectangle containing
the drawable or the texture coordinates.
```csharp
public bool Deform(Axis axis, float factor, float coord = 0, bool isRelative = true);
```

#### Copy

The following function performs a deep copy of the drawable element.
```csharp
public IDrawable Copy();
```

## Lines

There are two types of lines: the broken lines and the Bezier curves, shown below.
Their *style* can be specified by the following class that allows you to choose the line 
`thickness` in world units and its `color`.
```csharp
public LineStyle(float thickness, Color color);
```

#### Broken line

A broken line connects a list of `points` with segments. The points coordinates are in world units with the origin on the left bottom corner of the texture.
Specitying `isClosed` the last and the first points will be connected.
```csharp
public BrokenLine(Vector2[] points, bool isClosed, LineStyle style);

// Constructor with black line
public BrokenLine(Vector2[] points, bool isClosed, float thickness);
```

#### Bezier curve

GeomDraw draws quadratic and cubic Bezier curves (https://en.wikipedia.org/wiki/B%C3%A9zier_curve).
All the constructors use the convention that the first and the last points are the two points connected by the curve, the intermediate points (one for the quadratic and two for the cubic) are the control points defining the curvature.
```csharp

// Quadratic Bezier curve, starting from p1, ending in p3
public BezierCurve(Vector2 p1, Vector2 p2, Vector2 p3, LineStyle style);

// black line
public BezierCurve(Vector2 p1, Vector2 p2, Vector2 p3, float thickness);

// Cubic Bezier curve, starting from p1, ending in p4
public BezierCurve(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, LineStyle style);

// black line
public BezierCurve(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float thickness);
```
        
### Shapes

#### Circle

```csharp
public Circle(Vector2 center, float radius, Color color, LineStyle borderStyle = new LineStyle());
```

#### Circular sector

```csharp
public CircularSector(Vector2 center, float radius, float angle1, float angle2, Color color, LineStyle borderStyle);
```

#### Ellipse

```csharp
public Ellipse(
    Vector2 center,
    float semiAxisX,
    float semiAxisY,
    float rotationAngle, 
    Color color,
    LineStyle borderStyle = new LineStyle()
);

// No rotation
public Ellipse(Vector2 center, float semiAxisX, float semiAxisY, Color color, LineStyle borderStyle = new LineStyle());
```

#### Ellipse sector

```csharp
public EllipseSector(Vector2 center,
    float semiAxisX,
    float semiAxisY, 
    float startAngle,
    float endAngle,
    float rotationDegAngle,
    Color color,
    LineStyle borderStyle
);
```

#### Poligon

```csharp
public Poligon(Vector2[] vertices, Color color, LineStyle lineStyle = new LineStyle());
```

#### Regular poligon

```csharp
public PoligonRegular(int nVertices, Vector2 center, Vector2 scale, float rotation, Color color, LineStyle lineStyle = new LineStyle());

// No rotation
public PoligonRegular(int nVertices, Vector2 center, Vector2 scale, Color color, LineStyle lineStyle = new LineStyle());
```

#### Composite shape

```csharp
public CompositeShape(IDrawableLine[] lines, Color color, LineStyle lineStyle)
```

### Textures

## DrawnSprite component
