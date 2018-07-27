namespace ACSE.Generators
{
    public static class Generator
    {
        public static IGenerator GetGenerator(SaveGeneration Generation)
        {
            switch (Generation)
            {
                case SaveGeneration.GCN:
                    return new GCNGenerator();
                default:
                    return null;
            }
        }
    }
}
