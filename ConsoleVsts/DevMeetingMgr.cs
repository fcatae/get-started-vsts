using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleVsts
{
    class DevMeetingMgr
    {
        const int TASK_INTERNAL_BRAZIL_DEVMEETINGS = 397583;
        const string WORKITEM_LINKTYPE_CHILD = "System.LinkTypes.Hierarchy-Forward";

        VisualStudioRepository _vsts;

        public DevMeetingMgr(string vstsAccount, string vstsProject, string vstsToken)
        {
            _vsts = new VisualStudioRepository(vstsAccount, vstsProject, vstsToken);
        }

        public async Task ListMeetingsAsync()
        {
            var items = await _vsts.GetChildItemsAsync(TASK_INTERNAL_BRAZIL_DEVMEETINGS);
        }
    }
}
