using System;
using System.Collections.Generic;
using System.Text;
using Wby.Demo.Shared.DataModel;

namespace Wby.Demo.Shared.Dto
{
    public class UserInfoDto
    {
        public User User { get; set; }
        public List<Menu> Menus { get; set; }
    }
}
