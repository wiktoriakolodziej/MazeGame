using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MazeGame.Scenes;

public abstract class Scene(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, IServiceProvider serviceProvider, string contentRoot) : IDisposable
{
    protected ContentManager Content { get; } = new (serviceProvider, contentRoot);
    public bool IsDisposed { get; private set; }
    protected SpriteBatch SpriteBatch { get; } = spriteBatch;
    protected GraphicsDevice GraphicsDevice { get; } = graphicsDevice;

    public Action<ScreenType> OnSceneChanged { get; set; }

    ~Scene() => Dispose(false);
    public virtual void Initialize() => LoadContent();
    public virtual void LoadContent() { }
    public virtual void UnloadContent() => Content.Unload();
    public virtual void Update(GameTime gameTime) { }
    public virtual void Draw(GameTime gameTime) { }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            UnloadContent();
            Content.Dispose();
        }
        IsDisposed = true;
    }
}
