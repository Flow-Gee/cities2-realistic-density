namespace WorkforceRealismEnhancement
{
    public static class WorkforceFactors
    {
        public static WorkforceFactor Commercial = new (1.0f, 1.25f, 1.50f, 2.0f);
        public static WorkforceFactor Office = new (1.0f, 1.25f, 1.50f, 2.0f);
        public static WorkforceFactor IndustryExtractor = new(1.0f, 1.25f, 1.50f, 2.0f);
        public static WorkforceFactor IndustrySelling = new (1.0f, 1.25f, 1.50f, 2.0f);
        public static WorkforceFactor IndustryProcessing = new (1.0f, 1.25f, 1.50f, 2.0f);
        public static WorkforceFactor PowerPlant = new(1.0f, 4f, 8f, 10f);
        public static WorkforceFactor School = new (1.0f, 1.5f, 3f, 6f);
    }
}
