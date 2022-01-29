
The userControl which mimic cad canvas.Include zooming 、hightlighting and save screenshort into docx file.

# CoordTransform

首先，看一下 `CoordTransform` 类的原型，着重看一下它的成员变量的意思。

```csharp
public class CoordTransform
{   
    // 世界坐标系中一个单位长度与设备坐标系中一个单位长度的比值
    private double m_WcsToDcs = 1.0;
    // m_WcsToDcs 的倒数 
    private double m_DcsToWcs = 1.0;
    // 设备坐标系的原点，在世界坐标系中的坐标
    private PointF m_WndOrg = new PointF(); // Point which is mapped to window origin in world coordinate system
    // 缩放上下限，缩放倍数最大不超过100，最小不超过 1 / 100
    private double m_ZoomLimit = 100;

    //public CoordTransform() { }
    public void ViewAll(RectangleF bbox, RectangleF wndRect, int margin/*pixel*/){}

    public bool Set(PointF wndOrg, double wcsToDcs){}

    public void Pan(PointF lastPoint, PointF curPoint){}

    public bool Zoom(Point mousePos, int delta){}

    public void SetLimit(double limit){}
    public PointF WcsToDcs(PointF point){}
    public PointF DcsToWcs(PointF point){}
    public double WlToDl(double len){}
    public double DlToWl(double len){}
}
```

所有的图元坐标定义在世界坐标系中。 

我们以一条线段 `(0,0)`, `(100, 100)` 为例，初始状态下 `m_WndOrg = (0, 0)`, `m_WcsToDcs = 1`。

## `m_WcsToDcs` 与 `m_DcsToWcs`

> 存储世界坐标系与设备坐标系之间的长度比例关系，这个可以用来控制缩放

当 `m_WcsToDcs = 2` 时，表示世界作标系的 1 个单位长度等于设备坐标系中 2 个单位长度。 此时如果没有发生平移，即 `m_WndOrg = (0, 0)`，那么线段的设备坐标就是`(0, 0)` 和 `(200, 200)`，此时我们看到的线段就是放大了 2 倍。

## `m_WndOrg`

> 存储世界坐标系与设备坐标系位置关系，这个可以用来控制平移

当 `m_WndOrg = (100, 100)` 时，表示设备坐标系的原点在世界坐标系中 `(100, 100)` 位置。此时如果没有发生缩放，即 `m_WcsToDcs = 1`, 那么线段的设备坐标就是`(-100, -100)` 和 `(0, 0)`。
