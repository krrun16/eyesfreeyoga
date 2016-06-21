using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.EyesFreeYoga
{
    class TreeRight : Pose
    {
        Boolean goodCore = false, goodLegs = false, goodArms = false;
        public TreeRight () : base() {
            rules[isWobblyString] = true;
            rules[dontLeanString] = true;
            rules[symmetricHipsString] = true;
            rules[hipsLevelString] = true;
            rules[symmetricShouldersString] = true;
            rules[leftFootHigherString] = true;
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

            output = checkRule(leftFootHigherString);
            if (output != "") return output;

            if (!goodLegs)
            {
                goodLegs = true;
                MainWindow.playSoundResourceSync("legs_good");
            }

            //3. Check the arms next

            if (!goodArms)
            {
                goodArms = true;
                MainWindow.playSoundResourceSync("arms_good");
            }

            //4. Check for being wobbly
            output = checkRule(isWobblyString);
            if (output != "") return output;

            badGroup = "";
            return "";
        }
    }
}
