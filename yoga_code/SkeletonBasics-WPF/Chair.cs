using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.EyesFreeYoga
{
    class Chair : Pose
    {
        Boolean goodCore = false, goodLegs = false, goodArms = false;
        public Chair() : base() {
            rules[dontLeanString] = true;
            rules[symmetricHipsString] = true;
            rules[symmetricShouldersString] = true;
            rules[legsBentString] = true;
            rules[kneesInXString] = true;
            rules[symmetricKneesString] = true;
            rules[symmetricAnklesString] = true;
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
            object[] arguments = new object[4] { true, false, false, false };
            output = checkRule(dontLeanString, arguments);
            if (output != "") return output;

            output = checkRule(symmetricHipsString);
            if (output != "") return output;

            output = checkRule(symmetricShouldersString);
            if (output != "") return output;

            if (!goodCore)
            {
                goodCore = true;
                MainWindow.playSoundResourceSync("core_good");
            }

            // 2. Check the legs next

            // Check that the legs are bent
            output = checkRule(legsBentString);
            if (output != "") return output;

            output = checkRule(kneesInXString);
            if (output != "") return output;

            output = checkRule(symmetricKneesString);
            if (output != "") return output;

            output = checkRule(symmetricAnklesString);
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

            arguments = new object[1] { false };
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
