namespace WorkforceRealismEnhancement
{
    public struct WorkforceFactor(float @default, float low, float medium, float high)
    {
        public float Default = @default;
        public float Low = low;
        public float Medium = medium;
        public float High = high;
    }
}
