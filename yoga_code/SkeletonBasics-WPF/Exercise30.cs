using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Samples.Kinect.EyesFreeYoga
{
    class Exercise30 : Exercise
    {
        FileWriter log;
        Pose pose;
        Thread othread;
        bool learningMode;
        static int NUMPOSES = 9;

        public Exercise30(FileWriter log, bool learningMode) :  base(log)
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
                if (!MainWindow.DEMO && !learningMode) Exercise.incentivize(latestTimeDuration, honestTimeDuration, 30);
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
                    if (!MainWindow.DEMO && !learningMode) Exercise.incentivize(latestTimeDuration, honestTimeDuration, 30);
                    else MainWindow.logProgress((long)latestTimeDuration);
                }
            }
            if (!MainWindow.DEMO && !learningMode) Exercise.incentivize(latestTimeDuration, honestTimeDuration, 30);
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

        public override bool getWellBadge()
        {
            return true;
        }

        public override Thread advancePose()
        {
            log.WriteData("Advancing Pose: " + DateTime.Now.ToString() + "\n");
            if (pose == null)
            {
                pose = new LowerBackRelease();
                othread = new Thread(lowerBackRelease);
                othread.Start();
                return othread;
            }
            else if (pose is LowerBackRelease)
            {
                pose = new ThreadNeedle();
                othread = new Thread(threadNeedle);
                othread.Start();
                return othread;
            }
            else if (pose is ThreadNeedle)
            {
                pose = new Bridge();
                othread = new Thread(bridge);
                othread.Start();
                return othread;
            }
            else if (pose is Bridge)
            {
                pose = new BridgeFlow();
                othread = new Thread(bridgeFlow);
                othread.Start();
                return othread;
            }
            else if (pose is BridgeFlow)
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

        private void lowerBackRelease()
        {
            log.WriteData("Started Lower Back Release: " + DateTime.Now.ToString() + "\n");
            // lower back release
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("30_low_back_01");
            readResource("30_low_back_02");
            readResource("30_low_back_03");
            readResource("30_low_back_04");
            readResource("30_low_back_05");
            threadSleep(15000);
            readResource("30_low_back_06");
            readResource("30_low_back_07");
            threadSleep(15000);
            readResource("30_low_back_08");
            threadSleep(10000);
            readResource("30_low_back_09");
            readResource("30_low_back_10");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("30_low_back_11");
            threadSleep(10000);
            readResource("30_low_back_12");
            readResource("30_low_back_13");
            readResource("30_low_back_14");
            threadSleep(15000);
            readResource("30_low_back_15");
            readResource("30_low_back_16");
            threadSleep(15000);
            readResource("30_low_back_17");
            threadSleep(10000);
            readResource("30_low_back_18");
            readResource("30_low_back_19");
            threadSleep(15000);
            readResource("30_low_back_20");
            readResource("30_low_back_21");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("30_low_back_22");
        }

        private void threadNeedle()
        {
            log.WriteData("Started Thread the Needle Pose: " + DateTime.Now.ToString() + "\n");
            // thread the needle
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("30_thread_01");
            readResource("30_thread_02");
            readResource("30_thread_03");
            readResource("30_thread_04");
            readResource("30_thread_05");
            readResource("30_thread_06");
            readResource("30_thread_07");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("30_thread_08");
            readResource("30_thread_09");
            readResource("30_thread_10");
            readResource("30_thread_11");
            readResource("30_thread_12");
            readResource("30_thread_13");
            readResource("30_thread_14");
            readResource("30_thread_15");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("30_thread_16");
            readResource("30_thread_17");
        }

        private void bridge()
        {
            log.WriteData("Started Bridge Pose: " + DateTime.Now.ToString() + "\n");
            // bridge
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("30_bridge_01");
            readResource("30_bridge_02");
            readResource("30_bridge_03");
            readResource("30_bridge_04");
            readResource("30_bridge_05");
            readResource("30_bridge_06");
            readResource("30_bridge_07");
            readResource("30_bridge_08");
            readResource("30_bridge_09");
            readResource("30_bridge_10");
            readResource("30_bridge_11");
            readResource("30_bridge_12");
            readResource("30_bridge_13");
            readResource("30_bridge_14");
            threadSleep(15000);
            logPose(pose, learningMode);
        }

        private void bridgeFlow()
        {
            log.WriteData("Started Bridge Flow: " + DateTime.Now.ToString() + "\n");
            // bridge flow
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("30_flow_bridge_01");
            threadSleep(5000);
            readResource("30_flow_bridge_02");
            readResource("30_flow_bridge_03");
            readResource("30_flow_bridge_04");
            readResource("30_flow_bridge_05");
            readResource("30_flow_bridge_06");
            threadSleep(5000);
            readResource("30_flow_bridge_07");
            threadSleep(5000);
            readResource("30_flow_bridge_08");
            threadSleep(60000);
            logPose(pose, learningMode);
            readResource("30_flow_bridge_09");
            readResource("30_flow_bridge_10");
            readResource("30_flow_bridge_11");
            threadSleep(15000);
        }

        private void happyBaby()
        {
            log.WriteData("Started Happy Baby: " + DateTime.Now.ToString() + "\n");
            // happy baby
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("30_happy_01");
            threadSleep(5000);
            readResource("30_happy_02");
            readResource("30_happy_03");
            readResource("30_happy_04");
            readResource("30_happy_05");
            readResource("30_happy_06");
            readResource("30_happy_07");
            readResource("30_happy_08");
            readResource("30_happy_09");
            threadSleep(15000);
            logPose(pose, learningMode);
            readResource("30_happy_10");
        }

        private void boundAngle()
        {
            log.WriteData("Started Bound Angle Pose: " + DateTime.Now.ToString() + "\n");
            // bound angle
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("30_bound_01");
            readResource("30_bound_02");
            readResource("30_bound_03");
            readResource("30_bound_04");
            readResource("30_bound_05");
            readResource("30_bound_06");
            readResource("30_bound_07");
            threadSleep(15000);
            readResource("30_bound_08");
            threadSleep(60000);
            logPose(pose, learningMode);
            readResource("30_bound_09");
            readResource("30_bound_10");
        }

        private void reclinedTwist()
        {
            log.WriteData("Started Reclined Twist: " + DateTime.Now.ToString() + "\n");
            // reclined twist
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("30_reclined_01");
            readResource("30_reclined_02");
            readResource("30_reclined_03");
            readResource("30_reclined_04");
            readResource("30_reclined_05");
            threadSleep(60000);
            logPose(pose, learningMode);
            readResource("30_reclined_06");
            readResource("30_reclined_07");
            readResource("30_reclined_08");
            readResource("30_reclined_09");
            readResource("30_reclined_10");
            readResource("30_reclined_11");
            threadSleep(60000);
            logPose(pose, learningMode);
            readResource("30_reclined_12");
        }

        private void corpse()
        {
            log.WriteData("Started Corpse Pose: " + DateTime.Now.ToString() + "\n");
            // corpse
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("30_corpse_01");
            readResource("30_corpse_02");
            readResource("30_corpse_03");
            readResource("30_corpse_04");
            readResource("30_corpse_05");
            MainWindow.stating = true;
            readResource("30_corpse_06");
            MainWindow.stating = false;
            readResource("30_corpse_07");
            logPose(pose, learningMode);
            MainWindow.stating = true;
            readResource("30_corpse_08");
            MainWindow.stating = false;
            readResource("30_corpse_09");
            readResource("30_corpse_10");
            readResource("30_corpse_11");
            readResource("30_corpse_12");
            readResource("30_corpse_13");
            readResource("30_corpse_14");
            readResource("30_corpse_15");
            readResource("30_corpse_16");
            readResource("30_corpse_17");
            readResource("30_corpse_18");
            readResource("30_corpse_19");
            readResource("30_corpse_20");
            threadSleep(300000);
        }
        
        private void corpseExit()
        {
            log.WriteData("Started Corpse Exit Pose: " + DateTime.Now.ToString() + "\n");
            // corpse exit
            if (learningMode) MainWindow.playSoundResourceSync("feel_free");
            readResource("30_exit_01");
            readResource("30_exit_02");
            readResource("30_exit_03");
            threadSleep(15000);
            readResource("30_exit_04");
            threadSleep(5000);
            readResource("30_exit_05");
            readResource("30_exit_06");
            readResource("30_exit_07");
            readResource("30_exit_08");
            logPose(pose, learningMode);
        }
    }
}
