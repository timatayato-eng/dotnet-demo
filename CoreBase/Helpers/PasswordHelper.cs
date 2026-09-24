using System.Security.Cryptography;

namespace CoreBase.Helpers;

public static class PasswordHelper
{
  public static string Hash(string password)
  {
    var salt = RandomNumberGenerator.GetBytes(16);
    var hash = Rfc2898DeriveBytes.Pbkdf2(
      password,
      salt,
      iterations: 100_000,
      HashAlgorithmName.SHA256,
      outputLength: 32);

    return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
  }

  public static bool Verify(string password, string stored)
  {
    var parts = stored.Split('.', 2);
    if (parts.Length != 2) return false;

    var salt = Convert.FromBase64String(parts[0]);
    var expected = Convert.FromBase64String(parts[1]);
    var actual = Rfc2898DeriveBytes.Pbkdf2(
      password,
      salt,
      iterations: 100_000,
      HashAlgorithmName.SHA256,
      outputLength: 32);

    return CryptographicOperations.FixedTimeEquals(actual, expected);
  }
}
