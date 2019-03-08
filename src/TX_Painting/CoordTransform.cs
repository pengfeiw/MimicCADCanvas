using System.Drawing;
using System.Collections.Generic;

public class CoordTransform
{
    private double m_WcsToDcs = 1.0; // Length which is mapped to 1 pixel in world coordinate system
    private double m_DcsToWcs = 1.0; // 1 / m_WcsToDcs
    private PointF m_WndOrg = new PointF(); // Point which is mapped to window origin in world coordinate system
    private double m_ZoomLimit = 100;

    //public CoordTransform() { }
    public void ViewAll(RectangleF bbox, RectangleF wndRect, int margin/*pixel*/)
    {
        if (wndRect.Width <= 2 * margin || wndRect.Height < 2 * margin)
            return;
        double horRatio = bbox.Width / (wndRect.Width - 2 * margin);
        double verRatio = bbox.Height / (wndRect.Height - 2 * margin);
        double wcsToDcs, dx, dy;
        if (horRatio > verRatio)
        {
            wcsToDcs = horRatio;
            dx = margin * wcsToDcs;
            double dcsHeight = bbox.Height / wcsToDcs;
            double dcsTopMargin = (wndRect.Height - dcsHeight) / 2.0;
            dy = dcsTopMargin * wcsToDcs;
        }
        else
        {
            wcsToDcs = verRatio;
            dy = margin * wcsToDcs;
            double dcsWidth = bbox.Width / wcsToDcs;
            double dcsLeftMargin = (wndRect.Width - dcsWidth) / 2.0;
            dx = dcsLeftMargin * wcsToDcs;
        }
        Set(bbox.Location - new SizeF((float)dx, (float)dy), wcsToDcs);
    }


    public bool Set(PointF wndOrg, double wcsToDcs)
    {
        if (wcsToDcs > m_ZoomLimit)
            return false;
        else if (wcsToDcs < 1.0 / m_ZoomLimit)
            return false;
        m_WndOrg.X = wndOrg.X;
        m_WndOrg.Y = wndOrg.Y;
        m_WcsToDcs = wcsToDcs;
        m_DcsToWcs = 1.0 / wcsToDcs;
        return true;
    }

    //--------------------将point改为了pointf
    public void Pan(PointF lastPoint, PointF curPoint)
    {
        float dx = curPoint.X - lastPoint.X;
        float dy = curPoint.Y - lastPoint.Y;
        m_WndOrg.X -= (float)(dx * m_WcsToDcs);
        m_WndOrg.Y -= (float)(dy * m_WcsToDcs);
    }

    public bool Zoom(Point mousePos, int delta)
    {
        PointF wcsMousePos = DcsToWcs(mousePos);
        const double ratio = 1.25;
        const double ratio2 = 1 / ratio;
        delta /= 120;
        double dcsToWcs, wcsToDcs;
        PointF wndOrg = new PointF();
        dcsToWcs = m_DcsToWcs;
        if (delta > 0)
        {
            for (int i = 0; i < delta; ++i)
            {
                dcsToWcs *= ratio;
            }
        }
        else
        {
            for (int i = delta; i < 0; ++i)
            {
                dcsToWcs *= ratio2;
            }
        }
        wcsToDcs = 1.0 / dcsToWcs;
        wndOrg.X = (float)(wcsMousePos.X - mousePos.X * wcsToDcs);
        wndOrg.Y = (float)(wcsMousePos.Y - mousePos.Y * wcsToDcs);
        return Set(wndOrg, wcsToDcs);
    }

    public void SetLimit(double limit)
    {
        if (limit < 10.0)
            limit = 10.0;
        m_ZoomLimit = limit;
    }
    public PointF WcsToDcs(PointF point)
    {
        double x = (point.X - m_WndOrg.X) * m_DcsToWcs;
        double y = (point.Y - m_WndOrg.Y) * m_DcsToWcs;
        return new PointF((float)x, (float)y);
    }
    public PointF DcsToWcs(PointF point)
    {
        double x = point.X * m_WcsToDcs + m_WndOrg.X;
        double y = point.Y * m_WcsToDcs + m_WndOrg.Y;
        return new PointF((float)x, (float)y);
    }
    public double WlToDl(double len)
    {
        return len * m_DcsToWcs;
    }
    public double DlToWl(double len)
    {
        return len * m_WcsToDcs;
    }
}
