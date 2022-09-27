    [VxToolBox("Random VX", "VX-Test", "rnd", "Generate random numbers", false)]
    [VxDefaultValue(10.0f, "Range")]
    [VxHelpUrl("https://www.ventuz.com", "Ventuz Homepage")]
    public class RandomNumerVx : VxContentNode
    {
        Random RND = new Random();

        public float GenerateRandom(float Range) => (float)(RND.NextDouble() * Range);
    }
    
