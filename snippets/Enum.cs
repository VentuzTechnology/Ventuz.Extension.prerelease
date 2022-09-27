    [VxDefaultValue(MyDirection.Up, "Direction")]
    [VxDefaultValue(MyFlags.B, "Flags")]
    public class VX_ENUM : VxContentNode
    {
        public enum MyDirection
        {
            Left, Right, Up, Down
        }

        [VxFlagGroupName(0x0f, "Select one or more flags")]
        [VxFlagGroupName(0x30, "Select a color")]
        [Flags]
        public enum MyFlags
        {
            // 0x0f: ABCD (flags)

            [VxFlagGroupItem(0x0f, false, DisplayName = "For A")]
            A = 1,

            [VxFlagGroupItem(0x0f, false, DisplayName = "For B")]
            B = 2,

            [VxFlagGroupItem(0x0f, false, DisplayName = "For C")]
            C = 4,

            [VxFlagGroupItem(0x0f, false, DisplayName = "For D")]
            D = 8,

            // 0x30: color selection (enum)

            [VxFlagGroupItem(0x30, true, DisplayName = "Red")]
            R = 0x10,

            [VxFlagGroupItem(0x30, true, DisplayName = "Green")]
            G = 0x20,

            [VxFlagGroupItem(0x30, true, DisplayName = "Yellow")]
            Y = 0x30,

        }

        public int ValidateOut(MyDirection Direction, MyFlags Flags)
        {
            return (int)Direction + (int)Flags;
        }
    }
