using Microsoft.AspNetCore.Cryptography.KeyDerivation;
namespace OlympicsApp.Models;
using System.Security.Cryptography;
using System.Text.Json;
public class FileUserStore
{
    private readonly string _filePath;
    public FileUserStore(string filePath)
    {
        _filePath = filePath;
        // Если файл не существует, создаем его
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, JsonSerializer.Serialize(new Dictionary<string, string>()));
        }
    }
    public bool AddUser(string email, string password)
    {
        var users = LoadUsers();
        if (users.ContainsKey(email)) return false;
        string salt;
        string hash = HashPassword(password, out salt);
        var userData = new UserData
        {
            Salt = salt,
            Hash = hash
        };
        users[email] = JsonSerializer.Serialize(userData);
        SaveUsers(users);
        return true;
    }
    public bool ValidateUser(string email, string password)
    {
        var users = LoadUsers();
        if (users.TryGetValue(email, out var storedData))
        {
            // Десериализация в типизированный объект
            var userData = JsonSerializer.Deserialize<UserData>(storedData);
            if (userData != null)
            {
                string storedSalt = userData.Salt;
                string storedHash = userData.Hash;
                return VerifyPassword(password, storedSalt, storedHash);
            }
        }
        return false;
    }
    private Dictionary<string, string> LoadUsers()
    {
        var fileContent = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(fileContent) ?? new Dictionary<string, string>();
    }
    private void SaveUsers(Dictionary<string, string> users)
    {
        var json = JsonSerializer.Serialize(users);
        File.WriteAllText(_filePath, json);
    }
    private string HashPassword(string password, out string salt)
    {
        byte[] saltBytes = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        salt = Convert.ToBase64String(saltBytes);
        string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
        return hash;
    }
    private bool VerifyPassword(string password, string storedSalt, string storedHash)
    {
        byte[] saltBytes = Convert.FromBase64String(storedSalt);
        string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
        return hash == storedHash;
    }
}