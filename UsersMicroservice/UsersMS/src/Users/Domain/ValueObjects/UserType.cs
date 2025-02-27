﻿using UsersMS.Core.Domain.ValueObjects;
using UsersMS.src.Users.Domain.Exceptions;

namespace UsersMS.src.Users.Domain.ValueObjects
{
    public class UserType : IValueObject<UserType>
    {
        public static readonly string Operator = "Operator";
        public static readonly string Provider = "Provider";
        public static readonly string Driver = "Driver";
        public static readonly string Admin = "Admin";
        public string Type { get; }

        public UserType(string type)
        {
            if (type != Operator && type != Provider && type != Driver && type != Admin)
            {
                throw new InvalidUserTypeException($"Invalid policy type: {type}. Allowed values are: {Operator}, {Provider}, {Driver}, {Admin}.");
            }
            Type = type;
        }

        public string GetValue()
        {
            return Type;
        }

        public bool Equals(UserType other)
        {
            return Type == other.Type;
        }
    }
}
