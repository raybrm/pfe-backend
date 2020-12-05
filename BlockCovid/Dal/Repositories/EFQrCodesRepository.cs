using BlockCovid.Interfaces;
using BlockCovid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Dal.Repositories
{
    public class EFQrCodesRepository : IQrCodesRepository
    {
        public Task<QrCode> CreateQrCodeAsync(QrCode citizen)
        {
            throw new NotImplementedException();
        }

        public Task<QrCode> GetQrCodeByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<List<QrCode>> GetQrCodesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
