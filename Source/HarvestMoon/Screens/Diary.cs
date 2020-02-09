using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using HarvestMoon.Entities;
using System.Linq;
using MonoGame.Extended.Screens.Transitions;
using HarvestMoon.Entities.General;
using System.Collections.Generic;
using System;
using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework.Input;
using HarvestMoon.Input;
using MonoGame.Extended.Sprites;
using Microsoft.Xna.Framework.Media;

namespace HarvestMoon.Screens
{
    public class Diary : GUI
    {

        List<Panel> _panels;

        Panel _selectionPanel;
        Panel _guiTextPanel;

        private bool _triggered;

        private bool _eraseTrigger;

        private Paragraph _guiParagraph;
        private Paragraph _diary1Paragraph;
        private Paragraph _diary2Paragraph;

        private Sprite _sosTexture;
        private Sprite _tilesTexture;

        private Vector2 _mainSquareBottomPosition = new Vector2(320 - 640, 320 + 640);
        private Vector2 _mainSquareCenterPosition = new Vector2(320, 320);
        private Vector2 _mainSquareTopPosition = new Vector2(320 + 640, 320 - 640);

        private Vector2 _mainSquareCurrentPosition = new Vector2(320, 320);

        private Vector2 _sosTextureBottomPosition = new Vector2(320 - 640, 320 + 640);
        private Vector2 _sosTextureCenterPosition = new Vector2(320, 320);
        private Vector2 _sosTextureTopPosition = new Vector2(320 + 640, 320 - 640);

        private Vector2 _sosTextureCurrentPosition = new Vector2(320 + 640, 320 - 640);


        public Diary(Game game)
            : base(game)
        {
            _panels = new List<Panel>();
            _panels.Add(null);
            _panels.Add(null);


        }

        public override void Initialize()
        {
            // call base initialize func
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            var song = Content.Load<Song>("audio/music/diary");

            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;


            _sosTexture = new Sprite(Content.Load<Texture2D>("animations/sos"));
            _tilesTexture = new Sprite(Content.Load<Texture2D>("animations/title-squares"));

            // Load the compiled map
            _map = Content.Load<TiledMap>("maps/diary/screen");
            // Create the map renderer
            _mapRenderer = new TiledMapRenderer(GraphicsDevice, _map);

            foreach (var layer in _map.ObjectLayers)
            {
                if (layer.Name == "objects")
                {
                    foreach (var obj in layer.Objects)
                    {
                        if (obj.Type == "diary")
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X;
                            objectPosition.Y = obj.Position.Y;

                            // create a panel and position in center of screen
                            var textPanel = new Panel(new Vector2(obj.Size.Width, obj.Size.Height), 
                                                        PanelSkin.Default, 
                                                        Anchor.TopLeft, 
                                                        new Vector2(objectPosition.X, objectPosition.Y));

                            UserInterface.Active.AddEntity(textPanel);

                            var diary = HarvestMoon.Instance.GetDiary("diary-" + obj.Name);

                            string displayString = "\n         No Diary";

                            if(diary.PlayerName != default(string)){
                                displayString = diary.PlayerName + 
                                                "        " + "Year: " + 
                                                diary.YearNumber + 
                                                "\n    " + 
                                                diary.DayNumber + 
                                                " of " + 
                                                diary.Season;
                            }

                            var paragraph = new Paragraph(displayString);


                            textPanel.AddChild(paragraph);


                            if (obj.Name == "1")
                            {
                                _panels[0] = textPanel;

                                _selectionPanel = textPanel;

                                _diary1Paragraph = paragraph;

                                //_panels[0].Offset = new Vector2(170, 120);
                                //_panels[0].Size = new Vector2(470, 120);

                            }
                            else
                            {
                                _panels[1] = textPanel;
                                _diary2Paragraph = paragraph;
                                //_panels[1].Offset = new Vector2(170, 320);
                                //_panels[1].Size = new Vector2(470, 120);
                            }
                        }
                        else if(obj.Type == "gui")
                        {
                            var objectPosition = obj.Position;

                            objectPosition.X = obj.Position.X;
                            objectPosition.Y = obj.Position.Y;

                            // create a panel and position in center of screen
                            _guiTextPanel = new Panel(new Vector2(obj.Size.Width, obj.Size.Height),
                                                        PanelSkin.Default,
                                                        Anchor.TopLeft,
                                                        new Vector2(objectPosition.X, objectPosition.Y));

                            UserInterface.Active.AddEntity(_guiTextPanel);

                            _guiParagraph = new Paragraph("    " + "Choose Diary");

                            _guiTextPanel.AddChild(_guiParagraph);


                            //_guiTextPanel.Size = new Vector2(470, 70);
                            //_guiTextPanel.Offset = new Vector2(170, 460);

                        }
                    }
                }
                
            }

            HarvestMoon.Instance.Diary = "diary-1";
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            UserInterface.Active.RemoveEntity(_guiTextPanel);
            UserInterface.Active.RemoveEntity(_panels[0]);
            UserInterface.Active.RemoveEntity(_panels[1]);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _mapRenderer.Update(gameTime);
            var keyboardState = HarvestMoon.Instance.Input;

            if (((keyboardState.IsKeyDown(InputDevice.Keys.Up) && !_isUpButtonDown) || 
                (keyboardState.IsKeyDown(InputDevice.Keys.Down) && !_isDownButtonDown)) && !_triggered)
            {
                if (keyboardState.IsKeyDown(InputDevice.Keys.Up))
                {
                    _isUpButtonDown = true;
                }

                if (keyboardState.IsKeyDown(InputDevice.Keys.Down))
                {
                    _isDownButtonDown = true;
                }

                if (_selectionPanel == _panels[0])
                {
                    _selectionPanel = _panels[1];
                    HarvestMoon.Instance.Diary = "diary-1";
                }
                else
                {
                    _selectionPanel = _panels[0];
                    HarvestMoon.Instance.Diary = "diary-2";
                }

            }

