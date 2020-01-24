using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestMoon.Animation
{
    public class AnimationLoader
    {
        public static AnimatedSprite LoadAnimatedSprite(ContentManager content, 
                                                        string textureFile, 
                                                        string mapFile, 
                                                        string atlasName,
                                                        float frameDuration,
                                                        bool loop)
        {
            var characterTexture = content.Load<Texture2D>(textureFile);
            var characterMap = content.Load<Dictionary<string, Rectangle>>(mapFile);
            var characterAtlas = new TextureAtlas(atlasName, characterTexture, characterMap);

            Dictionary<string, List<KeyValuePair<string, int>>> animations = new Dictionary<string, List<KeyValuePair<string, int>>>();

            List<TextureRegion2D> regions = new List<TextureRegion2D>(characterAtlas.Regions);

            for (int i = 0; i < regions.Count; ++i)
            {
                var region = regions[i];

                var regionName = region.Name;
                int lastIndexOf = regionName.LastIndexOf("_");
                string animationName = regionName.Substring(0, lastIndexOf);
                string animationFrame = regionName.Substring(lastIndexOf + 1, regionName.Length - animationName.Length - 1);

                if (!animations.ContainsKey(animationName))
                {
                    animations[animationName] = new List<KeyValuePair<string, int>>();
                }

                animations[animationName].Add(new KeyValuePair<string, int>(animationFrame, i));
            }

            var characterAnimationFactory = new SpriteSheet();
            characterAnimationFactory.TextureAtlas = characterAtlas;

            foreach (var animation in animations)
            {
                var cycle = new SpriteSheetAnimationCycle();
                cycle.IsLooping = loop;
                cycle.IsPingPong = false;
                cycle.FrameDuration = frameDuration;

                var frames = new List<SpriteSheetAnimationFrame>();

                for (int i = 0; i < animation.Value.Count; ++i)
                {
                    frames.Add(new SpriteSheetAnimationFrame(animation.Value[i].Value));
                }

                cycle.Frames = frames;

                characterAnimationFactory.Cycles.Add(animation.Key, cycle);
            }

            return new AnimatedSprite(characterAnimationFactory);
        }
    }
}
