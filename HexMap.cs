using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexSystem
{
    public abstract class HexMap : HexMap<int>
    {
    }

    public class HexMap<T>
    {
        private readonly Dictionary<CubeCoordinate, T> _map;

        public HexMap()
        {
            _map = new Dictionary<CubeCoordinate, T>();
        }

        public void Set(CubeCoordinate coordinate, T obj)
        {
            _map[coordinate] = obj;
        }

        public T Get(CubeCoordinate coordinate)
        {
            return Get(coordinate.Q, coordinate.R);
        }

        public bool TryGet(CubeCoordinate coordinate, out T value)
        {
            if (TileExists(coordinate))
            {
                value = Get(coordinate);
                return true;
            }
            value = default;
            return false;
        }

        public List<T> GetAll()
        {
            return _map.Values.ToList();
        }

        public void Clear()
        {
            _map.Clear();
        }

        private T Get(int q, int r)
        {
            return _map[new CubeCoordinate(q, r)];
        }

        public bool TileExists(CubeCoordinate coordinate)
        {
            return _map.ContainsKey(coordinate);
        }

        #region Get Area Functions

        public static List<CubeCoordinate> GetNeighborsOfHex(CubeCoordinate coordinate)
        {
            var list = new List<CubeCoordinate>();
            foreach (var d in CubeCoordinate.Directions) list.Add(coordinate + d);
            return list;
        }

        public static List<CubeCoordinate> GetRectangle(CubeCoordinate coordinate, int range)
        {
            var list = new List<CubeCoordinate>();
            for (var q = -range; q <= range; q++)
            {
                for (var r = -range; r <= range; r++)
                {
                    var tile = new CubeCoordinate(q, r) + coordinate;
                    list.Add(tile);
                }
            }

            return list;
        }

        public static List<CubeCoordinate> GetLine(CubeCoordinate coordinate, CubeCoordinate direction, int range)
        {
            var list = new List<CubeCoordinate>();
            for (var i = 0; i < range; i++)
            {
                var newCoordinate = coordinate + direction * i;
                list.Add(newCoordinate);
            }

            return list;
        }

        public static List<CubeCoordinate> GetRing(CubeCoordinate center, int radius)
        {
            var list = new List<CubeCoordinate>();
            var hex = center + CubeCoordinate.DirS * radius;
            for (var i = 0; i < 6; i++)
            {
                for (var j = 0; j < radius; j++)
                {
                    list.Add(hex);
                    hex += CubeCoordinate.GetDirection(i);
                }
            }

            return list;
        }

        public static List<CubeCoordinate> GetLine(CubeCoordinate center, CubeCoordinate target)
        {
            var list = new List<CubeCoordinate>();
            var distance = CubeCoordinate.Distance(center, target);
            if (distance < 1) return list;
            var centerPos = GetPositionOfHex(center);
            var targetPos = GetPositionOfHex(target);
            var unitDistance = (targetPos - centerPos) / distance;
            for (var i = 0; i <= distance; i++)
            {
                var pos = centerPos + unitDistance * i;
                list.Add(WorldPositionToHexCoordinate(pos));
            }

            return list;
        }

        public static List<CubeCoordinate> GetTriangle(CubeCoordinate center, CubeCoordinate direction, int range)
        {
            var list = new List<CubeCoordinate>();
            var lineDir = CubeCoordinate.RotateDirection(direction, 2);
            var firstLine = GetLine(center, direction, range);
            for (var i = 0; i < firstLine.Count; i++)
            {
                list.AddRange(GetLine(firstLine[i], lineDir, i + 1));
            }

            return list;
        }

        public static List<CubeCoordinate> GetArea(CubeCoordinate coordinate, int range)
        {
            var list = new List<CubeCoordinate>();
            for (var x = -range; x <= range; x++)
            {
                for (var y = Mathf.Max(-range, -x - range); y <= Mathf.Min(range, -x + range); y++)
                {
                    var tile = new CubeCoordinate(x, y) + coordinate;
                    list.Add(tile);
                }
            }

            return list;
        }

        #endregion

        #region World Functions

        public static CubeCoordinate WorldPositionToHexCoordinate(Vector3 point, int hexSize = 1)
        {
            float q = (Mathf.Sqrt(3f) / 3f * point.x - 1f / 3f * -point.z) / hexSize;
            float r = (2f / 3f * -point.z) / hexSize;

            float x = q;
            float z = r;
            float y = -x - z;

            int rx = Mathf.RoundToInt(x);
            int ry = Mathf.RoundToInt(y);
            int rz = Mathf.RoundToInt(z);

            float xDiff = Mathf.Abs(rx - x);
            float yDiff = Mathf.Abs(ry - y);
            float zDiff = Mathf.Abs(rz - z);

            if (xDiff > yDiff && xDiff > zDiff)
            {
                rx = -ry - rz;
            }
            else if (yDiff > zDiff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }

            return new CubeCoordinate(rx, rz);
        }

        public static CubeCoordinate WorldPositionToHexCoordinate(Vector2 point, int hexSize = 1)
        {
            return WorldPositionToHexCoordinate(new Vector3(point.x, 0, point.y), hexSize);
        }

        public static Vector3 GetPositionOfHex(CubeCoordinate coordinate, int hexSize = 1)
        {
            var r = coordinate.R;
            var pos = new Vector3
            {
                x = Mathf.Sqrt(3.0f) * (coordinate.Q + r / 2.0f) * hexSize,
                z = -1.5f * r * hexSize
            };

            return pos;
        }

        public static Vector2 GetPositionOfHex2D(CubeCoordinate coordinate, int hexSize = 1)
        {
            var r = coordinate.R;
            var pos = new Vector2
            {
                x = Mathf.Sqrt(3.0f) * (coordinate.Q + r / 2.0f) * hexSize,
                y = -1.5f * r * hexSize
            };

            return pos;
        }

        #endregion
    }
}