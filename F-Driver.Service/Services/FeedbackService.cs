using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using F_Driver.Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.Services
{
    public class FeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<FeedbackCreateModel> CreateFeedbackAsync(FeedbackCreateModel feedbackModel)
        {
            // Logic kiểm tra chuyến đi và hành khách
            var tripMatch = await _unitOfWork.TripMatches.FindAsync(tm=>tm.Id==feedbackModel.MatchId);
            if (tripMatch == null || tripMatch.Status != "Complete")
            {
                throw new ArgumentException("The trip is either not found or is not in a 'Complete' status.");
            }
            var tripRequest = await _unitOfWork.TripRequests.FindAsync(tr=>tr.Id==tripMatch.TripRequestId);
            if (tripRequest == null)
            {
                throw new ArgumentException("The request trip of trip matches not found!");
            }

            if (tripRequest.UserId!= feedbackModel.PassengerId)
            {
                throw new ArgumentException("Passenger did not participate in this trip.");
            }

            // Tạo feedback entity
            var feedback = new Feedback
            {
                MatchId = feedbackModel.MatchId,
                DriverId = feedbackModel.DriverId,
                PassengerId = feedbackModel.PassengerId,
                Rating = feedbackModel.Rating,
                Comment = feedbackModel.Comment,
                CreatedAt = feedbackModel.CreatedAt
            };

            await _unitOfWork.Feedbacks.CreateAsync(feedback);
            await _unitOfWork.CommitAsync();

            // Trả về feedback model sau khi xử lý
            return feedbackModel;
        }
    }
}
