namespace RealisticDensity
{
    public static class WorkforceFactors
    {
        public static Factor4 Commercial = new (1.0f, 1.25f, 1.50f, 2.0f);
        public static Factor4 Office = new (1.0f, 1.25f, 1.50f, 2.0f);
        public static Factor4 IndustryExtractor = new(1.0f, 1.25f, 1.50f, 2.0f);
        public static Factor4 IndustrySelling = new (1.0f, 1.25f, 1.50f, 2.0f);
        public static Factor4 IndustryProcessing = new (1.0f, 1.25f, 1.50f, 2.0f);
        public static Factor4 PowerPlant = new(1.0f, 4f, 8f, 10f);
        public static Factor4 School = new (1.0f, 1.5f, 3f, 6f);
    }
}
