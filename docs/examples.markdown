---
layout: page
title: Examples
permalink: /examples/
---

# Outline

- [Examples](#examples)
  - [Bucket tool](#bucket-tool)
  - [Transformations](#transformations)
  - [Bezier curves](#bezier-curves)
  - [Poligon self intersections](#poligon-self-intersections)
  - [Composite shape](#composite-shape)
  - [Texture merging](#texture-merging)
- [Drawings](#drawings)
  - [Optical pentagon](#optical-pentagon)
  - [Random texture spawining](#random-texture-spawining)
  - [Ellipses](#ellipses)
  - [Geometric flower](#geometric-flower)


## Examples

### Bucket tool

As an example, the bucket tool is applied at coordinates `(0,0)` to the first image. The output is on the other three images at incresing level of sensitivity.


![bucket1](images/Bucket_exe1.png){:style="display:block; margin-left:auto; margin-right:auto" height="200px" width="200px"} | ![bucket2](images/Bucket_exe2.png){:style="display:block; margin-left:auto; margin-right:auto" height="200px" width="200px"}

![bucket3](images/Bucket_exe3.png){:style="display:block; margin-left:auto; margin-right:auto" height="200px" width="200px"} | ![bucket4](images/Bucket_exe4.png){:style="display:block; margin-left:auto; margin-right:auto" height="200px" width="200px"}

```csharp
SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
Drawer drawer = new Drawer(spriteRenderer);
drawer.NewEmptySprite(4, 4, 100, Color.white);

LineStyle style = new LineStyle(2.0f/100.0f, new Color(0.6f, 0.6f, 0.6f));
BrokenLine line = new BrokenLine(new Vector2[2] { new Vector2(4,0), new Vector2(0,4)}, false, style);
drawer.Draw(line);

PoligonRegular pentagon = new PoligonRegular(5, new Vector2(2, 2), new Vector2(2, 2), Color.black);
drawer.Draw(pentagon);

DrawnSprite draw = GetComponent<DrawnSprite>();
draw.SavePng("Bucket_exe1");

drawer.Bucket(new Vector2(0,0), Color.yellow, 0.0f);
draw.SavePng("Bucket_exe2");

draw.Undo(); // Undoing the last bucket
drawer.Bucket(new Vector2(0,0), Color.yellow, 0.5f);
draw.SavePng("Bucket_exe3");

draw.Undo(); // Undoing the last bucket
drawer.Bucket(new Vector2(0,0), Color.yellow, 1);
draw.SavePng("Bucket_exe4");
```

### Transformations

The initial pentagon is first expanded through a deformation, then translated and then rotated.
Note that deformation and rotation are with respect the relative center of the poligon which, by definition, is the center of the minimal rectangle containing the verices.
In this case this center does not correspond to the circle-center of the pentagon (the center of the circle in which the poligon is inscribed).
This results in asymmetries in the deformation and rotation.

![transf1](images/Transf_exe1.png){:style="display:block; margin-left:auto; margin-right:auto" height="200px" width="200px"} | ![transf2](images/Transf_exe2.png){:style="display:block; margin-left:auto; margin-right:auto" height="200px" width="200px"} 

![transf3](images/Transf_exe3.png){:style="display:block; margin-left:auto; margin-right:auto" height="200px" width="200px"} | ![transf4](images/Transf_exe4.png){:style="display:block; margin-left:auto; margin-right:auto" height="200px" width="200px"}

```csharp
SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
Drawer drawer = new Drawer(spriteRenderer);
drawer.NewEmptySprite(4, 4, 100, Color.white);

PoligonRegular pentagon = new PoligonRegular(5, new Vector2(1, 1), new Vector2(1, 1), new Color(1, 0, 0, 0.5f));
drawer.Draw(pentagon);
DrawnSprite draw = GetComponent<DrawnSprite>();
draw.SavePng("Transf_exe1");

pentagon.Deform(Axis.x, 2f);
pentagon.Deform(Axis.y, 2f);
drawer.Draw(pentagon);
draw.SavePng("Transf_exe2");

pentagon.Translate(new Vector2(2, 2));
drawer.Draw(pentagon);
draw.SavePng("Transf_exe3");

pentagon.Rotate(Mathf.PI / 5.0f, Vector2.zero);
drawer.Draw(pentagon);
draw.SavePng("Transf_exe4");
```

### Bezier curves

#### Quadratic curve

On the left the quadratic Bezier curve. On the right the same curve with the geometrical procedure underlying the definition of the Bezier curve. The blue points are the initial and final points (p1, p3), the red point is the control point.

![bezierQuad](images/BezierQuad_exe.png){:style="display:block; margin-left:auto; margin-right:auto" height="300px" width="450px"}

```csharp
SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
Drawer drawer = new Drawer(spriteRenderer);
drawer.NewEmptySprite(4, 3, 100, Color.white);

// First Bezier curve
Vector2 p1 = new Vector2(0.2f, 0.5f);
Vector2 p2 = new Vector2(1f, 2.8f);
Vector2 p3 = new Vector2(1.7f, 1.5f);
BezierCurve bc = new BezierCurve(p1, p2, p3, 2.0f/100f);
drawer.Draw(bc);

// Same Bezier curve translated
Vector2 translation = new Vector2(2, 0);
bc.Translate(translation);
drawer.Draw(bc);

// Envelop
p1 += translation;
p2 += translation;
p3 += translation;
LineStyle blOutStyle = new LineStyle(2.0f/100f, Color.gray);
BrokenLine blOut = new BrokenLine(new Vector2[3] { p1, p2, p3 }, false, blOutStyle);
drawer.Draw(blOut);

int nSteps = 10;
LineStyle blInStyle = new LineStyle(1.0f/100f, Color.gray);
for (int i=0; i<nSteps; i++){
    Vector2 pA = Vector2.Lerp(p1, p2, (i + 1)/(float)(nSteps + 1));
    Vector2 pB = Vector2.Lerp(p2, p3, (i + 1)/(float)(nSteps + 1));
    BrokenLine blIn = new BrokenLine(new Vector2[2] { pA, pB }, false, blInStyle);
    drawer.Draw(blIn);
}

// Highlighting the Bezier curve points
Circle point1Circ = new Circle(p1, 0.04f, Color.blue);
drawer.Draw(point1Circ);
Circle point2Circ = new Circle(p2, 0.04f, Color.red);
drawer.Draw(point2Circ);
Circle point3Circ = new Circle(p3, 0.04f, Color.blue);
drawer.Draw(point3Circ);

DrawnSprite draw = GetComponent<DrawnSprite>();
draw.SavePng("BezierQuad");
```

#### Cubic curve

On the left the cubic Bezier curve. On the right the same curve with the geometrical procedure underlying the definition of the Bezier curve. The blue points are the initial and final points (p1, p4), the red points are the control points.

![bezierCub](images/BezierCubic_exe.png){:style="display:block; margin-left:auto; margin-right:auto" height="300px" width="450px"}

```csharp
SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
Drawer drawer = new Drawer(spriteRenderer);
drawer.NewEmptySprite(4, 3, 150, Color.white);

// First Bezier curve
Vector2 p1 = new Vector2(0.2f, 0.5f);
Vector2 p2 = new Vector2(0.4f, 2.8f);
Vector2 p3 = new Vector2(1.7f, 1.8f);
Vector2 p4 = new Vector2(1.5f, 0.7f);
BezierCurve bc = new BezierCurve(p1, p2, p3, p4, 2.0f/100f);
drawer.Draw(bc);

// Same Bezier curve translated
Vector2 translation = new Vector2(2, 0);
bc.Translate(translation);
drawer.Draw(bc);

// Envelop
p1 += translation;
p2 += translation;
p3 += translation;
p4 += translation;
LineStyle blOutStyle = new LineStyle(2.0f/100f, Color.gray);
BrokenLine blOut = new BrokenLine(new Vector2[4] { p1, p2, p3, p4 }, false, blOutStyle);
drawer.Draw(blOut);

int nSteps = 12;
LineStyle blInStyle = new LineStyle(1.0f/100f, Color.white * 0.85f);
LineStyle blIn1Style = new LineStyle(1.0f/100f, Color.gray);
for (int i=0; i<nSteps; i++){
    Vector2 pA = Vector2.Lerp(p1, p2, (i + 1)/(float)(nSteps + 1));
    Vector2 pB = Vector2.Lerp(p2, p3, (i + 1)/(float)(nSteps + 1));
    Vector2 pC = Vector2.Lerp(p3, p4, (i + 1)/(float)(nSteps + 1));
    BrokenLine blIn = new BrokenLine(new Vector2[3] { pA, pB, pC }, false, blInStyle);
    Vector2 pA1 = Vector2.Lerp(pA, pB, (i + 1)/(float)(nSteps + 1));
    Vector2 pB1 = Vector2.Lerp(pB, pC, (i + 1)/(float)(nSteps + 1));
    BrokenLine blIn1 = new BrokenLine(new Vector2[2] { pA1, pB1 }, false, blIn1Style);
    drawer.Draw(blIn);
    drawer.Draw(blIn1);
}

// Highlighting the Bezier curve points
Circle point1Circ = new Circle(p1, 0.04f, Color.blue);
drawer.Draw(point1Circ);
Circle point2Circ = new Circle(p2, 0.04f, Color.red);
drawer.Draw(point2Circ);
Circle point3Circ = new Circle(p3, 0.04f, Color.red);
drawer.Draw(point3Circ);
Circle point4Circ = new Circle(p4, 0.04f, Color.blue);
drawer.Draw(point4Circ);

DrawnSprite draw = GetComponent<DrawnSprite>();
draw.SavePng("BezierCubic");
```

### Poligon self intersections

### Composite shape

### Texture merging

## Drawings

### Optical pentagon

### Random texture spawining

### Ellipses

### Geometric flower

