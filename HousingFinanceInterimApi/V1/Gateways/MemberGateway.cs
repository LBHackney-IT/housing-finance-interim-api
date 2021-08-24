using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    public class MemberGateway : IMemberGateway
    {

        private readonly DatabaseContext _context;

        public MemberGateway(DatabaseContext context)
        {
            _context = context;
        }
    }
}
