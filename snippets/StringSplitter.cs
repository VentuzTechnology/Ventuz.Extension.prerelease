    [VxToolBox("String Splitter VX", "VX-Test", "Split", "This node splits a text string into text fragments which can be accessed by their index.", false)]
    [VxIcon("NodeIcons.Logic.Expressions")]
    [VxCategory(0, "Text", false, "Input", "Current", "RemoveEmptyItems")]
    [VxCategory(1, "Separator", false, "Custom", "Lines", "Tabs")]
    [VxCategory(2, "Output", false, "Items", "Count", "IsNullOrWhitespace", "Item")]
    [VxDefaultValue("", "Input", "Custom")]
    [VxDefaultValue(0, "Current")]
    [VxDefaultValue(false, "RemoveEmptyItems", "Tabs")]
    [VxDefaultValue(true, "Lines")]
    public class StringSplitterVx : VxContentNode
    {
        protected char[] Validate_Seps(string Custom, bool Lines, bool Tabs)
        {
            string sep = "";
            if ( Custom != null ) sep += Custom.Replace("\n\r", "").Replace("\n", "");
            if ( Lines ) sep += "\n";
            if ( Tabs ) sep += "\t";
            return sep.ToCharArray();
        }

        protected (string[] Items, int Count, bool IsNullOrWhitespace) Validate2(string Input, char[] _Seps, bool RemoveEmptyItems)
        {
            string[] toks = null;
            if (Input != null && _Seps?.Length > 0)
            {
                Input = Input.Replace("\r", "");
                toks = Input.Split(_Seps, RemoveEmptyItems ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
            }
            return (toks, toks?.Length ?? 0, string.IsNullOrWhiteSpace(Input));
        }

        protected string ValidateItem(string[] Items, int Current, bool RemoveEmptyItems)
            => (Current >= 0 && Current < (Items?.Length ?? 0)) ? Items[Current] : "";
    }
    
