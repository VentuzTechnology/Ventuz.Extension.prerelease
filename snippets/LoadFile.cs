    [VxToolBox("Load File VX", "VX-Test", "Load", "This node load a file as text and returns a string", false)]
    [VxUri(VxDataPool.Data, "File")]
    public class LoadFileVx: VxContentNode
    {
        public string ValidateText(Uri File)
        {
            if (File != null)
            {
                try
                {
                    using ( var stream = VX.IO.GetReadStream(File))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    VX.Log.Error("Failed to load file", ex);
                }
            }
            return "";
        }
    }
    
