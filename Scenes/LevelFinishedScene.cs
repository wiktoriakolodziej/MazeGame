using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using Syncfusion.XForms.Android.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MazeGame.Scenes;

public class LevelFinishedScene : Scene
{
    private const string CONGRATS_TEXT = "Congratulations!";
    private const string YOURTIME_TEXT = "Your time is";
    private string time_text = "";
    private SpriteFont _robotoFont;
    private Color backgroundColor = new Color(2, 62, 138, 255);
    private Color textColor = new Color(173, 232, 244);

    private Vector2 congratsPos;
    private Vector2 congratsOrigin;
    private Vector2 yourtimePos;
    private Vector2 yourtimeOrigin;
    private Vector2 timePos;
    private Vector2 timeOrigin;

    private Panel panel;
    public override void Initialize()
    {
        base.Initialize();
        time_text = (Game1.timeScore / 1000.0f).ToString() + "s";
        congratsOrigin = _robotoFont.MeasureString(CONGRATS_TEXT) * 0.5f;
        congratsPos = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f, GraphicsDevice.PresentationParameters.BackBufferHeight * 0.25f);
        yourtimeOrigin = _robotoFont.MeasureString(YOURTIME_TEXT) * 0.5f;
        yourtimePos = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f, GraphicsDevice.PresentationParameters.BackBufferHeight * 0.5f);
        timeOrigin = _robotoFont.MeasureString(time_text) * 0.5f;
        timePos = yourtimePos;
        timePos.Y += _robotoFont.MeasureString(YOURTIME_TEXT).Y * 2.0f;

        InitializeUI();
    }
    public override void LoadContent()
    {
        _robotoFont = Content.Load<SpriteFont>("fonts/Roboto");
    }

    public override void Update(GameTime gameTime)
    {
        GumService.Default.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(backgroundColor);

        Game1._spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        Game1._spriteBatch.DrawString(_robotoFont, CONGRATS_TEXT, congratsPos, textColor, 0.0f, congratsOrigin, 2.0f, SpriteEffects.None, 0.0f);
        Game1._spriteBatch.DrawString(_robotoFont, YOURTIME_TEXT, yourtimePos, textColor, 0.0f, yourtimeOrigin, 1.0f, SpriteEffects.None, 0.0f);
        Game1._spriteBatch.DrawString(_robotoFont, time_text, timePos, textColor, 0.0f, timeOrigin, 1.5f, SpriteEffects.None, 0.0f);

        Game1._spriteBatch.End();

        GumService.Default.Draw();
    }

    private void CreateLevelFinishedPanel()
    {
        int buttonWidth = (int)(GraphicsDevice.PresentationParameters.BackBufferWidth * 0.4);
        int buttonHeight = (int)(GraphicsDevice.PresentationParameters.BackBufferHeight * 0.1);
        Console.WriteLine(buttonWidth);
        Console.WriteLine(buttonHeight);

        panel = new Panel();
        panel.Dock(Gum.Wireframe.Dock.Fill);
        panel.AddToRoot();

        var nextLevelButton = new Button();
        nextLevelButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        nextLevelButton.Visual.WidthUnits = Gum.DataTypes.DimensionUnitType.ScreenPixel;
        nextLevelButton.Visual.HeightUnits = Gum.DataTypes.DimensionUnitType.ScreenPixel;
        nextLevelButton.Visual.Width = buttonWidth;
        nextLevelButton.Visual.Height = buttonHeight;
        nextLevelButton.Visual.X = buttonWidth * 0.05f;
        nextLevelButton.Visual.Y = buttonHeight * -0.2f;
        nextLevelButton.Text = "NEXT LEVEL";
        nextLevelButton.Click += HandleNextLevelClicked;
        panel.AddChild(nextLevelButton);

        var backToMenuButton = new Button();
        backToMenuButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        backToMenuButton.Visual.WidthUnits = Gum.DataTypes.DimensionUnitType.ScreenPixel;
        backToMenuButton.Visual.HeightUnits = Gum.DataTypes.DimensionUnitType.ScreenPixel;
        backToMenuButton.Visual.Width = buttonWidth;
        backToMenuButton.Visual.Height = buttonHeight;
        backToMenuButton.Visual.X = buttonWidth * -0.05f;
        backToMenuButton.Visual.Y = buttonHeight * -0.2f;
        backToMenuButton.Text = "MAIN MENU";
        backToMenuButton.Click += HandleBackToMenuClicked;
        panel.AddChild(backToMenuButton);

    }

    private void HandleNextLevelClicked(object sender, EventArgs e)
    {

        OnSceneChanged(ScreenType.Gameplay);
    }

    private void HandleBackToMenuClicked(object sender, EventArgs e)
    {

        OnSceneChanged(ScreenType.Title);
    }

    private void InitializeUI()
    {
        // Clear out any previous UI in case we came here from
        // a different screen:
        GumService.Default.Root.Children.Clear();
        CreateLevelFinishedPanel();

    }
}
