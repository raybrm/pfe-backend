using BlockCovid.Models;
using BlockCovid.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Interfaces
{
    public interface IQrCodesRepository
    {
        Task<List<QrCode>> GetQrCodesAsync();
        Task<QrCode> GetQrCodeByIdAsync(long id);
        Task<QrCode> CreateQrCodeAsync(QrCode citizen);
        Task ScanQrCode(ScanQrCodeDto scanQrCodeDto);
    }
}
