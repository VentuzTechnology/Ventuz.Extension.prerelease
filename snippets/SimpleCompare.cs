
    [VxToolBox("Double_Compare", "VX Samples", "Cmp", "This Node compares values with simple operators", true)]
    [VxCategory(0, "Inputs", false, "ValueLeft", "ValueRight", "Operator")]
    [VxIcon("FirstVXTest.Resources.Icons.Equal.svg", 0)]
    [VxIcon("FirstVXTest.Resources.Icons.NotEqual.svg", 1)]
    [VxIcon("FirstVXTest.Resources.Icons.Smaller.svg", 2)]
    [VxIcon("FirstVXTest.Resources.Icons.Bigger.svg", 3)]
    [VxIcon("FirstVXTest.Resources.Icons.SmallerEqual.svg", 4)]
    [VxIcon("FirstVXTest.Resources.Icons.BiggerEqual.svg", 5)]
    [VxIcon("FirstVXTest.Resources.Icons.Error.svg", 6)]
    [VxDefaultValue(0, "Operator")]
    
    public class CompareDouble : VxContentNode, IIconIndex, ITooltip
    {
        public int IconIndex { get; set; }
        public string Tooltip { get; set; }

        [VxFlagGroupName(0x0F, "Operator")]
        public enum Operator
        {
            [VxFlagGroupItem(0x0f, true, DisplayName = "==")]
            Equal,
            [VxFlagGroupItem(0x0f, true, DisplayName = "!=")]
            NonEqual,
            [VxFlagGroupItem(0x0f, true, DisplayName = "<=")]
            SmallerEqual,
            [VxFlagGroupItem(0x0f, true, DisplayName = "<")]
            Smaller,
            [VxFlagGroupItem(0x0f, true, DisplayName = ">=")]
            BiggerEqual,
            [VxFlagGroupItem(0x0f, true, DisplayName = ">")]
            Bigger
        }

        public bool Compare<T>(Operator op, T left, T right) where T : IComparable
        {
            int cmp = left.CompareTo(right);
            Tooltip = $"Compare: {left} {op} {right}";

            switch (op)
            {
                case Operator.Equal:
                    IconIndex = 0;
                    return cmp == 0;
                case Operator.NonEqual:
                    IconIndex = 1;
                    return cmp != 0;
                case Operator.Smaller:
                    IconIndex = 2;
                    return cmp < 0;
                case Operator.Bigger:
                    IconIndex = 3;
                    return cmp > 0;
                case Operator.SmallerEqual:
                    IconIndex = 4;
                    return cmp <= 0;
                case Operator.BiggerEqual:
                    IconIndex = 5;
                    return cmp >= 0;
                default:
                    IconIndex = 6;
                    LogError($"Invalid comparison operator: {op}");
                    return false;
            }
        }

        public bool ValidateResult(double ValueLeft, double ValueRight, Operator Operator)
            => Compare(Operator, ValueLeft, ValueRight);

    }
