namespace ACSE.Generators
{
    public static class Generator
    {
        public static IGenerator GetGenerator(SaveGeneration generation)
        {
            switch (generation)
            {
                case SaveGeneration.GCN:
                    return new GCNGenerator();
                default:
                    return null;
            }
        }
    }
}
