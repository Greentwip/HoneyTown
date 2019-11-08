﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using System.Collections.Generic;

using MonoGame.Extended.Content;
using Microsoft.Xna.Framework.Content;

namespace HarvestMoon.Entities
{
    public class Hammer : Tool
    {
        public Hammer(ContentManager content, Vector2 initialPosition)
        : base("hammer", content, initialPosition, "maps/tools-room/items/hammer")
        {
        }

        public override void Update(GameTime gameTime)
        {

        }
    }

}
