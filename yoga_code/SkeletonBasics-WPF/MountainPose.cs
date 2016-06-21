using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Controls;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.EyesFreeYoga
{
    class MountainPose : Pose
    {
        Boolean goodCore = false, goodLegs = false, goodArms = false;
        
        public MountainPose() : base() {
            rules.Add(dontLeanString, true);
            rules.Add(symmetricHipsString, true);
            rules.Add(symmetricShouldersString, true);
            rules.Add(legsStraightString, true);
            rules.Add(symmetricKneesString, true);
            rules.Add(symmetricAnklesString, true);
            rules.Add(armsStraightString, true);
            rules.Add(armsLowString, true);
            rules.Add(symmetricElbowsString, true);
            rules.Add(symmetricWristsString, true);
            //rules.Add(ankleDistanceString, true);
        }

        public override String compareSkeleton(Skeleton skeleton)
        {
            base.compareSkeleton(skeleton);
            String output = "";

            // 1. Check the core first
            object[] arguments = new object[4] { true, true, false, true };
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
            output = checkRule(legsStraightString);
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

            output = checkRule(armsLowString);
            if (output != "") return output;

            output = checkRule(symmetricElbowsString);
            if (output != "") return output;

            arguments = new object[1] { true };
            output = checkRule(symmetricWristsString, arguments);
            if (output != "") return output;

            if (!goodArms) {
                goodArms = true;
                MainWindow.playSoundResourceSync("arms_good");
            }

            //4. Feet distance last
            //output = checkRule(ankleDistanceString);
            //if (output != "") return output;

            badGroup = "";
            return "";
        }
    }
}
