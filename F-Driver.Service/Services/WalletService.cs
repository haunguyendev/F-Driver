using AutoMapper;
using F_Driver.Repository.Interfaces;
using F_Driver.Service.DTO.VNPay;
using F_Driver.Service.Settings.VNPay;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace F_Driver.Service.Services
{
    public class WalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly VNPayService _vnPayService;
        private readonly VNPaySettings _vnPaySettings;

        public WalletService(IUnitOfWork unitOfWork, IMapper mapper, VNPayService vnPayService, IOptions<VNPaySettings> vnPaySettings
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _vnPayService = vnPayService;
            _vnPaySettings = vnPaySettings.Value;
        }


        //update wallet
        public async Task<(bool Success, string ErrorMessage, string PaymentUrl)> UpdateWalletAsync(int userId, decimal? amount)
        {
            var wallet = await _unitOfWork.Wallets.FindByCondition(w => w.UserId  == userId).FirstOrDefaultAsync();
            if (wallet == null)
            {
                return (false, null, null);
            }
            else
            {
                // create payment url
                var paymentUrl = string.Empty;
                
                    var vnPayRequest = new CreateVNPayModel(_vnPaySettings.Version,
                    _vnPaySettings.TmnCode, DateTime.Now, "127.0.0.1" ?? string.Empty, amount ?? 0, "VND",
                        "other", $"Update wallet's user id {userId}", _vnPaySettings.ReturnUrl, Guid.NewGuid().ToString() ?? string.Empty);
                    paymentUrl = _vnPayService.GetLink(_vnPaySettings.PaymentUrl, _vnPaySettings.HashSecret, vnPayRequest);
                    //Console.WriteLine(paymentUrl);
                return (true, null, paymentUrl);
            }
        }

        //update wallet with payment success
        public async Task<string> UpdatePaymentStatusAsync(UpdateVNPayModel updateVNPayModel)
        {
            if (updateVNPayModel == null)
            {
                return "Input data required"; // "RspCode":"99"
            }

            if (!_vnPayService.IsValidSignature(_vnPaySettings.HashSecret, updateVNPayModel))
            {
                return "Invalid signature"; // "RspCode":"97"
            }
            var match = Regex.Match(updateVNPayModel.vnp_OrderInfo, @"\d+");
            var wallet = await _unitOfWork.Wallets.FindByCondition(w => w.UserId == int.Parse(match.Value)).FirstOrDefaultAsync();
            if (wallet == null)
            {
                return "Wallet not found"; // "RspCode":"02"
            }
            wallet.Balance += updateVNPayModel.vnp_Amount/100;
            await _unitOfWork.Wallets.UpdateAsync(wallet);
            await _unitOfWork.CommitAsync();
            return "Success"; // "RspCode":"00"

        }
    }
}
