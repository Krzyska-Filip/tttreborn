using System;

using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [RoleAttribute("None")]
    public class NoneRole : TTTRole
    {
        public override Color Color => Color.Transparent;

        public override Type DefaultTeamType => typeof(NoneTeam);

        public NoneRole() : base()
        {

        }
    }
}
