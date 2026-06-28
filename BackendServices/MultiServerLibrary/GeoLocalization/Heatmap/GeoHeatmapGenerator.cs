using System.Text.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace MultiServerLibrary.GeoLocalization.Heatmap
{
    /// <summary>
    /// Generates world map heatmaps showing IP geolocation concentrations.
    /// Uses a Mercator projection to map geographic coordinates to image pixels.
    /// </summary>
    public class GeoHeatmapGenerator(
        int width = GeoHeatmapGenerator.DefaultWidth,
        int height = GeoHeatmapGenerator.DefaultHeight
    )
    {
        private const int DefaultWidth = 1600;
        private const int DefaultHeight = 800;

        private readonly int _mapWidth = width;
        private readonly int _mapHeight = height;

        /// <summary>
        /// Generates a heatmap image from IP geolocation data.
        /// </summary>
        /// <param name="geoLocations">Dictionary with IP addresses and their geographic data (lat, lon)</param>
        /// <returns>The heatmap image as bytes</returns>
        public async Task<byte[]> GenerateHeatmapAsync(
            Dictionary<string, (double latitude, double longitude)> geoLocations
        )
        {
            if (geoLocations == null || geoLocations.Count == 0)
                throw new ArgumentException(
                    "[GeoHeatmapGenerator] - No geolocation data provided",
                    nameof(geoLocations)
                );

            using (var image = new Image<Rgba32>(_mapWidth, _mapHeight))
            {
                var landFilePath = MultiServerLibraryConfiguration.GeoLandJsonPath;

                DrawOcean(image);
                DrawOceanGrid(image);

                if (File.Exists(landFilePath))
                    DrawWorldFromGeoJson(image, landFilePath);

                DrawHeatmap(image, BuildHeatmapData(geoLocations));

                await using var stream = new MemoryStream();
                await image.SaveAsPngAsync(stream).ConfigureAwait(false);

                return stream.ToArray();
            }
        }

        private void DrawOcean(Image<Rgba32> image)
        {
            var top = Color.ParseHex("183a4a").ToPixel<Rgba32>();
            var bottom = Color.ParseHex("0f2a38").ToPixel<Rgba32>();

            for (var y = 0; y < _mapHeight; y++)
            {
                float t = (float)y / _mapHeight;

                var r = (byte)(top.R + (bottom.R - top.R) * t);
                var g = (byte)(top.G + (bottom.G - top.G) * t);
                var b = (byte)(top.B + (bottom.B - top.B) * t);

                var color = new Rgba32(r, g, b);

                for (var x = 0; x < _mapWidth; x++)
                    image[x, y] = color;
            }
        }

        private void DrawOceanGrid(Image<Rgba32> image)
        {
            var gridColor = new Rgba32(255, 255, 255, 30);

            for (var lon = -180; lon <= 180; lon += 30)
            {
                var (x, _) = LatLonToPixel(0, lon);
                if (x < 0 || x >= _mapWidth)
                    continue;

                for (var y = 0; y < _mapHeight; y++)
                    image[x, y] = gridColor;
            }

            for (var lat = -80; lat <= 80; lat += 20)
            {
                var (_, y) = LatLonToPixel(lat, 0);
                if (y < 0 || y >= _mapHeight)
                    continue;

                for (var x = 0; x < _mapWidth; x++)
                    image[x, y] = gridColor;
            }
        }

        private void DrawWorldFromGeoJson(Image<Rgba32> image, string path)
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(path));

            foreach (var feature in doc.RootElement.GetProperty("features").EnumerateArray())
            {
                var geometry = feature.GetProperty("geometry");
                var type = geometry.GetProperty("type").GetString();

                if (type == "Polygon")
                    DrawPolygonCoordinates(image, geometry.GetProperty("coordinates"));
                else if (type == "MultiPolygon")
                {
                    foreach (var polygon in geometry.GetProperty("coordinates").EnumerateArray())
                        DrawPolygonCoordinates(image, polygon);
                }
            }
        }

        private void DrawPolygonCoordinates(Image<Rgba32> image, JsonElement coordinates)
        {
            foreach (var ring in coordinates.EnumerateArray())
            {
                var points = new List<(double lat, double lon)>();

                foreach (var coord in ring.EnumerateArray())
                    points.Add((coord[1].GetDouble(), coord[0].GetDouble()));

                FillPolygon(image, [.. points]);
            }
        }

        private void FillPolygon(Image<Rgba32> image, (double lat, double lon)[] points)
        {
            if (points.Length < 3)
                return;

            var pixelPoints = points.Select(p => LatLonToPixel(p.lat, p.lon)).ToArray();

            var minY = pixelPoints.Min(p => p.y);
            var maxY = pixelPoints.Max(p => p.y);

            minY = Math.Max(0, minY);
            maxY = Math.Min(_mapHeight - 1, maxY);

            for (var y = minY; y <= maxY; y++)
            {
                var nodes = new List<int>();

                for (var i = 0; i < pixelPoints.Length; i++)
                {
                    var (x1, y1) = pixelPoints[i];
                    var (x2, y2) = pixelPoints[(i + 1) % pixelPoints.Length];

                    if ((y1 < y && y2 >= y) || (y2 < y && y1 >= y))
                    {
                        var x = (int)(x1 + (double)(y - y1) / (y2 - y1) * (x2 - x1));
                        nodes.Add(x);
                    }
                }

                nodes.Sort();

                for (var i = 0; i < nodes.Count - 1; i += 2)
                {
                    for (
                        var x = Math.Max(0, nodes[i]);
                        x <= Math.Min(_mapWidth - 1, nodes[i + 1]);
                        x++
                    )
                    {
                        float t = (float)y / _mapHeight;

                        var color = new Rgba32(
                            (byte)(40 + t * 20),
                            (byte)(90 + t * 40),
                            (byte)(60 + t * 20),
                            255
                        );

                        image[x, y] = color;
                    }
                }
            }
        }

        private Dictionary<(int x, int y), int> BuildHeatmapData(
            Dictionary<string, (double latitude, double longitude)> geoLocations
        )
        {
            var map = new Dictionary<(int, int), int>();

            foreach (var (latitude, longitude) in geoLocations.Values)
            {
                var (x, y) = LatLonToPixel(latitude, longitude);

                if (x >= 0 && x < _mapWidth && y >= 0 && y < _mapHeight)
                {
                    var key = (x, y);
                    map[key] = map.TryGetValue(key, out var v) ? v + 1 : 1;
                }
            }

            return map;
        }

        private void DrawHeatmap(Image<Rgba32> image, Dictionary<(int, int), int> data)
        {
            if (data.Count == 0)
                return;

            var max = data.Values.Max();

            using var heatLayer = new Image<Rgba32>(_mapWidth, _mapHeight);

            foreach (var kv in data)
            {
                var norm = (double)kv.Value / max;

                DrawHeatPoint(heatLayer, kv.Key.Item1, kv.Key.Item2, GetHeatColor(norm), norm);
            }

            heatLayer.Mutate(x => x.GaussianBlur(2f));

            image.Mutate(ctx => ctx.DrawImage(heatLayer, 1f));
        }

        private void DrawHeatPoint(Image<Rgba32> image, int x, int y, Color color, double intensity)
        {
            var size = Math.Max(1, (int)(1 + intensity * 4));

            for (var dx = -size; dx <= size; dx++)
            {
                for (var dy = -size; dy <= size; dy++)
                {
                    if (dx * dx + dy * dy <= size * size)
                    {
                        var px = x + dx;
                        var py = y + dy;

                        if (px >= 0 && px < _mapWidth && py >= 0 && py < _mapHeight)
                            image[px, py] = BlendPixels(image[px, py], color);
                    }
                }
            }
        }

        private (int x, int y) LatLonToPixel(double lat, double lon)
        {
            lat = Math.Max(-85.05112878, Math.Min(85.05112878, lat));

            if (lon > 180)
                lon -= 360;
            if (lon < -180)
                lon += 360;

            return ((int)((_mapWidth / 360.0) * (lon + 180)), (int)((_mapHeight / 2) - (_mapHeight /
                (2 * (Math.Log(Math.Tan(Math.PI / 4 + (85.05112878 * Math.PI / 180) / 2)))))
                * (Math.Log(Math.Tan(Math.PI / 4 + (lat * Math.PI / 180) / 2)))));
        }

        private static Color GetHeatColor(double i)
        {
            if (i < 0.5)
                return Color.FromRgba((byte)(255 * i * 2), (byte)(100 * i * 2), 0, 200);
            return Color.FromRgba(255, (byte)(200 + i * 55), 0, 255);
        }

        private static Rgba32 BlendPixels(Rgba32 bg, Color fgColor)
        {
            var fg = (Rgba32)fgColor;
            float a = fg.A / 255f;

            return new Rgba32(
                (byte)(bg.R * (1 - a) + fg.R * a),
                (byte)(bg.G * (1 - a) + fg.G * a),
                (byte)(bg.B * (1 - a) + fg.B * a),
                255
            );
        }
    }
}
