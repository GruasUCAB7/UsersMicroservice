﻿namespace UsersMS.Core.Application.IdGenerator
{
    public interface IdGenerator<T>
    {
        T Generate();
    }
}
