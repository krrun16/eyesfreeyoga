// Kyle Rector
// UW Computer Science and Engineering
// rectorky@cs.washington.edu

namespace Microsoft.Samples.Kinect.EyesFreeYoga
{
    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Media;
    using System.Security.AccessControl;
    using System.Threading;
    using System.Timers;
    using System.Runtime.InteropServices;
    using System.Speech.Synthesis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Microsoft.Kinect;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;

    public partial class MainWindow : Window
    {
        private KinectSensor sensor;
        public static Skeleton savedSkeleton;
        private Skeleton skelToDraw;
        private KinectAudioSource kinectSource;
        private GLControl control = new GLControl();

        private SpeechRecognitionEngine speechRecognizer;
        private DispatcherTimer readyTimer;
        static SoundPlayer player;
        public static WMPLib.WindowsMediaPlayer background;

        Thread othread;
        static FileWriter log, allVarsCSV;
        public static String pwd = Directory.GetCurrentDirectory();
        String path = pwd + @"\Log\";
        public static Stopwatch timeOffScreen = new Stopwatch();
        static DateTime pauseStart, pauseFinish;
        public static DateTime start;

        private bool loaded = false, canExit = true;
        public static bool yes = false, cansayAnswer = false, canPlay = true, answered = false, toPause = false,
            toExit = false, beganWorkout = false, orient = false, stating = false, learningMode = true, 
            repeat = false, DEMO = false;
        public static int volumeLoud = 100, NUMPOSES = 27, volumeQuiet = 1, level, allowed = 0;
        public static double timeoffMS = 0;
        public static int[,] poseCounts = new int[NUMPOSES, 2];
        public static double[] levelUpTimes = new double[8] { 0, 30, 75, 142.5, 243.75, 395.625, 623.4375, 965.15625 }; // start with 30 and 1.5
        static Exercise exercise;
        static SpeechSynthesizer synth = new SpeechSynthesizer();
        Grammar inPainGrammar, gameControlsGrammar, gameNavigationGrammar, workoutNumberGrammar;

        private void SetupViewport()
        {
            int w = control.Width;
            int h = control.Height;
            GL.MatrixMode(MatrixMode.Projection);                        // Select The Projection Matrix
            GL.LoadIdentity();                           // Reset The Projection Matrix

            // Calculate The Aspect Ratio Of The Window
            OpenTK.Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, w / h, 0.1f, 100.0f);
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);                     // Select The Modelview Matrix
            GL.LoadIdentity();                           // Reset The Modelview Matrix
            GL.Ortho(0, w, 0, h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
        }

        void control_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (!loaded) // Play nice
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();

            GL.MatrixMode(MatrixMode.Modelview);

            GL.Color3(System.Drawing.Color.CornflowerBlue);

