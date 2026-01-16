using MazeGame.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using MazeGame.Maze;
using Syncfusion.XForms.Android.EffectsView;

namespace MazeGame.Services
{
    public class PhysicsService
    {
        public void InsideBounce(Sprite movingObject, Rectangle obstacleBounds)
        {
            var ballBounds = new Rectangle(
                (int)movingObject.Position.X,
                (int)movingObject.Position.Y,
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
                newPosition.X = obstacleBounds.Left;
            }
            else if (minDistance == distanceRight)
            {
                // Closest to the right edge.
                normal = -Vector2.UnitX;
                newPosition.X = obstacleBounds.Right - movingObject.Width;
            }
            else if (minDistance == distanceTop)
            {
                // Closest to the top edge.
                normal = Vector2.UnitY;
                newPosition.Y = obstacleBounds.Top;
            }
            else
            {
                // Closest to the bottom edge.
                normal = -Vector2.UnitY;
                newPosition.Y = obstacleBounds.Bottom - movingObject.Height;
            }

            // Reflect the velocity about the normal.
            movingObject.Velocity = Vector2.Reflect(movingObject.Velocity, normal) * 0.98f;
            // Set the new position of the ball.
            movingObject.Position = newPosition;

        }

        public void OutsideBounce(Sprite movingObject, List<CollidingCell> obstaclesBounds, Rectangle startPosition)
        {
            var ballBounds = new Rectangle(
                (int)movingObject.Position.X,
                (int)movingObject.Position.Y,
                (int)movingObject.Width,
                (int)movingObject.Height
            );
            //Console.WriteLine(ballBounds);
            //foreach ( var obstacle in obstaclesBounds ) Console.WriteLine(obstacle);
            var obstacles = obstaclesBounds.Where(r => r.Position.Intersects(ballBounds));
            //Console.WriteLine(obstacles.Count());
            if (!obstacles.Any())
            {
                movingObject.Position += movingObject.Velocity;
                movingObject.Velocity *= 0.98f;
                return;
            }

            foreach (var obstacle in obstacles)
            {
                if (OutsideBounce(movingObject, obstacle.Position, obstacle.IsTrap, startPosition))
                    break;
            }
        }

        private bool OutsideBounce(Sprite movingObject, Rectangle obstacleBounds, bool isTrap, Rectangle startPosition)
        {

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
                if (isTrap)
                {
                    movingObject.Velocity = Vector2.Zero;
                    movingObject.Position = new Vector2(startPosition.X, startPosition.Y);
                    return true;
                }
                Vector2 n_normal = (-1) * Vector2.Normalize(ray_n);
                //Console.WriteLine("normal " + n_normal + " v " + movingObject.Velocity + " v reflected " + Vector2.Reflect(movingObject.Velocity, n_normal) + " overlap " + overlap + " ray_n " + ray_n);
                if (ray_n == Vector2.Zero)
                {
                    //Console.WriteLine("first");
                    movingObject.Velocity = -movingObject.Velocity;
                    newPosition += overlap * Vector2.Normalize(movingObject.Velocity);
                }
                else
                {
                    //Console.WriteLine("second");
                    movingObject.Velocity = Vector2.Reflect(movingObject.Velocity, n_normal);
                    newPosition += n_normal * overlap + movingObject.Velocity;
                }
            }

            // Set the new position of the ball.
            movingObject.Position = newPosition;
            // Apply friction to the ball's velocity.
            movingObject.Velocity *= 0.98f;
            return false;
        }
    }
}
