using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
  public class EmailService
  {
    private DatabaseContext _databaseManager;
    private EmailSendService _emailSendService;
    public EmailService(DatabaseContext databaseContext, EmailSendService emailSendService)
    {
      _databaseManager = databaseContext;
      _emailSendService = emailSendService;
    }


    public async Task<bool> AddEmailCodeAsync(string Email)
    {
      EmailVerificationModel? model = await _databaseManager.EmailVerificationModels
      .FirstOrDefaultAsync(e => e.Email.Equals(Email));

      if (model != null && DateTime.UtcNow > model.ExpiresAt.ToUniversalTime())
      {
        _databaseManager.EmailVerificationModels.Remove(model);
        if (await _databaseManager.SaveChangesAsync() <= 0)
        {
          return false;
        }
      }
      string code = _emailSendService.GenerateVerificationCode();
      List<string> clients = new List<string> { Email };
      bool res = _emailSendService.SendEmailToClients(clients,
"""
Hello,
You requested a password reset for your account. Use the verification code below to continue:

{VERIFICATION_CODE}

This code will expire soon, so please use it promptly. If you did not request this, you can ignore this email
Thank you, The Support Team

2025 Savans. All rights reserved.
""".Replace("{VERIFICATION_CODE}", code), "Verification Code - Savans Restaurant");

      if (!res)
      {
        return false;
      }

        await _databaseManager.EmailVerificationModels.AddAsync(new EmailVerificationModel()
        {
          Email = Email,
          Code = code,
          ExpiresAt = DateTime.UtcNow.AddMinutes(_emailSendService.getOTP_Expiration_Minutes())
        });
        return await _databaseManager.SaveChangesAsync() > 0;
    }

    public async Task<EmailVerificationModel?> GetEmailVerificationAsync(string Email)
    {
      return await _databaseManager.EmailVerificationModels.FirstOrDefaultAsync(e => e.Email.Equals(Email));
    }

    public async Task<bool> DeleteEmailVerificationAsync(string Email)
    {
      EmailVerificationModel? model = await GetEmailVerificationAsync(Email);
      if (model == null)
      {
        return false;
      }
      _databaseManager.EmailVerificationModels.Remove(model);
      return await _databaseManager.SaveChangesAsync() > 0;
    }
  }
}
