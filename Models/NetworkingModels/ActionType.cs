using System;

namespace SEP3_Tier3.Models
{
    [Flags]
    public enum ActionType
    {
        USER_LOGIN,
        USER_REGISTER,
        USER_GET_BY_ID,
        USER_EDIT,
        USER_DELETE,
        USER_FRIEND_REQUEST_SEND,
        USER_FRIEND_REQUEST_RESPONSE,
        USER_FRIEND_REMOVE,
        USER_SHARE_TRAININGS,
        USER_SHARE_DIETS,
        USER_FOLLOW_PAGE,
        USER_RATE_PAGE,
        USER_REPORT,
        ADMIN_GET_USERS,
        ADMIN_GET_POSTS,
        HAS_IMAGES
    }
}