            if (this.skelToDraw != null)
            {
                GL.LineWidth(6.0f);
                GL.Begin(BeginMode.Lines);

                // head to neck
                if (skelToDraw.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.ShoulderCenter].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.Head].Position.X, skelToDraw.Joints[JointType.Head].Position.Y,
                    skelToDraw.Joints[JointType.Head].Position.Z * -1); // Head
                GL.Vertex3(skelToDraw.Joints[JointType.ShoulderCenter].Position.X, skelToDraw.Joints[JointType.ShoulderCenter].Position.Y,
                    skelToDraw.Joints[JointType.ShoulderCenter].Position.Z * -1); // Shoulder Center

                // Right Shoulder
                if (skelToDraw.Joints[JointType.ShoulderRight].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.ShoulderCenter].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.ShoulderCenter].Position.X, skelToDraw.Joints[JointType.ShoulderCenter].Position.Y,
                    skelToDraw.Joints[JointType.ShoulderCenter].Position.Z * -1); // Shoulder Center
                GL.Vertex3(skelToDraw.Joints[JointType.ShoulderRight].Position.X, skelToDraw.Joints[JointType.ShoulderRight].Position.Y,
                    skelToDraw.Joints[JointType.ShoulderRight].Position.Z * -1); // Shoulder Right

                // Left Shoulder
                if (skelToDraw.Joints[JointType.ShoulderLeft].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.ShoulderCenter].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.ShoulderCenter].Position.X, skelToDraw.Joints[JointType.ShoulderCenter].Position.Y,
                    skelToDraw.Joints[JointType.ShoulderCenter].Position.Z * -1); // Shoulder Center
                GL.Vertex3(skelToDraw.Joints[JointType.ShoulderLeft].Position.X, skelToDraw.Joints[JointType.ShoulderLeft].Position.Y,
                    skelToDraw.Joints[JointType.ShoulderLeft].Position.Z * -1); // Shoulder Left

                // Upper Spine
                if (skelToDraw.Joints[JointType.Spine].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.ShoulderCenter].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.ShoulderCenter].Position.X, skelToDraw.Joints[JointType.ShoulderCenter].Position.Y,
                    skelToDraw.Joints[JointType.ShoulderCenter].Position.Z * -1); // Shoulder Center
                GL.Vertex3(skelToDraw.Joints[JointType.Spine].Position.X, skelToDraw.Joints[JointType.Spine].Position.Y,
                    skelToDraw.Joints[JointType.Spine].Position.Z * -1); // Spine

                // Lower Spine
                if (skelToDraw.Joints[JointType.Spine].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.HipCenter].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.Spine].Position.X, skelToDraw.Joints[JointType.Spine].Position.Y,
                    skelToDraw.Joints[JointType.Spine].Position.Z * -1); // Spine
                GL.Vertex3(skelToDraw.Joints[JointType.HipCenter].Position.X, skelToDraw.Joints[JointType.HipCenter].Position.Y,
                    skelToDraw.Joints[JointType.HipCenter].Position.Z * -1); // Hip Center

                // Right Hip
                if (skelToDraw.Joints[JointType.HipRight].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.HipCenter].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.HipCenter].Position.X, skelToDraw.Joints[JointType.HipCenter].Position.Y,
                    skelToDraw.Joints[JointType.HipCenter].Position.Z * -1); // Hip Center
                GL.Vertex3(skelToDraw.Joints[JointType.HipRight].Position.X, skelToDraw.Joints[JointType.HipRight].Position.Y,
                    skelToDraw.Joints[JointType.HipRight].Position.Z * -1); // Hip Right

                // Left Hip
                if (skelToDraw.Joints[JointType.HipLeft].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.HipCenter].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.HipCenter].Position.X, skelToDraw.Joints[JointType.HipCenter].Position.Y,
                    skelToDraw.Joints[JointType.HipCenter].Position.Z * -1); // Hip Center
                GL.Vertex3(skelToDraw.Joints[JointType.HipLeft].Position.X, skelToDraw.Joints[JointType.HipLeft].Position.Y,
                    skelToDraw.Joints[JointType.HipLeft].Position.Z * -1); // Hip Left

                // Right Upper Arm
                if (skelToDraw.Joints[JointType.ShoulderRight].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.ElbowRight].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.ShoulderRight].Position.X, skelToDraw.Joints[JointType.ShoulderRight].Position.Y,
                    skelToDraw.Joints[JointType.ShoulderRight].Position.Z * -1); // Shoulder Right
                GL.Vertex3(skelToDraw.Joints[JointType.ElbowRight].Position.X, skelToDraw.Joints[JointType.ElbowRight].Position.Y,
                    skelToDraw.Joints[JointType.ElbowRight].Position.Z * -1); // Elbow Right

                // Left Upper Arm
                if (skelToDraw.Joints[JointType.ShoulderLeft].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.ElbowLeft].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.ShoulderLeft].Position.X, skelToDraw.Joints[JointType.ShoulderLeft].Position.Y,
                    skelToDraw.Joints[JointType.ShoulderLeft].Position.Z * -1); // Shoulder Left
                GL.Vertex3(skelToDraw.Joints[JointType.ElbowLeft].Position.X, skelToDraw.Joints[JointType.ElbowLeft].Position.Y,
                    skelToDraw.Joints[JointType.ElbowLeft].Position.Z * -1); // Elbow Left

                // Right Lower Arm
                if (skelToDraw.Joints[JointType.WristRight].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.ElbowRight].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.ElbowRight].Position.X, skelToDraw.Joints[JointType.ElbowRight].Position.Y,
                    skelToDraw.Joints[JointType.ElbowRight].Position.Z * -1); // Elbow Right
                GL.Vertex3(skelToDraw.Joints[JointType.WristRight].Position.X, skelToDraw.Joints[JointType.WristRight].Position.Y,
                    skelToDraw.Joints[JointType.WristRight].Position.Z * -1); // Wrist Right

                // Left Lower Arm
                if (skelToDraw.Joints[JointType.WristLeft].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.ElbowLeft].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.ElbowLeft].Position.X, skelToDraw.Joints[JointType.ElbowLeft].Position.Y,
                    skelToDraw.Joints[JointType.ElbowLeft].Position.Z * -1); // Elbow Left
                GL.Vertex3(skelToDraw.Joints[JointType.WristLeft].Position.X, skelToDraw.Joints[JointType.WristLeft].Position.Y,
                    skelToDraw.Joints[JointType.WristLeft].Position.Z * -1); // Wrist Left

                // Right Hand
                if (skelToDraw.Joints[JointType.WristRight].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.WristRight].Position.X, skelToDraw.Joints[JointType.WristRight].Position.Y,
                    skelToDraw.Joints[JointType.WristRight].Position.Z * -1); // Wrist Right
                GL.Vertex3(skelToDraw.Joints[JointType.HandRight].Position.X, skelToDraw.Joints[JointType.HandRight].Position.Y,
                    skelToDraw.Joints[JointType.HandRight].Position.Z * -1); // Hand Right

                // Left Hand
                if (skelToDraw.Joints[JointType.WristLeft].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.WristLeft].Position.X, skelToDraw.Joints[JointType.WristLeft].Position.Y,
                    skelToDraw.Joints[JointType.WristLeft].Position.Z * -1); // Wrist Left
                GL.Vertex3(skelToDraw.Joints[JointType.HandLeft].Position.X, skelToDraw.Joints[JointType.HandLeft].Position.Y,
                    skelToDraw.Joints[JointType.HandLeft].Position.Z * -1); // Hand Left

                // Right Upper Leg
                if (skelToDraw.Joints[JointType.HipRight].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.KneeRight].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.HipRight].Position.X, skelToDraw.Joints[JointType.HipRight].Position.Y,
                    skelToDraw.Joints[JointType.HipRight].Position.Z * -1); // Hip Right
                GL.Vertex3(skelToDraw.Joints[JointType.KneeRight].Position.X, skelToDraw.Joints[JointType.KneeRight].Position.Y,
                    skelToDraw.Joints[JointType.KneeRight].Position.Z * -1); // Knee Right

                // Right Lower Leg
                if (skelToDraw.Joints[JointType.AnkleRight].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.KneeRight].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.KneeRight].Position.X, skelToDraw.Joints[JointType.KneeRight].Position.Y,
                    skelToDraw.Joints[JointType.KneeRight].Position.Z * -1); // Knee Right
                GL.Vertex3(skelToDraw.Joints[JointType.AnkleRight].Position.X, skelToDraw.Joints[JointType.AnkleRight].Position.Y,
                    skelToDraw.Joints[JointType.AnkleRight].Position.Z * -1); // Ankle Right

                // Right Foot
                if (skelToDraw.Joints[JointType.AnkleRight].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.FootRight].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.AnkleRight].Position.X, skelToDraw.Joints[JointType.AnkleRight].Position.Y,
                    skelToDraw.Joints[JointType.AnkleRight].Position.Z * -1); // Ankle Right
                GL.Vertex3(skelToDraw.Joints[JointType.FootRight].Position.X, skelToDraw.Joints[JointType.FootRight].Position.Y,
                    skelToDraw.Joints[JointType.FootRight].Position.Z * -1); // Foot Right

                // Left Upper Leg
                if (skelToDraw.Joints[JointType.HipLeft].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.KneeLeft].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.HipLeft].Position.X, skelToDraw.Joints[JointType.HipLeft].Position.Y,
                    skelToDraw.Joints[JointType.HipLeft].Position.Z * -1); // Hip Left
                GL.Vertex3(skelToDraw.Joints[JointType.KneeLeft].Position.X, skelToDraw.Joints[JointType.KneeLeft].Position.Y,
                    skelToDraw.Joints[JointType.KneeLeft].Position.Z * -1); // Knee Left

                // Left Lower Leg
                if (skelToDraw.Joints[JointType.AnkleLeft].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.KneeLeft].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.KneeLeft].Position.X, skelToDraw.Joints[JointType.KneeLeft].Position.Y,
                    skelToDraw.Joints[JointType.KneeLeft].Position.Z * -1); // Knee Left
                GL.Vertex3(skelToDraw.Joints[JointType.AnkleLeft].Position.X, skelToDraw.Joints[JointType.AnkleLeft].Position.Y,
                    skelToDraw.Joints[JointType.AnkleLeft].Position.Z * -1); // Ankle Left

                // Left Foot
                if (skelToDraw.Joints[JointType.AnkleLeft].TrackingState == JointTrackingState.Tracked &
                    skelToDraw.Joints[JointType.FootLeft].TrackingState == JointTrackingState.Tracked)
                {
                    GL.Color3(System.Drawing.Color.CornflowerBlue);
                }
                else
                {
                    GL.Color3(System.Drawing.Color.White);
                }
                GL.Vertex3(skelToDraw.Joints[JointType.AnkleLeft].Position.X, skelToDraw.Joints[JointType.AnkleLeft].Position.Y,
                    skelToDraw.Joints[JointType.AnkleLeft].Position.Z * -1); // Ankle Left
                GL.Vertex3(skelToDraw.Joints[JointType.FootLeft].Position.X, skelToDraw.Joints[JointType.FootLeft].Position.Y,
                    skelToDraw.Joints[JointType.FootLeft].Position.Z * -1); // Foot Left

                //GL.Vertex3(0.0f, 0.0f, 1.0f);              // Top Left
                //GL.Vertex3(-1.0f, 2.0f, 3.0f);              // Top Right
                GL.End();
            }
            else
            {
                GL.ClearColor(System.Drawing.Color.Black);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.LoadIdentity();
            }
            control.SwapBuffers();
        }

        void control_Load(object sender, EventArgs e)
        {
            loaded = true;
            GL.ClearColor(System.Drawing.Color.Black);
            SetupViewport();
        }

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
                di.Attributes = FileAttributes.Directory;
            }
            String allVarsPath = path + "AllVars.csv";
            allVarsCSV = new FileWriter(allVarsPath);
            SettingsPropertyValueCollection spvc = Properties.Settings.Default.PropertyValues;
            SettingsPropertyCollection spc = Properties.Settings.Default.Properties;
            if (Properties.Settings.Default.NumberOfUses == 0)
            {
                // Set the variables
                Properties.Settings.Default["NumberOfUses"] = 1;
                Properties.Settings.Default["WeekNumber"] = 0;
                Properties.Settings.Default["DaysPerWeek"] = 0;
                Properties.Settings.Default["LevelNumber"] = 1;
                Properties.Settings.Default["RepeatBadgeCount"] = 0;
                Properties.Settings.Default["TimeBadgeCount"] = 0;
                Properties.Settings.Default["WellBadgeCount"] = 0;
                Properties.Settings.Default["TimeExercised"] = 0L;
                Properties.Settings.Default["LastDate"] = DateTime.Now.AddDays(-1).Date;
                Properties.Settings.Default["LastDayWell"] = DateTime.Now.AddDays(-1).Date;
                Properties.Settings.Default["FirstDayUsed"] = DateTime.Now.Date;
                Properties.Settings.Default.Save();

                foreach (SettingsProperty prop in spc)
                {
                    MainWindow.allVarsCSV.WriteData(Properties.Settings.Default.NumberOfUses.ToString() + "," + prop.Name + "," +
                        Properties.Settings.Default[prop.Name].ToString());
                }
            }

            foreach (SettingsProperty prop in spc)
            {
                MainWindow.allVarsCSV.WriteData(Properties.Settings.Default.NumberOfUses.ToString() + "," + prop.Name + "," +
                    Properties.Settings.Default[prop.Name].ToString());
            }

            String logPath = path + DateTime.Now.ToFileTimeUtc().ToString() + ".txt";

            if (DEMO) learningMode = false;
            
            // calc days in treatment
            int weekNum = -1;
            double usageLength = DateTime.Now.Date.Subtract(Properties.Settings.Default.FirstDayUsed).TotalDays;
            weekNum = (int)Math.Floor(usageLength / 7);
            if (weekNum != -1) 
            {
                if (Properties.Settings.Default.WeekNumber != weekNum)
                {
                    Properties.Settings.Default["WeekNumber"] = weekNum;
                    Properties.Settings.Default["DaysPerWeek"] = 0;
                }
            }

            Properties.Settings.Default["NumberOfUses"] = Properties.Settings.Default.NumberOfUses + 1;
            Properties.Settings.Default.Save();
            log = new FileWriter(logPath);
            allVarsCSV = new FileWriter(allVarsPath);
            log.WriteData("Game loading at: " + DateTime.Now.ToString() + "\n");
            level = Properties.Settings.Default.LevelNumber;

            InitializeComponent();

            // Load music to play on loop
            background = new WMPLib.WindowsMediaPlayer();
            background.settings.setMode("loop", true);
            background.settings.volume = volumeLoud;
            MainWindow.background.URL = MainWindow.pwd + "\\Resources\\music_level_" + MainWindow.level + ".mp3";
            
            timeOffScreen.Start();
            control.Width = (int)host.Width;
            control.Height = (int)host.Height;
            control.Load += control_Load;
            control.Paint += control_Paint;
        }

        void levelMusic_MediaEnded(object sender, EventArgs e)
        {
            MediaPlayer player = sender as MediaPlayer;
            if (player == null)
                return;

            player.Position = new TimeSpan(0);
            player.Play();
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //this.Hide();
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                TransformSmoothParameters smoothingParam = new TransformSmoothParameters();
                {
                    smoothingParam.Smoothing = 0.7f;
                    smoothingParam.Correction = 0.3f;
                    smoothingParam.Prediction = 1.0f;
                    smoothingParam.JitterRadius = 1.0f;
                    smoothingParam.MaxDeviationRadius = 1.0f;
                };

                this.sensor.SkeletonStream.Enable(smoothingParam);
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                this.speechRecognizer = this.CreateSpeechRecognizer();

                if (this.speechRecognizer != null)
                {
                    // NOTE: Need to wait 4 seconds for device to be ready to stream audio right after initialization
                    this.readyTimer = new DispatcherTimer();
                    this.readyTimer.Tick += this.ReadyTimerTick;
                    this.readyTimer.Interval = new TimeSpan(0, 0, 4);
                    this.readyTimer.Start();

                    this.Closing += this.WindowClosing;
                }

                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                //System.Windows.MessageBox.Show(pwd);
                playSoundResourceSync("kinect_not_plugged_in");
                Environment.Exit(-1);
            }
            host.Child = control;            
        }

        private void gameNavigation()
        {
            playSoundResourceSync("game_choice");
            background.settings.volume = volumeQuiet;
            this.speechRecognizer.Grammars[this.speechRecognizer.Grammars.IndexOf(inPainGrammar)].Enabled = false;
            this.speechRecognizer.Grammars[this.speechRecognizer.Grammars.IndexOf(gameControlsGrammar)].Enabled = false;
            this.speechRecognizer.Grammars[this.speechRecognizer.Grammars.IndexOf(gameNavigationGrammar)].Enabled = true;
            this.speechRecognizer.Grammars[this.speechRecognizer.Grammars.IndexOf(workoutNumberGrammar)].Enabled = false;
            RecognitionResult result = this.speechRecognizer.Recognize();
            while (result == null || result.Confidence < 0.8)
            {
                result = this.speechRecognizer.Recognize();
            }
            switch (result.Text.ToUpperInvariant())
            {
                case "TROPHY":
                    if (!DEMO)
                    {
                        log.WriteData("trophy " + DateTime.Now.ToString() + "\n");
                        background.settings.volume = volumeLoud;
                        announceTrophies();
                    }
                    break;

                case "LEARNING":
                    log.WriteData("learning " + DateTime.Now.ToString() + "\n");
                    learningMode = true;
                    background.settings.volume = volumeLoud;
                    playSoundResourceSync("beginning_learning");
                    playSoundResourceSync("learning_stand");
                    othread = new Thread(startGame);
                    othread.Start();
                    break;

                case "EXERCISE":
                    log.WriteData("exercise " + DateTime.Now.ToString() + "\n");
                    learningMode = false;
                    background.settings.volume = volumeLoud;
                    playSoundResourceSync("beginning_exercise");
                    playSoundResourceSync("yoga_mat_instructions");
                    orient = true;
                    break;

                default: break;
            }
        }

        private void checkAnkles()
        {
            double minAnkleX = -0.75, maxAnkleX = 0.75, minAnkleZ = 7.7, maxAnkleZ = 10.7;
            double anklesX = (savedSkeleton.Joints[JointType.AnkleLeft].Position.X +
                savedSkeleton.Joints[JointType.AnkleRight].Position.X) / 2;
            double anklesZ = (savedSkeleton.Joints[JointType.AnkleLeft].Position.Z +
                    savedSkeleton.Joints[JointType.AnkleRight].Position.Z) / 2;
            double shoulderSymm = savedSkeleton.Joints[JointType.ShoulderRight].Position.Z - savedSkeleton.Joints[JointType.ShoulderLeft].Position.Z;
            int distance = 0;

            if (shoulderSymm > .1) {
                playSoundResourceSync("twist_left");
                orient = true;
            }
            else if (shoulderSymm < -.1)
            {
                playSoundResourceSync("twist_right");
                orient = true;
            }
            else if (anklesX * 3.28084 < minAnkleX)
            {
                distance = (int)Math.Floor(minAnkleX - (anklesX * 3.28084));
                if (distance < 1) playSoundResourceSync("take_step_right");
                else if (distance == 1) playSoundResourceSync("move_one_right");
                else if (distance == 2) playSoundResourceSync("move_two_right");
                else if (distance == 3) playSoundResourceSync("move_three_right");
                else playSoundResourceSync("move_more_than_three_right");
                orient = true;
            }
            else if (anklesX * 3.28084 > maxAnkleX)
            {
                distance = (int)Math.Floor((anklesX * 3.28084) - maxAnkleX);
                if (distance < 1) playSoundResourceSync("take_step_left");
                else if (distance == 1) playSoundResourceSync("move_one_left");
                else if (distance == 2) playSoundResourceSync("move_two_left");
                else if (distance == 3) playSoundResourceSync("move_three_left");
                else playSoundResourceSync("move_more_than_three_left");
                orient = true;
            }
            else if (anklesZ * 3.28084 < minAnkleZ)
            {
                distance = (int)Math.Floor(minAnkleZ - (anklesZ * 3.28084));
                if (distance < 1) playSoundResourceSync("take_step_backward");
                else if (distance == 1) playSoundResourceSync("move_one_backward");
                else if (distance == 2) playSoundResourceSync("move_two_backward");
                else if (distance == 3) playSoundResourceSync("move_three_backward");
                else playSoundResourceSync("move_more_than_three_backward");
                orient = true;
            }
            else if (anklesZ * 3.28084 > maxAnkleZ)
            {
                distance = (int)Math.Floor((anklesZ * 3.28084) - maxAnkleZ);
                if (distance < 1) playSoundResourceSync("take_step_forward");
                else if (distance == 1) playSoundResourceSync("move_one_forward");
                else if (distance == 2) playSoundResourceSync("move_two_forward");
                else if (distance == 3) playSoundResourceSync("move_three_forward");
                else playSoundResourceSync("move_more_than_three_forward");
                orient = true;
            }
            else
            {
                orient = false;
                playSoundResourceSync("good_position");
                othread = new Thread(startGame);
                othread.Start();
            }
        }

        private void startGame()
        {
            if (DEMO)
            {
                playSoundResourceSync("starting_the_demo");
                exercise = new Exercise45(log, learningMode);
                start = DateTime.Now;
                othread = new Thread(startWorkout);
                othread.Start();
            }
            else
            {
                playSoundResourceSync("workout_choice");
                playSoundResourceSync("say_workout_length");
                playSoundResourceSync("second_note");
                background.settings.volume = volumeQuiet;
                this.speechRecognizer.Grammars[this.speechRecognizer.Grammars.IndexOf(inPainGrammar)].Enabled = false;
                this.speechRecognizer.Grammars[this.speechRecognizer.Grammars.IndexOf(gameControlsGrammar)].Enabled = false;
                this.speechRecognizer.Grammars[this.speechRecognizer.Grammars.IndexOf(gameNavigationGrammar)].Enabled = false;
                this.speechRecognizer.Grammars[this.speechRecognizer.Grammars.IndexOf(workoutNumberGrammar)].Enabled = true;
                RecognitionResult result = this.speechRecognizer.Recognize();
                while (result == null || result.Confidence < 0.8)
                {
                    result = this.speechRecognizer.Recognize();
                }
                switch (result.Text.ToUpperInvariant())
                {
                    case "ONE":
                        background.settings.volume = volumeLoud;
                        log.WriteData("20 minute workout " + DateTime.Now.ToString() + "\n");
                        playSoundResourceSync("starting_your_25_minute_workout");
                        exercise = new Exercise20(log, learningMode);
                        start = DateTime.Now;
                        othread = new Thread(startWorkout);
                        othread.Start();
                        break;
                    case "TWO":
                        background.settings.volume = volumeLoud;
                        playSoundResourceSync("starting_your_37_minute_workout");
                        exercise = new Exercise30(log, learningMode);
                        start = DateTime.Now;
                        othread = new Thread(startWorkout);
                        othread.Start();
                        break;
                    case "THREE":
                        background.settings.volume = volumeLoud;
                        playSoundResourceSync("starting_your_65_minute_workout");
                        exercise = new Exercise45(log, learningMode);
                        start = DateTime.Now;
                        othread = new Thread(startWorkout);
                        othread.Start();
                        break;
                    case "FOUR":
                        background.settings.volume = volumeLoud;
                        playSoundResourceSync("starting_your_77_minute_workout");
                        exercise = new Exercise60(log, learningMode);
                        start = DateTime.Now;
                        othread = new Thread(startWorkout);
                        othread.Start();
                        break;

                    default: break;
                }
            }
        }

        public static void stopSoundPlayer()
        {
            player.Stop();
        }

        public static void playSoundResourceSync(String clip)
        {
            while (MainWindow.toPause) { };
            player = new SoundPlayer(pwd + "\\Resources\\" + clip + ".wav");
            while (MainWindow.toPause) { };
            player.PlaySync();
            while (MainWindow.toPause) { };
        }

        public static void playSoundResources(String[] filenames, EventHandler callback)
        {
            while (MainWindow.toPause) { };
            canPlay = false;
            while (MainWindow.toPause) { };
            PlayAsync(pwd, filenames, callback);
            while (MainWindow.toPause) { };
        }

        static void PlayAsync(String wd, String[] filenames, EventHandler callback)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                for (int i = 0; i < filenames.Length; i++) {
                    using (SoundPlayer player = new SoundPlayer(wd + "\\Resources\\" + filenames[i]))
                    {
                        player.PlaySync();
                    }
                }
                if (callback != null) callback(null, EventArgs.Empty);
            });
        }

        private void ReadyTimerTick(object sender, EventArgs e)
        {
            var t = new Thread(Start);
            t.Start();
            this.readyTimer.Stop();
            this.readyTimer = null;
            if (DEMO)
            {
                playSoundResourceSync("yoga_mat_instructions");
                orient = true;
            }
            else
            {
                playSoundResourceSync("welcome_to_efy");
                gameNavigation();
            }
        }

        private void Start()
        {
            kinectSource = this.sensor.AudioSource;
            kinectSource.BeamAngleMode = BeamAngleMode.Adaptive;
            var kinectStream = kinectSource.Start();
            this.speechRecognizer.SetInputToAudioStream(
                kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            //this.speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.speechRecognizer != null && this.sensor != null)
            {
                this.sensor.AudioSource.Stop();
                this.sensor.Stop();
                this.speechRecognizer.RecognizeAsyncCancel();
                this.speechRecognizer.RecognizeAsyncStop();
            }

            if (this.readyTimer != null)
            {
                this.readyTimer.Stop();
                this.readyTimer = null;
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
                else
                {
                    timeOffScreen.Start();
                }
            }

            if (skeletons.Length != 0)
            {
                timeOffScreen.Stop();
                // pause stop watch
                foreach (Skeleton skel in skeletons)
                {   
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        savedSkeleton = skel;
                        this.skelToDraw = skel;
                        control.Invalidate();
                        if (1 == Interlocked.CompareExchange(ref allowed, 0, 1))
                        {
                            if (!Exercise.timeIsUp)
                            {
                                String output = exercise.getPose().compareSkeleton(savedSkeleton);
                                Thread checkOut = new Thread(() => Exercise.checkOutput(output, exercise.getPose()));
                                checkOut.Start();
                            }
                        }
                        if (orient)
                        {
                            Thread checkAnkle = new Thread(() => checkAnkles());
                            checkAnkle.Start();
                        }
                        orient = false;
                    }
                    else
                    {
                        control.Invalidate();
                    }
                }
            }
            else
            {
                this.skelToDraw = null;
                control.Invalidate();
                // continue stopwatch
                timeOffScreen.Start();
            }
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        private SpeechRecognitionEngine CreateSpeechRecognizer()
        {
            RecognizerInfo ri = GetKinectRecognizer();
            if (ri == null)
            {
                System.Windows.MessageBox.Show(
                    @"There was a problem initializing Speech Recognition. Ensure you have the Microsoft Speech SDK installed.",
                    "Failed to load Speech SDK",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                this.Close();
                return null;
            }

            SpeechRecognitionEngine sre;
            try
            {
                sre = new SpeechRecognitionEngine(ri.Id);
            }
            catch
            {
                System.Windows.MessageBox.Show(
                    @"There was a problem initializing Speech Recognition. Ensure you have the Microsoft Speech SDK installed 
                    and configured.",
                    "Failed to load Speech SDK",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                this.Close();
                return null;
            }

            var inPain = new Choices();
            inPain.Add("yes");
            inPain.Add("no");

            var gameControls = new Choices();
            gameControls.Add("exit the game");
            gameControls.Add("pause the game");
            gameControls.Add("skip the pose");
            gameControls.Add("repeat line");
            gameControls.Add("play");

            var gameNavigations = new Choices();
            gameNavigations.Add("trophy");
            gameNavigations.Add("learning");
            gameNavigations.Add("exercise");

            var workoutNumber = new Choices();
            workoutNumber.Add("one");
            workoutNumber.Add("two");
            workoutNumber.Add("three");
            workoutNumber.Add("four");
            
            var inPainBuilder = new GrammarBuilder { Culture = ri.Culture };
            inPainBuilder.Append(inPain);

            var gameControlsBuilder = new GrammarBuilder { Culture = ri.Culture };
            gameControlsBuilder.Append(gameControls);

            var gameNavigationBuilder = new GrammarBuilder { Culture = ri.Culture };
            gameNavigationBuilder.Append(gameNavigations);

            var workoutNumberBuilder = new GrammarBuilder { Culture = ri.Culture };
            workoutNumberBuilder.Append(workoutNumber);

            // Create the actual Grammar instance, and then load it into the speech recognizer.
            inPainGrammar = new Grammar(inPainBuilder);
            gameControlsGrammar = new Grammar(gameControlsBuilder);
            gameNavigationGrammar = new Grammar(gameNavigationBuilder);
            workoutNumberGrammar = new Grammar(workoutNumberBuilder);

            sre.LoadGrammar(inPainGrammar);
            sre.LoadGrammar(gameControlsGrammar);
            sre.LoadGrammar(gameNavigationGrammar);
            sre.LoadGrammar(workoutNumberGrammar);
            return sre;
        }

        private void announceTrophies()
        {

            if (Properties.Settings.Default.TimeBadgeCount > 0)
            {
                // Endurance badge
                int numTime10 = (int)Math.Floor((double)Properties.Settings.Default.TimeBadgeCount / 10);
                int numTime5 = (int)Math.Floor((double)(Properties.Settings.Default.TimeBadgeCount % 10) / 5);
                int numTime1 = (int)Math.Floor((double)Properties.Settings.Default.TimeBadgeCount % 5);
                playSoundResourceSync("you_have_received");
                synth.Speak(Properties.Settings.Default.TimeBadgeCount.ToString() + "");
                playSoundResourceSync("endurance_badges");
                for (int i = 0; i < numTime10; i++)
                {
                    playSoundResourceSync("time_10");
                }
                for (int i = 0; i < numTime5; i++)
                {
                    playSoundResourceSync("time_5");
                }
                for (int i = 0; i < numTime1; i++)
                {
                    playSoundResourceSync("time_1");
                }
            }

            // Consistency badge
            if (Properties.Settings.Default.RepeatBadgeCount > 0)
            {
                int numRepeat10 = (int)Math.Floor((double)Properties.Settings.Default.RepeatBadgeCount / 10);
                int numRepeat5 = (int)Math.Floor((double)(Properties.Settings.Default.RepeatBadgeCount % 10) / 5);
                int numRepeat1 = (int)Math.Floor((double)Properties.Settings.Default.RepeatBadgeCount % 5);

                playSoundResourceSync("you_have_received");
                synth.Speak(Properties.Settings.Default.RepeatBadgeCount.ToString() + "");
                playSoundResourceSync("consistency_badges");
                for (int i = 0; i < numRepeat10; i++)
                {
                    playSoundResourceSync("repeat_10");
                }
                for (int i = 0; i < numRepeat5; i++)
                {
                    playSoundResourceSync("repeat_5");
                }
                for (int i = 0; i < numRepeat1; i++)
                {
                    playSoundResourceSync("repeat_1");
                }
            }

            // Performance badge
            if (Properties.Settings.Default.WellBadgeCount > 0)
            {
                int numWell10 = (int)Math.Floor((double)Properties.Settings.Default.WellBadgeCount / 10);
                int numWell5 = (int)Math.Floor((double)(Properties.Settings.Default.WellBadgeCount % 10) / 5);
                int numWell1 = (int)Math.Floor((double)Properties.Settings.Default.WellBadgeCount % 5);

                playSoundResourceSync("you_have_received");
                synth.Speak(Properties.Settings.Default.WellBadgeCount.ToString() + "");
                playSoundResourceSync("performance_badges");
                for (int i = 0; i < numWell10; i++)
                {
                    playSoundResourceSync("well_10");
                }
                for (int i = 0; i < numWell5; i++)
                {
                    playSoundResourceSync("well_5");
                }
                for (int i = 0; i < numWell1; i++)
                {
                    playSoundResourceSync("well_1");
                }
            }

            if(Properties.Settings.Default.TimeBadgeCount == 0 && Properties.Settings.Default.RepeatBadgeCount == 0 &&
                Properties.Settings.Default.WellBadgeCount == 0)
            {
                playSoundResourceSync("no_badges");
            }
            System.Threading.Thread.Sleep(3000);
            gameNavigation();
        }

        public static void congratulateEndurance()
        {
            int newCount = Properties.Settings.Default.TimeBadgeCount;
            if (newCount == 0)
            {
                playSoundResourceSync("congrats_first_endurance");
                playSoundResourceSync("time_1");
            }
            else if (newCount % 10 == 0)
            {
                playSoundResourceSync("congrats_now_have");
                synth.Speak(newCount.ToString() + "");
                playSoundResourceSync("endurance_badges");
                playSoundResourceSync("time_10");
            }
            else if (newCount % 5 == 0)
            {
                playSoundResourceSync("congrats_now_have");
                synth.Speak(newCount.ToString() + "");
                playSoundResourceSync("endurance_badges");
                playSoundResourceSync("time_5");
            }
            else
            {
                playSoundResourceSync("congrats_another_endurance");
                playSoundResourceSync("time_1");
            }
        }

        private static void congratulatePerformance()
        {
            int newCount = Properties.Settings.Default.WellBadgeCount + 1;
            if (newCount == 0)
            {
                playSoundResourceSync("congrats_first_performance");
                playSoundResourceSync("well_1");
            }
            else if (newCount % 10 == 0)
            {
                playSoundResourceSync("congrats_now_have");
                synth.Speak(newCount.ToString());
                playSoundResourceSync("performance_badges");
                playSoundResourceSync("well_10");
            }
            else if (newCount % 5 == 0)
            {
                playSoundResourceSync("congrats_now_have");
                synth.Speak(newCount.ToString() + "");
                playSoundResourceSync("performance_badges");
                playSoundResourceSync("well_5");
            }
            else
            {
                playSoundResourceSync("congrats_another_performance");
                playSoundResourceSync("well_1");
            }
            Properties.Settings.Default["WellBadgeCount"] = Properties.Settings.Default.WellBadgeCount + 1;
            Properties.Settings.Default.Save();
        }

        private static void congratulateConsistency()
        {
            int newCount = Properties.Settings.Default.RepeatBadgeCount;
            if (newCount == 0)
            {
                playSoundResourceSync("congrats_first_consistency");
                playSoundResourceSync("repeat_1");
            }
            else if (newCount % 10 == 0)
            {
                playSoundResourceSync("congrats_now_have");
                synth.Speak(newCount.ToString());
                playSoundResourceSync("consistency_badges");
                playSoundResourceSync("repeat_10");
            }
            else if (newCount % 5 == 0)
            {
                playSoundResourceSync("congrats_now_have");
                synth.Speak(newCount.ToString() + "");
                playSoundResourceSync("consistency_badges");
                playSoundResourceSync("repeat_5");
            }
            else
            {
                playSoundResourceSync("congrats_another_consistency");
                playSoundResourceSync("repeat_1");
            }
        }

        private static void congratulateLevel()
        {
            playSoundResourceSync("congrats_on_level");
            switch (Properties.Settings.Default.LevelNumber)
            {
                case 1: playSoundResourceSync("1"); break;
                case 2: playSoundResourceSync("2"); break;
                case 3: playSoundResourceSync("3"); break;
                case 4: playSoundResourceSync("4"); break;
                case 5: playSoundResourceSync("5"); break;
                case 6: playSoundResourceSync("6"); break;
                case 7: playSoundResourceSync("7"); break;
                case 8: playSoundResourceSync("8"); break;
                default: playSoundResourceSync("8"); break;
            }
            playSoundResourceSync("level_up");
        }

        public static void logProgress(long time)
        {
            if (!learningMode)
            {
                Properties.Settings.Default["TimeExercised"] = Properties.Settings.Default.TimeExercised + time;
                Properties.Settings.Default.Save();
            }

            SettingsPropertyValueCollection spvc = Properties.Settings.Default.PropertyValues;
            SettingsPropertyCollection spc = Properties.Settings.Default.Properties;
            foreach (SettingsProperty prop in spc)
            {
                MainWindow.allVarsCSV.WriteData(Properties.Settings.Default.NumberOfUses.ToString() + "," + prop.Name + "," +
                    Properties.Settings.Default[prop.Name].ToString());
            }
        }

        public static void uponExit(bool getWellBadge) {
            System.Threading.Thread.Sleep(6000);
            
            MainWindow.log.WriteData("exit " + DateTime.Now.ToString());
            long honestTimeDuration = (long)DateTime.Now.Subtract(MainWindow.start).TotalMinutes - (long)(MainWindow.timeoffMS / (60*1000)) -
                    (MainWindow.timeOffScreen.ElapsedMilliseconds / (60*1000));
            
            if (!DEMO && !learningMode)
            {
                // time badge achievement?
                if (DateTime.Now.Date.CompareTo(Properties.Settings.Default.LastDate) != 0)
                {
                    if ((honestTimeDuration >= 20 && exercise is Exercise20) ||
                        (honestTimeDuration >= 30 && exercise is Exercise30) ||
                        (honestTimeDuration >= 45 && exercise is Exercise45) ||
                        (honestTimeDuration >= 60 && exercise is Exercise60))
                    {
                        Properties.Settings.Default["LastDate"] = DateTime.Now.Date;
                        Properties.Settings.Default.Save();
                        MainWindow.congratulateEndurance();
                    }
                }

                // well badge achievement?
                if (getWellBadge && (DateTime.Now.Date.CompareTo(Properties.Settings.Default.LastDayWell.Date) != 0))
                {
                    MainWindow.congratulatePerformance();
                }

                // repeat badge achievement?
                if (Properties.Settings.Default.DaysPerWeek == 3)
                {
                    MainWindow.congratulateConsistency();
                }

                // level up?
                if (MainWindow.level < 8)
                {
                    if (levelUpTimes[MainWindow.level] < Properties.Settings.Default.TimeExercised)
                    {
                        MainWindow.congratulateLevel();
                    }
                }
            }
            
            background.controls.stop();
            playSoundResourceSync("thank_you_for_playing");
            System.Threading.Thread.Sleep(2000);
            background.settings.volume = 0;
            Environment.Exit(0);
        }
  
        private void startWorkout()
        {
            if (exercise != null)
            {
                if (!DEMO && !learningMode)
                {
                    playSoundResourceSync("earn_performance_if");
                    if (exercise is Exercise20)
                    {
                        playSoundResourceSync("earn_endurance_if_20");
                    }
                    else if (exercise is Exercise30)
                    {
                        playSoundResourceSync("earn_endurance_if_30");
                    }
                    else if (exercise is Exercise45)
                    {
                        playSoundResourceSync("earn_endurance_if_45");
                    }
                    else if (exercise is Exercise60)
                    {
                        playSoundResourceSync("earn_endurance_if_60");
                    }
                    playSoundResourceSync("earn_consistency_if");

                    // calculate and say how long
                    double leftOver = levelUpTimes[Properties.Settings.Default.LevelNumber] - Properties.Settings.Default.TimeExercised;
                    int hour = (int)Math.Floor(leftOver / 60);
                    int minute = (int)Math.Floor(leftOver % 60);

                    if (hour > 0) {
                        // say how long until next level
                        playSoundResourceSync("you_have");

                        synth.Speak(hour.ToString() + "");
                        if (hour == 1)
                        {
                            playSoundResourceSync("hour");
                        }
                        else
                        {
                            playSoundResourceSync("hours");
                        }
                        if (minute > 0)
                        {
                            playSoundResourceSync("and");
                        }
                    }
                    
                    if (minute > 0)
                    {
                        if (hour == 0)
                        {
                            // say how long until next level
                            playSoundResourceSync("you_have");
                        }
                        synth.Speak(minute.ToString() + "");
                        if (minute == 1)
                        {
                            playSoundResourceSync("minute");
                        }
                        else
                        {
                            playSoundResourceSync("minutes");
                        }
                    }

                    // until next level
                    if (hour > 0 || minute > 0)
                    {
                        playSoundResourceSync("until_next_level");
                    }
                }
                if (!DEMO)
                {
                    stating = true;
                    playSoundResourceSync("to_exit");
                    playSoundResourceSync("to_pause");
                    playSoundResourceSync("to_skip");
                    stating = false;
                }
                this.speechRecognizer.Grammars[this.speechRecognizer.Grammars.IndexOf(inPainGrammar)].Enabled = true;
                this.speechRecognizer.Grammars[this.speechRecognizer.Grammars.IndexOf(gameControlsGrammar)].Enabled = true;
                this.speechRecognizer.Grammars[this.speechRecognizer.Grammars.IndexOf(gameNavigationGrammar)].Enabled = false;
                this.speechRecognizer.Grammars[this.speechRecognizer.Grammars.IndexOf(workoutNumberGrammar)].Enabled = false;
                this.speechRecognizer.SpeechRecognized += this.SreSpeechRecognized;
                this.speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                exercise.startWorkout();
            }
        }

        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence < 0.8)
            {
                return;
            }

            if (!stating)
            {
                switch (e.Result.Text.ToUpperInvariant())
                {
                    case "EXIT THE GAME":
                        if (canExit)
                        {
                            if (beganWorkout)
                            {
                                toExit = true;
                                exercise.killPose();
                            }
                        }
                        break;

                    case "PAUSE THE GAME":
                        if (beganWorkout)
                        {
                            canExit = false;
                            log.WriteData("pause " + DateTime.Now.ToString());
                            pauseStart = DateTime.Now;
                            toPause = true;
                        }
                        break;

                    case "REPEAT LINE":
                        if (beganWorkout)
                        {
                            log.WriteData("repeat " + DateTime.Now.ToString());
                            repeat = true;
                        }
                        break;

                    case "PLAY":
                        if (toPause)
                        {
                            pauseFinish = DateTime.Now;
                            timeoffMS += pauseFinish.Subtract(pauseStart).TotalMilliseconds;
                            log.WriteData("resume " + DateTime.Now.ToString());
                            canExit = true;
                            toPause = false;
                        }
                        break;

                    case "SKIP THE POSE":
                        if (beganWorkout)
                        {
                            exercise.killPose();
                        }
                        break;

                    default:
                        break;
                }
            }

            if (cansayAnswer)
            {
                switch (e.Result.Text.ToUpperInvariant())
                {
                    case "YES":
                        log.WriteData("yes " + DateTime.Now.ToString() + "\n");
                        yes = true;
                        answered = true;
                        break;

                    case "NO":
                        log.WriteData("no " + DateTime.Now.ToString() + "\n");
                        yes = false;
                        answered = true;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}