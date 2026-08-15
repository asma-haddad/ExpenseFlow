using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace ExpenseFlow.Application.Services.Helper;

public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        var passwordHasher = new PasswordHasher<object>();
        return passwordHasher.HashPassword(null, password);
    }

    public static PasswordVerificationResult CheckPassword(string password, string hash, out string newHash)
    {
        var passwordHasher = new PasswordHasher<object>();
        var result = passwordHasher.VerifyHashedPassword(null, hash, password);
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            newHash = HashPassword(password);
            return result;
        }
        else if (result == PasswordVerificationResult.Success)
        {
            newHash = hash;
            return result;
        }
        newHash = null;
        return result;
    }

    public static bool CheckPasswordPolicy(string password, PasswordCriteria opts = null)
    {
        opts ??= new PasswordCriteria();

        if (password.Length < opts.MinRequiredLength)
            return false;
        else if (password.Length > opts.MaxRequiredLength)
            return false;

        int digitCount = 0, lowerCount = 0, upperCount = 0, symbolCount = 0;
        var set = new HashSet<char>();

        foreach (var character in password)
        {
            if (char.IsDigit(character))
                digitCount++;
            else if (char.IsUpper(character))
                upperCount++;
            else if (char.IsLower(character))
                lowerCount++;
            else
                symbolCount++;

            set.Add(character);
        }
        return set.Count >= opts.RequiredUniqueChars
            && (!opts.RequireDigit || digitCount > 0)
            && (!opts.RequireLowercase || lowerCount > 0)
            && (!opts.RequireUppercase || upperCount > 0)
            && (!opts.RequireNonAlphanumeric || symbolCount > 0);
    }

    public static string GenerateStrongPassword(PasswordCriteria criteria = null)
    {
        criteria ??= new PasswordCriteria();

        const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digitChars = "0123456789";
        const string symbolChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        var passwordChars = new List<char>();
        var availableChars = new StringBuilder();

        using (var rng = RandomNumberGenerator.Create())
        {
            char GetRandomChar(string source)
            {
                byte[] bytes = new byte[4];
                rng.GetBytes(bytes);
                uint randomValue = BitConverter.ToUInt32(bytes, 0);
                return source[(int)(randomValue % source.Length)];
            }


            if (criteria.RequireLowercase)
            {
                availableChars.Append(lowercaseChars);
                passwordChars.Add(GetRandomChar(lowercaseChars));
            }
            if (criteria.RequireUppercase)
            {
                availableChars.Append(uppercaseChars);
                passwordChars.Add(GetRandomChar(uppercaseChars));
            }
            if (criteria.RequireDigit)
            {
                availableChars.Append(digitChars);
                passwordChars.Add(GetRandomChar(digitChars));
            }
            if (criteria.RequireNonAlphanumeric)
            {
                availableChars.Append(symbolChars);
                passwordChars.Add(GetRandomChar(symbolChars));
            }

            if (availableChars.Length == 0)
            {
                throw new InvalidOperationException("Password generation criteria require at least one character type.");
            }

            string allAvailableChars = availableChars.ToString();

            for (int i = passwordChars.Count; i < criteria.MinRequiredLength; i++)
            {
                passwordChars.Add(GetRandomChar(allAvailableChars));
            }

            for (int i = passwordChars.Count - 1; i > 0; i--)
            {
                byte[] bytes = new byte[4];
                rng.GetBytes(bytes);
                uint randomIndex = BitConverter.ToUInt32(bytes, 0) % (uint)(i + 1);

                (passwordChars[i], passwordChars[(int)randomIndex]) = (passwordChars[(int)randomIndex], passwordChars[i]);
            }
        }

        return new string(passwordChars.ToArray());
    }
}