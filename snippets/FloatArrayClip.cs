    public enum FilterModes
    {
        [VxFlagGroupItem(0, DisplayName = "Clip values to MinMax boundaries")]
        Clip = 0,
        [VxFlagGroupItem(0, DisplayName = "Only keep values in range [MinValue, Maxvalue]")]
        Filter = 1,
        [VxFlagGroupItem(0, DisplayName = "Only keep values in range ]MinValue, Maxvalue[")]
        FilterExclusive = 2,
    }

    [VxToolBox("ArrayClip", "Logic", "ArrayClip", null, false)]
    [VxCategory(0, "IN", false, "InputArray")]
    [VxCategory(1, "Range", false, "MinValue", "MaxValue")]
    [VxCategory(2, "Mode", false, "Filter")]
    [VxCategory(3, "OUT", false, "Clipped")]
    [VxDefaultValue(new float[] { 0.0f, 0.0f, 0.0f}, "InputArray")]
    [VxDefaultValue(0.0f, "MinValue")]
    [VxDefaultValue(10.0f, "MaxValue")]
    [VxDefaultValue(FilterModes.Clip, "Filter")]
    public class FloatArrayClip : VxContentNode
    {
        public float[] ValidateClipped(float[] InputArray, float MinValue, float MaxValue, FilterModes Filter)
        {
            if (InputArray == null)
                return null;
            if (InputArray.Length == 0)
                return new float[0];

            int len = InputArray.Length;

            if (Filter == FilterModes.Clip )
            {
                float[] outputArray = new float[len];

                if (Filter == FilterModes.Clip)
                {
                    for (int i = 0; i < len; i++)
                    {
                        float v = InputArray[i];
                        if (v >= MinValue && v <= MaxValue)
                            outputArray[i] = v;
                        else if (v < MinValue)
                            outputArray[i] = MinValue;
                        else if (v > MaxValue)
                            outputArray[i] = MaxValue;
                    } 
                }

                return outputArray;
            }
            else
            {
                List<float> outputList = new List<float>(len);

                if (Filter == FilterModes.Filter)
                {
                    for (int i = 0; i < len; i++)
                    {
                        float v = InputArray[i];
                        if (v >= MinValue && v <= MaxValue)
                            outputList.Add(v);
                    } 
                }
                else // FilterExclusive
                {
                    for (int i = 0; i < len; i++)
                    {
                        float v = InputArray[i];
                        if (v > MinValue && v < MaxValue)
                            outputList.Add(v);
                    }
                }

                return outputList.ToArray();
            }
        }
    }
