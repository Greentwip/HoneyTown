using Microsoft.Xna.Framework;
using System.Collections.Generic;

using HarvestMoon.Entities;
using System.Linq;
using HarvestMoon.Screens;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System.Xml.Serialization;
using System.IO;
using Windows.Storage;
using System;
using HarvestMoon.Entities.Ranch;

namespace HarvestMoon
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class HarvestMoon : Game
    {
        public string Diary { get; set; }
        public int GrassSeeds { get; set; }
        public int TurnipSeeds { get; set; }
        public int PotatoSeeds { get; set; }
        public int CornSeeds { get; set; }

        private ScreenManager ScreenManager = new ScreenManager();
        private bool _loaded = false;


        GraphicsDeviceManager _graphics;

        public RanchState RanchState { get; private set;}


        private List<string> _tools = new List<string>();


        public List<string> Tools { get => _tools; set => _tools = value; }

        public bool IsToolPacked(string toolName)
        {
            return _tools.Contains(toolName);
        }

        public string GetCurrentTool()
        {
            return _tools.LastOrDefault();
        }

        public string GetOtherTool()
        {
            return _tools.FirstOrDefault();
        }

        public string SwapTools()
        {
            var currentTool = GetCurrentTool();
            var otherTool = GetOtherTool();

            if(currentTool == otherTool)
            {
                otherTool = default(string);
            }

            _tools.Clear();

            if(currentTool != default(string))
            {
                _tools.Add(currentTool);
            }

            if(otherTool != default(string))
            {
                _tools.Add(otherTool);
            }

            return GetCurrentTool();
        }

        public string PackTool(string toolName)
        {
            var lastTool = GetCurrentTool();

            _tools.Add(toolName);

            if (_tools.Count == 3)
            {
                _tools.Remove(lastTool);
            } else if(_tools.Count == 1 || _tools.Count == 2)
            {
                lastTool = "none";
            }
            
            return lastTool;
        }

        public static HarvestMoon Instance { get; private set; }

        public enum DayTime
        {
            Sunrise,
            Morning,
            Evening,
            Afternoon
        }

        public string PlayerName { get; set; }
        public int DayNumber { get; set; }
        public string DayName { get; set; }
        public string Season { get; set; }
        public int YearNumber { get; set; }
        public int Gold { get; set; }

        public bool HasNotSeenTheRanch { get; set; }

        private float _dayTime;

        private float _day;

        private float _morning;
        private float _evening;
        private float _afternoon;

        public float RanchDayTime { get => _dayTime; set => _dayTime = value; }

        private Dictionary<DayTime, bool> _dayTimeTriggers = new Dictionary<DayTime, bool>();

        public bool GetDayTimeTriggered(DayTime trigger)
        {
            return _dayTimeTriggers[trigger];
        }

        public void SetDayTimeTriggered(DayTime trigger, bool triggered)
        {
            _dayTimeTriggers[trigger] = triggered;
        }

        public DayTime GetDayTime()
        {
            if(_dayTime < _morning)
            {
                return DayTime.Sunrise;
            }
            else if (_dayTime >= _morning && _dayTime <= _evening)
            {
                return DayTime.Morning;
            }
            else if (_dayTime >= _evening && _dayTime <= _afternoon)
            {
                return DayTime.Evening;

            }
            else
            {
                return DayTime.Afternoon;
            }

        }

        public HarvestMoon()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            Components.Add(ScreenManager);

            RanchState = new RanchState();

            ResetDay();

            Instance = this;
        }

        [Serializable]
        public struct SaveGame
        {
            public string PlayerName { get; set; }
            public string DayName { get; set; }
            public int DayNumber { get; set; }
            public string Season { get; set; }
            public int YearNumber { get; set; }

            public int Gold { get; set; }

            public List<string> Tools { get; set; }

            public bool HasNotSeenTheRanch { get; set; }

            public List<BigLog> BigLogs { get; set; }
            public List<BigRock> BigRocks { get; set; }
            public List<Bush> Bushes { get; set; }
            public List<SmallRock> SmallRocks { get; set; }
            public List<Soil> SoilSegments { get; set; }
            //public List<Crop> Crops { get; set; }
            public List<WoodPiece> WoodPieces { get; set; }
        }

        public void SaveGameState(string diaryFile)
        {
            SaveGame sg = new SaveGame();

            sg.PlayerName = "Kai";
            sg.DayNumber = Instance.DayNumber;
            sg.DayName = Instance.DayName;
            sg.Season = Instance.Season;
            sg.YearNumber = Instance.YearNumber;
            sg.Tools = Instance.Tools;
            sg.HasNotSeenTheRanch = Instance.HasNotSeenTheRanch;

            sg.BigLogs = new List<BigLog>(Instance.RanchState.Entities.Where(e => e is BigLog).Cast<BigLog>().ToArray());
            sg.BigRocks = new List<BigRock>(Instance.RanchState.Entities.Where(e => e is BigRock).Cast<BigRock>().ToArray());
            sg.Bushes = new List<Bush>(Instance.RanchState.Entities.Where(e => e is Bush).Cast<Bush>().ToArray());
            sg.SmallRocks = new List<SmallRock>(Instance.RanchState.Entities.Where(e => e is SmallRock).Cast<SmallRock>().ToArray());
            sg.SoilSegments = new List<Soil>(Instance.RanchState.Entities.Where(e => e is Soil).Cast<Soil>().ToArray());
            sg.WoodPieces = new List<WoodPiece>(Instance.RanchState.Entities.Where(e => e is WoodPiece).Cast<WoodPiece>().ToArray());

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = localFolder.CreateFileAsync(diaryFile + ".xml", CreationCollisionOption.ReplaceExisting).AsTask().Result;
            var fileStream = sampleFile.OpenStreamForWriteAsync().Result;

            // Convert the object to XML data and put it in the stream
            XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
            serializer.Serialize(fileStream, sg);

            fileStream.Flush();

            // Close the file
            fileStream.Dispose();

        }

        public SaveGame GetDiary(string diaryFile)
        {
            SaveGame saveGame = new SaveGame();

            try
            {

                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = localFolder.GetFileAsync(diaryFile + ".xml").AsTask().Result;
                var fileStream = sampleFile.OpenStreamForReadAsync().Result;

                // Convert the object to XML data and put it in the stream
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
                saveGame = (SaveGame)serializer.Deserialize(fileStream);

                // Close the file
                fileStream.Dispose();

            }
            catch
            {
                return new SaveGame();
            }

            return saveGame;
        }

        public void LoadGameState(string diaryFile)
        {
            SaveGame saveGame = new SaveGame();

            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = localFolder.GetFileAsync(diaryFile + ".xml").AsTask().Result;
                var fileStream = sampleFile.OpenStreamForReadAsync().Result;

                // Convert the object to XML data and put it in the stream
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
                saveGame = (SaveGame)serializer.Deserialize(fileStream);

                // Close the file
                fileStream.Dispose();


                Instance.PlayerName = saveGame.PlayerName;

                Instance.DayNumber = saveGame.DayNumber;
                Instance.DayName = saveGame.DayName;
                Instance.Season = saveGame.Season;
                Instance.YearNumber = saveGame.YearNumber;
                Instance.Gold = saveGame.Gold;

                Instance.Tools = saveGame.Tools;

                Instance.HasNotSeenTheRanch = saveGame.HasNotSeenTheRanch;

                

                List<BigLog> bigLogs = new List<BigLog>();
                List<BigRock> bigRocks = new List<BigRock>();
                List<Bush> bushes = new List<Bush>();
                List<SmallRock> smallRocks = new List<SmallRock>();
                List<Soil> soilSegments = new List<Soil>();
                List<WoodPiece> woodPieces = new List<WoodPiece>();

                for (int i = 0; i < saveGame.BigLogs.Count; ++i)
                {
                    bigLogs.Add(new BigLog(Content, new Vector2(saveGame.BigLogs[i].X, saveGame.BigLogs[i].Y)));
                }

                for (int i = 0; i < saveGame.BigRocks.Count; ++i)
                {
                    bigRocks.Add(new BigRock(Content, new Vector2(saveGame.BigRocks[i].X, saveGame.BigRocks[i].Y)));
                }

                for (int i = 0; i < saveGame.Bushes.Count; ++i)
                {
                    bushes.Add(new Bush(Content, new Vector2(saveGame.Bushes[i].X, saveGame.Bushes[i].Y)));
                }

                for (int i = 0; i < saveGame.SmallRocks.Count; ++i)
                {
                    smallRocks.Add(new SmallRock(Content, new Vector2(saveGame.SmallRocks[i].X, saveGame.SmallRocks[i].Y)));
                }

                for (int i = 0; i < saveGame.SoilSegments.Count; ++i)
                {
                    soilSegments.Add(new Soil(Content, 
                                                new Vector2(saveGame.SoilSegments[i].X, saveGame.SoilSegments[i].Y),
                                                saveGame.SoilSegments[i].IsPlanted,
                                                saveGame.SoilSegments[i].CropType,
                                                saveGame.SoilSegments[i].DaysWatered,
                                                saveGame.SoilSegments[i].SeasonPlanted));
                }

                for (int i = 0; i < saveGame.WoodPieces.Count; ++i)
                {
                    woodPieces.Add(new WoodPiece(Content, new Vector2(saveGame.WoodPieces[i].X, saveGame.WoodPieces[i].Y)));
                }

                

                RanchState.Entities.AddRange(bigLogs);
                RanchState.Entities.AddRange(bigRocks);
                RanchState.Entities.AddRange(bushes);
                RanchState.Entities.AddRange(smallRocks);
                RanchState.Entities.AddRange(soilSegments);
                RanchState.Entities.AddRange(woodPieces);

                RanchState.IsLoaded = true;
            }
            catch(Exception ex)
            {
                // no save game file found
            }

            if(saveGame.PlayerName == default(string))
            {
                Instance.DayNumber = 1;
                Instance.Season = "Spring";
                Instance.DayName = "Monday";
                Instance.YearNumber = 1;
                Instance.HasNotSeenTheRanch = true;
            }

        }
 


        public void ResetDay()
        {
            _dayTimeTriggers[DayTime.Sunrise] = false;
            _dayTimeTriggers[DayTime.Morning] = false;
            _dayTimeTriggers[DayTime.Evening] = false;
            _dayTimeTriggers[DayTime.Afternoon] = false;

            _day = 2.5f;

            _morning = (_day * 60.0f / 3) * 1;
            _evening = (_day * 60.0f / 3) * 2;
            _afternoon = (_day * 60.0f / 3) * 3;

            RanchDayTime = 0.0f;
        }

        public void IncrementDay()
        {
            if(Instance.DayName == "Monday")
            {
                Instance.DayName = "Tuesday";
            }
            else if (Instance.DayName == "Tuesday")
            {
                Instance.DayName = "Wednesday";
            }
            else if (Instance.DayName == "Wednesday")
            {
                Instance.DayName = "Thursday";
            }
            else if (Instance.DayName == "Thursday")
            {
                Instance.DayName = "Friday";
            }
            else if (Instance.DayName == "Friday")
            {
                Instance.DayName = "Saturday"; 
            }
            else if (Instance.DayName == "Saturday")
            {
                Instance.DayName = "Sunday";
            }
            else if (Instance.DayName == "Sunday")
            {
                Instance.DayName = "Monday";
            }

            Instance.DayNumber++;

            if(Instance.DayNumber >= 31)
            {
                Instance.DayNumber = 1;
                if(Instance.Season == "Spring")
                {
                    Instance.Season = "Summer";
                }
                else if(Instance.Season == "Summer")
                {
                    Instance.Season = "Autumn";
                }
                else if(Instance.Season == "Autumn")
                {
                    Instance.Season = "Winter";
                }
                else if(Instance.Season == "Winter")
                {
                    Instance.Season = "Spring";
                    Instance.YearNumber++;
                }
            }
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!_loaded)
            {
                _loaded = true;
                ScreenManager.LoadScreen(new Diary(this), new FadeTransition(GraphicsDevice, Color.Black, 1.0f));
            }
            

        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            


            base.Draw(gameTime);
        }
    }
}
