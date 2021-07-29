using DaJuTestDemo.Core;
using GeoAPI.Geometries;
using I3DMapOperation;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Sandwych.MapMatchingKit.Matching;
using Sandwych.MapMatchingKit.Roads;
using Sandwych.MapMatchingKit.Spatial;
using Sandwych.MapMatchingKit.Spatial.Geometries;
using Sandwych.MapMatchingKit.Topology;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DaJuTestDemo.Common
{
    public class MapPointHelper
    {
        private static readonly string s_dataDir = Environment.CurrentDirectory + "\\data\\";
        private IList<Trajectory> _trajectories;

        public MapPointHelper()
        {

        }

        public MapPointHelper(IList<Trajectory> trajectories)
        {
            _trajectories = trajectories;
        }
        public List<Trajectory> LoadDataAndTransfer()
        {
            var spatial = new GeographySpatialOperation();
            var mapBuilder = new RoadMapBuilder(spatial);

            LoggerHelper.Logger.Info("Loading road map...");
            Debug.WriteLine("Loading road map...");
            var roads = ReadRoads(spatial);
            var map = mapBuilder.AddRoads(roads).Build();

            LoggerHelper.Logger.Info("The road map has been loaded");
            Debug.WriteLine("The road map has been loaded");


            //var router = new PrecomputedDijkstraRouter<Road, RoadPoint>(map, Costs.TimePriorityCost, Costs.DistanceCost, 1000D);
            var router = new DijkstraRouter<Road, RoadPoint>();

            var matcher = new Matcher<MatcherCandidate, MatcherTransition, MatcherSample>(
                map, router, Costs.TimePriorityCost, spatial);
            matcher.MaxDistance = 500; // set maximum searching distance between two GPS points to 1000 meters.
            matcher.MaxRadius = 5; // sets maximum radius for candidate selection to 200 meters


            Debug.WriteLine("Loading GPS samples...");
            //var samples = ReadSamples().OrderBy(s => s.Time).ToList();
            var samples = ReadSamples(_trajectories).OrderBy(s => s.Time).ToList();
            LoggerHelper.Logger.Info("GPS samples loaded. [count={0}]", samples.Count);
            Debug.WriteLine("GPS samples loaded. [count={0}]", samples.Count);

            Debug.WriteLine("Starting Offline map-matching...");
            var res = OfflineMatch(matcher, samples);
            return res;
        }

        private static IEnumerable<RoadInfo> ReadRoads(ISpatialOperation spatial)
        {
            var json = File.ReadAllText(Path.Combine(s_dataDir, @"road.geojson"));
            var reader = new GeoJsonReader();
            var fc = reader.Read<FeatureCollection>(json);
            foreach (var feature in fc.Features)
            {
                var lineGeom = feature.Geometry as ILineString;
                yield return new RoadInfo(
                    Convert.ToInt64(feature.Attributes["Gid"]),
                    Convert.ToInt64(feature.Attributes["Source"]),
                    Convert.ToInt64(feature.Attributes["Target"]),
                    (bool)feature.Attributes["Oneway"],
                    0,
                    Convert.ToSingle(feature.Attributes["Priority"]),
                    Convert.ToSingle(feature.Attributes["MaxForwardSpeed"]),
                    Convert.ToSingle(feature.Attributes["MaxBackwardSpeed"]),
                    Convert.ToSingle(spatial.Length(lineGeom)),
                    lineGeom);
            }
        }

        public IList<Trajectory> ReadRoads()
        {
            var json = File.ReadAllText(Path.Combine(s_dataDir, @"road.geojson"));
            var reader = new GeoJsonReader();
            var fc = reader.Read<FeatureCollection>(json);
            List<Trajectory> res = new List<Trajectory>();
            var time = DateTime.Now.AddDays(-3);

            //var feature = fc.Features[1];
            //var lineGeom = feature.Geometry as ILineString;
            //var coordinates = lineGeom.Coordinates;
            //foreach (var item in coordinates)
            //{
            //    var nextTime = time.AddSeconds(2);
            //    res.Add(new Trajectory { Longitude = item.X, Latitude = item.Y, Altitude = item.Z, Speed = 30.0, Time = nextTime.ToString() });
            //    time = nextTime;
            //}

            foreach (var feature in fc.Features)
            {
                var lineGeom = feature.Geometry as ILineString;
                var coordinates = lineGeom.Coordinates;
                foreach (var item in coordinates)
                {
                    var nextTime = time.AddSeconds(2);
                    res.Add(new Trajectory { Longitude = item.X, Latitude = item.Y, Altitude = item.Z, Speed = 30.0, Time = nextTime.ToString() });
                    time = nextTime;
                }
            }
            return res;
        }

        private static IEnumerable<MatcherSample> ReadSamples()
        {
            var json = File.ReadAllText(Path.Combine(s_dataDir, @"samples.oneday.geojson"));
            var reader = new GeoJsonReader();
            var fc = reader.Read<FeatureCollection>(json);
            var timeFormat = "yyyy-MM-dd-HH.mm.ss";
            //var samples = new List<MatcherSample>();
            foreach (var i in fc.Features)
            {
                var p = i.Geometry as IPoint;
                var coord2D = new Coordinate2D(p.X, p.Y);
                var timeStr = i.Attributes["time"].ToString().Substring(0, timeFormat.Length);
                var time = DateTimeOffset.ParseExact(timeStr, timeFormat, CultureInfo.InvariantCulture);
                var longTime = time.ToUnixTimeMilliseconds();
                yield return new MatcherSample(longTime, time, coord2D);
            }
        }

        private IEnumerable<MatcherSample> ReadSamples(IList<Trajectory> trajectories)
        {
            var timeFormat = "yyyy-MM-dd HH:mm:ss";
            foreach (Trajectory item in trajectories)
            {
                var coord2D = new Coordinate2D(item.Longitude, item.Latitude);
                var timeStr = item.Time;
                var time = DateTimeOffset.ParseExact(timeStr, timeFormat, CultureInfo.InvariantCulture);
                var longTime = time.ToUnixTimeMilliseconds();
                yield return new MatcherSample(longTime, time, coord2D);
            }
        }

        private List<Trajectory> OfflineMatch(
            Matcher<MatcherCandidate, MatcherTransition, MatcherSample> matcher,
            IReadOnlyList<MatcherSample> samples)
        {
            var kstate = new MatcherKState();

            //Do the offline map-matching
            Debug.WriteLine("Doing map-matching...");
            var startedOn = DateTime.Now;
            foreach (var sample in samples)
            {
                var vector = matcher.Execute(kstate.Vector(), kstate.Sample, sample);
                kstate.Update(vector, sample);
            }

            Debug.WriteLine("Fetching map-matching results...");
            var candidatesSequence = kstate.Sequence();
            var timeElapsed = DateTime.Now - startedOn;

            LoggerHelper.Logger.Info("Map-matching elapsed time: {0}, Speed={1} samples/second", timeElapsed, samples.Count / timeElapsed.TotalSeconds);
            LoggerHelper.Logger.Info("Results: [count={0}]", candidatesSequence.Count());
            Debug.WriteLine("Map-matching elapsed time: {0}, Speed={1} samples/second", timeElapsed, samples.Count / timeElapsed.TotalSeconds);
            Debug.WriteLine("Results: [count={0}]", candidatesSequence.Count());

            List<Trajectory> csvLines = new List<Trajectory>();
            int matchedCandidateCount = 0;
            foreach (var cand in candidatesSequence)
            {
                var roadId = cand.Point.Edge.RoadInfo.Id; // original road id
                var heading = cand.Point.Edge.Headeing; // heading
                var coord = cand.Point.Coordinate; // GPS position (on the road)
                csvLines.Add(new Trajectory()
                {
                    Time = cand.Sample.Time.ToString(),
                    Location = 314.2,
                    Speed = 30,
                    Longitude = coord.X,
                    Latitude = coord.Y
                });
                
                if (cand.HasTransition)
                {
                    var geom = cand.Transition.Route.ToGeometry(); // path geometry(LineString) from last matching candidate
                    var edges = cand.Transition.Route.Edges; // Road segments between two GPS position
                }
                matchedCandidateCount++;
            }

            LoggerHelper.Logger.Info("Matched Candidates: {0}, Rate: {1}%", matchedCandidateCount, matchedCandidateCount * 100 / samples.Count());
            Debug.WriteLine("Matched Candidates: {0}, Rate: {1}%", matchedCandidateCount, matchedCandidateCount * 100 / samples.Count());
            return csvLines;
        }
    }
}
