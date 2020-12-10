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
        Task<QrCodeDto> CreateQrCodeAsync(QrCodeDto qrCode,long participantID);
        Task ScanQrCode(ScanQrCodeDto scanQrCodeDto);
        bool QrCodeExists(String id);
        bool CitizenExists(long id);
    }
}
