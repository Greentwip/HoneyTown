using Microsoft.Xna.Framework;
using System.Collections.Generic;

using HarvestMoon.Entities;
using System.Linq;
using HarvestMoon.Screens;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System.Xml.Serialization;
using System.IO;
#if WINDOWS_UAP
using Windows.Storage;
using Windows.Security.ExchangeActiveSyncProvisioning;
#endif
using System;
using HarvestMoon.Entities.Ranch;
using Microsoft.Xna.Framework.Input;

using HarvestMoon.Input;
using HarvestMoon.Text;
using HarvestMoon.GUI;
using GeonBit.UI;
using MonoGame.Extended.Tiled.Renderers;

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
        public int TomatoSeeds { get; set; }
        public int Cows { get; set; }
        public int Sheeps { get; set; }
        public int Chickens { get; set; }

        public int Stamina { get; set; }
        public int MaxStamina { get; set; }

        public List<KeyValuePair<string, bool>> CutsceneTriggers { get; set; }


        public ScreenManager ScreenManager = new ScreenManager();
        private bool _loaded = false;

        public enum Arrival
        {
            Diary,
            Wake,
            House,
            Tools,
            Barn,
            Ranch,
            Library,
            Dealer,
            Librarian,
            Mountain,
            Hill,
            Town
        }

        public GraphicsDeviceManager Graphics;

        public RanchState RanchState { get; private set;}


        private List<string> _tools = new List<string>();


        public List<string> Tools { get => _tools; set => _tools = value; }


        public string PlayerName { get; set; }
        public int DayNumber { get; set; }
        public string DayName { get; set; }
        public string Season { get; set; }
        public int YearNumber { get; set; }
        public int Gold { get; set; }
        public int TodayGold { get; set; }

        public int FeedPieces { get; set; }
        public int Planks { get; set; }

        public bool HasNotSeenTheRanch { get; set; }

        private float _dayTime;

        private float _day;

        private float _morning;
        private float _evening;
        private float _afternoon;

        public float RanchDayTime { get => _dayTime; set => _dayTime = value; }

        private Dictionary<DayTime, bool> _dayTimeTriggers = new Dictionary<DayTime, bool>();

        public InputDevice Input;

        public StringsManager Strings;

        public GUIManager GUI;

        public HarvestMoon()
        {
            Season = "Spring";
            DayName = "Monday";
            DayNumber = 1;

            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            Components.Add(ScreenManager);

            RanchState = new RanchState();

            ResetDay();

#if WINDOWS_UAP
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += (object sender, Windows.UI.Core.BackRequestedEventArgs args) => { args.Handled = true; };

            EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();

            if(deviceInfo.OperatingSystem == "WINDOWS")
            {

                if (deviceInfo.SystemProductName.Contains("Xbox One"))
                {
                    Input = new GamepadInputDevice();
                }
                else
                {
                    Input = new KeyboardInputDevice();
                }
            }
            else
            {
                Input = new KeyboardInputDevice();
            }
#else
            Input = new KeyboardInputDevice();
#endif

            Strings = new StringsManager();

            Instance = this;

            MaxStamina = 60;

        }


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

            if (currentTool == otherTool)
            {
                otherTool = default(string);
            }

            _tools.Clear();

            if (currentTool != default(string))
            {
                _tools.Add(currentTool);
            }

            if (otherTool != default(string))
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
            }
            else if (_tools.Count == 1 || _tools.Count == 2)
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

        private bool _nightTriggered;

        public bool GetDayTimeTriggered(DayTime trigger)
        {
            return _dayTimeTriggers[trigger];
        }

        public bool HasNightTriggered()
        {
            return _nightTriggered;
        }

        public void SetNightTriggered(bool triggered)
        {
            _nightTriggered = triggered;
        }

        public void SetDayTimeTriggered(DayTime trigger, bool triggered)
        {
            _dayTimeTriggers[trigger] = triggered;
        }

        public DayTime GetDayTime()
        {
            if (_dayTime < _morning)
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

        [Serializable]
        public struct SaveGame
        {
            public string PlayerName { get; set; }
            public string DayName { get; set; }
            public int DayNumber { get; set; }
            public string Season { get; set; }
            public int YearNumber { get; set; }

            public int Gold { get; set; }
            public int TodayGold { get; set; }

            public int MaxStamina { get; set; }

            public List<string> Tools { get; set; }

            public bool HasNotSeenTheRanch { get; set; }

            public List<BigLog> BigLogs { get; set; }
            public List<BigRock> BigRocks { get; set; }
            public List<Bush> Bushes { get; set; }
            public List<SmallRock> SmallRocks { get; set; }
            public List<Soil> SoilSegments { get; set; }
            //public List<Crop> Crops { get; set; }
            public List<WoodPiece> WoodPieces { get; set; }

            public List<KeyValuePair<string, bool>> CutsceneTriggers { get; set; }

            public int Cows { get; set; }
            public int Sheeps { get; set; }
            public int Chickens { get; set; }

            public int FeedPieces { get; set; }
            public int Planks { get; set; }
        }

        public void SaveGameState(string diaryFile)
        {
            SaveGame sg = new SaveGame();

            sg.PlayerName = "Kai";
            sg.DayNumber = Instance.DayNumber;
            sg.DayName = Instance.DayName;
            sg.Season = Instance.Season;
            sg.YearNumber = Instance.YearNumber;
            sg.Gold = Instance.Gold;
            sg.TodayGold = Instance.TodayGold;
            sg.Tools = Instance.Tools;
            sg.FeedPieces = Instance.FeedPieces;
            sg.Planks = Instance.Planks;
            sg.HasNotSeenTheRanch = Instance.HasNotSeenTheRanch;
            sg.Cows = Instance.Cows;
            sg.Sheeps = Instance.Sheeps;
            sg.Chickens = Instance.Chickens;

            sg.MaxStamina = Instance.MaxStamina;

            sg.CutsceneTriggers = Instance.CutsceneTriggers;

            sg.BigLogs = new List<BigLog>(Instance.RanchState.Entities.Where(e => e is BigLog).Cast<BigLog>().ToArray());
            sg.BigRocks = new List<BigRock>(Instance.RanchState.Entities.Where(e => e is BigRock).Cast<BigRock>().ToArray());
            sg.Bushes = new List<Bush>(Instance.RanchState.Entities.Where(e => e is Bush).Cast<Bush>().ToArray());
            sg.SmallRocks = new List<SmallRock>(Instance.RanchState.Entities.Where(e => e is SmallRock).Cast<SmallRock>().ToArray());
            sg.SoilSegments = new List<Soil>(Instance.RanchState.Entities.Where(e => e is Soil).Cast<Soil>().ToArray());
            sg.WoodPieces = new List<WoodPiece>(Instance.RanchState.Entities.Where(e => e is WoodPiece).Cast<WoodPiece>().ToArray());

#if WINDOWS_UAP
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = localFolder.CreateFileAsync(diaryFile + ".xml", CreationCollisionOption.ReplaceExisting).AsTask().Result;
            var fileStream = sampleFile.OpenStreamForWriteAsync().Result;
#else
            string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify), "HM");

            if (!Directory.Exists(appData))
            {
                Directory.CreateDirectory(appData);
            }

            string savefile = Path.Combine(appData, diaryFile + ".xml");

            StreamWriter fileStream = new StreamWriter(savefile);
