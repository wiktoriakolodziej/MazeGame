using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Syncfusion.XForms.Android.Core;
using System;
using System.Collections.Generic;
using Java.Util;

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

    public delegate void SceneChangedHandler(
        ScreenType screen,
        Dictionary<string, object>? data = null);

    public event SceneChangedHandler? OnSceneChanged;

    protected void RaiseSceneChanged(
        ScreenType screen,
        Dictionary<string, object>? data = null)
    {
        OnSceneChanged?.Invoke(screen, data);
    }

    ~Scene() => Dispose(false);
    public virtual void Initialize() => LoadContent();
    public virtual void LoadContent() { }
    public virtual void UnloadContent() => Content.Unload();
    public virtual void Update(GameTime gameTime) { }
    public virtual void Draw(GameTime gameTime) { }

    public void RaiseScreenChanged(ScreenType type, Dictionary<string, object>? args = null)
    {
        OnSceneChanged?.Invoke(type, args);
    }
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
