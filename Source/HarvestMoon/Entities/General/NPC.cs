using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities.General
{
    public class NPC : Interactable
    {
        public enum NPCMenu
        {
            YesNo,
            Livestock,
            FarmGoods
        }

        protected List<Action> _callbacks = new List<Action>();

        protected List<string> _strings = new List<string>();

        protected string _message;

        public string Message => _message;

        public List<string> Strings => _strings;
        public List<Action> Callbacks => _callbacks;

        public bool DeploysMenu { get; set; }

        public bool ShowsMessage { get; set; }

        public NPCMenu DeployableMenu { get; set; }

        public NPC(Vector2 initialPosition,
                    Size2 size,
                    bool showsMessage = false,
                    string message = default(string),
                    bool deploysMenu = false,
                    NPCMenu deployableMenu = NPCMenu.YesNo,
                    List<string> strings = null,
                    List<Action> callbacks = null)
        {
            BoundingRectangle = new RectangleF(new Vector2(initialPosition.X - 32,
                                                            initialPosition.Y - 32),
                                               new Size2(size.Width, size.Height));

            X = initialPosition.X;
            Y = initialPosition.Y;

            Planked = true;
            Carryable = false;
            Interacts = true;
            IsNPC = true;

            _message = message;

            ShowsMessage = showsMessage;

            DeploysMenu = deploysMenu;

            DeployableMenu = deployableMenu;

            if (strings != null)
            {
                _strings = strings;
            }

            if(callbacks != null)
            {
                _callbacks = callbacks;
            }
        }

        public virtual string GetMessage(Item item)
        {
            return Message;
        }

        public virtual void Interact(Item item)
        {

        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}
