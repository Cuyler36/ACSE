namespace ACSE.Core.Generators
{
    public interface IGenerator
    {
        ushort[] Generate(int? seed = null);
    }
}
