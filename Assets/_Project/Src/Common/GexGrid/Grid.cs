using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace _Project.Src.Common.GexGrid
{
    // Generated code -- CC0 -- No Rights Reserved -- http://www.redblobgames.com/grids/hexagons/

    public struct Point
    {
        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public readonly float x;
        public readonly float y;
    }

    public struct Hex : IEquatable<Hex>
    {
        public Hex(int q, int r, int s)
        {
            this.q = q;
            this.r = r;
            this.s = s;
            if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
        }

        public readonly int q;
        public readonly int r;
        public readonly int s;

        public Hex Add(Hex b)
        {
            return new Hex(q + b.q, r + b.r, s + b.s);
        }


        public Hex Subtract(Hex b)
        {
            return new Hex(q - b.q, r - b.r, s - b.s);
        }


        public Hex Scale(int k)
        {
            return new Hex(q * k, r * k, s * k);
        }


        public Hex RotateLeft()
        {
            return new Hex(-s, -q, -r);
        }


        public Hex RotateRight()
        {
            return new Hex(-r, -s, -q);
        }

        public static readonly List<Hex> Directions = new()
        {
            new Hex(1, 0, -1), new Hex(1, -1, 0), new Hex(0, -1, 1), new Hex(-1, 0, 1), new Hex(-1, 1, 0),
            new Hex(0, 1, -1)
        };

        public static Hex Direction(int direction)
        {
            return Directions[direction];
        }


        public Hex Neighbor(int direction)
        {
            return Add(Direction(direction));
        }

        public static readonly List<Hex> Diagonals = new()
        {
            new Hex(2, -1, -1), new Hex(1, -2, 1), new Hex(-1, -1, 2), new Hex(-2, 1, 1), new Hex(-1, 2, -1),
            new Hex(1, 1,  -2)
        };

        public Hex DiagonalNeighbor(int direction)
        {
            return Add(Diagonals[direction]);
        }


        public int Length()
        {
            return (math.abs(q) + math.abs(r) + math.abs(s)) / 2;
        }


        public int Distance(Hex b)
        {
            return Subtract(b).Length();
        }


        public string qrs => $"[{q},{r},{s}]";

        public bool Equals(Hex other)
        {
            return q == other.q && r == other.r && s == other.s;
        }

        public override bool Equals(object obj)
        {
            return obj is Hex other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(q, r, s);
        }
    }

    internal struct FractionalHex
    {
        public FractionalHex(float q, float r, float s)
        {
            this.q = q;
            this.r = r;
            this.s = s;
            if (math.round(q + r + s) != 0)
                throw new ArgumentException("q + r + s must be 0");
        }

        public readonly float q;
        public readonly float r;
        public readonly float s;

        public Hex HexRound()
        {
            var qi = (int)(math.round(q));
            var ri = (int)(math.round(r));
            var si = (int)(math.round(s));
            var qDiff = math.abs(qi - q);
            var rDiff = math.abs(ri - r);
            var sDiff = math.abs(si - s);
            if (qDiff > rDiff && qDiff > sDiff)
            {
                qi = -ri - si;
            }
            else if (rDiff > sDiff)
            {
                ri = -qi - si;
            }
            else
            {
                si = -qi - ri;
            }

            return new Hex(qi, ri, si);
        }


        public FractionalHex HexLerp(FractionalHex b, float t)
        {
            return new FractionalHex(q * (1.0f - t) + b.q * t, r * (1.0f - t) + b.r * t, s * (1.0f - t) + b.s * t);
        }


        public static List<Hex> HexLineDraw(Hex a, Hex b)
        {
            var n = a.Distance(b);
            var aNudge = new FractionalHex(a.q + 1e-06f, a.r + 1e-06f, a.s - 2e-06f);
            var bNudge = new FractionalHex(b.q + 1e-06f, b.r + 1e-06f, b.s - 2e-06f);
            var results = new List<Hex> { };
            var step = 1.0f / math.max(n, 1);
            for (var i = 0; i <= n; i++)
            {
                results.Add(aNudge.HexLerp(bNudge, step * i).HexRound());
            }

            return results;
        }

    }

    internal struct OffsetCoord
    {
        public OffsetCoord(int col, int row)
        {
            this.col = col;
            this.row = row;
        }

        public readonly int col;
        public readonly int row;
        public static readonly int Even = 1;
        public static readonly int Odd = -1;

        public static OffsetCoord QoffsetFromCube(int offset, Hex h)
        {
            var parity = h.q & 1;
            var col = h.q;
            var row = h.r + (h.q + offset * parity) / 2;
            if (offset != Even && offset != Odd)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }

            return new OffsetCoord(col, row);
        }


        public static Hex QOffsetToCube(int offset, OffsetCoord h)
        {
            var parity = h.col & 1;
            var q = h.col;
            var r = h.row - (h.col + offset * parity) / 2;
            var s = -q    - r;
            if (offset != Even && offset != Odd)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }

            return new Hex(q, r, s);
        }


        public static OffsetCoord RoffsetFromCube(int offset, Hex h)
        {
            var parity = h.r & 1;
            var col = h.q + (h.r + offset * parity) / 2;
            var row = h.r;
            if (offset != Even && offset != Odd)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }

            return new OffsetCoord(col, row);
        }


        public static Hex RoffsetToCube(int offset, OffsetCoord h)
        {
            var parity = h.row & 1;
            var q = h.col - (h.row + offset * parity) / 2;
            var r = h.row;
            var s = -q - r;
            if (offset != Even && offset != Odd)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }

            return new Hex(q, r, s);
        }


        public static OffsetCoord QOffsetFromQDoubled(int offset, DoubledCoord h)
        {
            var parity = h.col & 1;
            return new OffsetCoord(h.col, (h.row + offset * parity) / 2);
        }


        public static DoubledCoord QOffsetToQDoubled(int offset, OffsetCoord h)
        {
            var parity = h.col & 1;
            return new DoubledCoord(h.col, 2 * h.row - offset * parity);
        }


        public static OffsetCoord RoffsetFromRDoubled(int offset, DoubledCoord h)
        {
            var parity = h.row & 1;
            return new OffsetCoord((h.col + offset * parity) / 2, h.row);
        }


        public static DoubledCoord RoffsetToRDoubled(int offset, OffsetCoord h)
        {
            var parity = h.row & 1;
            return new DoubledCoord(2 * h.col - offset * parity, h.row);
        }

    }

    internal readonly struct DoubledCoord
    {
        public DoubledCoord(int col, int row)
        {
            this.col = col;
            this.row = row;
        }

        public readonly int col;
        public readonly int row;

        public static DoubledCoord QDoubledFromCube(Hex h)
        {
            var col = h.q;
            var row = 2 * h.r + h.q;
            return new DoubledCoord(col, row);
        }


        public Hex QDoubledToCube()
        {
            var q = col;
            var r = (row - col) / 2;
            var s = -q - r;
            return new Hex(q, r, s);
        }


        public static DoubledCoord RDoubledFromCube(Hex h)
        {
            var col = 2 * h.q + h.r;
            var row = h.r;
            return new DoubledCoord(col, row);
        }


        public Hex RDoubledToCube()
        {
            var q = (col - row) / 2;
            var r = row;
            var s = -q - r;
            return new Hex(q, r, s);
        }

    }

    internal struct Orientation
    {
        public Orientation(float f0, float f1, float f2, float f3, float b0, float b1, float b2, float b3,
            float startAngle)
        {
            this.f0 = f0;
            this.f1 = f1;
            this.f2 = f2;
            this.f3 = f3;
            this.b0 = b0;
            this.b1 = b1;
            this.b2 = b2;
            this.b3 = b3;
            this.startAngle = startAngle;
        }

        public readonly float f0;
        public readonly float f1;
        public readonly float f2;
        public readonly float f3;
        public readonly float b0;
        public readonly float b1;
        public readonly float b2;
        public readonly float b3;
        public readonly float startAngle;
    }

    internal struct Layout
    {
        public Layout(Orientation orientation, Point size, Point origin)
        {
            this.orientation = orientation;
            this.size = size;
            this.origin = origin;
        }

        public readonly Orientation orientation;
        public readonly Point size;
        public readonly Point origin;

        public static Orientation Pointy = new(math.sqrt(3.0f), math.sqrt(3.0f) / 2.0f, 0.0f, 3.0f / 2.0f,
            math.sqrt(3.0f) / 3.0f, -1.0f / 3.0f, 0.0f, 2.0f / 3.0f, 0.5f);

        public static Orientation Flat = new(3.0f / 2.0f, 0.0f, math.sqrt(3.0f) / 2.0f, math.sqrt(3.0f),
            2.0f                                  / 3.0f, 0.0f, -1.0f           / 3.0f, math.sqrt(3.0f) / 3.0f, 0.0f);

        public Point HexToPixel(Hex h)
        {
            var m = orientation;
            var x = (m.f0 * h.q + m.f1 * h.r) * size.x;
            var y = (m.f2 * h.q + m.f3 * h.r) * size.y;
            return new Point(x + origin.x, y + origin.y);
        }


        public FractionalHex PixelToHexFractional(Point p)
        {
            var m = orientation;
            var pt = new Point((p.x - origin.x) / size.x, (p.y - origin.y) / size.y);
            var q = m.b0 * pt.x               + m.b1 * pt.y;
            var r = m.b2 * pt.x               + m.b3 * pt.y;
            return new FractionalHex(q, r, -q - r);
        }


        public Hex PixelToHexRounded(Point p)
        {
            return PixelToHexFractional(p).HexRound();
        }


        public Point HexCornerOffset(int corner)
        {
            var m = orientation;
            var angle = 2.0f * math.PI * (m.startAngle - corner) / 6.0f;
            return new Point(size.x                              * math.cos(angle), size.y * math.sin(angle));
        }


        public List<Point> PolygonCorners(Hex h)
        {
            var corners = new List<Point> { };
            var center = HexToPixel(h);
            for (var i = 0; i < 6; i++)
            {
                var offset = HexCornerOffset(i);
                corners.Add(new Point(center.x + offset.x, center.y + offset.y));
            }

            return corners;
        }

    }
}