using AutoMapper;
using F_Driver.Repository.Interfaces;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.BusinessModels.QueryParameters;
using F_Driver.Service.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.Services
{
    public class TransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region get history transaction of user
        public async Task<PaginatedList<TransactionResponseModel>> GetTransactionsByUserIdAsync(int userId, TransactionQueryParameters parameters)
        {
            var wallet = await _unitOfWork.Wallets
                .FindAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                throw new ArgumentException("User wallet not found.");
            }

            // Query cơ bản
            var query = _unitOfWork.Transactions
                .FindByCondition(t => t.WalletId == wallet.Id);

            // Lọc theo số tiền tối thiểu và tối đa
            if (parameters.MinAmount.HasValue)
            {
                query = query.Where(t => t.Amount >= parameters.MinAmount.Value);
            }


            if (parameters.MaxAmount.HasValue)
            {
                query = query.Where(t => t.Amount <= parameters.MaxAmount.Value);
            }

            // Lọc theo loại giao dịch
            if (!string.IsNullOrWhiteSpace(parameters.Type))
            {
                query = query.Where(t => t.Type == parameters.Type);
            }

            // Sắp xếp theo trường và thứ tự
            if (!string.IsNullOrWhiteSpace(parameters.Sort))
            {
                var sortBy = parameters.Sort;
                var sortOrder = parameters.SortOrder?.ToLower() == "desc" ? "desc" : "asc";

                query = sortOrder == "desc" ? query.OrderByDescending(t => EF.Property<object>(t, sortBy))
                                            : query.OrderBy(t => EF.Property<object>(t, sortBy));
            }
            else
            {
                // Sắp xếp mặc định theo TransactionDate nếu không có tham số Sort
                query = query.OrderByDescending(t => t.TransactionDate);
            }

            // Tính toán tổng số giao dịch
            var totalCount = await query.CountAsync();

            // Phân trang
            var transactions = await query.Skip((parameters.Page - 1) * parameters.PageSize)
                                          .Take(parameters.PageSize)
                                          .ToListAsync();

            // Map Transaction entity sang TransactionResponse
            var transactionDtos = _mapper.Map<List<TransactionResponseModel>>(transactions);

            // Trả về kết quả phân trang
            return new PaginatedList<TransactionResponseModel>(transactionDtos, totalCount, parameters.Page, parameters.PageSize);
        }


        #endregion
    }
}
