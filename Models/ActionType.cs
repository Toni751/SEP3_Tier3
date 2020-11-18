using System;

namespace SEP3_Tier3.Models
{
    [Flags]
    public enum ActionType
    {
        USER_LOGIN,
        USER_REGISTER,
        USER_GET_BY_ID
    }
}