using System;

class Program
{
    static void Main(string[] args)
    {
        string password = "password123";
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        Console.WriteLine($"Hashed Password: {hashedPassword}");
    }
}
