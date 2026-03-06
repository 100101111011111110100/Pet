using System.Security.Cryptography;

using Datasource.Iinterfaces;

namespace Datasource.Services;

public class RandomGenerator: IRandomGenerator
{
    public int GetRandomNumber(int low, int high)=> RandomNumberGenerator.GetInt32(low, high);
}
