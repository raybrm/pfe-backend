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
        Task<List<QrCodeDto>> GetQrCodesByLoginAsync(string login);
        Task<QrCode> CreateQrCodeAsync(QrCode qrCode);
        Task ScanQrCode(ScanQrCodeDto scanQrCodeDto);
    }
}
