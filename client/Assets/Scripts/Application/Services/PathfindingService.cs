using client.Assets.Scripts.Domain.Entities;
using client.Assets.Scripts.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace client.Assets.Scripts.Application.Services
{
    public class PathfindingService : IPathfindingService
    {
        private static readonly Vector2Int[] Directions = 
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, GameField field, List<Unit> units)
        {
            var openList = new List<PathNode>();
            var closedList = new HashSet<Vector2Int>();
            var startNode = new PathNode(start, 0, GetHeuristic(start, end), null);
            
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                var currentNode = openList.OrderBy(n => n.FCost).First();
                openList.Remove(currentNode);
                closedList.Add(currentNode.Position);

                if (currentNode.Position == end)
                {
                    return ReconstructPath(currentNode);
                }

                foreach (var neighbor in GetNeighbors(currentNode.Position, field))
                {
                    if (closedList.Contains(neighbor)) continue;
                    if (IsPositionBlocked(neighbor, field, units) && neighbor != end) continue;

                    var gCost = currentNode.GCost + 1;
                    var hCost = GetHeuristic(neighbor, end);
                    var neighborNode = new PathNode(neighbor, gCost, hCost, currentNode);

                    var existingNode = openList.FirstOrDefault(n => n.Position == neighbor);
                    if (existingNode == null)
                    {
                        openList.Add(neighborNode);
                    }
                    else if (gCost < existingNode.GCost)
                    {
                        existingNode.GCost = gCost;
                        existingNode.Parent = currentNode;
                    }
                }
            }

            return new List<Vector2Int>();
        }

        public bool HasPath(Vector2Int start, Vector2Int end, GameField field, List<Unit> units)
        {
            var path = FindPath(start, end, field, units);
            return path.Count > 0;
        }

        public int GetPathLength(Vector2Int start, Vector2Int end, GameField field, List<Unit> units)
        {
            var path = FindPath(start, end, field, units);
            return path.Count;
        }

        public bool IsPositionReachable(Vector2Int start, Vector2Int target, int maxDistance, GameField field, List<Unit> units)
        {
            var path = FindPath(start, target, field, units);
            return path.Count > 0 && path.Count <= maxDistance;
        }

        public List<Vector2Int> GetReachablePositions(Vector2Int start, int maxDistance, GameField field, List<Unit> units)
        {
            var reachablePositions = new List<Vector2Int>();
            var visited = new HashSet<Vector2Int>();
            var queue = new Queue<(Vector2Int position, int distance)>();
            
            queue.Enqueue((start, 0));
            visited.Add(start);

            while (queue.Count > 0)
            {
                var (currentPos, currentDistance) = queue.Dequeue();
                
                if (currentDistance > 0 && currentDistance <= maxDistance)
                {
                    reachablePositions.Add(currentPos);
                }

                if (currentDistance < maxDistance)
                {
                    foreach (var neighbor in GetNeighbors(currentPos, field))
                    {
                        if (!visited.Contains(neighbor) && !IsPositionBlocked(neighbor, field, units))
                        {
                            visited.Add(neighbor);
                            queue.Enqueue((neighbor, currentDistance + 1));
                        }
                    }
                }
            }

            return reachablePositions;
        }

        public bool IsDirectPathClear(Vector2Int start, Vector2Int end, GameField field)
        {
            var dx = Math.Abs(end.x - start.x);
            var dy = Math.Abs(end.y - start.y);
            var x = start.x;
            var y = start.y;
            var xInc = start.x < end.x ? 1 : -1;
            var yInc = start.y < end.y ? 1 : -1;
            var error = dx - dy;

            dx *= 2;
            dy *= 2;

            while (true)
            {
                var currentPos = new Vector2Int(x, y);
                
                if (currentPos != start && currentPos != end)
                {
                    if (field.Obstacles.Contains(currentPos)) return false;
                }

                if (x == end.x && y == end.y) break;

                if (error > 0)
                {
                    x += xInc;
                    error -= dy;
                }
                else
                {
                    y += yInc;
                    error += dx;
                }
            }

            return true;
        }

        public List<Vector2Int> GetNeighbors(Vector2Int position, GameField field)
        {
            var neighbors = new List<Vector2Int>();

            foreach (var direction in Directions)
            {
                var neighbor = position + direction;
                if (IsWithinBounds(neighbor, field))
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private bool IsWithinBounds(Vector2Int position, GameField field)
        {
            return position.x >= 0 && position.x < field.Width &&
                   position.y >= 0 && position.y < field.Height;
        }

        private bool IsPositionBlocked(Vector2Int position, GameField field, List<Unit> units)
        {
            if (field.Obstacles.Contains(position)) return true;
            return units.Any(unit => unit.IsAlive && unit.Position == position);
        }

        private int GetHeuristic(Vector2Int from, Vector2Int to) =>
            Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);

        private List<Vector2Int> ReconstructPath(PathNode endNode)
        {
            var path = new List<Vector2Int>();
            var currentNode = endNode;

            while (currentNode != null)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path.Skip(1).ToList();
        }

        private class PathNode
        {
            public Vector2Int Position { get; }
            public int GCost { get; set; }
            public int HCost { get; }
            public int FCost => GCost + HCost;
            public PathNode Parent { get; set; }

            public PathNode(Vector2Int position, int gCost, int hCost, PathNode parent)
            {
                Position = position;
                GCost = gCost;
                HCost = hCost;
                Parent = parent;
            }
        }
    }
}