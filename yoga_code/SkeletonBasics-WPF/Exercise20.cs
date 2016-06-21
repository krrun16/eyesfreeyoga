using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Samples.Kinect.EyesFreeYoga
{
    class Exercise20 : Exercise
    {
        FileWriter log;
        Pose pose;
        Thread othread;
        bool learningMode;
        int wellCount = 0;
        static int NUMEVAL = 1;
        static int NUMPOSES = 7;

        public Exercise20(FileWriter log, bool learningMode) :  base(log)
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
            while (j < NUMPOSES) {
                MainWindow.playSoundResourceSync("moving_to_next");
                latestTimeDuration = DateTime.Now.Subtract(MainWindow.start).TotalMinutes - (MainWindow.timeoffMS / (60*1000)) -
                    (MainWindow.timeOffScreen.ElapsedMilliseconds / (60*1000)) - honestTimeDuration;
                honestTimeDuration = DateTime.Now.Subtract(MainWindow.start).TotalMinutes - (MainWindow.timeoffMS / (60 * 1000)) -
                    (MainWindow.timeOffScreen.ElapsedMilliseconds / (60 * 1000));
                if (!MainWindow.DEMO && !learningMode) Exercise.incentivize(latestTimeDuration, honestTimeDuration, 20);
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
                    if (!MainWindow.DEMO && !learningMode) Exercise.incentivize(latestTimeDuration, honestTimeDuration, 20);
                    else MainWindow.logProgress((long)latestTimeDuration);
                }
            }
            if (!MainWindow.DEMO && !learningMode) Exercise.incentivize(latestTimeDuration, honestTimeDuration, 20);
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
                pose = new StandingFold();
                othread = new Thread(standingFold);
                othread.Start();
                return othread;
            }
            else if (pose is StandingFold)
            {
                pose = new StandingFoldFlow();
                othread = new Thread(standingFoldFlow);
                othread.Start();
                return othread;
            }
            else if (pose is StandingFoldFlow)
            {
                pose = new MountainPose();
                othread = new Thread(mountainPose);
                othread.Start();
                return othread;
            }
            else
            {
                return null;
            }
        }

        private void catCow()
        {
            log.WriteData("Started CatCow Pose: " + DateTime.Now.ToString() + "\n");
            // cat cow
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("20_cat_cow_01");
            readResource("20_cat_cow_02");
            readResource("20_cat_cow_03");
            readResource("20_cat_cow_04");
            readResource("20_cat_cow_05");
            readResource("20_cat_cow_06");
            readResource("20_cat_cow_07");
            readResource("20_cat_cow_08");
            readResource("20_cat_cow_09");
            readResource("20_cat_cow_10");
            readResource("20_cat_cow_11");
            threadSleep(30000);
            logPose(pose, learningMode);
            readResource("20_cat_cow_12");
            readResource("20_cat_cow_13");
        }

        private void childPose()
        {
            log.WriteData("Started Child's Pose: " + DateTime.Now.ToString() + "\n");
            // child
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("20_child_01");
            MainWindow.stating = true;
            readResource("20_child_02");
            MainWindow.stating = false;
            readResource("20_child_03");
            MainWindow.stating = true;
            readResource("20_child_04");
            MainWindow.stating = false;
            readResource("20_child_05");
            readResource("20_child_06");
            readResource("20_child_07");
            readResource("20_child_08");
            threadSleep(15000);
            readResource("20_child_09");
            threadSleep(15000);
            readResource("20_child_10");
            threadSleep(15000);
            readResource("20_child_11");
            threadSleep(60000);
            logPose(pose, learningMode);
            readResource("20_child_12");
            readResource("20_child_13");
        }

        private void downwardDog()
        {
            log.WriteData("Started Downward Dog: " + DateTime.Now.ToString() + "\n");
            // downward dog
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("20_downdog_01");
            readResource("20_downdog_02");
            readResource("20_downdog_03");
            readResource("20_downdog_04");
            readResource("20_downdog_05");
            readResource("20_downdog_06");
            readResource("20_downdog_07");
            readResource("20_downdog_08");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("20_downdog_09");
            readResource("20_downdog_10");
            readResource("20_downdog_11");
        }

        private void downwardDogFlow()
        {
            log.WriteData("Started Downward Dog Flow: " + DateTime.Now.ToString() + "\n");
            // downward dog flow
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("20_down_flow_01");
            readResource("20_down_flow_02");
            readResource("20_down_flow_03");
            readResource("20_down_flow_04");
            readResource("20_down_flow_05");
            readResource("20_down_flow_06");
            readResource("20_down_flow_07");
            readResource("20_down_flow_08");
            readResource("20_down_flow_09");
            readResource("20_down_flow_10");
            threadSleep(60000);
            logPose(pose, learningMode);
            readResource("20_down_flow_11");
            readResource("20_down_flow_12");
            readResource("20_down_flow_13");
        }

        private void standingFold()
        {
            log.WriteData("Started Standing Fold: " + DateTime.Now.ToString() + "\n");
            // standing forward bend
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("20_stand_01");
            readResource("20_stand_02");
            readResource("20_stand_03");
            readResource("20_stand_04");
            readResource("20_stand_05");
            readResource("20_stand_06");
            readResource("20_stand_07");
            readResource("20_stand_08");
            readResource("20_stand_09");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("20_stand_10");
            readResource("20_stand_11");
            readResource("20_stand_12");
        }

        private void standingFoldFlow()
        {
            log.WriteData("Started Standing Forward Fold Flow: " + DateTime.Now.ToString() + "\n");
            // standing forward flow
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("20_stand_flow_01");
            readResource("20_stand_flow_02");
            readResource("20_stand_flow_03");
            readResource("20_stand_flow_04");
            readResource("20_stand_flow_05");
            readResource("20_stand_flow_06");
            readResource("20_stand_flow_07");
            readResource("20_stand_flow_08");
            readResource("20_stand_flow_09");
            readResource("20_stand_flow_10");
            readResource("20_stand_flow_11");
            readResource("20_stand_flow_12");
            readResource("20_stand_flow_13");
            threadSleep(60000);
            logPose(pose, learningMode);
            readResource("20_stand_flow_14");
            readResource("20_stand_flow_15");
        }

        private void mountainPose()
        {
            Interlocked.Exchange(ref MainWindow.allowed, 0);
            MainWindow.background.settings.volume = MainWindow.volumeLoud;
            log.WriteData("Started Mountain Pose: " + DateTime.Now.ToString() + "\n");

            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("20_mountain_01");
            readResource("20_mountain_02");
            readResource("20_mountain_03");
            readResource("20_mountain_04");
            readResource("20_mountain_05");
            readResource("20_mountain_06");
            readResource("20_mountain_07");
            readResource("20_mountain_08");
            readResource("20_mountain_09");
            readResource("20_mountain_10");
            readResource("20_mountain_11");
            readResource("20_mountain_12");
            readResource("20_mountain_13");
            readResource("20_mountain_14");
            if (this.learningMode)
            {
                threadSleep(60000);
            }
            else
            {
                if (hold60(pose)) wellCount++;
            }
            logPose(pose, learningMode);
            MainWindow.stopSoundPlayer();

            readResource("20_mountain_15");
            readResource("20_mountain_16");
            readResource("20_mountain_17");
            readResource("20_mountain_18");
        }

        public override bool getWellBadge()
        {
            if (wellCount / NUMEVAL >= 0.5) return true;
            else return false;
        }
    }
}