#endif
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

#if WINDOWS_UAP
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = localFolder.GetFileAsync(diaryFile + ".xml").AsTask().Result;
                var fileStream = sampleFile.OpenStreamForReadAsync().Result;
#else
                string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify), "HM");

                if (!Directory.Exists(appData))
                {
                    Directory.CreateDirectory(appData);
                }

                string savefile = Path.Combine(appData, diaryFile + ".xml");

                StreamReader fileStream = new StreamReader(savefile);
#endif

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
#if WINDOWS_UAP
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = localFolder.GetFileAsync(diaryFile + ".xml").AsTask().Result;
                var fileStream = sampleFile.OpenStreamForReadAsync().Result;
#else
                string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify), "HM");

                if (!Directory.Exists(appData))
                {
                    Directory.CreateDirectory(appData);
                }

                string savefile = Path.Combine(appData, diaryFile + ".xml");

                StreamReader fileStream = new StreamReader(savefile);
#endif
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
                Instance.TodayGold = saveGame.TodayGold;
                Instance.FeedPieces = saveGame.FeedPieces;
                Instance.Planks = saveGame.Planks;
                Instance.Tools = saveGame.Tools;

                Instance.Cows = saveGame.Cows;
                Instance.Sheeps = saveGame.Sheeps;
                Instance.Chickens = saveGame.Chickens;

                Instance.MaxStamina = saveGame.MaxStamina != 0 ? saveGame.MaxStamina : 60;

                var cutsceneTriggers = new List<KeyValuePair<string, bool>>();
                cutsceneTriggers.Add(new KeyValuePair<string, bool>("onboarding", false));

                Instance.CutsceneTriggers = saveGame.CutsceneTriggers.Count == 0 ?
                    cutsceneTriggers : saveGame.CutsceneTriggers;
                    

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
                                                saveGame.SoilSegments[i].IsWatered,
                                                saveGame.SoilSegments[i].IsPlanted,
                                                saveGame.SoilSegments[i].CropType,
                                                saveGame.SoilSegments[i].DaysWatered,
                                                saveGame.SoilSegments[i].DaysPlanted));
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

        public bool HasTriggeredCutscene(string cutsceneName)
        {
            bool triggered = false;

            foreach(var keypair in CutsceneTriggers)
            {
                if(keypair.Key == cutsceneName)
                {
                    triggered = keypair.Value;
                }
            }

            return triggered;
        }

        public void SetCutsceneTriggered(string cutsceneName, bool trigger)
        {

            for (int i = 0; i<CutsceneTriggers.Count; ++i)
            {
                var keypair = CutsceneTriggers[i];

                if (keypair.Key == cutsceneName)
                {
                    CutsceneTriggers[i] = new KeyValuePair<string, bool>(cutsceneName, trigger);
                }
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

            if(Instance.DayNumber >= 30)
            {
                Instance.DayNumber = 1;
                if(Instance.Season == "Spring")
                {
                    Instance.Season = "Summer";
                }
                else if(Instance.Season == "Summer")
                {
                    Instance.Season = "Fall";
                }
                else if(Instance.Season == "Fall")
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

            Window.Title = "PROJECT LILA";


            // GeonBit.UI: Init the UI manager using the "hd" built-in theme
            if (UserInterface.Active == null)
            {
                UserInterface.Initialize(Content, BuiltinThemes.hd);
            }

            GUI = new GUIManager(Content);

            ResetDayTime();

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

        public TiledMapEffect DayTimeEffect;

        Slide<Color> _sunriseToMorningColor;
        Slide<Color> _morningToEveningColor;
        Slide<Color> _eveningToAfternoonColor;
        Slide<Color> _afternoonToSunriseColor;

        Slide<Color> _currentDayTimeColorSlider;

        Color _currentDayTimeColor;

        private void AdvanceDayTime()
        {
            if (_currentDayTimeColorSlider == _afternoonToSunriseColor)
            {
                _currentDayTimeColorSlider = _sunriseToMorningColor;
            }
            else if (_currentDayTimeColorSlider == _sunriseToMorningColor)
            {
                _currentDayTimeColorSlider = _morningToEveningColor;
            }
            else if (_currentDayTimeColorSlider == _morningToEveningColor)
            {
                _currentDayTimeColorSlider = _eveningToAfternoonColor;
            }
            /*else if(_currentDayTimeColor == _eveningToAfternoonColor)
            {
                _currentDayTimeColor = _afternoonToSunriseColor;
            }
            else
            {
                ResetDayTime();
            }*/
        }

        public void ResetDayTime()
        {
            _dayTime = 0.0f;

            _day = 2.5f;

            _morning = (_day * 60.0f / 3) * 1;
            _evening = (_day * 60.0f / 3) * 2;
            _afternoon = (_day * 60.0f / 3) * 3;


            _sunriseToMorningColor = new Slide<Color>(Color.White, Color.LightYellow, 2000d, Color.Lerp);
            _morningToEveningColor = new Slide<Color>(Color.LightYellow, new Color(220, 220, 180), 2000d, Color.Lerp);
            _eveningToAfternoonColor = new Slide<Color>(new Color(220, 220, 180), Color.DarkGray, 2000d, Color.Lerp);
            _afternoonToSunriseColor = new Slide<Color>(Color.DarkGray, Color.White, 2000d, Color.Lerp);

            _currentDayTimeColorSlider = _afternoonToSunriseColor;

            DayTimeEffect = new TiledMapEffect(GraphicsDevice);

            DayTimeEffect.TextureEnabled = true;
            DayTimeEffect.DiffuseColor = Color.White;

            /*
            switch (HarvestMoon.Instance.GetDayTime())
            {
                case HarvestMoon.DayTime.Sunrise:
                    _currentDayTimeColorSlider = _afternoonToSunriseColor;
                    break;

                case HarvestMoon.DayTime.Morning:
                    _currentDayTimeColorSlider = _sunriseToMorningColor;
                    break;

                case HarvestMoon.DayTime.Evening:
                    _currentDayTimeColorSlider = _morningToEveningColor;
                    break;

                case HarvestMoon.DayTime.Afternoon:
                    _currentDayTimeColorSlider = _eveningToAfternoonColor;
                    break;
            }
            */

        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!_loaded)
            {
                _loaded = true;
                ScreenManager.LoadScreen(new Diary(this), new FadeTransition(GraphicsDevice, Color.Black, 1.0f));
            }
            if ((ScreenManager.ActiveScreen is Ranch || ScreenManager.ActiveScreen is Town) && !GUI.IsPresenting)
            {
                var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

                _dayTime += deltaSeconds;

                if (_dayTime >= _morning && _dayTime <= _evening)
                {
                    if (_currentDayTimeColorSlider == _afternoonToSunriseColor)
                    {
                        AdvanceDayTime();
                    }

                }
                else if (_dayTime >= _evening && _dayTime <= _afternoon)
                {
                    if (_currentDayTimeColorSlider == _sunriseToMorningColor)
                    {
                        AdvanceDayTime();
                    }

                }
                else if (_dayTime >= _afternoon)
                {
                    if (_currentDayTimeColorSlider == _morningToEveningColor)
                    {
                        AdvanceDayTime();
                    }
                }

                if (HarvestMoon.Instance.GetDayTimeTriggered(HarvestMoon.Instance.GetDayTime()))
                {
                    switch (HarvestMoon.Instance.GetDayTime())
                    {
                        case HarvestMoon.DayTime.Sunrise:
                            _currentDayTimeColor = Color.White;
                            break;

                        case HarvestMoon.DayTime.Morning:
                            _currentDayTimeColor = Color.LightYellow;
                            break;

                        case HarvestMoon.DayTime.Evening:
                            _currentDayTimeColor = new Color(220, 220, 180);
                            break;

                        case HarvestMoon.DayTime.Afternoon:
                            _currentDayTimeColor = Color.DarkGray;
                            break;
                    }
                }
                else
                {
                    _currentDayTimeColor = _currentDayTimeColorSlider.Update(gameTime);

                    if (_currentDayTimeColor == Color.White)
                    {
                        if (HarvestMoon.Instance.GetDayTime() == HarvestMoon.DayTime.Sunrise)
                        {
                            HarvestMoon.Instance.SetDayTimeTriggered(HarvestMoon.DayTime.Sunrise, true);
                        }
                    }
                    else if (_currentDayTimeColor == Color.LightYellow)
                    {
                        if (HarvestMoon.Instance.GetDayTime() == HarvestMoon.DayTime.Morning)
                        {
                            HarvestMoon.Instance.SetDayTimeTriggered(HarvestMoon.DayTime.Morning, true);
                        }
                    }
                    else if (_currentDayTimeColor == new Color(220, 220, 180))
                    {
                        if (HarvestMoon.Instance.GetDayTime() == HarvestMoon.DayTime.Evening)
                        {
                            HarvestMoon.Instance.SetDayTimeTriggered(HarvestMoon.DayTime.Evening, true);
                        }

                    }
                    else if (_currentDayTimeColor == Color.DarkGray)
                    {
                        if (HarvestMoon.Instance.GetDayTime() == HarvestMoon.DayTime.Afternoon)
                        {
                            HarvestMoon.Instance.SetDayTimeTriggered(HarvestMoon.DayTime.Afternoon, true);
                        }

                    }

                }

                DayTimeEffect.DiffuseColor = _currentDayTimeColor;


            }

            GUI.Update(gameTime);

            base.Update(gameTime);

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
