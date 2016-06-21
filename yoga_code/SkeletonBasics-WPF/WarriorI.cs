using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.EyesFreeYoga
{
    class WarriorI : Pose
    {
        Boolean goodCore = false, goodLegs = false, goodArms = false;
        public WarriorI() : base() {
            rules[dontLeanString] = true;
            rules[symmetricHipsString] = true;
            rules[hipsLevelString] = true;
            rules[symmetricShouldersString] = true;
            rules[warriorILegString] = true;
            rules[rightKneeFarString] = true;
            rules[rightKneeInXString] = true;
            rules[armsStraightString] = true;
            rules[armsRaisedString] = true;
            rules[symmetricElbowsString] = true;
            rules[symmetricWristsString] = true;
        }

        public override String compareSkeleton(Skeleton skeleton)
        {
            base.compareSkeleton(skeleton);
            String output = "";

            // 1. Check the core first
            object[] arguments = new object[4] { true, true, false, false };
            output = checkRule(dontLeanString, arguments);
            if (output != "") return output;

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
            output = checkRule(warriorILegString);
            if (output != "") return output;

            output = checkRule(rightKneeFarString);
            if (output != "") return output;

            output = checkRule(rightKneeInXString);
            if (output != "") return output;

            if (!goodLegs)
            {
                goodLegs = true;
                MainWindow.playSoundResourceSync("legs_good");
            }

            //3. Check the arms next
            output = checkRule(armsStraightString);
            if (output != "") return output;

            output = checkRule(armsRaisedString);
            if (output != "") return output;

            output = checkRule(symmetricElbowsString);
            if (output != "") return output;

            arguments = new object[1] { true };
            output = checkRule(symmetricWristsString, arguments);
            if (output != "") return output;

            if (!goodArms)
            {
                goodArms = true;
                MainWindow.playSoundResourceSync("arms_good");
            }

            badGroup = "";
            return "";
        }
    }
}