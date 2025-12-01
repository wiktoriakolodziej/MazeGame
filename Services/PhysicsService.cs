using MazeGame.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;

namespace MazeGame.Services
{
    public class PhysicsService
    {
        public void InsideBounce(Sprite movingObject, Rectangle obstacleBounds)
        {
            var ballBounds = new Rectangle(
                (int)(movingObject.Position.X - movingObject.Width/2),
                (int)(movingObject.Position.Y - movingObject.Height/2),
                (int)movingObject.Width,
                (int)movingObject.Height
            );

            if (obstacleBounds.Contains(ballBounds))
                return;

            var newPosition = movingObject.Position + movingObject.Velocity;
            
            // Ball would move outside the screen
            // First find the distance from the edge of the ball to each edge of the screen.
            float distanceLeft = Math.Abs(obstacleBounds.Left - ballBounds.Left);
            float distanceRight = Math.Abs(obstacleBounds.Right - ballBounds.Right);
            float distanceTop = Math.Abs(obstacleBounds.Top - ballBounds.Top);
            float distanceBottom = Math.Abs(obstacleBounds.Bottom - ballBounds.Bottom);

            // Determine which screen edge is the closest.
            float minDistance = Math.Min(
                Math.Min(distanceLeft, distanceRight),
                Math.Min(distanceTop, distanceBottom)
            );

            // Determine the normal vector based on which screen edge is the closest.
            Vector2 normal;
            if (minDistance == distanceLeft)
            {
                // Closest to the left edge.
                normal = Vector2.UnitX;
                newPosition.X = movingObject.Width/2;
            }
            else if (minDistance == distanceRight)
            {
                // Closest to the right edge.
                normal = -Vector2.UnitX;
                newPosition.X = obstacleBounds.Right - movingObject.Width/2;
            }
            else if (minDistance == distanceTop)
            {
                // Closest to the top edge.
                normal = Vector2.UnitY;
                newPosition.Y = movingObject.Height/2;
            }
            else
            {
                // Closest to the bottom edge.
                normal = -Vector2.UnitY;
                newPosition.Y = obstacleBounds.Bottom - movingObject.Height/2;
            }

            // Reflect the velocity about the normal.
            movingObject.Velocity = Vector2.Reflect(movingObject.Velocity, normal) * 0.99f;
            // Set the new position of the ball.
            movingObject.Position = newPosition;

        }

        public void OutsideBounce(Sprite movingObject, List<Rectangle> obstaclesBounds)
        {
            var ballBounds = new Rectangle(
                (int)movingObject.Position.X,
                (int)movingObject.Position.Y,
                (int)movingObject.Width,
                (int)movingObject.Height
            );
            var obstacles = obstaclesBounds.Where(r => r.Intersects(ballBounds));

            foreach (var rect in obstacles)
                OutsideBounce(movingObject, rect);
        }

        public void OutsideBounce(Sprite movingObject, Rectangle obstacleBounds)
        {
           
            // Get the bounds of the ball as a rectangle.
            var ballBounds = new Rectangle(
                (int)movingObject.Position.X,
                (int)movingObject.Position.Y,
                (int)movingObject.Width,
                (int)movingObject.Height
            );

            // Calculate the new position of the ball based on the velocity.
            var newPosition = movingObject.Position + movingObject.Velocity;
            //Console.WriteLine("velocity " + movingObject.Velocity);

            var ballRadius = (int)(movingObject.Width / 2);
            var ballCenter = new Vector2((int)movingObject.Position.X + ballRadius, (int)movingObject.Position.Y + ballRadius);

            // Detect if the ball object is within the screen bounds.
            float nx = Math.Max(obstacleBounds.Left, Math.Min(obstacleBounds.Right, ballCenter.X));
            float ny = Math.Max(obstacleBounds.Top, Math.Min(obstacleBounds.Bottom, ballCenter.Y));
            Vector2 ray_n = new Vector2(nx - ballCenter.X, ny - ballCenter.Y);
            float n_magnitude = ray_n.Length();
            float overlap = ballRadius - n_magnitude;
            if (overlap > 0)
            {
                Vector2 n_normal = (-1) * Vector2.Normalize(ray_n);
                //Console.WriteLine("normal " + n_normal + " v " + movingObject.Velocity + " v reflected " + Vector2.Reflect(movingObject.Velocity, n_normal) + " overlap " + overlap);
                movingObject.Velocity = Vector2.Reflect(movingObject.Velocity, n_normal);
                newPosition += n_normal * overlap + movingObject.Velocity;
            }

            // Set the new position of the ball.
            movingObject.Position = newPosition;
            // Apply friction to the ball's velocity.
            movingObject.Velocity *= 0.99f;
        }

