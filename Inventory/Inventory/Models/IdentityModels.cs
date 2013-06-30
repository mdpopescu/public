using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Renfield.Inventory.Models
{
  // Modify the User class to add extra user information
  public class User : IUser
  {
    public User()
      : this(String.Empty)
    {
    }

    public User(string userName)
    {
      UserName = userName;
      Id = Guid.NewGuid().ToString();
    }

    [Key]
    public string Id { get; set; }

    public string UserName { get; set; }
  }

  public class UserLogin : IUserLogin
  {
    [Key, Column(Order = 0)]
    public string LoginProvider { get; set; }
    [Key, Column(Order = 1)]
    public string ProviderKey { get; set; }

    public string UserId { get; set; }

    public UserLogin() { }

    public UserLogin(string userId, string loginProvider, string providerKey)
    {
      LoginProvider = loginProvider;
      ProviderKey = providerKey;
      UserId = userId;
    }
  }

  public class UserSecret : IUserSecret
  {
    public UserSecret()
    {
    }

    public UserSecret(string userName, string secret)
    {
      UserName = userName;
      Secret = secret;
    }

    [Key]
    public string UserName { get; set; }
    public string Secret { get; set; }
  }

  public class UserRole : IUserRole
  {
    [Key, Column(Order = 0)]
    public string RoleId { get; set; }
    [Key, Column(Order = 1)]
    public string UserId { get; set; }
  }

  public class Role : IRole
  {
    public Role()
      : this(String.Empty)
    {
    }

    public Role(string roleName)
    {
      Id = roleName;
    }

    [Key]
    public string Id { get; set; }
  }
}