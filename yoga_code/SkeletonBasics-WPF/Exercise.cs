using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace Microsoft.Samples.Kinect.EyesFreeYoga
{
    class Exercise
    {
        static FileWriter log;
        static String oldRule = "";
        static bool doneCheck = true, allRight = false, incentivizeTime = false, incentivizeLevel = false, playTime = false, playLevel = false;
        static int annoyed = 0;
        static int fixCount = 0;
        public static System.Timers.Timer timeNotUp;
        public static bool timeIsUp = true;

        public Exercise(FileWriter log)
        {
            Exercise.log = log;
        }

        public int poseEnum(Pose pose)
        {
            if (pose is MountainPose) return 0;
            else if (pose is WarriorI) return 1;
            else if (pose is WarriorILeft) return 2;
            else if (pose is WarriorIIRight) return 3;
            else if (pose is WarriorII) return 4;
            else if (pose is ReverseWarrior) return 5;
            else if (pose is ReverseWarriorLeft) return 6;
            else if (pose is TreeRight) return 7;
            else if (pose is Tree) return 8;
            else if (pose is Chair) return 9;
            else if (pose is CatCow) return 10;
            else if (pose is Child) return 11;
            else if (pose is DownwardDog) return 12;
            else if (pose is DownwardDogFlow) return 13;
            else if (pose is StandingFold) return 14;
            else if (pose is StandingFoldFlow) return 15;
            else if (pose is LowerBackRelease) return 16;
            else if (pose is ThreadNeedle) return 17;
            else if (pose is Bridge) return 18;
            else if (pose is BridgeFlow) return 19;
            else if (pose is HappyBaby) return 20;
            else if (pose is BoundAngle) return 21;
            else if (pose is ReclinedTwist) return 22;
            else if (pose is Corpse) return 23;
            else if (pose is CorpseExit) return 24;
            else if (pose is Plank) return 25;
            else if (pose is Cobra) return 26;
            else return -1;
        }

        public void logPose(Pose pose, bool learning)
        {
            if (learning) MainWindow.poseCounts[poseEnum(pose), 0] += 1;
            else MainWindow.poseCounts[poseEnum(pose), 1] += 1;
        }

        public static void incentivize(double time, double honestTime, double goalTime)
        {
            MainWindow.logProgress((long)time);
            if (goalTime - honestTime < 5 && !incentivizeTime)
            {
                if (DateTime.Now.Date.CompareTo(Properties.Settings.Default.LastDate.Date) != 0)
                {
                    MainWindow.playSoundResourceSync("under_five_endurance");
                    incentivizeTime = true;
                    if (Properties.Settings.Default.DaysPerWeek == 2)
                    {
                        MainWindow.playSoundResourceSync("under_five_consistency");
                    }
                }
            }
            else if (goalTime < honestTime && !playTime)
            {
                playTime = true;
                if (DateTime.Now.Date.CompareTo(Properties.Settings.Default.LastDate.Date) != 0)
                {
                    MainWindow.playSoundResourceSync("time_1");
                    Properties.Settings.Default["TimeBadgeCount"] = Properties.Settings.Default.TimeBadgeCount + 1;
                    Properties.Settings.Default["DaysPerWeek"] = Properties.Settings.Default.DaysPerWeek + 1;
                    if (Properties.Settings.Default.DaysPerWeek == 3)
                    {
                        Properties.Settings.Default["RepeatBadgeCount"] = Properties.Settings.Default.RepeatBadgeCount + 1;
                        MainWindow.playSoundResourceSync("repeat_1");
                    }
                    Properties.Settings.Default.Save();
                }
            }

            if (MainWindow.level < 8)
            {
                if (MainWindow.levelUpTimes[MainWindow.level] - (time + Properties.Settings.Default.TimeExercised) < 5 && !incentivizeLevel)
                {
                    MainWindow.playSoundResourceSync("under_five_level");
                    incentivizeLevel = true;
                }
                else if (MainWindow.levelUpTimes[MainWindow.level] < (time + Properties.Settings.Default.TimeExercised) && !playLevel)
                {
                    MainWindow.playSoundResourceSync("level_up");
                    playLevel = true;
                    Properties.Settings.Default["LevelNumber"] = Properties.Settings.Default.LevelNumber + 1;
                    Properties.Settings.Default.Save();
                    MainWindow.level = MainWindow.level + 1;
                    MainWindow.background.URL = MainWindow.pwd + "\\Resources\\music_level_" + MainWindow.level + ".mp3";
                }
            }
        }

        public virtual void startWorkout() { }

        public virtual Thread advancePose() { return null; }

        public virtual void killPose() { }

        public virtual bool getWellBadge() { return false; }

        public virtual Pose getPose() { return null; }

        public void threadSleep(int length)
        {
            while (MainWindow.toPause) { };
            for (int i = 0; i < length; i += 1000)
            {
                while (MainWindow.toPause) { };
                Thread.Sleep(1000);
                while (MainWindow.toPause) { };
            }
        }

        //public bool yesNoNormal(String path)
        //{
        //    log.WriteData("yes no question " + DateTime.Now.ToString() + "\n");
        //    MainWindow.yes = false;
        //    while (MainWindow.toPause) { };
        //    MainWindow.playSoundResourceSync(path);
        //    while (MainWindow.toPause) { };
        //    MainWindow.cansayAnswer = true;
        //    MainWindow.background.settings.volume = MainWindow.volumeQuiet;
        //    while (!MainWindow.answered) { }
        //    MainWindow.cansayAnswer = false;
        //    MainWindow.background.settings.volume = MainWindow.volumeLoud;
        //    MainWindow.answered = false;
        //    return MainWindow.yes;
        //}

        public bool hold15(Pose pose)
        {
            MainWindow.playSoundResourceSync("pose_with_feedback");
            MainWindow.playSoundResourceSync("when_you_hear");
            MainWindow.playSoundResourceSync("second_note");
            MainWindow.playSoundResourceSync("followed_the_directions");
            doneCheck = false;
            timeIsUp = false;
            timeNotUp = new System.Timers.Timer(15000);
            timeNotUp.AutoReset = false;
            timeNotUp.Elapsed += timeNotUp_Elapsed;
            timeNotUp.Start();

            // thread safe version of allowed = true
            Interlocked.Exchange(ref MainWindow.allowed, 1); // allow for fixes to happen
            while (!doneCheck) { } // hang until done with being checked
            fixCount = 0;
            return allRight;
        }

        public bool hold60(Pose pose)
        {
            MainWindow.playSoundResourceSync("pose_with_feedback");
            MainWindow.playSoundResourceSync("when_you_hear");
            MainWindow.playSoundResourceSync("second_note");
            MainWindow.playSoundResourceSync("followed_the_directions");
            doneCheck = false;
            timeIsUp = false;
            timeNotUp = new System.Timers.Timer(60000);
            timeNotUp.AutoReset = false;
            timeNotUp.Elapsed += timeNotUp_Elapsed;
            timeNotUp.Enabled = true;
            
            
            Interlocked.Exchange(ref MainWindow.allowed, 1); // allow for fixes to happen
            while (!doneCheck) { } // hang until done with being checked
            fixCount = 0;
            return allRight;
        }

        void timeNotUp_Elapsed(object sender, ElapsedEventArgs e)
        {
            timeIsUp = true;
            timeNotUp.Enabled = false;
            doneCheck = true;
        }

        public void yesNoPain(Pose pose, String havePain, String yesPain, bool isKnee)
        {
            if (!MainWindow.learningMode)
            {
                log.WriteData("yes no pain " + DateTime.Now.ToString() + "\n");
                while (MainWindow.toPause) { };
                MainWindow.playSoundResourceSync(havePain);
                while (MainWindow.toPause) { };
                MainWindow.cansayAnswer = true;
                MainWindow.background.settings.volume = MainWindow.volumeQuiet;
                while (!MainWindow.answered) { }
                if (MainWindow.yes)
                {
                    MainWindow.background.settings.volume = MainWindow.volumeLoud;
                    readResource(yesPain);
                    if (isKnee)
                    {
                        if (pose is WarriorI)
                        {
                            log.WriteData("Knee pain with Warrior One: " + DateTime.Now.ToString() + "\n");
                            pose.forgetRule("warriorILeg");
                        }
                        else if (pose is WarriorILeft)
                        {
                            log.WriteData("Knee pain with Warrior One Left: " + DateTime.Now.ToString() + "\n");
                            pose.forgetRule("warriorILeftLeg");
                        }
                        else if (pose is WarriorII)
                        {
                            log.WriteData("Knee pain with Warrior Two: " + DateTime.Now.ToString() + "\n");
                            pose.forgetRule("warriorIILeg");
                        }
                        else if (pose is WarriorIIRight)
                        {
                            log.WriteData("Knee pain with Warrior Two Right: " + DateTime.Now.ToString() + "\n");
                            pose.forgetRule("warriorIIRightLeg");
                        }
                        else if (pose is ReverseWarrior)
                        {
                            log.WriteData("Knee pain with Reverse Warrior: " + DateTime.Now.ToString() + "\n");
                            pose.forgetRule("reverseWarriorLeg");
                        }
                        else if (pose is ReverseWarriorLeft)
                        {
                            log.WriteData("Knee pain with Reverse Warrior Left: " + DateTime.Now.ToString() + "\n");
                            pose.forgetRule("reverseWarriorLeftLeg");
                        }
                        else if (pose is Chair)
                        {
                            log.WriteData("Knee pain with Chair: " + DateTime.Now.ToString() + "\n");
                            pose.forgetRule("legsBent");
                        }
                    }
                    else
                    {
                        if (pose is WarriorI)
                        {
                            log.WriteData("Back pain with Warrior One: " + DateTime.Now.ToString() + "\n");
                            pose.forgetRule("armsRaised");
                            pose.forgetRule("armsStraight");
                        }
                        else if (pose is WarriorILeft)
                        {
                            log.WriteData("Back pain with Warrior One Left: " + DateTime.Now.ToString() + "\n");
                            pose.forgetRule("armsRaised");
                            pose.forgetRule("armsStraight");
                        }
                        else if (pose is Chair)
                        {
                            log.WriteData("Back pain with Chair: " + DateTime.Now.ToString() + "\n");
                            pose.forgetRule("armsRaised");
                            pose.forgetRule("armsStraight");
                        }
                    }
                }
                MainWindow.answered = false;
                MainWindow.cansayAnswer = false;
                MainWindow.background.settings.volume = MainWindow.volumeLoud;
            }
        }

        public static void checkOutput(String output, Pose pose)
        {
            if (!Exercise.timeIsUp)
            {
                if (output == " ")
                {
                    annoyed = 0;
                    MainWindow.playSoundResourceSync("second_note");
                    log.WriteData("Fixed body issue, annoyed = 0 " + DateTime.Now.ToString() + "\n");
                    Interlocked.Exchange(ref MainWindow.allowed, 1);
                }
                else if (output != "")
                {
                    fixCount += 1;
                    log.WriteData("adjustment = " + output + " " + DateTime.Now.ToString() + "\n");

                    // interpret output string and keep track of rule
                    String[] ruleOut = output.Split(':');
                    String rule = ruleOut[0];
                    output = ruleOut[1];
                    if (rule == oldRule)
                    {
                        annoyed++;
                        log.WriteData("annoyed = " + annoyed.ToString() + " " + DateTime.Now.ToString() + "\n");
                    }
                    else
                    {
                        annoyed = 0;
                        log.WriteData("annoyed = 0 " + DateTime.Now.ToString() + "\n");
                    }
                    oldRule = rule;

                    int threshold = 2;
                    if (annoyed > threshold)
                    {
                        log.WriteData("give up " + DateTime.Now.ToString() + "\n");
                        MainWindow.playSoundResourceSync("second_note");
                        pose.forgetRule(rule);
                        annoyed = 0;
                    }
                    else
                    {
                        String[] result;
                        if (output.Contains("inch") || output.Contains("a foot"))
                        {
                            String[] sep = new String[] { " by " };
                            result = output.Split(sep, StringSplitOptions.None);
                            MainWindow.playSoundResourceSync(fixToResource[result[0]]);
                            MainWindow.playSoundResourceSync(fixToResource["By " + result[1]]);
                        }
                        else
                        {
                            MainWindow.playSoundResourceSync(fixToResource[output]);
                        }
                    }
                    Interlocked.Exchange(ref MainWindow.allowed, 1);
                }
                else
                {
                    log.WriteData("finished! = " + DateTime.Now.ToString() + "\n");
                    annoyed = 0;
                    allRight = true;
                }
            }
        }

        public void readResource(String clip)
        {
            while (MainWindow.toPause) { };
            MainWindow.playSoundResourceSync(clip);
            while (MainWindow.toPause) { };
            threadSleep(3000);
            while (MainWindow.toPause) { };
            if (MainWindow.repeat)
            {
                MainWindow.repeat = false;
                readResource(clip);
            }
        }

        public static Dictionary<string, string> fixToResource = new Dictionary<string, string>()
        {
            {"Bend your left leg further", "bend_your_left_leg_further"},
            {"Bend your legs further", "bend_your_legs_further"},
            {"Bend your right leg further", "bend_your_right_leg_further"},
            {"Bring your arms closer to your head", "bring_arms_closer_to_head"},
            {"Bring your left arm closer to your head", "bring_left_arm_closer_to_head"},
            {"Bring your right arm closer to your head", "bring_right_arm_closer_to_head"},
            {"By at least a foot.", "by_at_least_a_foot"},
            {"By 8 inches.", "by_8_inches"},
            {"By 11 inches.", "by_11_inches"},
            {"By 5 inches.", "by_5_inches"},
            {"By 4 inches.", "by_4_inches"},
            {"By 9 inches.", "by_9_inches"},
            {"By one inch", "by_1_inch"},
            {"By 7 inches.", "by_7_inches"},
            {"By 6 inches.", "by_6_inches"},
            {"By 10 inches.", "by_10_inches"},
            {"By 3 inches.", "by_3_inches"},
            {"By 2 inches.", "by_2_inches"},
            {"Lean backward", "lean_backward"},
            {"Lean forward", "lean_forward"},
            {"Lean sideways toward your left", "lean_left"},
            {"Lean sideways toward your right", "lean_right"},
            {"Lift your left foot off the ground and place on the inside of your right leg", 
                "lift_left_foot_off_ground"},
            {"Lift your right foot off the ground and place on the inside of your left leg", 
                "lift_right_foot_off_ground"},
            {"Lower your arms", "lower_arms"},
            {"Lower your left arm", "lower_left_arm"},
            {"Lower your right arm", "lower_right_arm"},
            {"Move feet closer together", "move_feet_closer_together"},
            {"Move feet further apart", "move_feet_further_apart"},
            {"Move your hips backward", "move_your_hips_backward"},
            {"Move your knees outward so they are above your ankles.", "knees_outward"},
            {"Move your left elbow backward", "move_left_elbow_backward"},
            {"Move your left elbow forward", "move_left_elbow_forward"},
            {"Move your left foot forward or your right foot backward so your toes are at the edge of the yoga mat", 
                "align_feet_on_yoga_mat"},
            {"Move your left hip downward so it is level with your right hip.", "move_left_hip_down"},
            {"Move your left knee backward behind your ankle", "move_left_knee_behind_ankle"},
            {"Move your left knee forward", "move_left_knee_forward"},
            {"Move your left knee to your left so it is above your ankle.", "move_left_knee_left"},
            {"Move your left knee to your left so it is above your ankle. You should feel a stretch in your right inner thigh.", 
                "move_left_knee_left_stretch"},
            {"Move your left wrist backward", "move_left_wrist_backward"},
            {"Move your left wrist forward", "move_left_wrist_forward"},
            {"Move your right elbow backward", "move_right_elbow_backward"},
            {"Move your right elbow forward", "move_right_elbow_forward"},
            {"Move your right foot forward or your left foot backward so your toes are at the edge of the yoga mat", 
                "align_feet_on_yoga_mat_2"},
            {"Move your right hip downward so it is level with your left hip.", "move_right_hip_down"},
            {"Move your right knee backward behind your ankle", "move_right_knee_backward"},
            {"Move your right knee forward", "move_right_knee_forward"},
            {"Move your right knee to your right so it is above your ankle.", "move_right_knee_right"},
            {"Move your right knee to your right so it is above your ankle. You should feel a stretch in your left inner thigh.", 
                "move_right_knee_right_stretch"},
            {"Move your right wrist backward", "move_right_wrist_backward"},
            {"Move your right wrist forward", "move_right_wrist_foward"},
            {"Move your wrists backward", "move_wrists_backward"},
            {"Move your wrists forward", "move_wrists_forward"},
            {"Rest your left hand on your left leg. Do not apply too much pressure", 
                "rest_left_hand_on_left_leg"},
            {"Rest your right hand on your right leg. Do not apply too much pressure", 
                "rest_right_hand_on_right_leg"},
            {"Rotate your hips left", "rotate_hips_left"},
            {"Rotate your hips right", "rotate_hips_right"},
            {"Rotate your shoulders left", "rotate_shoulders_left"},
            {"Rotate your shoulders right", "rotate_shoulders_right"},
            {"Straighten your arms", "straighten_your_arms"},
            {"Straighten your left arm", "straighten_left_arm"},
            {"Straighten your left leg", "straighten_left_leg"},
            {"Straighten your legs", "straighten_your_legs"},
            {"Straighten your right arm", "straighten_right_arm"},
            {"Straighten your right leg", "straighten_right_leg"},
            {"Engage your core muscles to stay stable", "engage_core_muscles"}
        };
    }
}