        public void MoveCircleWithCCD(Sprite movingObject, List<Rectangle> obstacles)
        {
            float radius = movingObject.Width / 2;
            float remaining = 1.0f; // normalized frame time (1 == full dt)
            int maxIterations = 4;   // tweak: more iterations = more accurate but costlier

            for (int iter = 0; iter < maxIterations && remaining > 1e-4f; ++iter)
            {
                Vector2 start = movingObject.Position;
                Vector2 sweepVel = movingObject.Velocity * remaining; // portion to sweep this iteration

                float earliestTOI = float.MaxValue;
                Vector2 hitNormal = Vector2.Zero;
                Rectangle hitRect = default;
                bool hit = false;

                // find earliest impact among obstacles
                foreach (var rect in obstacles)
                {
                    if (SweepCircleAABB(start, sweepVel, radius, rect, out float toi, out Vector2 normal))
                    {
                        if (toi < earliestTOI)
                        {
                            earliestTOI = toi;
                            hitNormal = normal;
                            hitRect = rect;
                            hit = true;
                        }
                    }
                }

                if (!hit)
                {
                    // no collision in the remaining sub-step: move full remaining distance and done
                    movingObject.Position = start + sweepVel;
                    break;
                }

                // Move to contact point
                movingObject.Position = start + sweepVel * earliestTOI;

                // Reflect velocity about the normal (optionally apply restitution)
                float restitution = 1.0f;
                movingObject.Velocity = Vector2.Reflect(movingObject.Velocity, hitNormal) * restitution;

                // Subtract elapsed time and continue with remaining time
                remaining = remaining * (1.0f - earliestTOI);

                // small push to avoid re-colliding at the same contact due to numerical issues
                movingObject.Position += hitNormal * 0.001f;
            }

            // After movement, apply friction or damping once per frame
            movingObject.Velocity *= 0.99f;
        }


        private bool SweepCircleAABB(Vector2 start, Vector2 velocity, float radius, Rectangle rect, out float toi, out Vector2 normal)
        {
            toi = float.MaxValue;
            normal = Vector2.Zero;

            // If velocity is nearly zero -> no sweep
            const float EPS = 1e-6f;
            if (velocity.LengthSquared() <= EPS * EPS)
                return false;

            // Expand the rectangle by radius (treat the circle center as a point).
            Rectangle expanded = new Rectangle(
                rect.Left - (int)radius,
                rect.Top - (int)radius,
                rect.Width + (int)(2 * radius),
                rect.Height + (int)(2 * radius)
            );

            // Ray vs AABB slab method:
            // ray: p(t) = start + velocity * t, for t in [0,1]
            float tmin = 0.0f;
            float tmax = 1.0f;

            // X axis
            if (Math.Abs(velocity.X) < EPS)
            {
                // Ray parallel to X planes: must be inside slab
                if (start.X < expanded.Left || start.X > expanded.Right) return false;
            }
            else
            {
                float inv = 1.0f / velocity.X;
                float t1 = (expanded.Left - start.X) * inv;
                float t2 = (expanded.Right - start.X) * inv;
                if (t1 > t2) { var tmp = t1; t1 = t2; t2 = tmp; }
                tmin = Math.Max(tmin, t1);
                tmax = Math.Min(tmax, t2);
                if (tmin > tmax) return false;
            }

            // Y axis
            if (Math.Abs(velocity.Y) < EPS)
            {
                if (start.Y < expanded.Top || start.Y > expanded.Bottom) return false;
            }
            else
            {
                float inv = 1.0f / velocity.Y;
                float t1 = (expanded.Top - start.Y) * inv;
                float t2 = (expanded.Bottom - start.Y) * inv;
                if (t1 > t2) { var tmp = t1; t1 = t2; t2 = tmp; }
                tmin = Math.Max(tmin, t1);
                tmax = Math.Min(tmax, t2);
                if (tmin > tmax) return false;
            }

            // If the earliest intersection time is outside [0,1], ignore
            if (tmin < 0f || tmin > 1f) return false;

            // We have a hit at tmin. Now compute the contact normal.
            Vector2 contactPoint = start + velocity * tmin;

            // Determine which face gave tmin by checking which axis produced tmin.
            // Recompute per-axis t entry values to find the axis which equals tmin (within epsilon).
            float txEntry = float.NegativeInfinity;
            float tyEntry = float.NegativeInfinity;
            if (Math.Abs(velocity.X) > EPS)
            {
                float inv = 1.0f / velocity.X;
                float t1 = (expanded.Left - start.X) * inv;
                float t2 = (expanded.Right - start.X) * inv;
                txEntry = Math.Min(t1, t2);
            }
            if (Math.Abs(velocity.Y) > EPS)
            {
                float inv = 1.0f / velocity.Y;
                float t1 = (expanded.Top - start.Y) * inv;
                float t2 = (expanded.Bottom - start.Y) * inv;
                tyEntry = Math.Min(t1, t2);
            }

            // Compare which entry time is the tmin (use EPS)
            if (Math.Abs(tmin - txEntry) < 1e-4f)
            {
                // X face collision
                if (velocity.X > 0)
                    normal = new Vector2(-1, 0); // hit left face of expanded rect => normal left
                else
                    normal = new Vector2(1, 0);  // hit right face => normal right
            }
            else if (Math.Abs(tmin - tyEntry) < 1e-4f)
            {
                // Y face collision
                if (velocity.Y > 0)
                    normal = new Vector2(0, -1); // hit top face => normal up
                else
                    normal = new Vector2(0, 1);  // hit bottom face => normal down
            }
            else
            {
                // Corner case: neither axis exactly equals tmin due to numerical conditions.
                // Compute closest point on (original) rectangle to the contact point and use that normal.
                float closestX = Math.Max(rect.Left, Math.Min(rect.Right, contactPoint.X));
                float closestY = Math.Max(rect.Top, Math.Min(rect.Bottom, contactPoint.Y));
                Vector2 v = new Vector2(contactPoint.X - closestX, contactPoint.Y - closestY);
                if (v.LengthSquared() > EPS)
                    normal = Vector2.Normalize(v);
                else
                    normal = -Vector2.Normalize(velocity); // fallback
            }

            toi = tmin;
            return true;
        }

    }
}
