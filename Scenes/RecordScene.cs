using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using Microsoft.Data.Sqlite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MazeGame.Scenes;

public class RecordScene : Scene
{
    private const string HIGHSCORES_TEXT = "High scores";
    private string boardSize_text = "10x10 board";
    private string scores_text = "";
    private SpriteFont _robotoFont;
    private Color backgroundColor = new Color(2, 62, 138, 255);
    private Color textColor = new Color(173, 232, 244);
    private Color buttonColor = new Color(6, 212, 153);

    private Vector2 highScoresPos;
    private Vector2 highScoresOrigin;
    private Vector2 boardSizePos;
    private Vector2 boardSizeOrigin;
    private Vector2 scoresPos;
    private Vector2 scoresOrigin;

    private Panel panel;
    public override void Initialize()
    {
        base.Initialize();

        var sb = new StringBuilder();
        using var connection = new SqliteConnection($"Data Source={Game1.sqlitePath}");
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT time FROM scores WHERE maze_size LIKE \"10x10\"";
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var score = reader.GetString(0);

            sb.AppendLine(score);
        }
        scores_text = sb.ToString();

        highScoresOrigin = _robotoFont.MeasureString(HIGHSCORES_TEXT) * 0.5f;
        highScoresPos = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f, GraphicsDevice.PresentationParameters.BackBufferHeight * 0.1f);
        boardSizeOrigin = _robotoFont.MeasureString(boardSize_text) * 0.5f;
        boardSizePos = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f, GraphicsDevice.PresentationParameters.BackBufferHeight * 0.2f);
        scoresOrigin = _robotoFont.MeasureString(scores_text) * 0.5f;
        scoresOrigin.Y = 0;
        scoresPos = boardSizePos;
        scoresPos.Y += _robotoFont.MeasureString(boardSize_text).Y * 2.0f;

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

        Game1._spriteBatch.DrawString(_robotoFont, HIGHSCORES_TEXT, highScoresPos, textColor, 0.0f, highScoresOrigin, 2.0f, SpriteEffects.None, 0.0f);
        Game1._spriteBatch.DrawString(_robotoFont, boardSize_text, boardSizePos, textColor, 0.0f, boardSizeOrigin, 1.5f, SpriteEffects.None, 0.0f);
        Game1._spriteBatch.DrawString(_robotoFont, scores_text, scoresPos, textColor, 0.0f, scoresOrigin, 1.0f, SpriteEffects.None, 0.0f);

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

        var backToMenuButton = new Button();
        backToMenuButton.Anchor(Gum.Wireframe.Anchor.Bottom);
        backToMenuButton.Visual.WidthUnits = Gum.DataTypes.DimensionUnitType.ScreenPixel;
        backToMenuButton.Visual.HeightUnits = Gum.DataTypes.DimensionUnitType.ScreenPixel;
        backToMenuButton.Visual.Width = buttonWidth;
        backToMenuButton.Visual.Height = buttonHeight;
        //backToMenuButton.Visual.X = buttonWidth * -0.05f;
        backToMenuButton.Visual.Y = buttonHeight * -0.2f;
        var menuVisual = (ButtonVisual)backToMenuButton.Visual;
        menuVisual.Background.Color = buttonColor;
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
