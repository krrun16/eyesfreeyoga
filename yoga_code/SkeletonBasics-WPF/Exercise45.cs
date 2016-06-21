using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Samples.Kinect.EyesFreeYoga
{
    class Exercise45 : Exercise
    {
        FileWriter log;
        Pose pose;
        Thread othread;
        bool learningMode;
        int wellCount = 0;
        static int NUMEVAL = 10;
        static int NUMPOSES = 17;
        static int DEMOPOSES = 3;

        public Exercise45(FileWriter log, bool learningMode) :  base(log)
        {
            this.log = log;
            this.learningMode = learningMode;
        }

        public override Pose getPose()
        {
            return pose;
        }

        public override void startWorkout()
        {
            MainWindow.beganWorkout = true;
            int j = 0;
            double honestTimeDuration = 0, latestTimeDuration = 0;
            if (MainWindow.DEMO) { 
                NUMPOSES = DEMOPOSES;  
            }
            while (j < NUMPOSES)
            {
                MainWindow.playSoundResourceSync("moving_to_next");
                latestTimeDuration = DateTime.Now.Subtract(MainWindow.start).TotalMinutes - (MainWindow.timeoffMS / (60 * 1000)) -
                    (MainWindow.timeOffScreen.ElapsedMilliseconds / (60 * 1000)) - honestTimeDuration;
                honestTimeDuration = DateTime.Now.Subtract(MainWindow.start).TotalMinutes - (MainWindow.timeoffMS / (60 * 1000)) -
                    (MainWindow.timeOffScreen.ElapsedMilliseconds / (60 * 1000));
                if (!MainWindow.DEMO && !learningMode) Exercise.incentivize(latestTimeDuration, honestTimeDuration, 45);
                else MainWindow.logProgress((long)latestTimeDuration);
                if (!MainWindow.toExit)
                {
                    Thread thread = advancePose();
                    if (thread != null) thread.Join();
                    j++;
                }
                else
                {
                    j = NUMPOSES;
                    if (!MainWindow.DEMO && !learningMode) Exercise.incentivize(latestTimeDuration, honestTimeDuration, 45);
                    else MainWindow.logProgress((long)latestTimeDuration);
                }
            }
            if (!MainWindow.DEMO && !learningMode) Exercise.incentivize(latestTimeDuration, honestTimeDuration, 45);
            else MainWindow.logProgress((long)latestTimeDuration);
            MainWindow.uponExit(getWellBadge());
        }

        public override void killPose()
        {
            if (othread != null)
            {
                othread.Abort();
            }
        }

        public override Thread advancePose()
        {
            log.WriteData("Advancing Pose: " + DateTime.Now.ToString() + "\n");
            if (MainWindow.DEMO)
            {
                if (pose == null)
                {
                    pose = new WarriorILeft();
                    othread = new Thread(warriorILeft);
                    othread.Start();
                    return othread;
                }
                else if (pose is WarriorILeft)
                {
                    pose = new TreeRight();
                    othread = new Thread(treeRight);
                    othread.Start();
                    return othread;
                }
                else if (pose is TreeRight)
                {
                    pose = new Chair();
                    othread = new Thread(chair);
                    othread.Start();
                    return othread;
                }
                else return null;
            }
            else
            {
                if (pose == null)
                {
                    pose = new MountainPose();
                    othread = new Thread(mountain);
                    othread.Start();
                    return othread;
                }
                else if (pose is MountainPose)
                {
                    pose = new WarriorILeft();
                    othread = new Thread(warriorILeft);
                    othread.Start();
                    return othread;
                }
                else if (pose is WarriorILeft)
                {
                    pose = new WarriorI();
                    othread = new Thread(warriorI);
                    othread.Start();
                    return othread;
                }
                else if (pose is WarriorI)
                {
                    pose = new WarriorIIRight();
                    othread = new Thread(warriorIIRight);
                    othread.Start();
                    return othread;
                }
                else if (pose is WarriorIIRight)
                {
                    pose = new WarriorII();
                    othread = new Thread(warriorII);
                    othread.Start();
                    return othread;
                }
                else if (pose is WarriorII)
                {
                    pose = new ReverseWarrior();
                    othread = new Thread(reverseWarrior);
                    othread.Start();
                    return othread;
                }
                else if (pose is ReverseWarrior)
                {
                    pose = new ReverseWarriorLeft();
                    othread = new Thread(reverseWarriorLeft);
                    othread.Start();
                    return othread;
                }
                else if (pose is ReverseWarriorLeft)
                {
                    pose = new TreeRight();
                    othread = new Thread(treeRight);
                    othread.Start();
                    return othread;
                }
                else if (pose is TreeRight)
                {
                    pose = new Tree();
                    othread = new Thread(tree);
                    othread.Start();
                    return othread;
                }
                else if (pose is Tree)
                {
                    pose = new Chair();
                    othread = new Thread(chair);
                    othread.Start();
                    return othread;
                }
                else if (pose is Chair)
                {
                    pose = new StandingFold();
                    othread = new Thread(standingFold);
                    othread.Start();
                    return othread;
                }
                else if (pose is StandingFold)
                {
                    pose = new DownwardDog();
                    othread = new Thread(downwardDog);
                    othread.Start();
                    return othread;
                }
                else if (pose is DownwardDog)
                {
                    pose = new Plank();
                    othread = new Thread(plank);
                    othread.Start();
                    return othread;
                }
                else if (pose is Plank)
                {
                    pose = new Cobra();
                    othread = new Thread(cobra);
                    othread.Start();
                    return othread;
                }
                else if (pose is Cobra)
                {
                    pose = new ReclinedTwist();
                    othread = new Thread(reclinedTwist);
                    othread.Start();
                    return othread;
                }
                else if (pose is ReclinedTwist)
                {
                    pose = new Corpse();
                    othread = new Thread(corpse);
                    othread.Start();
                    return othread;
                }
                else if (pose is Corpse)
                {
                    pose = new CorpseExit();
                    othread = new Thread(corpseExit);
                    othread.Start();
                    return othread;
                }
                else
                {
                    return null;
                }
            }
        }

        public override bool getWellBadge()
        {
            if (wellCount / NUMEVAL >= 0.5) return true;
            else return false;
        }

        private void mountain()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Mountain Pose: " + DateTime.Now.ToString() + "\n");

            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("45_mountain_01");
            readResource("45_mountain_02");
            readResource("45_mountain_03");
            readResource("45_mountain_04");
            readResource("45_mountain_05");
            readResource("45_mountain_06");
            readResource("45_mountain_07");
            readResource("45_mountain_08");
            readResource("45_mountain_09");
            readResource("45_mountain_10");
            readResource("45_mountain_11");
            readResource("45_mountain_12");
            readResource("45_mountain_13");
            readResource("45_mountain_14");
            readResource("45_mountain_15");
            if (this.learningMode)
            {
                threadSleep(15000);
            }
            else
            {
                if (hold15(pose)) wellCount++;
            }
            logPose(pose, learningMode);
            MainWindow.stopSoundPlayer();
            readResource("45_mountain_16");
        }

        private void warriorILeft()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Warrior I Left Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            if (!MainWindow.DEMO)
            {
                readResource("45_warriorI_01");
                readResource("45_warriorI_02");
                readResource("45_warriorI_03");
                readResource("45_warriorI_04");
                readResource("45_warriorI_05");
                readResource("45_warriorI_06");
                readResource("45_warriorI_07");
                readResource("45_warriorI_08");
                readResource("45_warriorI_09");
                readResource("45_warriorI_10");
                yesNoPain(pose, "45_warriorI_11", "45_warriorI_11a", true);
                readResource("45_warriorI_12");
                yesNoPain(pose, "45_warriorI_13", "45_warriorI_13a", false);
                readResource("45_warriorI_14");
            }
            readResource("45_warriorI_15");
            if (this.learningMode)
            {
                threadSleep(15000);
            }
            else
            {
                if (hold15(pose)) wellCount++;
            }
            logPose(pose, learningMode);
            MainWindow.stopSoundPlayer();
            readResource("45_warriorI_16");
            readResource("45_warriorI_17");
        }

        private void warriorI()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Warrior I Right Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("45_warriorI_18");
            readResource("45_warriorI_19");
            readResource("45_warriorI_20");
            readResource("45_warriorI_21");
            readResource("45_warriorI_22");
            readResource("45_warriorI_23");
            readResource("45_warriorI_24");
            readResource("45_warriorI_25");
            readResource("45_warriorI_26");
            yesNoPain(pose, "45_warriorI_27", "45_warriorI_27a", true);
            readResource("45_warriorI_28");
            yesNoPain(pose, "45_warriorI_29", "45_warriorI_29a", false);
            readResource("45_warriorI_30");
            readResource("45_warriorI_31");
            if (this.learningMode)
            {
                threadSleep(15000);
            }
            else
            {
                if (hold15(pose)) wellCount++;
            }
            logPose(pose, learningMode);
            MainWindow.stopSoundPlayer();
            readResource("45_warriorI_32");
        }

        private void warriorIIRight()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Warrior II Right Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("45_warriorII_01");
            readResource("45_warriorII_02");
            readResource("45_warriorII_03");
            readResource("45_warriorII_04");
            readResource("45_warriorII_05");
            readResource("45_warriorII_06");
            readResource("45_warriorII_07");
            readResource("45_warriorII_08");
            yesNoPain(pose, "45_warriorII_09", "45_warriorII_09a", true);
            readResource("45_warriorII_10");
            readResource("45_warriorII_11");
            readResource("45_warriorII_12");
            readResource("45_warriorII_13");
            readResource("45_warriorII_14");
            readResource("45_warriorII_15");
            if (this.learningMode)
            {
                threadSleep(15000);
            }
            else
            {
                if (hold15(pose)) wellCount++;
            }
            logPose(pose, learningMode);
            MainWindow.stopSoundPlayer();
            readResource("45_warriorII_16");
            readResource("45_warriorII_17");
        }

        private void warriorII()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Warrior II Left Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("45_warriorII_18");
            readResource("45_warriorII_19");
            readResource("45_warriorII_20");
            readResource("45_warriorII_21");
            readResource("45_warriorII_22");
            readResource("45_warriorII_23");
            yesNoPain(pose, "45_warriorII_24", "45_warriorII_24a", true);
            readResource("45_warriorII_25");
            readResource("45_warriorII_26");
            readResource("45_warriorII_27");
            readResource("45_warriorII_28");
            readResource("45_warriorII_29");
            readResource("45_warriorII_30");
            if (this.learningMode)
            {
                threadSleep(15000);
            }
            else
            {
                if (hold15(pose)) wellCount++;
            }
            logPose(pose, learningMode);
            MainWindow.stopSoundPlayer();
            readResource("45_warriorII_31");
        }

        private void reverseWarrior()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Reverse Warrior Right Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("45_reverse_01");
            readResource("45_reverse_02");
            readResource("45_reverse_03");
            yesNoPain(pose, "45_reverse_04", "45_reverse_04a", true);
            readResource("45_reverse_05");
            readResource("45_reverse_06");
            readResource("45_reverse_07");
            readResource("45_reverse_08");
            readResource("45_reverse_09");
            readResource("45_reverse_10");
            if (this.learningMode)
            {
                threadSleep(15000);
            }
            else
            {
                if (hold15(pose)) wellCount++;
            }
            logPose(pose, learningMode);
            MainWindow.stopSoundPlayer();
            readResource("45_reverse_11");
        }

        private void reverseWarriorLeft()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Reverse Warrior Left Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("45_reverse_12");
            readResource("45_reverse_13");
            readResource("45_reverse_14");
            yesNoPain(pose, "45_reverse_15", "45_reverse_15a", true);
            readResource("45_reverse_16");
            readResource("45_reverse_17");
            readResource("45_reverse_18");
            readResource("45_reverse_19");
            readResource("45_reverse_20");
            readResource("45_reverse_21");
            if (this.learningMode)
            {
                threadSleep(15000);
            }
            else
            {
                if (hold15(pose)) wellCount++;
            }
            logPose(pose, learningMode);
            MainWindow.stopSoundPlayer();
            readResource("45_reverse_22");
        }

        private void treeRight()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Tree Pose Right Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            if (!MainWindow.DEMO)
            {
                readResource("45_tree_01");
                readResource("45_tree_02");
                readResource("45_tree_03");
                readResource("45_tree_04");
                readResource("45_tree_05");
                readResource("45_tree_06");
                readResource("45_tree_07");
                readResource("45_tree_08");
                readResource("45_tree_09");
                readResource("45_tree_10");
                readResource("45_tree_11");
                readResource("45_tree_12");
                readResource("45_tree_13");
                readResource("45_tree_14");
                readResource("45_tree_15");
            }
            readResource("45_tree_16");
            if (this.learningMode)
            {
                threadSleep(15000);
            }
            else
            {
                if (hold15(pose)) wellCount++;
            }
            logPose(pose, learningMode);
            MainWindow.stopSoundPlayer();
            readResource("45_tree_17");
            readResource("45_tree_18");
        }

        private void tree()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Tree Pose Left Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("45_tree_19");
            readResource("45_tree_20");
            readResource("45_tree_21");
            readResource("45_tree_22");
            readResource("45_tree_23");
            readResource("45_tree_24");
            readResource("45_tree_25");
            readResource("45_tree_26");
            if (this.learningMode)
            {
                threadSleep(15000);
            }
            else
            {
                if (hold15(pose)) wellCount++;
            }
            logPose(pose, learningMode);
            MainWindow.stopSoundPlayer();
            readResource("45_tree_27");
        }

        private void chair()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Chair: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            if (!MainWindow.DEMO)
            {
                readResource("45_chair_01");
                readResource("45_chair_02");
                readResource("45_chair_03");
                readResource("45_chair_04");
                readResource("45_chair_05");
                readResource("45_chair_06");
                readResource("45_chair_07");
                yesNoPain(pose, "45_chair_08", "45_chair_08a", true);
                yesNoPain(pose, "45_chair_09", "45_chair_09a", false);
                readResource("45_chair_10");
                readResource("45_chair_11");
                readResource("45_chair_12");
            }
            readResource("45_chair_13");
            if (this.learningMode)
            {
                threadSleep(15000);
            }
            else
            {
                if (hold15(pose)) wellCount++;
            }
            logPose(pose, learningMode);
            MainWindow.stopSoundPlayer();
            readResource("45_chair_14");
        }

        private void standingFold()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Standing Fold: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("45_stand_01");
            readResource("45_stand_02");
            readResource("45_stand_03");
            readResource("45_stand_04");
            readResource("45_stand_05");
            MainWindow.stating = true;
            readResource("45_stand_06");
            MainWindow.stating = false;
            readResource("45_stand_07");
            readResource("45_stand_08");
            readResource("45_stand_09");
            readResource("45_stand_10");
            readResource("45_stand_11");
            readResource("45_stand_12");
            readResource("45_stand_13");
            readResource("45_stand_14");
            readResource("45_stand_15");
            readResource("45_stand_16");
            readResource("45_stand_17");
            threadSleep(15000);
            logPose(pose, learningMode);
        }

        private void downwardDog()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Downward Dog: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("45_stand_18");
            readResource("45_stand_19");
            readResource("45_stand_20");
            readResource("45_stand_21");
            readResource("45_stand_22");
            readResource("45_stand_23");
            readResource("45_stand_24");
            readResource("45_stand_25");
            readResource("45_stand_26");
            threadSleep(15000);
            logPose(pose, learningMode);
        }

        private void plank()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Plank: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("45_stand_27");
            readResource("45_stand_28");
            readResource("45_stand_29");
            readResource("45_stand_30");
            readResource("45_stand_31");
            readResource("45_stand_32");
            readResource("45_stand_33");
            readResource("45_stand_34");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("45_stand_35");
        }

        private void cobra()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Cobra: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("45_cobra_01");
            threadSleep(15000);
            readResource("45_cobra_02");
            readResource("45_cobra_03");
            readResource("45_cobra_04");
            readResource("45_cobra_05");
            readResource("45_cobra_06");
            readResource("45_cobra_07");
            readResource("45_cobra_08");
            readResource("45_cobra_09");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("45_cobra_10");
            threadSleep(15000);
            readResource("45_cobra_11");
            readResource("45_cobra_12");
            readResource("45_cobra_13");
            readResource("45_cobra_14");
            readResource("45_cobra_15");
            readResource("45_cobra_16");
            readResource("45_cobra_17");
            readResource("45_cobra_18");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("45_cobra_19");
            threadSleep(15000);
            readResource("45_cobra_20");
            readResource("45_cobra_21");
            readResource("45_cobra_22");
            readResource("45_cobra_23");
            readResource("45_cobra_24");
            readResource("45_cobra_25");
            readResource("45_cobra_26");
            readResource("45_cobra_27");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("45_cobra_28");
            threadSleep(15000);
            readResource("45_cobra_29");
            readResource("45_cobra_30");
            MainWindow.stating = true;
            readResource("45_cobra_31");
            MainWindow.stating = false;
            readResource("45_cobra_32");
            readResource("45_cobra_33");
            readResource("45_cobra_34");
            readResource("45_cobra_35");
            readResource("45_cobra_36");
            readResource("45_cobra_37");
            readResource("45_cobra_38");
            threadSleep(60000);
        }

        private void reclinedTwist()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Reclined Twist: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("45_reclined_01");
            readResource("45_reclined_02");
            readResource("45_reclined_03");
            readResource("45_reclined_04");
            readResource("45_reclined_05");
            readResource("45_reclined_06");
            threadSleep(60000);
            logPose(pose, learningMode);
            readResource("45_reclined_07");
            readResource("45_reclined_08");
            readResource("45_reclined_09");
            readResource("45_reclined_10");
            readResource("45_reclined_11");
            readResource("45_reclined_12");
            threadSleep(60000);
            logPose(pose, learningMode);
            readResource("45_reclined_13");
        }

        private void corpse()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Corpse: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("45_corpse_01");
            readResource("45_corpse_02");
            readResource("45_corpse_03");
            readResource("45_corpse_04");
            MainWindow.stating = true;
            readResource("45_corpse_05");
            MainWindow.stating = false;
            readResource("45_corpse_06");
            readResource("45_corpse_07");
            readResource("45_corpse_08");
            readResource("45_corpse_09");
            readResource("45_corpse_10");
            readResource("45_corpse_11");
            readResource("45_corpse_12");
            readResource("45_corpse_13");
            logPose(pose, learningMode);
            threadSleep(240000);
        }

        private void corpseExit()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Corpse Exit: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("45_exit_01");
            readResource("45_exit_02");
            readResource("45_exit_03");
            threadSleep(15000);
            readResource("45_exit_04");
            threadSleep(5000);
            readResource("45_exit_05");
            readResource("45_exit_06");
            readResource("45_exit_07");
            readResource("45_exit_08");
            logPose(pose, learningMode);
        }
    }
}
