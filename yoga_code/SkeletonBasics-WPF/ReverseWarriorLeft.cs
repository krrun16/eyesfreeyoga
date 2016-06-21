using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.EyesFreeYoga
{
    class ReverseWarriorLeft : Pose
    {
        Boolean goodCore = false, goodLegs = false, goodArms = false;
        public ReverseWarriorLeft() : base() {
            rules[symmetricHipsString] = true;
            rules[hipsLevelString] = true;
            rules[symmetricShouldersString] = true;
            rules[reverseWarriorLeftLegString] = true;
            rules[leftKneeFarString] = true;
            rules[armsStraightString] = true;
            rules[leftArmRaisedString] = true;
            rules[rightArmTouchingString] = true;
            rules[symmetricElbowsString] = true;
            rules[symmetricWristsString] = true;
            rules[dontLeanString] = true;
        }

        public override String compareSkeleton(Skeleton skeleton)
        {
            base.compareSkeleton(skeleton);
            String output = "";

            // 1. Check the core first
            output = checkRule(symmetricHipsString);
            if (output != "") return output;

            output = checkRule(hipsLevelString);
            if (output != "") return output;

            output = checkRule(symmetricShouldersString);
            if (output != "") return output;

            if (!goodCore)
            {
                goodCore = true;
                MainWindow.playSoundResourceSync("core_good");
            }

            // 2. Check the legs next
            output = checkRule(reverseWarriorLeftLegString);
            if (output != "") return output;

            output = checkRule(leftKneeFarString);
            if (output != "") return output;

            if (!goodLegs)
            {
                goodLegs = true;
                MainWindow.playSoundResourceSync("legs_good");
            }

            //3. Check the arms next
            output = checkRule(armsStraightString);
            if (output != "") return output;

            output = checkRule(leftArmRaisedString);
            if (output != "") return output;

            output = checkRule(rightArmTouchingString);
            if (output != "") return output;

            output = checkRule(symmetricElbowsString);
            if (output != "") return output;

            object[] arguments = new object[1] { true };
            output = checkRule(symmetricWristsString, arguments);
            if (output != "") return output;

            if (!goodArms)
            {
                goodArms = true;
                MainWindow.playSoundResourceSync("arms_good");
            }

            // 4. Lean backward last
            arguments = new object[4] { false, true, true, false };
            output = checkRule(dontLeanString, arguments);
            if (output != "") return output;

            badGroup = "";
            return "";
        }
    }
}