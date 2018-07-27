namespace ACSE.Generators
{
    public interface IGenerator
    {
        ushort[] Generate(int? Seed = null);
    }
}
