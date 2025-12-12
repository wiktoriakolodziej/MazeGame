using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Syncfusion.XForms.Android.Core;
using System;

namespace MazeGame.Scenes;

public abstract class Scene : IDisposable
{
    public Scene()
    {
        // Create a content manager for the scene
        Content = new ContentManager(Game1.Content.ServiceProvider);

        // Set the root directory for content to the same as the root directory
        // for the game's content.
        Content.RootDirectory = Game1.Content.RootDirectory;
        SpriteBatch = Game1._spriteBatch;
        GraphicsDevice = Game1.GraphicsDevice;
    }
    protected ContentManager Content { get; }
    public bool IsDisposed { get; private set; }
    protected SpriteBatch SpriteBatch { get; }
    protected GraphicsDevice GraphicsDevice { get; }

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
