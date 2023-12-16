---
layout: page
title: Code Documentation
permalink: /codedocs/
---

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

### Draw a geometric object or another texture

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


## Drawable elements

## DrawnSprite component
