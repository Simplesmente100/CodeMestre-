namespace CivilTopoPlugin.Geometry;

public static class CoordinateConverter
{
    private const double A = 6378137.0;
    private const double F = 1.0 / 298.257222101;
    private const double K0 = 0.9996;
    private const double E2 = F * (2 - F);
    private const double EPrime2 = E2 / (1 - E2);

    public static (double Latitude, double Longitude) UtmParaGeografico(double e, double n, int fuso, bool sul)
    {
        var x = e - 500000.0;
        var y = sul ? n - 10000000.0 : n;
        var lon0 = (fuso * 6 - 183) * Math.PI / 180.0;

        var m = y / K0;
        var mu = m / (A * (1 - E2 / 4 - 3 * Math.Pow(E2, 2) / 64 - 5 * Math.Pow(E2, 3) / 256));

        var e1 = (1 - Math.Sqrt(1 - E2)) / (1 + Math.Sqrt(1 - E2));
        var j1 = 3 * e1 / 2 - 27 * Math.Pow(e1, 3) / 32;
        var j2 = 21 * Math.Pow(e1, 2) / 16 - 55 * Math.Pow(e1, 4) / 32;
        var j3 = 151 * Math.Pow(e1, 3) / 96;
        var j4 = 1097 * Math.Pow(e1, 4) / 512;

        var fp = mu + j1 * Math.Sin(2 * mu) + j2 * Math.Sin(4 * mu) + j3 * Math.Sin(6 * mu) + j4 * Math.Sin(8 * mu);
        var c1 = EPrime2 * Math.Pow(Math.Cos(fp), 2);
        var t1 = Math.Pow(Math.Tan(fp), 2);
        var r1 = A * (1 - E2) / Math.Pow(1 - E2 * Math.Pow(Math.Sin(fp), 2), 1.5);
        var n1 = A / Math.Sqrt(1 - E2 * Math.Pow(Math.Sin(fp), 2));
        var d = x / (n1 * K0);

        var lat = fp - (n1 * Math.Tan(fp) / r1) *
            (Math.Pow(d, 2) / 2 - (5 + 3 * t1 + 10 * c1 - 4 * Math.Pow(c1, 2) - 9 * EPrime2) * Math.Pow(d, 4) / 24
            + (61 + 90 * t1 + 298 * c1 + 45 * Math.Pow(t1, 2) - 252 * EPrime2 - 3 * Math.Pow(c1, 2)) * Math.Pow(d, 6) / 720);

        var lon = lon0 +
            (d - (1 + 2 * t1 + c1) * Math.Pow(d, 3) / 6
            + (5 - 2 * c1 + 28 * t1 - 3 * Math.Pow(c1, 2) + 8 * EPrime2 + 24 * Math.Pow(t1, 2)) * Math.Pow(d, 5) / 120) / Math.Cos(fp);

        return (lat * 180 / Math.PI, lon * 180 / Math.PI);
    }

    public static (double Este, double Norte) GeograficoParaUtm(double lat, double lon, int fuso)
    {
        var latR = lat * Math.PI / 180.0;
        var lonR = lon * Math.PI / 180.0;
        var lon0 = (fuso * 6 - 183) * Math.PI / 180.0;

        var n = A / Math.Sqrt(1 - E2 * Math.Pow(Math.Sin(latR), 2));
        var t = Math.Pow(Math.Tan(latR), 2);
        var c = EPrime2 * Math.Pow(Math.Cos(latR), 2);
        var a = Math.Cos(latR) * (lonR - lon0);

        var m = A * ((1 - E2 / 4 - 3 * Math.Pow(E2, 2) / 64 - 5 * Math.Pow(E2, 3) / 256) * latR
            - (3 * E2 / 8 + 3 * Math.Pow(E2, 2) / 32 + 45 * Math.Pow(E2, 3) / 1024) * Math.Sin(2 * latR)
            + (15 * Math.Pow(E2, 2) / 256 + 45 * Math.Pow(E2, 3) / 1024) * Math.Sin(4 * latR)
            - (35 * Math.Pow(E2, 3) / 3072) * Math.Sin(6 * latR));

        var easting = K0 * n * (a + (1 - t + c) * Math.Pow(a, 3) / 6 + (5 - 18 * t + Math.Pow(t, 2) + 72 * c - 58 * EPrime2) * Math.Pow(a, 5) / 120) + 500000;
        var northing = K0 * (m + n * Math.Tan(latR) * (Math.Pow(a, 2) / 2 + (5 - t + 9 * c + 4 * Math.Pow(c, 2)) * Math.Pow(a, 4) / 24 + (61 - 58 * t + Math.Pow(t, 2) + 600 * c - 330 * EPrime2) * Math.Pow(a, 6) / 720));
        if (lat < 0) northing += 10000000;

        return (easting, northing);
    }
}