            if (_selectionPanel != null)
            {
                _selectionPanel.Opacity = 255;

                _panels.First(p => p != _selectionPanel).Opacity = 150;

                if (keyboardState.IsKeyDown(InputDevice.Keys.A) && !_isActionButtonDown && !_triggered)
                {
                    _isActionButtonDown = true;
                    if (_selectionPanel == _panels[0])
                    {
                        HarvestMoon.Instance.Diary = "diary-1";
                    }
                    else
                    {
                        HarvestMoon.Instance.Diary = "diary-2";
                    }

                    _triggered = true;
                }

                if (keyboardState.IsKeyDown(InputDevice.Keys.X) && !_isRunButtonDown && !_eraseTrigger)
                {
                    _isRunButtonDown = true;

                    var diary = HarvestMoon.Instance.GetDiary(HarvestMoon.Instance.Diary);

                    if (diary.PlayerName != default(string))
                    {
                        _guiParagraph.Text = "Erase Diary?";
                        _eraseTrigger = true;
                    }
                }
                else if (keyboardState.IsKeyDown(InputDevice.Keys.X) && !_isRunButtonDown && _eraseTrigger)
                {
                    _isRunButtonDown = true;

                    var diary = HarvestMoon.Instance.GetDiary(HarvestMoon.Instance.Diary);

                    if (diary.PlayerName != default(string))
                    {
                        HarvestMoon.Instance.EraseDiary(HarvestMoon.Instance.Diary);
                        _guiParagraph.Text = "Choose Diary";

                        string displayString = "\n         No Diary";

                        if (_selectionPanel == _panels[0])
                        {
                            _diary1Paragraph.Text = displayString;
                        }
                        else
                        {
                            _diary2Paragraph.Text = displayString;
                        }


                        _eraseTrigger = false;


                    }
                }
            }

            if (_triggered)
            {
                if (MediaPlayer.Volume > 0.0f && MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Volume -= 0.1f;
                }

                if (MediaPlayer.Volume <= 0)
                {
                    MediaPlayer.Stop();
                    MediaPlayer.Volume = 1.0f;

                    var screen = new House(Game, HarvestMoon.Arrival.Diary);
                    var transition = new FadeTransition(GraphicsDevice, Color.Black, 2.0f);
                    ScreenManager.LoadScreen(screen, transition);

                }
            }

            _mainSquareCurrentPosition = new Vector2(_mainSquareCurrentPosition.X - 1, _mainSquareCurrentPosition.Y + 1);
            _sosTextureCurrentPosition = new Vector2(_mainSquareCurrentPosition.X - 1, _mainSquareCurrentPosition.Y + 1);

            if (_mainSquareCurrentPosition == _mainSquareBottomPosition)
            {
                _mainSquareCurrentPosition = _mainSquareTopPosition;
            }

            if(_sosTextureCurrentPosition == _sosTextureBottomPosition)
            {
                _sosTextureCurrentPosition = _sosTextureTopPosition;
            }

            //_camera.LookAt(new Vector2(_map.Width * 4 / 2, _map.Height * 4 / 2));

        }

        public override void Draw(GameTime gameTime)
        {
            _guiTextPanel.Size = new Vector2(_guiTextPanel.Size.X, 64);
            GraphicsDevice.Clear(Color.Black);
            var cameraMatrix = _camera.GetViewMatrix();

            _spriteBatch.Begin(transformMatrix: cameraMatrix, samplerState: SamplerState.PointClamp);

            _spriteBatch.Draw(_tilesTexture, _mainSquareCurrentPosition);

            _spriteBatch.Draw(_tilesTexture, new Vector2(_mainSquareCurrentPosition.X, _mainSquareCurrentPosition.Y - 640));
            _spriteBatch.Draw(_tilesTexture, new Vector2(_mainSquareCurrentPosition.X + 640, _mainSquareCurrentPosition.Y));
            _spriteBatch.Draw(_tilesTexture, new Vector2(_mainSquareCurrentPosition.X + 640, _mainSquareCurrentPosition.Y - 640));
            _spriteBatch.Draw(_tilesTexture, new Vector2(_mainSquareCurrentPosition.X - 640, _mainSquareCurrentPosition.Y));
            _spriteBatch.Draw(_tilesTexture, new Vector2(_mainSquareCurrentPosition.X - 640, _mainSquareCurrentPosition.Y + 640));
            _spriteBatch.Draw(_tilesTexture, new Vector2(_mainSquareCurrentPosition.X, _mainSquareCurrentPosition.Y + 640));

            _spriteBatch.Draw(_sosTexture, _sosTextureCurrentPosition);

            _spriteBatch.End();

            float scaleY = HarvestMoon.Instance.Graphics.GraphicsDevice.Viewport.Height / 480.0f;


            cameraMatrix.Translation = new Vector3(cameraMatrix.Translation.X, cameraMatrix.Translation.Y - 24 * scaleY, cameraMatrix.Translation.Z);

            _spriteBatch.Begin(transformMatrix: cameraMatrix, samplerState: SamplerState.PointClamp);


            for (int i = 0; i < _map.Layers.Count; ++i)
            {
                _mapRenderer.Draw(_map.Layers[i], cameraMatrix);

            }
            // End the sprite batch
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
