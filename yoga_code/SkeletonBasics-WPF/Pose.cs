using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.Kinect;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.Samples.Kinect.EyesFreeYoga
{
    class Pose
    {
        protected double neck, quadLeft, calveLeft, quadRight, calveRight, shoulderToElbowLeft, headToElbowLeft, shoulderToElbowRight, headToElbowRight, feetDistance, ankleToHipLeft,
            ankleToHipRight, shoulderToWristLeft, shoulderToWristRight, upperArmLeft, upperArmRight, lowerArmLeft, lowerArmRight, backX, backZ, shoulderSymm, elbowSymm,
            wristSymm, handSymm, hipSymm, kneeSymm, ankleSymm, elbowToShoulderLeftZ, elbowToShoulderRightZ, wristToElbowLeftZ, wristToElbowRightZ, leftKneeToAnkleZ,
            spineToElbowRight, spineToElbowLeft, shoulderCorrectedToElbowRight, shoulderCorrectedToElbowLeft, upperSpine, rightKneeToAnkleX, handToLeftLeg, handToRightLeg, leftKneeToAnkleX,
            rightKneeToAnkleZ, rightToLeftAnkleY, leftToRightAnkleY, lowerLegSlope, lowerLegIntercept, kneeAnkleX, projectedFoot, lowerFootRange, higherFootRange, hipSymmY, wobbleLineX,
            wobbleLineY, wobbleLineZ;
        protected double oldWobbleX = 0, oldWobbleY = 0, oldWobbleZ = 0, oldDistanceFromTarget = 0;
        protected SkeletonPoint shoulderCenter;
        String badString = "";
        protected Dictionary<String, Boolean> rules = new Dictionary<String, Boolean>();
        protected const String dontLeanString = "dontLean", symmetricHipsString = "symmetricHips", symmetricShouldersString = "symmetricShoulders",
            legsStraightString = "legsStraight", symmetricKneesString = "symmetricKnees", symmetricAnklesString = "symmetricAnkles", armsStraightString = "armsStraight",
            armsLowString = "armsLow", symmetricElbowsString = "symmetricElbows", symmetricWristsString = "symmetricWrists", ankleDistanceString = "ankleDistance",
            hipsLevelString = "hipsLevel", warriorILegString = "warriorILeg", warriorILeftLegString = "warriorILeftLeg", rightKneeFarString = "rightKneeFar", rightKneeInXString = "rightKneeInX",
            leftKneeInXString = "leftKneeInX", warriorIIRightLegString = "warriorIIRightLeg", reverseWarriorLeftLegString = "reverseWarriorLeftLeg",
            armsRaisedString = "armsRaised", warriorIILegString = "warriorIILeg", leftKneeFarString = "leftKneeFar", leftKneeFarZString = "leftKneeFarZ", leftKneeInZString = "leftKneeInZ",
            armsSidewaysString = "armsSideways", reverseWarriorLegString = "reverseWarriorLeg", rightKneeFarXString = "rightKneeFarX", rightKneeInZString = "rightKneeInZ",
            rightArmRaisedString = "rightArmRaised", leftArmTouchingString = "leftArmTouching", legsBentString = "legsBent", kneesInXString = "kneesInX",
            isWobblyString = "isWobbly", rightFootHigherString = "rightFootHigher", leftFootHigherString = "leftFootHigher", leftArmRaisedString = "leftArmRaised", rightArmTouchingString = "rightArmTouching";
        protected String badGroup = "";
        protected String A = "Group A", B = "Group B", group = "";

        public void forgetRule(String rule)
        {
            rules[rule] = false;
            badGroup = "";
        }

        public void removeRules()
        {
            rules.Clear();
        }
        
        private double distance(Joint first, Joint second)
        {
            return Math.Sqrt(Math.Pow(first.Position.X - second.Position.X, 2) + Math.Pow(first.Position.Y - second.Position.Y, 2)
                + Math.Pow(first.Position.Z - second.Position.Z, 2));
        }

        private double distanceWithPoint(SkeletonPoint point, Joint second)
        {
            return Math.Sqrt(Math.Pow(point.X - second.Position.X, 2) + Math.Pow(point.Y - second.Position.Y, 2) +
                Math.Pow(point.Z - second.Position.Z, 2));
        }

        public Pose() { }

        public virtual String compareSkeleton(Skeleton skeleton)
        {
            // Torso
            SkeletonPoint shoulderCenter = new SkeletonPoint();
            shoulderCenter.X = (skeleton.Joints[JointType.ShoulderRight].Position.X + skeleton.Joints[JointType.ShoulderLeft].Position.X) / 2;
            shoulderCenter.Y = (skeleton.Joints[JointType.ShoulderRight].Position.Y + skeleton.Joints[JointType.ShoulderLeft].Position.Y) / 2;
            shoulderCenter.Z = (skeleton.Joints[JointType.ShoulderRight].Position.Z + skeleton.Joints[JointType.ShoulderLeft].Position.Z) / 2;
            this.shoulderCenter = shoulderCenter;
            this.neck = distanceWithPoint(shoulderCenter, skeleton.Joints[JointType.Head]);
            this.backX = skeleton.Joints[JointType.ShoulderCenter].Position.X - skeleton.Joints[JointType.HipCenter].Position.X;
            this.backZ = skeleton.Joints[JointType.ShoulderCenter].Position.Z - skeleton.Joints[JointType.HipCenter].Position.Z;
            this.upperSpine = distance(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);
            this.wobbleLineX = this.shoulderCenter.X - skeleton.Joints[JointType.HipCenter].Position.X;
            this.wobbleLineY = this.shoulderCenter.Y - skeleton.Joints[JointType.HipCenter].Position.Y;
            this.wobbleLineZ = this.shoulderCenter.Z - skeleton.Joints[JointType.HipCenter].Position.Z;

            // Left Arm
            this.upperArmLeft = distance(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
            this.lowerArmLeft = distance(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
            this.shoulderToElbowLeft = distanceWithPoint(this.shoulderCenter, skeleton.Joints[JointType.ElbowLeft]);
            this.headToElbowLeft = distance(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ElbowLeft]);
            this.shoulderToWristLeft = distance(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.WristLeft]);
            this.spineToElbowLeft = distance(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.ElbowLeft]);
            this.shoulderCorrectedToElbowLeft = distanceWithPoint(this.shoulderCenter, skeleton.Joints[JointType.ElbowLeft]);

            // Right arm
            this.upperArmRight = distance(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
            this.lowerArmRight = distance(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
            this.shoulderToElbowRight = distanceWithPoint(this.shoulderCenter, skeleton.Joints[JointType.ElbowRight]);
            this.headToElbowRight = distance(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ElbowRight]);
            this.shoulderToWristRight = distance(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.WristRight]);
            this.spineToElbowRight = distance(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.ElbowRight]);
            this.shoulderCorrectedToElbowRight = distanceWithPoint(this.shoulderCenter, skeleton.Joints[JointType.ElbowRight]);

            // Left Leg
            this.quadLeft = distance(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft]);
            this.calveLeft = distance(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft]);
            this.ankleToHipLeft = distance(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.HipLeft]);
            this.handToLeftLeg = Math.Abs(skeleton.Joints[JointType.KneeLeft].Position.X - skeleton.Joints[JointType.WristLeft].Position.X);
            this.kneeAnkleX = (skeleton.Joints[JointType.KneeLeft].Position.X + skeleton.Joints[JointType.AnkleLeft].Position.X) / 2;
            this.lowerFootRange = skeleton.Joints[JointType.AnkleLeft].Position.Y;
            this.higherFootRange = skeleton.Joints[JointType.KneeLeft].Position.Y;

            // Right Leg
            this.quadRight = distance(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight]);
            this.calveRight = distance(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight]);
            this.ankleToHipRight = distance(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.HipRight]);
            this.lowerLegIntercept = skeleton.Joints[JointType.AnkleRight].Position.Y;
            this.lowerLegSlope = (skeleton.Joints[JointType.KneeRight].Position.Y - this.lowerLegIntercept) /
                (skeleton.Joints[JointType.KneeRight].Position.X - skeleton.Joints[JointType.AnkleRight].Position.X);
            this.projectedFoot = this.lowerLegSlope * this.kneeAnkleX + this.lowerLegIntercept;
            this.handToRightLeg = Math.Abs(skeleton.Joints[JointType.KneeRight].Position.X - skeleton.Joints[JointType.WristRight].Position.X);

            // Feet distance apart
            this.feetDistance = distance(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.AnkleRight]);
            this.rightToLeftAnkleY = skeleton.Joints[JointType.AnkleRight].Position.Y - skeleton.Joints[JointType.AnkleLeft].Position.Y;
            this.leftToRightAnkleY = skeleton.Joints[JointType.AnkleLeft].Position.Y - skeleton.Joints[JointType.AnkleRight].Position.Y;

            // Body symmetry
            this.shoulderSymm = skeleton.Joints[JointType.ShoulderRight].Position.Z - skeleton.Joints[JointType.ShoulderLeft].Position.Z;
            this.elbowSymm = skeleton.Joints[JointType.ElbowRight].Position.Z - skeleton.Joints[JointType.ElbowLeft].Position.Z;
            this.wristSymm = skeleton.Joints[JointType.WristRight].Position.Z - skeleton.Joints[JointType.WristLeft].Position.Z;
            this.handSymm = skeleton.Joints[JointType.HandRight].Position.Z - skeleton.Joints[JointType.HandLeft].Position.Z;
            this.hipSymm = skeleton.Joints[JointType.HipRight].Position.Z - skeleton.Joints[JointType.HipLeft].Position.Z;
            this.kneeSymm = skeleton.Joints[JointType.KneeRight].Position.Z - skeleton.Joints[JointType.KneeLeft].Position.Z;
            this.ankleSymm = skeleton.Joints[JointType.AnkleRight].Position.Z - skeleton.Joints[JointType.AnkleLeft].Position.Z;
            this.elbowToShoulderLeftZ = skeleton.Joints[JointType.ElbowLeft].Position.Z - skeleton.Joints[JointType.ShoulderLeft].Position.Z;
            this.elbowToShoulderRightZ = skeleton.Joints[JointType.ElbowRight].Position.Z - skeleton.Joints[JointType.ShoulderRight].Position.Z;
            this.wristToElbowLeftZ = skeleton.Joints[JointType.WristLeft].Position.Z - skeleton.Joints[JointType.ElbowLeft].Position.Z;
            this.wristToElbowRightZ = skeleton.Joints[JointType.WristRight].Position.Z - skeleton.Joints[JointType.ElbowRight].Position.Z;
            this.leftKneeToAnkleZ = skeleton.Joints[JointType.KneeLeft].Position.Z - skeleton.Joints[JointType.AnkleLeft].Position.Z;
            this.rightKneeToAnkleZ = skeleton.Joints[JointType.KneeRight].Position.Z - skeleton.Joints[JointType.AnkleRight].Position.Z;
            this.leftKneeToAnkleX = skeleton.Joints[JointType.KneeLeft].Position.X - skeleton.Joints[JointType.AnkleLeft].Position.X;
            this.rightKneeToAnkleX = skeleton.Joints[JointType.KneeRight].Position.X - skeleton.Joints[JointType.AnkleRight].Position.X;
            this.hipSymmY = skeleton.Joints[JointType.HipRight].Position.Y - skeleton.Joints[JointType.HipLeft].Position.Y;
            return "";
        }

        /// <summary>
        /// Calculates the body angle corresponding to the opposing side of the triangle using the law of cosines (in degrees)
        /// </summary>
        /// <param name="opp">Opposing side of the triangle</param>
        /// <param name="adj1">One adjacent side to angle</param>
        /// <param name="adj2">Other adjacent side to angle</param>
        private double calcAngle(double opp, double adj1, double adj2)
        {
            double cosine = (Math.Pow(adj1, 2) + Math.Pow(adj2, 2) - Math.Pow(opp, 2)) / (2 * adj1 * adj2);
            return Math.Acos(cosine) * 57.2957795;
        }

        protected String checkRule(String rule, Object[] arguments = null)
        {
            String output = "";
            if (rules[rule])
            {
                // Check that the person isn't leaning
                if (badGroup == "" || badGroup == rule)
                {
                    Type thisType = this.GetType().BaseType;
                    MethodInfo theMethod = thisType.GetMethod(rule);
                    output = (String)theMethod.Invoke(this, arguments);
                }
                if (output != "")
                {
                    badGroup = rule;
                    return badGroup + ":" + output;
                }
                else if (output == "" && badGroup == rule)
                {
                    badGroup = "";
                    return " ";
                }
            }
            return "";
        }

        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        public String ankleDistance()
        {
            String measure = "";
            // Ankles should be .17-.21 meters apart
            if (feetDistance < .17)
            {
                double inchesToMove = Math.Ceiling((.17 - feetDistance) * 39.3701);
                
                if (inchesToMove > 1 && inchesToMove < 12)
                {
                    measure = " by " + inchesToMove.ToString() + " inches.";
                    badString = "Move feet further apart" + measure;
                }
                else if (inchesToMove >= 12)
                {
                    measure = " by at least a foot.";
                    badString = "Move feet further apart" + measure;
                }
                else
                {
                    badString = "";
                }
            }
            else if (feetDistance > .21)
            {
                double inchesToMove = Math.Ceiling((feetDistance - .21) * 39.3701);

                if (inchesToMove > 1 && inchesToMove < 12)
                {
                    measure = " by " + inchesToMove.ToString() + " inches.";
                    badString = "Move feet closer together" + measure;
                }
                else if (inchesToMove >= 12)
                {
                    measure = " by at least a foot.";
                    badString = "Move feet closer together" + measure;
                }
                else
                {
                    badString = "";
                }
            }
            else
            {
                badString = "";
            }
            return badString;
        }

        public String armsRaised()
        {
            double leftArmAngle = calcAngle(headToElbowLeft, neck, shoulderToElbowLeft);
            double rightArmAngle = calcAngle(headToElbowRight, neck, shoulderToElbowRight);

            // Shoulder center to elbow angle should be under 90 degrees
            if (leftArmAngle > 90 && rightArmAngle > 90) badString = "Bring your arms closer to your head";
            else if (leftArmAngle > 90) badString = "Bring your left arm closer to your head";
            else if (rightArmAngle > 90) badString = "Bring your right arm closer to your head";
            else badString = "";

            return badString;
        }

        public String rightArmRaised()
        {
            double rightArmAngle = calcAngle(headToElbowRight, neck, shoulderToElbowRight);

            // Shoulder center to elbow angle should be under 120 degrees
            if (rightArmAngle > 60) badString = "Bring your right arm closer to your head";
            else badString = "";

            return badString;
        }

        public String leftArmRaised()
        {
            double leftArmAngle = calcAngle(headToElbowLeft, neck, shoulderToElbowLeft);

            // Shoulder center to elbow angle should be under 120 degrees
            if (leftArmAngle > 60) badString = "Bring your left arm closer to your head";
            else badString = "";

            return badString;
        }

        public String leftArmTouching()
        {
            if (handToLeftLeg > .4) badString = "Rest your left hand on your left leg. Do not apply too much pressure";
            else badString = "";

            return badString;
        }

        public String rightArmTouching()
        {
            if (handToRightLeg < -.4) badString = "Rest your right hand on your right leg. Do not apply too much pressure";
            else badString = "";

            return badString;
        }

        public String armsSideways()
        {
            double leftArmAngle = calcAngle(spineToElbowLeft, upperSpine, shoulderCorrectedToElbowLeft);
            double rightArmAngle = calcAngle(spineToElbowRight, upperSpine, shoulderCorrectedToElbowRight);

            int armLower = 70;
            int armHigher = 100;

            // Armpit angle should be over 80 degrees
            if (leftArmAngle < armLower && rightArmAngle < armLower) badString = "Bring your arms closer to your head";
            else if (leftArmAngle < armLower) badString = "Bring your left arm closer to your head";
            else if (rightArmAngle < armLower) badString = "Bring your right arm closer to your head";
            // Armpit angle should be under 100 degrees
            else if (leftArmAngle > armHigher && rightArmAngle > armHigher) badString = "Lower your arms";
            else if (leftArmAngle > armHigher) badString = "Lower your left arm";
            else if (rightArmAngle > armHigher) badString = "Lower your right arm";
            else badString = "";
            return badString;
        }

        public String armsLow()
        {
            double leftArmAngle = calcAngle(spineToElbowLeft, upperSpine, shoulderCorrectedToElbowLeft);
            double rightArmAngle = calcAngle(spineToElbowRight, upperSpine, shoulderCorrectedToElbowRight);

            int armHigher = 60;

            // Armpit angle should be under 60 degrees
            if (leftArmAngle > armHigher && rightArmAngle > armHigher) badString = "Lower your arms";
            else if (leftArmAngle > armHigher) badString = "Lower your left arm";
            else if (rightArmAngle > armHigher) badString = "Lower your right arm";
            else badString = "";
            return badString;
        }

        public String legsStraight()
        {
            double kneeAngleLeft = calcAngle(ankleToHipLeft, quadLeft, calveLeft);
            double kneeAngleRight = calcAngle(ankleToHipRight, quadRight, calveRight);

            // Knee angle should be at least 165 degrees
            if (kneeAngleLeft < 165 && kneeAngleRight < 165) badString = "Straighten your legs";
            else if (kneeAngleLeft < 165) badString = "Straighten your left leg";
            else if (kneeAngleRight < 165) badString = "Straighten your right leg";
            else badString = "";

            return badString;
        }

        public String legsBent()
        {
            double kneeAngleLeft = calcAngle(ankleToHipLeft, quadLeft, calveLeft);
            double kneeAngleRight = calcAngle(ankleToHipRight, quadRight, calveRight);

            // Knee angle should be less than 150 degrees
            if (kneeAngleLeft > 170 && kneeAngleRight > 170) badString = "Bend your legs further";
            else if (kneeAngleLeft > 170) badString = "Bend your legs further";
            else if (kneeAngleRight > 170) badString = "Bend your legs further";
            else if (leftKneeToAnkleZ < -.1 && rightKneeToAnkleZ < -.1) badString = "Move your hips backward";
            else badString = "";

            return badString;
        }

        public String warriorILeg()
        {
            double kneeAngleLeft = calcAngle(ankleToHipLeft, quadLeft, calveLeft);
            double kneeAngleRight = calcAngle(ankleToHipRight, quadRight, calveRight);

            // Left knee angle should be at least 155 degrees
            if (kneeAngleLeft < 155) badString = "Straighten your left leg";
            // Right knee angle should be less than 150 degrees
            else if (kneeAngleRight > 170) badString = "Bend your right leg further";
            else badString = "";
            return badString;
        }

        public String warriorILeftLeg()
        {
            double kneeAngleLeft = calcAngle(ankleToHipLeft, quadLeft, calveLeft);
            double kneeAngleRight = calcAngle(ankleToHipRight, quadRight, calveRight);

            // Right knee angle should be at least 155 degrees
            if (kneeAngleRight < 155) badString = "Straighten your right leg";
            // Left knee angle should be less than 170 degrees
            else if (kneeAngleLeft > 170) badString = "Bend your left leg further";
            else badString = "";
            return badString;
        }

        public String reverseWarriorLeg()
        {
            double kneeAngleLeft = calcAngle(ankleToHipLeft, quadLeft, calveLeft);
            double kneeAngleRight = calcAngle(ankleToHipRight, quadRight, calveRight);

            // Left knee angle should be at least 165 degrees
            if (kneeAngleLeft < 155) badString = "Straighten your left leg";
            // Right knee angle should be less than 150 degrees
            else if (kneeAngleRight > 170) badString = "Bend your right leg further";
            else badString = "";
            return badString;
        }

        public String reverseWarriorLeftLeg()
        {
            double kneeAngleLeft = calcAngle(ankleToHipLeft, quadLeft, calveLeft);
            double kneeAngleRight = calcAngle(ankleToHipRight, quadRight, calveRight);

            // Right knee angle should be at least 155 degrees
            if (kneeAngleRight < 155) badString = "Straighten your right leg";
            // Left knee angle should be less than 170 degrees
            else if (kneeAngleLeft > 170) badString = "Bend your left leg further";
            else badString = "";
            return badString;
        }

        public String rightKneeFar()
        {
            // The right knee should be further back than the right ankle in Z
            if (rightKneeToAnkleZ < 0) badString = "Move your right knee backward behind your ankle";
            else badString = "";
            return badString;
        }

        public String leftKneeFarZ()
        {
            // The left knee should be further back than the left ankle in Z
            if (leftKneeToAnkleZ < 0) badString = "Move your left knee backward behind your ankle";
            else badString = "";
            return badString;
        }

        public String rightKneeFarX()
        {
            // The right knee should be further back than the right ankle in Z
            if (rightKneeToAnkleX > 0) badString = "Move your right knee backward behind your ankle";
            else badString = "";

            return badString;
        }

        public String kneesInX()
        {
            if (rightKneeToAnkleX < -0.05 && leftKneeToAnkleX > 0.05) badString = "Move your knees outward so they are above your ankles.";
            // Right Knee X – Right Ankle X should be > -threshold
            else if (rightKneeToAnkleX < -0.05) badString = "Move your knees outward so they are above your ankles.";
            else if (leftKneeToAnkleX > 0.05) badString = "Move your knees outward so they are above your ankles.";
            else badString = "";
            return badString;
        }

        public String rightKneeInX()
        {
            // Right Knee X – Right Ankle X should be > -threshold
            if (rightKneeToAnkleX < -0.05) badString = "Move your right knee to your right so it is above your ankle. You should feel a stretch in your left inner thigh.";
            else badString = "";
            return badString;
        }

        public String leftKneeInX()
        {
            // Left Knee X – Left Ankle X should be > -threshold
            if (leftKneeToAnkleX > 0.05) badString = "Move your left knee to your left so it is above your ankle. You should feel a stretch in your right inner thigh.";
            else badString = "";
            return badString;
        }

        public String rightKneeInZ()
        {
            // Right Knee Z – Right Ankle Z should be > -threshold
            if (rightKneeToAnkleZ < 0.0) badString = "Move your right knee to your right so it is above your ankle. You should feel a stretch in your left inner thigh.";
            else badString = "";
            return badString;
        }

        public String leftKneeInZ()
        {
            // Left Knee Z – Left Ankle Z should be > threshold
            if (leftKneeToAnkleZ < 0.0) badString = "Move your left knee to your left so it is above your ankle. You should feel a stretch in your right inner thigh.";
            else badString = "";
            return badString;
        }

        public String warriorIILeg()
        {
            double kneeAngleLeft = calcAngle(ankleToHipLeft, quadLeft, calveLeft);
            double kneeAngleRight = calcAngle(ankleToHipRight, quadRight, calveRight);

            // Right knee angle should be at least 165 degrees
            if (kneeAngleRight < 155) badString = "Straighten your right leg";
            // Left knee angle should be less than 150 degrees
            else if (kneeAngleLeft > 170) badString = "Bend your left leg further";
            else badString = "";
            return badString;
        }

        public String warriorIIRightLeg()
        {
            double kneeAngleLeft = calcAngle(ankleToHipLeft, quadLeft, calveLeft);
            double kneeAngleRight = calcAngle(ankleToHipRight, quadRight, calveRight);

            // Left knee angle should be at least 155 degrees
            if (kneeAngleLeft < 155) badString = "Straighten your left leg";
            // Right knee angle should be less than 170 degrees
            else if (kneeAngleRight > 170) badString = "Bend your right leg further";
            else badString = "";
            return badString;
        }

        public String leftKneeFar()
        {
            // The left knee should be further back than the right ankle in X
            if (leftKneeToAnkleX < 0) badString = "Move your left knee backward behind your ankle";
            else badString = "";
            return badString;
        }

        public String rightFootHigher()
        {
            // The left knee should be further back than the right ankle in X
            if (rightToLeftAnkleY < 0) badString = "Lift your right foot off the ground and place on the inside of your left leg";
            else badString = "";
            return badString;
        }

        public String leftFootHigher()
        {
            // The left knee should be further back than the right ankle in X
            if (leftToRightAnkleY < 0) badString = "Lift your left foot off the ground and place on the inside of your right leg";
            else badString = "";
            return badString;
        }

        public String armsStraight()
        {
            double elbowAngleLeft = calcAngle(shoulderToWristLeft, lowerArmLeft, upperArmLeft);
            double elbowAngleRight = calcAngle(shoulderToWristRight, lowerArmRight, upperArmRight);

            // Elbow angle should be at least 150 degrees
            if (elbowAngleLeft < 150 && elbowAngleRight < 150) badString = "Straighten your arms";
            else if (elbowAngleLeft < 150) badString = "Straighten your left arm";
            else if (elbowAngleRight < 150) badString = "Straighten your right arm";
            else badString = "";
            return badString;
        }

        public String dontLean(Boolean useSide = true, Boolean useFront = true, Boolean reverseLeft = false, Boolean mountain = false)
        {
            // Abs(X) between shoulder center and hip center < .0254
            if (backX > .0762 && useSide) badString = "Lean sideways toward your left";
            else if (backX < -.0762 && useSide) badString = "Lean sideways toward your right";
            // Abs(X) between shoulder center and hip center > .0254
            else if (backX > -.0254 && !useSide && !reverseLeft) badString = "Lean sideways toward your left";
            else if (backX < .0254 && !useSide && reverseLeft) badString = "Lean sideways toward your right";
            else if (backZ > .1524 && useFront && mountain) badString = "Lean forward";
            else if (backZ > .0762 && useFront && !mountain) badString = "Lean forward";
            else if (backZ < -.0762 && useFront) badString = "Lean backward";
            else badString = "";
            return badString;
        }

        public String isWobbly()
        {    
            double threshold = 0.02;
            //If constantly in motion
            if ((oldWobbleX != 0.0) && (oldWobbleX - wobbleLineX > threshold || oldWobbleY - wobbleLineY > threshold || oldWobbleZ - wobbleLineZ > threshold))
                badString = "Engage your core muscles to stay stable";
            else badString = "";
            oldWobbleX = wobbleLineX;
            oldWobbleY = wobbleLineY;
            oldWobbleZ = wobbleLineZ;
            return badString;
        }

        public String symmetricAnkles()
        {
            double anklesensitivity = .1;
            // Body symmetry Abs(Z) between right and left < .1 (ankle)
            if (ankleSymm > anklesensitivity) badString = "Move your right foot forward or your left foot backward so your toes are at the edge of the yoga mat";
            else if (ankleSymm < -anklesensitivity) badString = "Move your left foot forward or your right foot backward so your toes are at the edge of the yoga mat";
            else badString = "";
            return badString;
        }

        public String symmetricKnees()
        {
            double kneesensitivity = .1;
            // Body symmetry Abs(Z) between right and left < .1 (ankle)
            if (kneeSymm > kneesensitivity) badString = "Move your right knee forward";
            else if (kneeSymm < -kneesensitivity) badString = "Move your left knee forward";
            else badString = "";
            return badString;
        }

        public String symmetricHips()
        {
            double hipsensitivity = .1;
            // Body symmetry Abs(Z) between right and left < bodysensitivity (hips)
            if (hipSymm > hipsensitivity) badString = "Rotate your hips left";
            else if (hipSymm < -hipsensitivity) badString = "Rotate your hips right";
            else badString = "";
            return badString;
        }

        public String hipsLevel()
        {
            double hipsensitivity = .05;
            // Body symmetry Abs(Y) between right and left < bodysensitivity (hips)
            if (hipSymmY > hipsensitivity) badString = "Move your right hip downward so it is level with your left hip.";
            else if (hipSymmY < -hipsensitivity) badString = "Move your left hip downward so it is level with your right hip.";
            else badString = "";
            return badString;
        }

        public String symmetricShoulders()
        {
            double shouldersensitivity = .1;
            // Body symmetry Abs(Z) between right and left < bodysensitivity (shoulders)
            if (shoulderSymm > shouldersensitivity) badString = "Rotate your shoulders left";
            else if (shoulderSymm < -shouldersensitivity) badString = "Rotate your shoulders right";
            else badString = "";
            return badString;
        }

        public String symmetricElbows()
        {
            double elbowsensitivity = .1;
            // Body symmetry Abs(Z) between right and left < bodysensitivity (elbow)
            if (elbowSymm > elbowsensitivity)
            {
                if (elbowToShoulderLeftZ < -elbowsensitivity) badString = "Move your left elbow backward";
                else if (elbowToShoulderRightZ > elbowsensitivity) badString = "Move your right elbow forward";
            }
            else if (elbowSymm < -elbowsensitivity)
            {
                if (elbowToShoulderRightZ < -elbowsensitivity) badString = "Move your right elbow backward";
                else if (elbowToShoulderLeftZ > elbowsensitivity) badString = "Move your left elbow forward";
            }
            else badString = "";
            return badString;
        }
        
        public String symmetricWrists(Boolean armsperpendicular = true)
        {
            double wristsensitivity = .1;
            // Body symmetry Abs(Z) between right and left < bodysensitivity (wrist)
            if (wristToElbowLeftZ < -wristsensitivity && wristToElbowRightZ < -wristsensitivity && armsperpendicular) badString = "Move your wrists backward";
            else if (wristToElbowLeftZ > wristsensitivity && wristToElbowRightZ > wristsensitivity) badString = "Move your wrists forward";
            else if (wristSymm > wristsensitivity)
            {
                if (wristToElbowLeftZ < -wristsensitivity) badString = "Move your left wrist backward";
                else if (wristToElbowRightZ > wristsensitivity) badString = "Move your right wrist forward";
            }
            else if (wristSymm < -wristsensitivity)
            {
                if (wristToElbowRightZ < -wristsensitivity) badString = "Move your right wrist backward";
                else if (wristToElbowLeftZ > wristsensitivity) badString = "Move your left wrist forward";
            }
            else badString = "";
        return badString;
        }
    }

    class CatCow : Pose { public CatCow() : base() { } }
    class Child : Pose { public Child() : base() { } }
    class DownwardDog : Pose { public DownwardDog() : base() { } }
    class DownwardDogFlow : Pose { public DownwardDogFlow() : base() { } }
    class StandingFold : Pose { public StandingFold() : base() { } }
    class StandingFoldFlow : Pose { public StandingFoldFlow() : base() { } }
    class LowerBackRelease : Pose { public LowerBackRelease() : base() { } }
    class ThreadNeedle : Pose { public ThreadNeedle() : base() { } }
    class Bridge : Pose { public Bridge() : base() { } }
    class BridgeFlow : Pose { public BridgeFlow() : base() { } }
    class HappyBaby : Pose { public HappyBaby() : base() { } }
    class BoundAngle : Pose { public BoundAngle() : base() { } }
    class ReclinedTwist : Pose { public ReclinedTwist() : base() { } }
    class Corpse : Pose { public Corpse() : base() { } }
    class CorpseExit : Pose { public CorpseExit() : base() { } }
    class Plank : Pose { public Plank() : base() { } }
    class Cobra : Pose { public Cobra() : base() { } }
}
