using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

namespace CivilTopoPlugin.Civil3D;

public class Civil3DIntegrationService
{
    public void EnriquecerAltitudesComSuperficie(CivilDocument civilDoc, List<Point3d> pontos, ObjectId surfaceId)
    {
        try
        {
            var db = civilDoc.Database;
            using var tr = db.TransactionManager.StartTransaction();
            var surf = (TinSurface)tr.GetObject(surfaceId, OpenMode.ForRead);
            for (int i = 0; i < pontos.Count; i++)
                pontos[i] = new Point3d(pontos[i].X, pontos[i].Y, surf.FindElevationAtXY(pontos[i].X, pontos[i].Y));
            tr.Commit();
        }
        catch { }
    }

    public List<Point3d> LerVerticesDeParcela(CivilDocument civilDoc)
    {
        var pts = new List<Point3d>();
        try
        {
            using var tr = civilDoc.Database.TransactionManager.StartTransaction();
            foreach (ObjectId siteId in civilDoc.GetSiteIds())
            {
                var site = (Site)tr.GetObject(siteId, OpenMode.ForRead);
                foreach (ObjectId parcelId in site.GetParcelIds())
                {
                    var parcel = (Parcel)tr.GetObject(parcelId, OpenMode.ForRead);
                    foreach (ParcelSegment seg in parcel.ParcelSegments)
                    {
                        pts.Add(seg.StartPoint); pts.Add(seg.EndPoint);
                    }
                }
            }
            tr.Commit();
        }
        catch { }
        return pts.Distinct().ToList();
    }

    public List<ObjectId> ListarSuperficies(CivilDocument civilDoc)
    {
        try { return civilDoc.GetSurfaceIds().Cast<ObjectId>().ToList(); } catch { return new List<ObjectId>(); }
    }

    public ObjectId SelecionarSuperficie(CivilDocument civilDoc)
    {
        try { return ListarSuperficies(civilDoc).FirstOrDefault(); } catch { return ObjectId.Null; }
    }
}
