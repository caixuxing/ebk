using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueJia.Ebk.Application.Contracts.SysUserApp.Commands
{
    public class UpdatePasswordSysUserCmd
    {
        public string OldPassword { get; set; } = default!;
        public string NewFirstPassword { get; set; } = default!;
        public string NewConfirmPassword { get; set; } = default!;

    }
}
