using System;
using System.Collections.Generic;
using UnityEngine;

public class CubeCoordinate : IEquatable<CubeCoordinate>
{
    private const int DirectionCount = 6;
    public int Q { get; }
    public int R { get; }
    public int S { get; }

    public static readonly CubeCoordinate DirQ = new(0, -1);
    public static readonly CubeCoordinate DirR = new(1, 0);
    public static readonly CubeCoordinate DirS = new(-1, 1);
    public static List<CubeCoordinate> Directions => new() { DirQ, -DirS, DirR, -DirQ, DirS, -DirR };


    public CubeCoordinate(int q, int r)
    {
        Q = q;
        R = r;
        S = -q - r;
    }

    public CubeCoordinate(int q, int r, int s)
    {
        Q = q;
        R = r;
        S = s;
    }

    #region Operators

    public static bool operator ==(CubeCoordinate c1, CubeCoordinate c2)
    {
        if (c1 is null) return false;
        if (c2 is null) return false;
        return c1.Q == c2.Q && c1.R == c2.R;
    }

    public static bool operator !=(CubeCoordinate c1, CubeCoordinate c2)
    {
        if (c1 is null) return false;
        if (c2 is null) return false;
        return c1.Q != c2.Q || c1.R != c2.R;
    }

    public static CubeCoordinate operator +(CubeCoordinate c1, CubeCoordinate c2)
    {
        return new CubeCoordinate(c1.Q + c2.Q, c1.R + c2.R);
    }

    public static CubeCoordinate operator *(CubeCoordinate c1, int c2)
    {
        return new CubeCoordinate(c1.Q * c2, c1.R * c2);
    }

    public static CubeCoordinate operator -(CubeCoordinate c1)
    {
        return new CubeCoordinate(-c1.Q, -c1.R);
    }

    #endregion

    #region Direction Functions

    public static CubeCoordinate GetDirection(int d)
    {
        d = Mathf.Clamp(d, 0, 6);
        return Directions[d];
    }

    public static CubeCoordinate RotateDirection(CubeCoordinate direction, int amount)
    {
        if (amount < 0) amount = DirectionCount - -amount % DirectionCount;
        var index = Directions.IndexOf(direction) + amount;
        return Directions[index % DirectionCount];
    }

    #endregion

    #region Inherited Functions

    public override int GetHashCode()
    {
        return HashCode.Combine(Q, R);
    }

    public override bool Equals(object obj)
    {
        return obj is not null && Equals((CubeCoordinate)obj);
    }

    public bool Equals(CubeCoordinate coordinate)
    {
        return this == coordinate;
    }

    public override string ToString()
    {
        return Q + "|" + R;
    }

    #endregion
}
