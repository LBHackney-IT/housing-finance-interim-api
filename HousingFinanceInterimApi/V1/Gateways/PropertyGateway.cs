using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{

    public class PropertyGateway : IPropertyGateway
    {

        private readonly DatabaseContext _context;

        public PropertyGateway(DatabaseContext context)
        {
            _context = context;
        }
    }
}
