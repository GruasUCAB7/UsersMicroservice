﻿namespace UsersMS.src.Users.Application.Commands.CreateUser.Types
{
    public record CreateUserResponse(string Id, List<string>? Errors = null);
}
