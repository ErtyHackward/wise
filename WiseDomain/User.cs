using System;
using System.Collections.Generic;
using System.Text;

namespace WiseDomain
{
    public class User
    {
        public int Id { get; set; }

        public string DisplayName { get; set; }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public string Ip { get; set; }

        public string AvatarUrl { get; set; }

        public DateTime RegisteredAt { get; set; }

        public DateTime LastVisitedAt { get; set; }

        public List<UserGroupJoin> UserGroups { get; set; }
    }

    public class UserGroup
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool IsAdmin { get; set; }

        public List<UserGroupJoin> UserGroups { get; set; }
    }

    public class UserGroupJoin
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int GroupId { get; set; }
        public UserGroup Group { get; set; }
    }
}
