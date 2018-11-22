using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.ViewModels
{
    public class ApplicationUserDTO
    {
        [Display(Name ="ID")]
        public string ID { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Display(Name = "电子邮箱地址")]
        public string Email { get; set; }

        [Display(Name = "角色")]
        public IList<IdentityRole> Roles { get; set; }
    }

    #region Index
    public class UserManageIndexViewModel
    {
        public IList<ApplicationUserDTO> ApplicationUserDTOs { get; set; }
    }
    #endregion

    #region Details
    public class UserManageDetailsViewModel
    {
        [Display(Name ="ID")]
        public string ID { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }


        [Display(Name = "电子邮箱地址")]
        public string Email { get; set; }

        [Display(Name = "角色")]
        public IList<IdentityRole> Roles { get; set; }
    }
    #endregion

    #region Edit
    public class UserManageEditViewModel
    {
        [Required]
        [Display(Name = "ID")]
        public string ID { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }


        [Display(Name = "电子邮箱地址")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "角色")]
        public IList<string> RoleIDs { get; set; }

        //
        public IList<IdentityRole> IdentityRoles {get;set;}
    }
    #endregion
}
