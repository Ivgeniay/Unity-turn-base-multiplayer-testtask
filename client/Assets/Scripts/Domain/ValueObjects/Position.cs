using System;

namespace client.Assets.Scripts.Domain.ValueObjects
{
    public struct Position : IEquatable<Position>
    {
        public int x;
        public int y;
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Position Create(int x, int y) => new Position(x, y);
        public static Position zero => new Position(0, 0);
        public static Position one => new Position(1, 1);
        public static Position up => new Position(0, 1);
        public static Position down => new Position(0, -1);
        public static Position left => new Position(-1, 0);
        public static Position right => new Position(1, 0);


        public static float Distance(Position from, Position to)
        {
            int deltaX = from.x - to.x;
            int deltaY = from.y - to.y;
            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        public float DistanceTo(Position other)
        {
            int deltax = x - other.x;
            int deltay = y - other.y;
            return (float)Math.Sqrt(deltax * deltax + deltay * deltay);
        }

        public int ManhattanDistanceTo(Position other)
        {
            return Math.Abs(x - other.x) + Math.Abs(y - other.y);
        }

        public Position Add(int deltaX, int deltaY)
        {
            return new Position(x + deltaX, y + deltaY);
        }

        public Position Add(Position other)
        {
            return new Position(x + other.x, y + other.y);
        }

        public Position Subtract(Position other)
        {
            return new Position(x - other.x, y - other.y);
        }

        public Position Multiply(int scalar)
        {
            return new Position(x * scalar, y * scalar);
        }
        
        public bool Equals(Position other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            return obj is Position other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public static bool operator ==(Position left, Position right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !left.Equals(right);
        }

        public static Position operator +(Position left, Position right)
        {
            return left.Add(right);
        }

        public static Position operator -(Position left, Position right)
        {
            return left.Subtract(right);
        }

        public static Position operator *(Position position, int scalar)
        {
            return position.Multiply(scalar);
        }

        public static Position operator *(int scalar, Position position)
        {
            return position.Multiply(scalar);
        }
    }
}