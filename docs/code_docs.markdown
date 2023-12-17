---
layout: page
title: Code Documentation
permalink: /codedocs/
---

## Outline

- [Intro and typical workflow](#Typical-workflow)
- [Drawer](#Drawable-elements)
  - [Constructor](#Constructor)
  - [Create an empty sprite](#Create-an-empty-sprite)
  - [Draw](#Draw)
  - [Bucket tool](#Bucket-tool)
- [Drawable elements](#Drawable-elements)
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


## Typical workflow

Drawing a geometric object on a sprite with **GeomDraw** typically consists in the following steps:
- create an object of the class `Drawer`, associating it with a `SpriteRenderer` over which you want to draw
- Create a geometric object (or a texture) belonging to the `IDrawable` class
- Use the `Drawer` object to draw the `IDrawable`
- (Sometimes) save the texture as png using from the  `DrawnSprite` class that has been added to the `SpriteRenderer` gameObject.

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

![pentagon](images/pentagon.png){:style="display:block; margin-left:auto; margin-right:auto" height="200px" width="200px"}

## Drawer

### Constructor

The `Drawer` class allows you to draw over the texture attached to a `SpriteRenderer`.
We first need to build an object of this class assigning a `SpriteRenderer` thorugh the constructor
```csharp
public Drawer(SpriteRenderer spriteRenderer)
```

### Create an empty sprite

The function `NewEmptySprite` substitutes the current texture of the `SpriteRenderer` (or creates a new one if empty) with a new mono-color texture of given size and pixels per units.
```csharp
public void NewEmptySprite(
    float width,
    float height,
    float pixelsPerUnity,
    Color backgroundColor
)
```
`width` and `height` are in world units, `pixelsPerUnity` is the factor epressing the number of pixels per world unit, `backgroundColor` sets the color.

### Draw

The main function `Draw` edits the texture of the `SpriteRenderer` drawing on it a geometric element or a texture. 
These elements are istances of `IDrawable` and can be created as discussed below. 
```csharp
public void Draw(IDrawable drawable, bool updateDrawnSprite = true)
```
updateDrawnSprite flags if the class DrawnSprite should be updated with this operation. This class will be introduced later.


### Bucket tool

The bucket tool colors all the neighbours pixels of a point having a "similar" color of the pixel at that point.
```csharp
public void Bucket(Vector2 point, Color color, float sensitivity)
```
The point has coordinates in world units, and the origin is the bottom left corner of the texture.
sensitivity is the parameter that sets how much similar is the neighbouring color to be considered as a neighbours. It is normalized between 0 and 1.

As an example, the bucket tool is applied at coordinates `(0,0)` to the first texture. The output is the second image.

![bucket1](images/bucket_exe1.png){:style="display:block; margin-left:auto; margin-right:auto" height="100px" width="100px"}
![bucket2](images/bucket_exe2.png){:style="display:block; margin-left:auto; margin-right:auto" height="100px" width="100px"}

## Drawable elements

### Lines

#### Broken line

#### Bezier curve

### Shapes

#### Circle

#### Circular sector

#### Ellipse

#### Ellipse sector

#### Poligon

#### Regular poligon

### Textures

## DrawnSprite component
