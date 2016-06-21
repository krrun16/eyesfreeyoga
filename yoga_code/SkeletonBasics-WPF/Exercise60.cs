using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Samples.Kinect.EyesFreeYoga
{
    class Exercise60 : Exercise
    {
        FileWriter log;
        Pose pose;
        Thread othread;
        bool learningMode;
        int wellCount = 0;
        static int NUMEVAL = 9;
        static int NUMPOSES = 21;

        public Exercise60(FileWriter log, bool learningMode) :  base(log)
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
            while (j < NUMPOSES)
            {
                MainWindow.playSoundResourceSync("moving_to_next");
                latestTimeDuration = DateTime.Now.Subtract(MainWindow.start).TotalMinutes - (MainWindow.timeoffMS / (60 * 1000)) -
                    (MainWindow.timeOffScreen.ElapsedMilliseconds / (60 * 1000)) - honestTimeDuration;
                honestTimeDuration = DateTime.Now.Subtract(MainWindow.start).TotalMinutes - (MainWindow.timeoffMS / (60 * 1000)) -
                    (MainWindow.timeOffScreen.ElapsedMilliseconds / (60 * 1000));
                if (!MainWindow.DEMO && !learningMode) Exercise.incentivize(latestTimeDuration, honestTimeDuration, 60);
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
                    if (!MainWindow.DEMO && !learningMode) Exercise.incentivize(latestTimeDuration, honestTimeDuration, 60);
                    else MainWindow.logProgress((long)latestTimeDuration);
                }
            }
            if (!MainWindow.DEMO && !learningMode) Exercise.incentivize(latestTimeDuration, honestTimeDuration, 60);
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
            if (pose == null)
            {
                pose = new CatCow();
                othread = new Thread(catCow);
                othread.Start();
                return othread;
            }
            else if (pose is CatCow)
            {
                pose = new Child();
                othread = new Thread(childPose);
                othread.Start();
                return othread;
            }
            else if (pose is Child)
            {
                pose = new DownwardDog();
                othread = new Thread(downwardDog);
                othread.Start();
                return othread;
            }
            else if (pose is DownwardDog)
            {
                pose = new DownwardDogFlow();
                othread = new Thread(downwardDogFlow);
                othread.Start();
                return othread;
            }
            else if (pose is DownwardDogFlow)
            {
                pose = new Plank();
                othread = new Thread(plank);
                othread.Start();
                return othread;
            }
            else if (pose is Plank)
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
                pose = new Bridge();
                othread = new Thread(bridge);
                othread.Start();
                return othread;
            }
            else if (pose is Bridge)
            {
                pose = new HappyBaby();
                othread = new Thread(happyBaby);
                othread.Start();
                return othread;
            }
            else if (pose is HappyBaby)
            {
                pose = new BoundAngle();
                othread = new Thread(boundAngle);
                othread.Start();
                return othread;
            }
            else if (pose is BoundAngle)
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

        public override bool getWellBadge()
        {
            if (wellCount / NUMEVAL >= 0.5) return true;
            else return false;
        }

        private void catCow()
        {
            log.WriteData("Started CatCow Pose: " + DateTime.Now.ToString() + "\n");
            // cat cow
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_cat_cow_01");
            readResource("60_cat_cow_02");
            readResource("60_cat_cow_03");
            readResource("60_cat_cow_04");
            readResource("60_cat_cow_05");
            readResource("60_cat_cow_06");
            readResource("60_cat_cow_07");
            readResource("60_cat_cow_08");
            readResource("60_cat_cow_09");
            readResource("60_cat_cow_10");
            readResource("60_cat_cow_11");
            threadSleep(30000);
            logPose(pose, learningMode);
            readResource("60_cat_cow_12");
            readResource("60_cat_cow_13");
            threadSleep(2000);
        }

        private void childPose()
        {
            log.WriteData("Started Child's Pose: " + DateTime.Now.ToString() + "\n");
            // child
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_child_01");
            MainWindow.stating = true;
            readResource("60_child_02");
            MainWindow.stating = false;
            readResource("60_child_03");
            MainWindow.stating = true;
            readResource("60_child_04");
            MainWindow.stating = false;
            readResource("60_child_05");
            readResource("60_child_06");
            readResource("60_child_07");
            readResource("60_child_08");
            threadSleep(15000);
            readResource("60_child_09");
            threadSleep(15000);
            readResource("60_child_10");
            threadSleep(15000);
            readResource("60_child_11");
            threadSleep(60000);
            logPose(pose, learningMode);
            readResource("60_child_12");
            readResource("60_child_13");
            threadSleep(2000);
        }

        private void downwardDog()
        {
            log.WriteData("Started Downward Dog Pose: " + DateTime.Now.ToString() + "\n");
            // downward dog
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_downdog_01");
            readResource("60_downdog_02");
            readResource("60_downdog_03");
            readResource("60_downdog_04");
            readResource("60_downdog_05");
            readResource("60_downdog_06");
            readResource("60_downdog_07");
            readResource("60_downdog_08");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("60_downdog_09");
            readResource("60_downdog_10");
            readResource("60_downdog_11");
            threadSleep(2000);
        }

        private void downwardDogFlow()
        {
            log.WriteData("Started Downward Dog Flow: " + DateTime.Now.ToString() + "\n");
            // downward dog flow
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_down_flow_01");
            readResource("60_down_flow_02");
            readResource("60_down_flow_03");
            readResource("60_down_flow_04");
            readResource("60_down_flow_05");
            readResource("60_down_flow_06");
            readResource("60_down_flow_07");
            readResource("60_down_flow_08");
            readResource("60_down_flow_09");
            readResource("60_down_flow_10");
            threadSleep(60000);
            logPose(pose, learningMode);
            readResource("60_down_flow_11");
            readResource("60_down_flow_12");
            threadSleep(2000);
        }

        private void plank()
        {
            log.WriteData("Started Plank: " + DateTime.Now.ToString() + "\n");
            // plank
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_plank_01");
            readResource("60_plank_02");
            readResource("60_plank_03");
            readResource("60_plank_04");
            readResource("60_plank_05");
            readResource("60_plank_06");
            readResource("60_plank_07");
            readResource("60_plank_08");
            readResource("60_plank_09");
            readResource("60_plank_10");
            readResource("60_plank_11");
            readResource("60_plank_12");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("60_plank_13");
            readResource("60_plank_14");
            readResource("60_plank_15");
            readResource("60_plank_16");
            readResource("60_plank_17");
            threadSleep(60000);
            readResource("60_plank_18");
            readResource("60_plank_19");
            readResource("60_plank_20");
        }

        private void chair()
        {
            log.WriteData("Started Chair: " + DateTime.Now.ToString() + "\n");
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Chair: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_chair_01");
            readResource("60_chair_02");
            readResource("60_chair_03");
            readResource("60_chair_04");
            readResource("60_chair_05");
            readResource("60_chair_06");
            readResource("60_chair_07");
            yesNoPain(pose, "60_chair_08", "60_chair_08a", true);
            yesNoPain(pose, "60_chair_09", "60_chair_09a", false);
            readResource("60_chair_10");
            readResource("60_chair_11");
            readResource("60_chair_12");
            readResource("60_chair_13");
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
            readResource("60_chair_14");
        }

        private void standingFold()
        {
            log.WriteData("Started Standing Fold: " + DateTime.Now.ToString() + "\n");
            // standing forward bend
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_stand_01");
            readResource("60_stand_02");
            readResource("60_stand_03");
            readResource("60_stand_04");
            readResource("60_stand_05");
            readResource("60_stand_06");
            readResource("60_stand_07");
            readResource("60_stand_08");
            readResource("60_stand_09");
            readResource("60_stand_10");
            readResource("60_stand_11");
            readResource("60_stand_12");
            readResource("60_stand_13");
            readResource("60_stand_14");
            readResource("60_stand_15");
            readResource("60_stand_16");
            threadSleep(30000);
            logPose(pose, learningMode);
            readResource("60_stand_17");
            readResource("60_stand_18");
            threadSleep(2000);
        }

        private void treeRight()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Tree Pose Right Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_tree_01");
            readResource("60_tree_02");
            readResource("60_tree_03");
            readResource("60_tree_04");
            readResource("60_tree_05");
            readResource("60_tree_06");
            readResource("60_tree_07");
            readResource("60_tree_08");
            readResource("60_tree_09");
            readResource("60_tree_10");
            readResource("60_tree_11");
            readResource("60_tree_12");
            readResource("60_tree_13");
            readResource("60_tree_14");
            readResource("60_tree_15");
            readResource("60_tree_16");
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
            readResource("60_tree_17");
            readResource("60_tree_18");
        }

        private void tree()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Tree Pose Left Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_tree_19");
            readResource("60_tree_20");
            readResource("60_tree_21");
            readResource("60_tree_22");
            readResource("60_tree_23");
            readResource("60_tree_24");
            readResource("60_tree_25");
            readResource("60_tree_26");
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
            readResource("60_tree_27");
        }

        private void warriorILeft()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Warrior I Left Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_warriorI_01");
            readResource("60_warriorI_02");
            readResource("60_warriorI_03");
            readResource("60_warriorI_04");
            readResource("60_warriorI_05");
            readResource("60_warriorI_06");
            readResource("60_warriorI_07");
            readResource("60_warriorI_08");
            readResource("60_warriorI_09");
            readResource("60_warriorI_10");
            yesNoPain(pose, "60_warriorI_11", "60_warriorI_11a", true);
            readResource("60_warriorI_12");
            yesNoPain(pose, "60_warriorI_13", "60_warriorI_13a", false);
            readResource("60_warriorI_14");
            readResource("60_warriorI_15");
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
            for (int i = 16; i <= 17; i++)
            readResource("60_warriorI_16");
            readResource("60_warriorI_17");
        }

        private void warriorI()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Warrior I Right Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_warriorI_18");
            readResource("60_warriorI_19");
            readResource("60_warriorI_20");
            readResource("60_warriorI_21");
            readResource("60_warriorI_22");
            readResource("60_warriorI_23");
            readResource("60_warriorI_24");
            readResource("60_warriorI_25");
            readResource("60_warriorI_26");
            yesNoPain(pose, "60_warriorI_27", "60_warriorI_27a", true);
            readResource("60_warriorI_28");
            yesNoPain(pose, "60_warriorI_29", "60_warriorI_29a", false);
            readResource("60_warriorI_30");
            readResource("60_warriorI_31");
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
            readResource("60_warriorI_32");
        }

        private void warriorIIRight()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Warrior II Right Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_warriorII_01");
            readResource("60_warriorII_02");
            readResource("60_warriorII_03");
            readResource("60_warriorII_04");
            readResource("60_warriorII_05");
            readResource("60_warriorII_06");
            readResource("60_warriorII_07");
            readResource("60_warriorII_08");
            yesNoPain(pose, "60_warriorII_09", "60_warriorII_09a", true);
            readResource("60_warriorII_10");
            readResource("60_warriorII_11");
            readResource("60_warriorII_12");
            readResource("60_warriorII_13");
            readResource("60_warriorII_14");
            readResource("60_warriorII_15");
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
            readResource("60_warriorII_16");
            readResource("60_warriorII_17");
        }

        private void warriorII()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Warrior II Left Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_warriorII_18");
            readResource("60_warriorII_19");
            readResource("60_warriorII_20");
            readResource("60_warriorII_21");
            readResource("60_warriorII_22");
            readResource("60_warriorII_23");
            yesNoPain(pose, "60_warriorII_24", "60_warriorII_24a", true);
            readResource("60_warriorII_25");
            readResource("60_warriorII_26");
            readResource("60_warriorII_27");
            readResource("60_warriorII_28");
            readResource("60_warriorII_29");
            readResource("60_warriorII_30");
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
            readResource("60_warriorII_31");
        }

        private void reverseWarrior()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Reverse Warrior Right Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_reverse_01");
            readResource("60_reverse_02");
            readResource("60_reverse_03");
            yesNoPain(pose, "60_reverse_04", "60_reverse_04a", true);
            readResource("60_reverse_05");
            readResource("60_reverse_06");
            readResource("60_reverse_07");
            readResource("60_reverse_08");
            readResource("60_reverse_09");
            readResource("60_reverse_10");
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
            readResource("60_reverse_11");
        }

        private void reverseWarriorLeft()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Reverse Warrior Left Leg: " + DateTime.Now.ToString() + "\n");
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_reverse_12");
            readResource("60_reverse_13");
            readResource("60_reverse_14");
            yesNoPain(pose, "60_reverse_15", "60_reverse_15a", true);
            readResource("60_reverse_16");
            readResource("60_reverse_17");
            readResource("60_reverse_18");
            readResource("60_reverse_19");
            readResource("60_reverse_20");
            readResource("60_reverse_21");
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
            readResource("60_reverse_22");
        }

        private void bridge()
        {
            log.WriteData("Started Bridge Pose: " + DateTime.Now.ToString() + "\n");
            // bridge
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_bridge_01");
            readResource("60_bridge_02");
            readResource("60_bridge_03");
            readResource("60_bridge_04");
            readResource("60_bridge_05");
            readResource("60_bridge_06");
            readResource("60_bridge_07");
            readResource("60_bridge_08");
            readResource("60_bridge_09");
            readResource("60_bridge_10");
            readResource("60_bridge_11");
            readResource("60_bridge_12");
            readResource("60_bridge_13");
            readResource("60_bridge_14");
            readResource("60_bridge_15");
            readResource("60_bridge_16");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("60_bridge_17");
            readResource("60_bridge_18");
            readResource("60_bridge_19");
            readResource("60_bridge_20");
            threadSleep(15000);
        }

        private void happyBaby()
        {
            log.WriteData("Started Happy Baby Pose: " + DateTime.Now.ToString() + "\n");
            // happy baby
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_happy_01");
            threadSleep(5000);
            readResource("60_happy_02");
            readResource("60_happy_03");
            readResource("60_happy_04");
            readResource("60_happy_05");
            readResource("60_happy_06");
            readResource("60_happy_07");
            readResource("60_happy_08");
            readResource("60_happy_09");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("60_happy_10");
        }

        private void boundAngle()
        {
            log.WriteData("Started Bound Angle Pose: " + DateTime.Now.ToString() + "\n");
            // bound angle
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_bound_01");
            readResource("60_bound_02");
            readResource("60_bound_03");
            readResource("60_bound_04");
            readResource("60_bound_05");
            readResource("60_bound_06");
            readResource("60_bound_07");
            threadSleep(15000);
            readResource("60_bound_08");
            threadSleep(60000);
            logPose(pose, learningMode);
            readResource("60_bound_09");
            readResource("60_bound_10");
        }

        private void reclinedTwist()
        {
            log.WriteData("Started Reclined Twist Pose: " + DateTime.Now.ToString() + "\n");
            // reclined twist
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_reclined_01");
            readResource("60_reclined_02");
            readResource("60_reclined_03");
            readResource("60_reclined_04");
            readResource("60_reclined_05");
            threadSleep(60000);
            logPose(pose, learningMode);
            readResource("60_reclined_06");
            readResource("60_reclined_07");
            readResource("60_reclined_08");
            readResource("60_reclined_09");
            readResource("60_reclined_10");
            readResource("60_reclined_11");
            threadSleep(60000);
            logPose(pose, learningMode);
            readResource("60_reclined_12");
        }

        private void corpse()
        {
            log.WriteData("Started Corpse Pose: " + DateTime.Now.ToString() + "\n");
            // corpse
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_corpse_01");
            readResource("60_corpse_02");
            readResource("60_corpse_03");
            readResource("60_corpse_04");
            readResource("60_corpse_05");
            MainWindow.stating = true;
            readResource("60_corpse_06");
            MainWindow.stating = false;
            readResource("60_corpse_07");
            logPose(pose, learningMode);
            MainWindow.stating = true;
            readResource("60_corpse_08");
            MainWindow.stating = false;
            readResource("60_corpse_09");
            readResource("60_corpse_10");
            readResource("60_corpse_11");
            readResource("60_corpse_12");
            readResource("60_corpse_13");
            readResource("60_corpse_14");
            readResource("60_corpse_15");
            readResource("60_corpse_16");
            readResource("60_corpse_17");
            readResource("60_corpse_18");
            readResource("60_corpse_19");
            readResource("60_corpse_20");
            threadSleep(300000);
        }

        private void corpseExit()
        {
            log.WriteData("Started Corpse Exit: " + DateTime.Now.ToString() + "\n");
            // corpse exit
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("60_exit_01");
            readResource("60_exit_02");
            readResource("60_exit_03");
            threadSleep(15000);
            readResource("60_exit_04");
            threadSleep(5000);
            readResource("60_exit_05");
            readResource("60_exit_06");
            readResource("60_exit_07");
            readResource("60_exit_08");
            logPose(pose, learningMode);
        }
    }
}
