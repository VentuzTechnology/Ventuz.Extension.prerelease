    // This node creates a 'boolean impulse' with a specific duration like this:
    //
    // true        ------
    //            |      |
    //            |      |
    // fale: -----        --------
    [VxToolBox("VX Impulse", "VX-Test", "Impulse", "Impulse with customizable duration", false)]
    [VxDefaultValue(1, "Duration")]
    [VxDefaultValue(false, "Invert")]
    public class VX_Impulse : VxContentNode
    {
        int countdownDuration = 0;
        int countdown = 0;

        public void MethodTrigger()
        {
            countdown = countdownDuration;          // begin countdown
            this.Invalidate();                      // force node validation
        }

        public bool GenerateValue(int Duration, bool Invert)
        {
            countdownDuration = Duration;

            if ( !Invert )
            {
                // false -> true -> false
                if ( countdown > 0 )
                {
                    countdown--;
                    return true;
                }

                return false;
            }
            else
            {
                // true -> false -> true
                if ( countdown > 0 )
                {
                    countdown--;
                    return false;
                }

                return true;
            }
        }
    }
